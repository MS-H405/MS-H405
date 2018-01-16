// ---------------------------------------------------------
// KnifeManager.cs
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
  
public class KnifeManager : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    List<Knife> _knifeList = new List<Knife>();

    #endregion

    #region method

    /// <summary>
    /// 実行処理
    /// </summary>
    private IEnumerator Run()
    {
        float time = 0.0f;
        while(time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        int index = 0;
        while (index < _knifeList.Count)
        {
            time = 0.0f;
            while (time < 0.4f)
            {
                time += Time.deltaTime;
                yield return null;
            }
            StaticCoroutine.Instance.StartStaticCoroutine(_knifeList[index].Run());
            index++;
        }
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _knifeList.Add(transform.GetChild(i).GetComponent<Knife>());
        }
        StaticCoroutine.Instance.StartStaticCoroutine(Run());
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        if (transform.childCount > 0)
            return;

        Destroy(gameObject);
    }

    #endregion
}  