// ---------------------------------------------------------
// MechaPieroLaser.cs
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
  
public class MechaPieroLaser : MonoBehaviour
{
    #region unity_event
   
    /// <summary>
    /// 当たり判定処理
    /// </summary>
    private void OnTriggerEnter (Collider col)
    {
        if (col.tag != "Player")
            return;

        col.GetComponent<Player>().Damage();
    }

    #endregion
}  