// ---------------------------------------------------------
// Needle.cs
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
  
public class Needle : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    private Vector3 _initPos = Vector3.zero;
    private bool _isAttack = false;

    #endregion

    #region method

    /// <summary>
    /// トゲ飛ばし実行処理
    /// </summary>
    public IEnumerator Run()
    {
        float time = 0.0f;
        _isAttack = true;

        while (time < 5.0f)
        {
            time += Time.deltaTime;
            transform.position += transform.up * 20.0f * Time.deltaTime;
            yield return null;
        }

        _isAttack = false;
        transform.localPosition = Vector3.zero; 
    }

    /// <summary>
    /// トゲ再生成処理
    /// </summary>
    public IEnumerator Reload()
    {
        float time = 0.0f;
        Vector3 nowPos = transform.position;

        while (time < 1.0f)
        {
            time += Time.deltaTime * 1.666f;
            transform.position = Vector3.Lerp(nowPos, _initPos, time);
            yield return null;
        }

        transform.position = _initPos;
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {
        _initPos = transform.position;
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    private void OnTriggerEnter(Collider col)
    {
        if (!_isAttack)
            return;

        if (col.tag == "Player")
        {
            if (!col.GetComponent<Player>().Damage())
                return;

            GameEffectManager.Instance.Play("NeedleHit", transform.position);
        }
    }

    #endregion
}  