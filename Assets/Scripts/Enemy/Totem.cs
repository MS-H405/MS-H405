// ---------------------------------------------------------
// Totem.cs
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
  
public class Totem : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    [SerializeField] float _childTotemSpeed = 1.0f;
    [SerializeField] int _childTotemAmount = 6;              // 子分トーテムの数
    [SerializeField] GameObject _childTotemPrefab = null;    // 子分トーテムのプレハブ

    // 子分トーテムのリスト
    private List<ChildTotem> _childTotemList = new List<ChildTotem>();
   
    #endregion

    #region method

    /// <summary>
    /// 行動ループ処理
    /// </summary>
    private IEnumerator Run()
    {
        // TODO : 自分を隠す処理
        transform.position = new Vector3(0, -100, 0);
        EnemyManager.Instance.Active = false;

        // 通常攻撃処理
        int index = 0;
        while(index < _childTotemList.Count)
        {
            _childTotemList[index].gameObject.SetActive(true);
            StartCoroutine(_childTotemList[index].NormalAtack(_childTotemSpeed));
            index++;

            yield return new WaitForSeconds(_childTotemSpeed * 4.5f);
        }

        // TODO : 自分生える処理
        transform.position = new Vector3(0.0f, 1.5f, 0.0f);
        EnemyManager.Instance.Active = true;

        // 奥義処理
        for (int i = 0; i < _childTotemList.Count; i++)
        {
            StartCoroutine(_childTotemList[i].SpecialAtack());
        }
        // TODO : 自分の処理も
        yield return new WaitForSeconds(10.0f);

        // 子分トーテム消す
        for (int i = 0; i < _childTotemAmount; i++)
        {
            _childTotemList[i].gameObject.SetActive(false);
        }
        // 自分も消す
        transform.position = new Vector3(0.0f, -100.0f, 0.0f);
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        // 子分トーテムの生成処理
        for(int i = 0; i < _childTotemAmount; i++)
        {
            ChildTotem instance = Instantiate(_childTotemPrefab).GetComponent<ChildTotem>();
            instance.gameObject.SetActive(false);
            _childTotemList.Add(instance);
        }

        // 行動開始
        StartCoroutine(Run());
    }

    #endregion
}  