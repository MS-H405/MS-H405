// ---------------------------------------------------------
// Totem.cs
// 概要 : 
// 作成者: Shota_Obora
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
  
public class Totem : EnemyBase
{
    #region define

    private enum eAction
    {
        TotemPushUp = 0,
        ChildTotemPushUp,
        SpecialAttack,
        WindAttack,
        Max,
    };

    #endregion

    #region variable

    // トーテムの現在の状態を保持
    [SerializeField] eAction _action = eAction.TotemPushUp;
    private int _headAmount = 3;
    private bool _isAtack = false;  //  攻撃中かのフラグ
    private bool _isLook = true;

    [SerializeField] float _oneBlockSize = 1.0f;
    [SerializeField] float _oneBlockUpSpeed = 0.25f;         // 頭ひとつぶんの飛び出る

    [SerializeField] int _childTotemAmount = 5;              // 子分トーテムの数
    [SerializeField] GameObject _childTotemPrefab = null;    // 子分トーテムのプレハブ

    // 子分トーテムのリスト
    private List<ChildTotem> _childTotemList = new List<ChildTotem>();

    private Rigidbody _rigidbody = null;
    private float _fallHeight = 50.0f;

    // 演出用変数
    private ShakeCamera _shakeCamera = null;
    [SerializeField] string _appearEffectName = "TS_boss_appear";
    List<ManualRotation> _totemHeadList = new List<ManualRotation>();
    private EffekseerEmitter _totemLaser = null;
    private CapsuleCollider _totemLaserCol = null;
    [SerializeField] GameObject _stanEffect = null;

    #endregion

    #region method

    /// <summary>
    /// 行動ループ処理
    /// </summary>
    private IEnumerator Run()
    {
        if(_isDebug)
        {
            yield break;
        }

        // 待機
        float time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        eAction oldAction = _action;
        while (true)
        {
            // 行動ルーチン実行
            IEnumerator enumerator = RunAction();

            // 終わるまで待機する
            oldAction = _action;
            while (_action == oldAction)
            {
                // スタン状態なら一時停止
                if (IsStan)
                {
                    StaticCoroutine.Instance.StopCoroutine(enumerator);

                    while (IsStan)
                    {
                        // スタン演出
                        yield return null;
                    }
                    
                    yield return new WaitForSeconds(1.0f);
                }

                yield return null;
            }

            // 一周してたらループさせる
            if(_action >= eAction.Max)
            {
                _action = eAction.TotemPushUp;
            }
        }
    }
    
    /// <summary>
    /// 行動開始処理
    /// </summary>
    private IEnumerator RunAction()
    {
        switch (_action)
        {
            case eAction.TotemPushUp:
                return StaticCoroutine.Instance.StartStaticCoroutine(TotemPushUp());

            case eAction.ChildTotemPushUp:
                return StaticCoroutine.Instance.StartStaticCoroutine(ChildTotemPushUp());

            case eAction.SpecialAttack:
                return StaticCoroutine.Instance.StartStaticCoroutine(SpecialAtack());

            case eAction.WindAttack:
                return StaticCoroutine.Instance.StartStaticCoroutine(WindAttack());

            default:
                break;
        }
        return null;
    }

