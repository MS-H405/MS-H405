// ---------------------------------------------------------
// FixHeight.cs
// 概要 : 影など、地面に固定して配置したいオブジェクトにアタッチ
// 作成者: Shota_Obora
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
  
public class FixHeight : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    #endregion

    #region method

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {

    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {

    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update ()
    {
        Vector3 pos = transform.position;
        pos.y = 0.0f;
        transform.position = pos;
    }

    #endregion
}  