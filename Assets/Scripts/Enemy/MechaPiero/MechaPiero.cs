// ---------------------------------------------------------
// MechaPiero.cs
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
  
public class MechaPiero : EnemyBase
{
    #region define

    private enum eAction
    {
        Wait = 0,
        KnifeAttack,
        RideBall,
        ThornsAttack,
        CannonAttack,
        Max,
    };

    #endregion

    #region variable

    private eAction _nowAction;             // 現在の行動を保持
    private bool _isNext = false;           // 次の行動へ行くか

    // 行動用変数
    [SerializeField] GameObject _knifePrefab = null;
    private NeedleManager _needleManager = null;
    //private AutoRotation _rideBallRot = null;
    private Animator _ballAnimator = null;
    private EffekseerEmitter _laserEffect = null;
    private CapsuleCollider _laserCollider = null;
    private EffekseerEmitter _eyeLight = null;

    #endregion

    #region method

    /// <summary>
    /// 行動ループ処理
    /// </summary>
    private IEnumerator Run()
    {
        _nowAction = eAction.Wait;
        eAction oldAction = _nowAction;
        while (true)
        {
            // 行動ルーチン実行
            _nowAction = SelectAction();
            IEnumerator enumerator = RunAction();
            _isNext = false;

            // 終わるまで待機する
            while (!_isNext)
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
        }
    }

    /// <summary>
    /// 行動選択処理
    /// </summary>
    private eAction SelectAction()
    {
        switch (_nowAction)
        {
            case eAction.Wait:

                // --- DEBUG ---
                if (_isDebug)
                {
                    if (Input.GetKey(KeyCode.H))
                    {
                        return eAction.KnifeAttack;
                    }
                    if (Input.GetKey(KeyCode.J))
                    {
                        return eAction.RideBall;
                    }
                    if (Input.GetKey(KeyCode.K))
                    {
                        return eAction.ThornsAttack;
                    }
                    if (Input.GetKey(KeyCode.L))
                    {
                        return eAction.CannonAttack;
                    }
                }

                return eAction.Wait;
                //return eAction.KnifeAttack;     // 突進攻撃
                //return eAction.RideBall;        // その場でファイアー
                //return eAction.ThornsAttack;    // 回転攻撃
                //return eAction.CannonAttack;    // 回転してファイアー

            case eAction.KnifeAttack:
                return eAction.Wait;

            case eAction.RideBall:
                return eAction.Wait;

            case eAction.ThornsAttack:
                return eAction.CannonAttack;

            case eAction.CannonAttack:
                return eAction.Wait;

            default:
                break;
        }
        return eAction.Wait;
    }

    /// <summary>
    /// 行動開始処理
    /// </summary>
    private IEnumerator RunAction()
    {
        switch (_nowAction)
        {
            case eAction.Wait:
                return StaticCoroutine.Instance.StartStaticCoroutine(Wait());

            case eAction.KnifeAttack:
                return StaticCoroutine.Instance.StartStaticCoroutine(KnifeAttack());

            case eAction.RideBall:
                return StaticCoroutine.Instance.StartStaticCoroutine(RideBall());

            case eAction.ThornsAttack:
                return StaticCoroutine.Instance.StartStaticCoroutine(ThornsAttack());

            case eAction.CannonAttack:
                return StaticCoroutine.Instance.StartStaticCoroutine(CannonAttack());

            default:
                break;
        }
        return null;
    }

    #endregion

    #region action_method

