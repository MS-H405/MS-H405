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

    [SerializeField] float _height = 0.0f;

    #endregion

    #region method

    #endregion

    #region unity_event

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update ()
    {
        Vector3 pos = transform.position;
        pos.y = _height;
        transform.position = pos;
    }

    #endregion
}  