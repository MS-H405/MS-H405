// ---------------------------------------------------------
// FireAtack.cs
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
  
public class FireAtack : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    #endregion

    #region method

    #endregion

    #region unity_event

    /// <summary>
    /// 当たり判定処理
    /// </summary>
    private void OnParticleCollision(GameObject obj)
    {
        if (obj.gameObject.tag == "Player")
        {
            obj.gameObject.GetComponent<Player>().Damage();
        }
    }

    #endregion
}  