// ---------------------------------------------------------
// JugglingAtack.cs
// 概要 : 攻撃したら敵にまっすぐ飛んでく。あたったら跳ね返る（ランダム）。キャッチしたら速度上げる
// 作成者: Shota_Obora
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class JugglingAtack : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    // 共有変数
    static private int _nowJugglingAmount = 0;
    static private int _nowPinValue = 0;
    static public int NowJugglingAmount { get { return _nowJugglingAmount; } set { _nowJugglingAmount = value; } }

    static private float _commonAttackSpeed = 1.0f;         // 共有する攻撃スピード
    static private bool _isPlay = true;                     // キャッチしたり投げたりできる状態かを保持
    static public bool IsPlay { get { return _isPlay; } } 
    private readonly float MaxAttackSpeed = 3.0f;           // 最大スピード

    private GameObject _targetObj = null;                   // 
    private float _atackSpeed = 0.0f;                       // 攻撃スピード

    private bool _isAttack = false;                         // 攻撃中判定
    private bool _isReflect = false;                        // 反射判定
    private bool _isCatch = false;                          // このオブジェクトの生存判定

    [SerializeField] GameObject _nextJuggling = null;
    [SerializeField] GameObject _dropPointEffect = null;
    [SerializeField] GameObject _pinThrowEffect = null;
    private int _myPinValue = 0;
    [SerializeField] Mesh[] _pinMesh = new Mesh[3];

    #endregion

    #region method

    public void Run(EnemyBase target, int mesh)
    {
        // 使用中の個数を増加
        _nowJugglingAmount++;
        _nowPinValue++;

        // スタン中ならキャッチする
        if (target && target.IsStan)
        {
            Stop();
            return;
        }
        // それ以外なら
        if(!_isPlay || !PlayerManager.Instance.CheckActionType(ActionManager.eActionType.Juggling))
        {
            Stop();
            return;
        }

        // 初期化処理
        _atackSpeed = 10.0f * _commonAttackSpeed;
        if (mesh < 0)
        {
            int value = _nowPinValue - 1;
            if(_nowPinValue <= 0)
            {
                _nowPinValue = 1;
            }
            else if (_nowPinValue >= _pinMesh.Length)
            {
                _nowPinValue = _pinMesh.Length;
            }
            transform.GetComponentInChildren<MeshFilter>().mesh = _pinMesh[_nowPinValue - 1];
            _myPinValue = _nowPinValue - 1;
        }
        else
        {
            transform.GetComponentInChildren<MeshFilter>().mesh = _pinMesh[mesh];
            _myPinValue = mesh;
        }
        Vector3 initPos = transform.position;
        initPos.y = 1.0f;
        transform.position = initPos;
        if (target)
        {
            _targetObj = target.gameObject;
            Vector3 lookPos = _targetObj.transform.position;
            lookPos.y = 1.0f;
            transform.LookAt(lookPos);
        }

        // アクション実行
        StaticCoroutine.Instance.StartStaticCoroutine(ActionFlow());
    }

    /// <summary>
    /// 攻撃命令から地面に落ちるまでの処理フロー
    /// </summary>
    private IEnumerator ActionFlow()
    {
        // 投げるモーションを実行
        _isPlay = false;
        PlayerManager.Instance.Anim.SetTrigger("CatchThrow");
        transform.SetParent(PlayerManager.Instance.PlayerLeftHand);
        transform.localPosition = new Vector3(-0.5f, -0.6f, 0.0f);
        transform.localEulerAngles = new Vector3(180, 0, 0);

        // アニメーションが終了するまで待つ
        AnimatorStateInfo animStateInfo = PlayerManager.Instance.Anim.GetCurrentAnimatorStateInfo(0);
        while (animStateInfo.IsName("Base.Idle") || animStateInfo.IsName("Base.Walk"))
        {
            animStateInfo = PlayerManager.Instance.Anim.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }
        while (animStateInfo.normalizedTime <= 0.7f)
        {
            animStateInfo = PlayerManager.Instance.Anim.GetCurrentAnimatorStateInfo(0);

            if(PlayerManager.Instance.DamageAnimation())
            {
                PinDestroy(false);
                yield break;
            }

            yield return null;
        }
        transform.SetParent(null);
        transform.position = PlayerManager.Instance.GetPlayerForward() + new Vector3(0,1.75f,0);
        transform.position -= PlayerManager.Instance.Player.transform.right / 3.0f;
        transform.eulerAngles = PlayerManager.Instance.Player.transform.eulerAngles;
        GetComponentInChildren<AutoRotation>().enabled = true;
        StaticCoroutine.Instance.StartStaticCoroutine(ReplayWait(animStateInfo));
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.Player_Throw);
        Instantiate(_pinThrowEffect, transform.position, _pinThrowEffect.transform.rotation);

        // 投げる処理実行
        _isAttack = true;
        transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = true;
        Vector3 startPos = PlayerManager.Instance.PlayerObj.transform.position; // 投げてから当たるまでの距離を計算するため
        float atackTime = 0.0f;

        // 敵にあたって反射するか地形外に行くまで直進
        while (!_isReflect)
        {
            if (!this)
            {
                yield break;
            }

            Vector3 moveAmount = transform.forward * _atackSpeed * Time.deltaTime;
            transform.position += moveAmount;

            atackTime += Time.deltaTime;
            yield return null;
        }

        // 反射したので適当な位置を落下地点として設定
        Vector3 dropPoint = RandomDropPoint(startPos);
        dropPoint.y = 0.0f;

        GameObject dropEffect = Instantiate(_dropPointEffect, dropPoint, _dropPointEffect.transform.rotation);
        dropEffect.transform.position += new Vector3(0.0f, 0.01f, 0.0f);
        Vector3 initEffectScale = dropEffect.transform.localScale;

        if(atackTime < 0.25f)
        {
            atackTime = 0.25f;
        }
        else if(atackTime > 3.0f)
        {
            atackTime = 3.0f;
        }

        // 跳ね返り処理
        BezierCurve.tBez bez = new BezierCurve.tBez();  // 曲線移動のためベジエ曲線を使用
        bez.start = transform.position;
        bez.middle = Vector3.Lerp(transform.position, dropPoint, 0.5f) + new Vector3(0.0f, atackTime * 20.0f, 0.0f);
        bez.end = dropPoint;
        AutoRotation autoRot = GetComponentInChildren<AutoRotation>();
        Vector3 initRot = autoRot.RotDegreeAmount;

        while (1.0f > bez.time && !_isCatch)
        {
            transform.position = BezierCurve.CulcBez(bez, true);

            float time = Time.deltaTime * (0.35f / atackTime);

            if (bez.time > 0.8f)
            {
                time = Time.deltaTime * 0.1f;
                autoRot.RotDegreeAmount = initRot * 0.5f;

                if (dropEffect.transform.localScale.x > 0.3f)
                {
                    dropEffect.transform.localScale -= initEffectScale * (Time.deltaTime / 0.75f);
                }
                else
                {
                    dropEffect.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                }
            }

            bez.time += time;
            yield return null;
        }
        autoRot.RotDegreeAmount = initRot;

        dropEffect.GetComponent<JugglingDropEffect>().End();
        RunNext();
    }

    /// <summary>
    /// ピンの後処理
    /// </summary>
    private void RunNext()
    {
        // キャッチ判定
        PinDestroy(!_isCatch);
        if (_isCatch)
        {
            // 速度アップ
            if (_commonAttackSpeed < MaxAttackSpeed)
            {
                _commonAttackSpeed += (MaxAttackSpeed - 1.0f) * 0.1f;
            }
            // 次生成
            GameObject obj = Instantiate(_nextJuggling, transform.position, Quaternion.identity);
            obj.GetComponent<JugglingAtack>().Run(EnemyManager.Instance.BossEnemy, _myPinValue);
        }
        else
        {
            // 速度初期化
            _commonAttackSpeed = 1.0f;
            GameEffectManager.Instance.Play("PinLose", transform.position);
        }

        // 破棄処理
        Destroy(gameObject);
    }

    /// <summary>
    /// 処理停止
    /// </summary>
    private void Stop()
    {
        PinDestroy(true);
        Destroy(gameObject);
        GameEffectManager.Instance.Play("PinLose", transform.position);
    }

    /// <summary>
    /// ランダムな落下位置を返す
    /// </summary>
    private Vector3 RandomDropPoint(Vector3 dropPoint)
    {
        Vector3 range = PlayerManager.Instance.PlayerObj.transform.right * 3.0f;
        Vector3 rand = new Vector3(Random.Range(-range.x, range.x), 0.0f, Random.Range(-range.z, range.z));
        return dropPoint + rand;
    }

    /// <summary>
    /// 破棄処理
    /// 引数 : すぐにピンを補充しない場合Trueを入れる
    /// </summary>
    private void PinDestroy(bool isReload)
    {
        if (isReload)
        {
            JugglingReloader.Instance.Reload();
        }
        else
        {
            _nowJugglingAmount--; 
        }
        _nowPinValue--;
    }

    /// <summary>
    /// ピン投げのアニメーション実行処理
    /// </summary>
    private void PlayAnimation(string stateName)
    {
        _isPlay = false;
        PlayerManager.Instance.Anim.SetTrigger(stateName);
    }

    /// <summary>
    /// 行動再開待ちルーチン
    /// </summary>
    private IEnumerator ReplayWait(AnimatorStateInfo animStateInfo)
    {
        while (!animStateInfo.IsName("Base.Idle"))
        {
            animStateInfo = PlayerManager.Instance.Anim.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }
        _isPlay = true;
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        transform.eulerAngles = PlayerManager.Instance.Player.transform.forward;
        transform.GetChild(0).eulerAngles = Vector3.zero;
        GetComponentInChildren<AutoRotation>().enabled = false;
        var collider = GetComponentInChildren<CapsuleCollider>();
        collider.radius = 0.12f;
        collider.height = 1.55f;
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    private void OnTriggerEnter (Collider col)
    {
        // 跳ね返り中のピンにPlayerが触れたらキャッチ判定
        if (col.tag == "Player")
        {
            if (!_isReflect || !_isPlay)
                return;

            if (!PlayerManager.Instance.CheckActionType(ActionManager.eActionType.Juggling))
                return;

            _isCatch = true;
            transform.position = col.transform.position;
            transform.rotation = col.transform.rotation;
            return;
        }

        if (!_isAttack || _isReflect)
            return;

        // 敵にあたった場合、ダメージ処理をして跳ね返す
        if (col.tag == "Enemy")
        {
            GameObject obj = col.gameObject;
            while(obj.transform.parent)
            {
                obj = obj.transform.parent.gameObject;
            }
            obj.GetComponent<EnemyBase>().Damage();
            _isReflect = true;
            SoundManager.Instance.PlaySE(SoundManager.eSeValue.Player_SlowHit);
            GameEffectManager.Instance.Play("PinAttack", transform.position);
            var collider = GetComponentInChildren<CapsuleCollider>();
            collider.radius *= 3.0f;
            collider.height *= 1.25f;
            return;
        }

        if(col.tag == "ChildTotem")
        {
            _isReflect = true;
            SoundManager.Instance.PlaySE(SoundManager.eSeValue.Player_SlowHit);
            GameEffectManager.Instance.Play("PinAttack", transform.position);
            var collider = GetComponentInChildren<CapsuleCollider>();
            collider.radius *= 3.0f;
            collider.height *= 1.25f;
        }

        // Field外に行ってしまったら跳ね返す
        if (col.tag == "AutoRange")
        {
            RunNext();
            return;
        }
    }

    #endregion
}  