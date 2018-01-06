// ---------------------------------------------------------
// AutoRotation.cs
// 概要 : 自動で回転する
// 作成者: Shota_Obora
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
  
public class AutoRotation : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    [SerializeField] Vector3 _rotDegreeAmount_Sec = Vector3.zero;
    public Vector3 RotDegreeAmount { get { return _rotDegreeAmount_Sec; } set { _rotDegreeAmount_Sec = value; } }

    #endregion

    #region method

    #endregion

    #region unity_event

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update ()
    {
        transform.Rotate(_rotDegreeAmount_Sec * Time.deltaTime);
    }

    #endregion
}  