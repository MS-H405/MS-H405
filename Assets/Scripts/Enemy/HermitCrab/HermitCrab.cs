// ---------------------------------------------------------
// HermitCrab.cs
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
  
public class HermitCrab : EnemyBase
{
    #region define

    private enum eAction
    {
        Wait = 0,
        Assault,
        RightAttack,
        LeftAttack,
        RollAtack,
        ChargeFire,
        RollFire,
        Max,
    };

    readonly float NearRange = 2.0f;        // Playerを近くと判定する距離
    readonly float WeakPercentage = 0.25f;  //

    #endregion

    #region variable

    private eAction _nowAction;             // 現在の行動を保持
    private bool _isNext = false;           // 次の行動へ行くか

    private float _nearTime = 0.0f;         // Playerが近くにいる時の継続時間

    // 行動用変数
    private BoxCollider _leftScissors  = null;
    private BoxCollider _rightScissors = null;

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
                return (eAction)Random.Range(2,5);
                //return eAction.Assault;

            case eAction.Assault:
                return eAction.Wait;

            case eAction.RightAttack:
                return eAction.Wait;

            case eAction.LeftAttack:
                return eAction.Wait;

            case eAction.RollAtack:
                return eAction.Wait;

            case eAction.ChargeFire:
                return eAction.Wait;

            case eAction.RollFire:
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

            case eAction.Assault:
                return StaticCoroutine.Instance.StartStaticCoroutine(Assault());

            case eAction.RightAttack:
            case eAction.LeftAttack:
            case eAction.RollAtack:
                return StaticCoroutine.Instance.StartStaticCoroutine(ScissorAttack(_nowAction));

            case eAction.ChargeFire:
            //return StaticCoroutine.Instance.StartStaticCoroutine();

            case eAction.RollFire:
            //return StaticCoroutine.Instance.StartStaticCoroutine();

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
        _animator.SetBool("Walk", false);

        float time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime / 5.0f;
            yield return null;
        }

        _isNext = true;
        Debug.Log("Wait End");
    }

    /// <summary>
    /// 突進攻撃処理
    /// ※突進が微妙なので、とりあえずPlayerの方を向くだけの行動になっている※
    /// </summary>
    private IEnumerator Assault()
    {
        float time = 0.0f;
        float speed = 0.0f;
        _animator.SetBool("Walk", true);
        _animator.speed = 1.5f;

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

        speed = Mathf.Abs(startRot.y - targetRot.y) / 45.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime / speed;
            if (time > 1.0f) time = 1.0f;
            transform.eulerAngles = Vector3.Lerp(startRot, targetRot, time);
            yield return null;
        }

        _isNext = true;
        _animator.speed = 1.0f;
        yield break;
        // TODO : 突進が微妙なので一旦なしにしている
        
        time = 0.0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = PlayerManager.Instance.Player.transform.position;
        speed = Vector3.Distance(startPos, targetPos) / 10.0f;
        _animator.speed = 2.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime / speed;
            if (time > 1.0f) time = 1.0f;
            transform.position = Vector3.Lerp(startPos, targetPos, time);
            yield return null;
        }

        _animator.SetBool("Walk", false);
        _animator.speed = 1.0f;

        _isNext = true;
    }

    /// <summary>
    /// 処理
    /// </summary>
    private IEnumerator ScissorAttack(eAction action)
    {
        switch(action)
        {
            case eAction.RightAttack:
                _rightScissors.enabled = true;
                _animator.SetTrigger("RightAttack");
                Debug.Log("RightAttack");
                break;

            case eAction.LeftAttack:
                _leftScissors.enabled = true;
                _animator.SetTrigger("LeftAttack");
                Debug.Log("LeftAttack");
                break;

            case eAction.RollAtack:
                IsInvincible = true;
                _rightScissors.enabled = true;
                _animator.SetTrigger("RollAttack");
                Debug.Log("RollAttack");
                break;

            default:
                break;
        }

        AnimatorStateInfo animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitWhile(() =>
        {
            animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log("a");
            return animStateInfo.IsName("Base.Idle") || animStateInfo.IsName("Base.Walk");
        });
        yield return new WaitWhile(() =>
        {
            animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log("b");
            return !animStateInfo.IsName("Base.Idle");
        });

        _isNext = true;
        IsInvincible = false;
        _leftScissors.enabled = false;
        _rightScissors.enabled = false;
        Debug.Log(action + " : End");
    }


    /// <summary>
    /// 処理
    /// </summary>
    private IEnumerator ChargeFire()
    {
        yield return null;
    }

    /// <summary>
    /// 処理
    /// </summary>
    private IEnumerator RollFire()
    {
        yield return null;
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private new void Awake()
    {
        base.Awake();

        // はさみのColliderを取得
        List<GameObject> allChild = GetAllChildren.GetAll(gameObject);
        _leftScissors = allChild.Where(_ => _.name == "LeftScissors").FirstOrDefault().GetComponent<BoxCollider>();
        _leftScissors.enabled = false;
        _rightScissors = allChild.Where(_ => _.name == "RightScissors").FirstOrDefault().GetComponent<BoxCollider>();
        _rightScissors.enabled = false;
    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {
        // 行動開始
        StaticCoroutine.Instance.StartStaticCoroutine(Run());
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update ()
    {
        float distance = Vector3.Distance(transform.position, PlayerManager.Instance.Player.transform.position);
        if(distance <= NearRange)
        {
            _nearTime += Time.deltaTime;
        }
        else
        {
            _nearTime = 0.0f;
        }
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            col.GetComponent<Player>().Damage();
        }
    }

    #endregion

    /// <summary>
    /// 攻撃パターンは突進、ハサミ、ハサミ２、ファイヤー、ファイヤー２
    /// 
    /// ハサミは横振り攻撃
    /// 
    /// ハサミ２は殻にこもってハサミだけ出して１回転なぎ払い
    /// 
    /// ハサミ２はプレイヤーが近距離にずっといる場合に行う
    /// 
    /// ファイヤーはその場で殻にこもって全方位ファイヤー
    /// 
    /// ファイヤー２はこもってコマみたいにフィールドを回ってファイヤー
    /// 
    /// ファイヤー２はボスHPが一定値まで減ったら行う
    /// 
    /// 殻にこもっている間はプレイヤーの攻撃を受けない
    /// 
    /// 足はあんまり早くないけどファイヤー２の動きは早いイメージ
    /// </summary>
}