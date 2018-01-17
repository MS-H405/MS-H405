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

    private Vector3 _initRotDegreeAmount = Vector3.zero;

    #endregion

    #region method

    /// <summary>
    /// 速度変更処理
    /// </summary>
    public void ChangeSpeed(float speed)
    {
        _rotDegreeAmount_Sec = _initRotDegreeAmount * speed;
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        _initRotDegreeAmount = _rotDegreeAmount_Sec;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        transform.Rotate(_rotDegreeAmount_Sec * Time.deltaTime);
    }

    #endregion
}  