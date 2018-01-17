// ---------------------------------------------------------
// BackGroundScroll.cs
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
  
public class BackGroundScroll : MonoBehaviour
{
    #region define

    readonly float StartPos = 1982.0f + 960.0f;
    readonly float EndPos = -1982.0f + 960.0f;

    #endregion

    #region variable

    [SerializeField] float _speed_sec = 200.0f;

    #endregion

    #region method

    #endregion

    #region unity_event
  
    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update ()
    {
        transform.position -= new Vector3(_speed_sec * Time.deltaTime, 0, 0);
        
        if(transform.position.x > EndPos)
            return;

        Vector3 pos = transform.position;
        pos.x = StartPos;
        transform.position = pos;
    }

    #endregion
}  