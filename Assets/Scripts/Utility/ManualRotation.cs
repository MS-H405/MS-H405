// ---------------------------------------------------------
// ManualRotation.cs
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
  
public class ManualRotation : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    #endregion

    #region method

    /// <summary>
    /// 実行処理
    /// </summary>
    public void Run(Vector3 rotAmount, float rotTime)
    {
        StaticCoroutine.Instance.StartStaticCoroutine(RunCoroutine(rotAmount, rotTime));
    }

    private IEnumerator RunCoroutine(Vector3 amount, float time)
    {
        float nowTime = 0.0f;
        Vector3 initRot = transform.localEulerAngles;
        Vector3 endRot = initRot + amount;
        while(nowTime < 1.0f)
        {
            nowTime += Time.deltaTime / time;
            transform.localEulerAngles = Vector3.Lerp(initRot, endRot, nowTime);
            yield return null;
        }
        transform.localEulerAngles = endRot;
    }

    #endregion

    #region unity_event

    #endregion
}  