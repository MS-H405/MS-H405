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

    static private float _commonAtackSpeed = 1.0f;  // 共有する攻撃スピード
    private readonly float MaxAtackSpeed = 2.0f;    // 最大スピード

    private GameObject _targetObj = null;       // 
    [SerializeField] float _atackSpeed = 0.0f;  // 攻撃スピード
    private bool _isReflect = false;            // 反射判定

    private bool _isEnd = false;    // このオブジェクトの生存判定

    #endregion

    #region method

    public IEnumerator Run(GameObject target)
    {
        // 初期化処理
        _targetObj = target;
        _atackSpeed *= _commonAtackSpeed;
        transform.LookAt(_targetObj.transform.position);

        // アクション実行
        var actionCourutine = StartCoroutine(ActionFlow());
        yield return actionCourutine;

        // 破棄処理
        Destroy(gameObject);
    }

    /// <summary>
    /// 攻撃命令から地面に落ちるまでの処理フロー
    /// </summary>
    IEnumerator ActionFlow()
    {
        Vector3 startPos = transform.position; // 投げてから当たるまでの距離を計算するため
        float atackTime = 0.0f;

        // 敵にあたって反射するか地形外に行くまで直進
        while (!_isReflect)
        {
            Vector3 moveAmount = transform.forward * _atackSpeed * Time.deltaTime;
            transform.position += moveAmount;

            if (_isEnd)
            {
                yield break;
            }

            atackTime += Time.deltaTime;
            yield return null;
        }

        // 反射したので適当な位置を落下地として設定
        Vector3 dropPoint = startPos;   // TODO : ランダム算出を実装予定

        // 跳ね返り処理
        BezierCurve.tBez bez = new BezierCurve.tBez();  // 曲線移動のためベジエ曲線を使用
        bez.start = transform.position;
        bez.middle = Vector3.Lerp(transform.position, dropPoint, 0.5f) + new Vector3(0,atackTime * 10.0f,0); // 中間地点を指定
        bez.end = dropPoint;

        while (!_isEnd)
        {
            transform.position = BezierCurve.CulcBez(bez, true);
            bez.time += Time.deltaTime * (1.0f / atackTime);
            _isEnd = 1.0f <= bez.time;
            yield return null;
        }
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        transform.GetChild(0).eulerAngles += new Vector3(720 * Time.deltaTime, 0, 0);
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    private void OnTriggerEnter (Collider col)
    {
        if (col.tag != "Enemy")
            return;

        _isReflect = true;
    }

    #endregion
}  