    /// <summary>
    /// 待機処理（Playerのターン）
    /// </summary>
    private IEnumerator Wait()
    {
        _animator.SetBool("BallWalk", false);

        float time = 0.0f;
        float waitTime = _isDebug ? 0.1f : Random.Range(1.0f, 5.0f);
        while (time < waitTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        _isNext = true;
        //Debug.Log("Wait");
    }

    /// <summary>
    /// ナイフ投げ攻撃
    /// </summary>
    private IEnumerator KnifeAttack()
    {
        _animator.SetTrigger("KnifeStart");

        float time = 0.0f;
        while (time < 1.5f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        GameObject obj = Instantiate(_knifePrefab, transform.position, Quaternion.identity);

        _animator.speed = 0.0f;
        time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        _animator.speed = 1.0f;

        while (obj)
        {
            yield return null;
        }

        _isNext = true;
        Debug.Log("KnifeAttack");
    }

    /// <summary>
    /// 玉乗り攻撃
    /// </summary>
    private IEnumerator RideBall()
    {
        // 必要Player情報の取得
        float time = 0.0f;
        float speed = 0.0f;

        Vector3 startRot = transform.eulerAngles;
        transform.LookAt(PlayerManager.Instance.GetVerticalPos(transform.position));
        Vector3 targetRot = transform.eulerAngles;
        transform.eulerAngles = startRot;

        // 無駄な回転量が出ないようにする
        if (targetRot.y - startRot.y > 180.0f)
        {
            targetRot.y -= 360.0f;
        }
        else if (targetRot.y - startRot.y < -180.0f)
        {
            targetRot.y += 360.0f;
        }

        time = 0.0f;
        speed = Mathf.Abs(startRot.y - targetRot.y) / 240.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime / speed;
            if (time > 1.0f) time = 1.0f;
            transform.eulerAngles = Vector3.Lerp(startRot, targetRot, time);
            yield return null;
        }

        _animator.SetTrigger("Pointhing");
        time = 0.0f;
        while(time < 1.15f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        int count = 0;
        while (count < 5)
        {
            // 外壁にぶつかるまで止めないよう設定
            bool isOutRange = false;
            var disposable = new SingleAssignmentDisposable();
            disposable.Disposable = this.OnCollisionEnterAsObservable()
                .Subscribe(col =>
                {
                    if (col.gameObject.tag != "AutoRange")
                        return;
                    
                    isOutRange = true;
                    disposable.Dispose();
                });

            //_rideBallRot.ChangeSpeed(3.0f);
            while (!isOutRange)
            {
                transform.position += transform.forward * 11.5f * Time.deltaTime;
                yield return null;
            }
            //_rideBallRot.ChangeSpeed(1.0f);

            // 敵の方を向く
            startRot = transform.eulerAngles;
            if (count != 4)
            {
                transform.LookAt(PlayerManager.Instance.GetVerticalPos(transform.position));
            }
            else
            {
                Vector3 lookPos = Vector3.zero;
                lookPos.y = transform.position.y;
                transform.LookAt(lookPos);
            }
            targetRot = transform.eulerAngles;
            transform.eulerAngles = startRot;

            if (targetRot.y - startRot.y > 180.0f)
            {
                targetRot.y -= 360.0f;
            }
            else if (targetRot.y - startRot.y < -180.0f)
            {
                targetRot.y += 360.0f;
            }

            time = 0.0f;
            speed = Mathf.Abs(startRot.y - targetRot.y) / 240.0f;
            while (time < 1.0f)
            {
                time += Time.deltaTime / speed;
                if (time > 1.0f) time = 1.0f;
                transform.eulerAngles = Vector3.Lerp(startRot, targetRot, time);
                yield return null;
            }

            count++;
        }

        time = 0.0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = Vector3.zero;
        endPos.y = transform.position.y;
        while(time < 1.0f)
        {
            time += Time.deltaTime / 2.0f;
            transform.position = Vector3.Lerp(startPos, endPos, time);
            yield return null;
        }

        _isNext = true;
        Debug.Log("RideBall");
    }

    /// <summary>
    /// トゲ飛ばし攻撃
    /// </summary>
    private IEnumerator ThornsAttack()
    {
        float time = 0.0f;

        // トゲ飛ばしアニメーション実行
        _ballAnimator.enabled = false;
        _animator.SetTrigger("Roll");
        time = 0.0f;
        while (time < 0.7f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // 実行処理
        float life = 3.0f;
        _needleManager.Run(life);
        //_rideBallRot.ChangeSpeed(0.0f);

        time = 0.0f;
        while(time < life)
        {
            time += Time.deltaTime;
            yield return null;
        }

        _isNext = true;
        Debug.Log("ThornsAttack");
    }

    /// <summary>
    /// キャノン攻撃攻撃
    /// </summary>
    private IEnumerator CannonAttack()
    {
        float time = 0.0f;
        IEnumerator lookEnumerator = StaticCoroutine.Instance.StartStaticCoroutine(LookPlayer(45.0f));

        // 砲台を出す
        _ballAnimator.enabled = true;
        _ballAnimator.SetBool("Laser", true);
        time = 0.0f;
        while(time < 3.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        _animator.SetTrigger("Pointhing");
        time = 0.0f;
        while (time < 1.15f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        _eyeLight.Play();

        // 砲撃実行
        _laserEffect.Play();
        time = 0.0f;
        while(time < 4.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // 当たり判定をON
        _laserCollider.enabled = true;
        time = 0.0f;
        while (time < 2.5f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        _laserCollider.enabled = false;
        StaticCoroutine.Instance.StopCoroutine(lookEnumerator);

        // 終了の煙エフェクト
        GameEffectManager.Instance.Play("LaserEnd", _laserEffect.transform.position);
        time = 0.0f;
        while (time < 3.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // 砲台しまう
        _ballAnimator.SetTrigger("LaserEnd");
        time = 0.0f;
        while (time < 2.33f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // トゲを生やす
        _needleManager.Reload();
        Debug.Log("Reload");
        time = 0.0f;
        while (time < 0.6f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        _ballAnimator.SetBool("Laser", false);

        _isNext = true;
        //_rideBallRot.ChangeSpeed(1.0f);
        Debug.Log("CannonAttack");
    }

    /// <summary>
    /// キャノン攻撃攻撃
    /// </summary>
    private IEnumerator LookPlayer(float magni)
    {
        while (true)
        {
            float time = 0.0f;
            Vector3 startRot = transform.eulerAngles;
            transform.LookAt(PlayerManager.Instance.GetVerticalPos(transform.position));
            Vector3 targetRot = transform.eulerAngles;
            transform.eulerAngles = startRot;

            // 無駄な回転量が出ないようにする
            if (targetRot.y - startRot.y > 180.0f)
            {
                targetRot.y -= 360.0f;
            }
            else if (targetRot.y - startRot.y < -180.0f)
            {
                targetRot.y += 360.0f;
            }

            float speed = Mathf.Abs(startRot.y - targetRot.y) / magni;
            while (time < 1.0f)
            {
                time += Time.deltaTime / speed;
                if (time > 1.0f) time = 1.0f;
                transform.eulerAngles = Vector3.Lerp(startRot, targetRot, time);
                yield return null;
            }
        }
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private new void Awake()
    {
        base.Awake();

        List<GameObject> allChild = gameObject.GetAll();
        //_rideBallRot = allChild.Where(_ => _.name.Contains("RideBall")).FirstOrDefault().GetComponent<AutoRotation>();
        _needleManager = allChild.Where(_ => _.name.Contains("group1")).FirstOrDefault().GetComponent<NeedleManager>();
        _ballAnimator = transform.Find("RideBall").GetComponent<Animator>();
        _laserEffect = _ballAnimator.transform.Find("LaserAttack").GetComponent<EffekseerEmitter>();
        _laserCollider = _ballAnimator.transform.Find("LaserCollision").GetComponent<CapsuleCollider>();
        _laserCollider.enabled = false;
        _eyeLight = allChild.Where(_ => _.name.Contains("Bossitem_pCylinder2")).FirstOrDefault().GetComponent<EffekseerEmitter>();
    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {
        StaticCoroutine.Instance.StartStaticCoroutine(Run());
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private new void Update ()
    {
        base.Update();
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    private void OnCollisionEnter(Collision col)
    {

    }

    #endregion
}  