    /// <summary>
    /// 突き上げエフェクト処理
    /// </summary>
    private void AppearEffect()
    {
        _shakeCamera.Shake();
        GameEffectManager.Instance.PlayOnHeightZero(_appearEffectName, transform.position);
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.Totem_Attack);
    }

    /// <summary>
    /// トーテムの回転処理
    /// </summary>
    private void TotemRot(bool isUp, int headAmount, int rot = 360)
    {
        float speed = _oneBlockUpSpeed * headAmount;
        Vector3 rotAmount = new Vector3(0, rot, 0) * headAmount;
        if(isUp)
        {
            rotAmount *= -1;
        }

        // 回転実行
        foreach (ManualRotation head in _totemHeadList)
        {
            head.Run(rotAmount, speed);
            rotAmount *= -1;
        }
    }

    /// <summary>
    /// Playerの真下の座標を返す
    /// </summary>
    private Vector3 PlayerPos()
    {
        Vector3 pos = PlayerManager.Instance.PlayerObj.transform.position;
        pos.y = -_oneBlockSize * _headAmount;

        if(CheckNearTotem(gameObject, pos))
        {
            pos = RandomPos();
        }

        return pos;
    }

    /// <summary>
    /// ランダムな座標を返す
    /// </summary>
    private Vector3 RandomPos()
    {
        float range = StageData.FieldSize;
        Vector3 pos = new Vector3(-StageData.FieldSize, -_oneBlockSize * _headAmount, -StageData.FieldSize);

        float t, f;
        t = Random.Range(0, 65536) / 65536.0f * 2.0f * Mathf.PI;
        f = Random.Range(0, 65536) / 65536.0f * 2.0f * Mathf.PI;

        pos.x = range * Mathf.Sin(t) * Mathf.Cos(f) + 1.0f;
        pos.z = range * Mathf.Cos(t) + 1.0f;

        while(CheckNearTotem(gameObject, pos))
        {
            pos = RandomPos();
        }

        return pos;
    }

    /// <summary>
    /// 近くに他のトーテムがいないかチェック
    /// </summary>
    public bool CheckNearTotem(GameObject my, Vector3 pos)
    {
        pos.y = 0.0f;

        List<GameObject> objList = new List<GameObject>();
        objList.Add(gameObject);
        foreach(ChildTotem child in _childTotemList)
        {
            objList.Add(child.gameObject);
        }
        objList.Remove(my);

        foreach (GameObject obj in objList)
        {
            Vector3 objPos = obj.transform.position;
            objPos.y = 0.0f;
            if (Vector3.Distance(objPos, pos) > 3.5f)
            {
                continue;
            }

            return true;
        }
        return false;
    }

    /// <summary>
    /// 各自のスタンエフェクト再生処理
    /// </summary>
    protected override IEnumerator StanEffectUnique()
    {
        _animator.speed = 1.0f;
        SoundManager.Instance.PlayBGM(SoundManager.eBgmValue.Enemy_Stan);

        GameObject stanEffect = Instantiate(_stanEffect, transform.position + new Vector3(0, 9, 0), Quaternion.identity);
        stanEffect.transform.SetParent(_totemHeadList[0].transform);

        yield break;
    }


    #endregion

    #region actuin_method

    /// <summary>
    /// 突き上げ攻撃処理
    /// </summary>
    private IEnumerator TotemPushUp()
    {
        int amount = 1;
        float time = 0.0f;
        while (amount < _headAmount)
        {
            EnemyManager.Instance.Active = false;
            amount++;
            transform.position = PlayerPos();

            // 土煙を出す
            AppearEffect();
            time = 0.0f;
            while (time < 2.0f)
            {
                time += Time.deltaTime;
                yield return null;
            }

            time = 0.0f;
            _isAtack = true;
            AppearEffect();
            TotemRot(true, amount);
            Vector3 initPos = transform.position;
            Vector3 endPos = initPos + new Vector3(0, _oneBlockSize * amount, 0);
            while (time < amount)
            {
                transform.position = Vector3.Lerp(initPos, endPos, time / amount);
                time += Time.deltaTime / _oneBlockUpSpeed;

                // 半分出たところで通常視点に戻す
                if (time >= amount / 2.0f)
                {
                    EnemyManager.Instance.Active = true;
                }

                yield return null;
            }
            transform.position = endPos;
            _isAtack = false;

            // 待機
            float waitTime = 0.0f;
            while (waitTime < 7.5f)
            {
                waitTime += Time.deltaTime;
                yield return null;
            }
            EnemyManager.Instance.Active = false;

            // 潜る処理
            AppearEffect();
            TotemRot(false, amount);
            while (time > 0.0f)
            {
                transform.position -= new Vector3(0, _oneBlockSize * (Time.deltaTime / _oneBlockUpSpeed), 0);
                time -= Time.deltaTime / _oneBlockUpSpeed;
                yield return null;
            }

            // 待機処理
            time = 0.0f;
            while (time < 2.0f)
            {
                time += Time.deltaTime;
                yield return null;
            }
        }

        // 次の行動へ
        _action += 1;
    }

    /// <summary>
    /// 子分と同時突き上げ処理
    /// </summary>
    private IEnumerator ChildTotemPushUp()
    {
        float time = 0.0f;

        // 子分の突き上げ処理
        for (int i = 0; i < _childTotemAmount; i++)
        {
            _childTotemList[i].gameObject.SetActive(true);
            StaticCoroutine.Instance.StartStaticCoroutine(_childTotemList[i].PushUp(_oneBlockUpSpeed));

            time = 0.0f;
            _shakeCamera.Shake();
            while (time < (_oneBlockUpSpeed * 3.0f) * 0.75f)
            {
                time += Time.deltaTime;
                yield return null;
            }
        }

        // 本体突き上げ処理
        transform.position = PlayerPos();
        //transform.LookAt(PlayerManager.Instance.GetVerticalPos(transform.position));

        // 土煙を出す
        AppearEffect();
        time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        time = 0.0f;
        _isAtack = true;
        AppearEffect();
        TotemRot(true, _headAmount);
        Vector3 initPos = transform.position;
        Vector3 endPos = initPos + new Vector3(0, _oneBlockSize * _headAmount, 0);
        while (time < _headAmount)
        {
            transform.position = Vector3.Lerp(initPos, endPos, time / _headAmount);
            time += Time.deltaTime / _oneBlockUpSpeed;

            // 半分出たところで通常視点に戻す
            if (time >= _headAmount / 2.0f)
            {
                EnemyManager.Instance.Active = true;
            }

            yield return null;
        }
        transform.position = endPos;
        _isAtack = false;

        // 待機
        time = 0.0f;
        while (time < 10.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // 子分を潜らせる
        _shakeCamera.Shake();
        for (int i = 0; i < _childTotemAmount; i++)
        {
            StaticCoroutine.Instance.StartStaticCoroutine(_childTotemList[i].Dive(_oneBlockUpSpeed));
        }

        // 待機
        time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // 次の行動へ
        _action += 1;
    }

    /// <summary>
    /// 特殊攻撃処理
    /// </summary>
    private IEnumerator SpecialAtack()
    {
        // 子分に特殊攻撃の実行を通知
        _shakeCamera.Shake();
        for (int i = 0; i < _childTotemAmount; i++)
        {
            _childTotemList[i].gameObject.SetActive(true);
            StaticCoroutine.Instance.StartStaticCoroutine(_childTotemList[i].SpecialAtack(_oneBlockUpSpeed, _fallHeight, i == 0));
        }

        // 待機
        while (true)
        {
            int cnt = 0;
            for (int i = 0; i < _childTotemAmount; i++)
            {
                if (_childTotemList[i].gameObject.activeSelf)
                    continue;

                cnt++;
            }

            if (cnt >= _childTotemAmount)
            {
                break;
            }
            yield return null;
        }

        // 待機
        float time = 0.0f;
        while (time < 2.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // 次の行動へ
        _action += 1;
    }

    /// <summary>
    /// 風砲攻撃処理
    /// </summary>
    private IEnumerator WindAttack()
    {
        _isLook = false;
        _totemLaser.Play();
        _animator.SetTrigger("Roar");
        _animator.speed = 0.25f;

        float time = 0.0f;
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.ObserveEveryValueChanged(_ => time >= 4.0f)
            .Where(_ => time >= 4.0f)
            .Subscribe(_ =>
            {
                _animator.speed = 0.0f;
                _totemLaserCol.enabled = true;
                disposable.Dispose();
            });

        // 終了待ち
        while (time < 7.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // ビーム後処理
        _animator.speed = 1.0f;
        _totemLaserCol.enabled = false;

        time = 0.0f;
        while (time < 3.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // 本体も潜らせる
        time = 0.0f;
        AppearEffect();
        TotemRot(false, _headAmount);
        while (time < _headAmount)
        {
            transform.position -= new Vector3(0, _oneBlockSize * (Time.deltaTime / _oneBlockUpSpeed), 0);
            time += Time.deltaTime / _oneBlockUpSpeed;
            yield return null;
        }
        EnemyManager.Instance.Active = false;

        // 待機
        time = 0.0f;
        while (time < 3.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        _isLook = true;
        _action += 1;
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private new void Awake()
    {
        base.Awake();

        // 必要コンポーネントの取得
        _rigidbody = GetComponent<Rigidbody>();
        _shakeCamera = Camera.main.GetComponent<ShakeCamera>();

        // 子分トーテムの生成処理
        for (int i = 0; i < _childTotemAmount; i++)
        {
            ChildTotem instance = Instantiate(_childTotemPrefab).GetComponent<ChildTotem>();
            instance.Init(this);
            instance.gameObject.SetActive(false);
            _childTotemList.Add(instance);
        }

        // トーテムの顔ごとの回転処理用コンポーネントを取得
        for(int i = 0; i < _headAmount; i++)
        {
            _totemHeadList.Add(transform.GetChild(i).GetComponent<ManualRotation>());
        }

        // トーテムの先端が見えてしまうので頭を一つ増やした状態で処理
        _headAmount += 1;

        // トーテムのレーザー攻撃準備
        _totemLaser = transform.GetChild(1).GetComponentInChildren<EffekseerEmitter>();
        _totemLaserCol = _totemLaser.GetComponentInChildren<CapsuleCollider>();
        _totemLaserCol.enabled = false;
    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {
        // 行動開始
        StaticCoroutine.Instance.StartStaticCoroutine(Run());

        // アニメーション処理
        Vector3 oldRot = Vector3.zero;
        this.LateUpdateAsObservable()
            .Subscribe(_ =>
            {
                if (_animator.GetBool("Stan") || !_isLook)
                {
                    Vector3 rot = transform.eulerAngles;
                    rot.y = oldRot.y;
                    transform.eulerAngles = rot;
                }
                else
                {
                    transform.LookAt(PlayerManager.Instance.GetVerticalPos(transform.position));
                }
                oldRot = transform.eulerAngles;
            });
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player" && _isAtack)
        {
            col.gameObject.GetComponent<Player>().Damage(1.5f);
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<Player>().Damage();
        }
    }

    #endregion
}  