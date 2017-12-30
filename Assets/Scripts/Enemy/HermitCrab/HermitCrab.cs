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
        RightAtack,
        LeftAtack,
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
    private float _nearTime = 0.0f;         // Playerが近くにいる時の継続時間

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
            //_nowAction = SelectAction();
            IEnumerator enumerator = RunAction();

            // 終わるまで待機する
            oldAction = _nowAction;
            while (_nowAction == oldAction)
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
        if(_nowAction == eAction.Wait)
        {
            return eAction.Assault;
        }
        else
        {
            return eAction.Wait;
        }
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

            case eAction.RightAtack:
            //return StaticCoroutine.Instance.StartStaticCoroutine();

            case eAction.LeftAtack:
            //return StaticCoroutine.Instance.StartStaticCoroutine();

            case eAction.RollAtack:
            //return StaticCoroutine.Instance.StartStaticCoroutine();

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

    private IEnumerator Wait()
    {
        _animator.SetBool("Walk", false);

        float time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime / 1.0f;
            yield return null;
        }

        _nowAction = eAction.Assault;
    }

    private IEnumerator Assault()
    {
        float time = 0.0f;
        float speed = 0.0f;
        _animator.SetBool("Walk", true);

        Vector3 startRot = transform.eulerAngles;
        transform.LookAt(PlayerManager.Instance.GetVerticalPos(transform.position));
        Vector3 targetRot = transform.eulerAngles;
        transform.eulerAngles = startRot;

        // TODO : 無駄な回転量が出ないようにする

        speed = Mathf.Abs(startRot.y - targetRot.y) / 180.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime / speed;
            if (time > 1.0f) time = 1.0f;
            transform.eulerAngles = Vector3.Lerp(startRot, targetRot, time);
            yield return null;
        }
        
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

        _nowAction = eAction.Wait;
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private new void Awake()
    {
        base.Awake();


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