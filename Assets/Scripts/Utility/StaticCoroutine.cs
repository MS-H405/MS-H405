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

    private List<IEnumerator> _enumeratorList = new List<IEnumerator>();

    #endregion

    #region method

    /// <summary>
    /// コルーチン実行処理処理
    /// </summary>
    public IEnumerator StartStaticCoroutine(IEnumerator enumerator)
    {
        _enumeratorList.Add(enumerator);
        Instance.StartCoroutine(enumerator);
        return enumerator;
    }

    /// <summary>
    /// 登録コルーチン再開処理
    /// </summary>
    public void AllStartCoroutine()
    {
        List<IEnumerator> _removeList = new List<IEnumerator>();
        foreach (IEnumerator enumerator in _enumeratorList)
        {
            if (!enumerator.MoveNext())
            {
                _removeList.Add(enumerator);
                continue;
            }

            Instance.StartCoroutine(enumerator);
        }

        foreach (IEnumerator enumerator in _removeList)
        {
            _enumeratorList.Remove(enumerator);
        }
    }

    /// <summary>
    /// 登録コルーチン停止処理
    /// </summary>
    public void AllStopCoroutine()
    {
        foreach (IEnumerator enumerator in _enumeratorList)
        {
            Instance.StopCoroutine(enumerator);
        }
    }

    #endregion

    #region unity_event

    /// <summary>
    /// アクティブ変更時処理
    /// </summary>
    private void OnEnable()
    {
        AllStartCoroutine();
    }

    private void OnDisable()
    {
        AllStopCoroutine();
    }

    #endregion
}  