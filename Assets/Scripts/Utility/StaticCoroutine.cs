// ---------------------------------------------------------
// Coroutine.cs
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
  
public class StaticCoroutine : SingletonMonoBehaviour<StaticCoroutine>
{
    #region define

    #endregion

    #region variable

    private List<IEnumerator> _courutineList = new List<IEnumerator>();

    #endregion

    #region method

    /// <summary>
    /// コルーチン実行処理処理
    /// </summary>
    public IEnumerator StartStaticCoroutine(IEnumerator coroutine)
    {
        _courutineList.Add(coroutine);
        Instance.StartCoroutine(coroutine);
        return coroutine;
    }


    /// <summary>
    /// アクティブ変更時処理
    /// </summary>
    public void AllStopCoroutine()
    {
        for (int i = 0; i < _courutineList.Count; i++)
        {
            Instance.StopCoroutine(_courutineList[i]);
        }
    }

    #endregion

    #region unity_event

    /// <summary>
    /// アクティブ変更時処理
    /// </summary>
    private void OnEnable()
    {
        for(int i = 0; i < _courutineList.Count; i++)
        {
            Instance.StartCoroutine(_courutineList[i]);
        }
    }

    #endregion
}  