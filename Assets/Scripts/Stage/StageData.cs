// ---------------------------------------------------------
// StageData.cs
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
  
public class StageData : SingletonMonoBehaviour<StageData>
{
    #region define

    static public readonly float FieldSize = 13.5f; 

    #endregion

    #region variable

    [SerializeField] int _stageNumber = 0;
    public int StageNumber { get { return _stageNumber; } private set { _stageNumber = value; } }

    #endregion
}  