// ---------------------------------------------------------
// EffekseerTest.cs
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
  
public class EffekseerTest : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    private EffekseerEmitter _effekseerEmitter = null;

    #endregion

    #region method

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        _effekseerEmitter = GetComponent<EffekseerEmitter>();
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
        if(Input.GetKeyDown(KeyCode.M))
        {
            _effekseerEmitter.Play();
        }
    }

    #endregion
}  