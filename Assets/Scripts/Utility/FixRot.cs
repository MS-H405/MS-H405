// ---------------------------------------------------------
// FixRot.cs
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
  
public class FixRot : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    Vector3 _oldRot = Vector3.zero;

    #endregion

    #region method

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        _oldRot = transform.eulerAngles;
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
        transform.eulerAngles = _oldRot;
        _oldRot = transform.eulerAngles;
    }

    #endregion
}  