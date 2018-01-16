// ---------------------------------------------------------
// NeedleManager.cs
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
  
public class NeedleManager : MonoBehaviour
{
    #region define

    readonly int NeedleAmount = 20;

    #endregion

    #region variable

    [SerializeField] GameObject _needlePrefab = null;
    private List<Needle> _needleList = new List<Needle>();

    #endregion

    #region method

    /// <summary>
    /// 発射処理
    /// </summary>
    public void Run()
    {
        foreach (Needle needle in _needleList)
        {
            StaticCoroutine.Instance.StartCoroutine(needle.Run());
        }
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    public void Reload()
    {
        foreach (Needle needle in _needleList)
        {
            StaticCoroutine.Instance.StartCoroutine(needle.Reload());
        }
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {
        float angle = 360.0f / NeedleAmount / 2.0f;
        for(int i = 0; i < NeedleAmount; i++)
        {
            GameObject obj = Instantiate(_needlePrefab, transform.position, _needlePrefab.transform.rotation);
            obj.transform.SetParent(transform);
            obj.transform.eulerAngles += new Vector3(0, 0, angle);
            obj.transform.position += obj.transform.up * 1.32f;
            angle += 360.0f / NeedleAmount;
            _needleList.Add(obj.GetComponent<Needle>());
        }
    }

    #endregion
}  