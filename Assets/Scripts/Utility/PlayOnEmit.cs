// ---------------------------------------------------------
// PlayOnEmit.cs
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
  
public class PlayOnEmit : MonoBehaviour
{
    #region variable

    [SerializeField] int _emitAmount = 100;

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        GetComponent<ParticleSystem>().Emit(_emitAmount);
    }

    #endregion
}  