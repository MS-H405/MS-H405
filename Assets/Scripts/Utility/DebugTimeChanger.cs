// ---------------------------------------------------------
// DebugTimeChanger.cs
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
  
public class DebugTimeChanger : MonoBehaviour
{
    #region unity_event

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Time.timeScale = 1.0f;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Time.timeScale = 1.5f;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Time.timeScale = 2.0f;
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1.0f;
    }

    #endregion
}  