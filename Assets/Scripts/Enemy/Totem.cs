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

    [SerializeField] float _oneBlockSize = 1.0f;

    [SerializeField] float _upSpeed = 0.25f;
    [SerializeField] int _childTotemAmount = 5;              // 子分トーテムの数
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
        while (true)
        {
            // 通常攻撃１
            yield return TotemPushUp();

            // 通常攻撃２
            yield return ChildTotemPushUp();

            // 奥義処理
            /*for (int i = 0; i < _childTotemList.Count; i++)
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
            */
            yield return null;
        }
    }

    /// <summary>
    /// 突き上げ攻撃処理
    /// </summary>
    private IEnumerator TotemPushUp()
    {   
        int amount = 0;
        while (amount < 3)
        {
            amount++;
            transform.position = new Vector3(Random.Range(-10.0f, 10.0f), -_oneBlockSize * 3.0f, Random.Range(-10.0f, 10.0f));

            float time = 0.0f;
            while (time < amount)
            {
                transform.position += new Vector3(0, _oneBlockSize * (Time.deltaTime / _upSpeed), 0);
                time += Time.deltaTime / _upSpeed;
                yield return null;
            }
            Vector3 pos = transform.position;
            pos.y = (-_oneBlockSize * 3.0f) + (_oneBlockSize * amount);
            transform.position = pos; 

            // 待機
            EnemyManager.Instance.Active = true;
            yield return new WaitForSeconds(2.0f);
            EnemyManager.Instance.Active = false;

            while (time > 0.0f && amount < 3)
            {
                transform.position -= new Vector3(0, _oneBlockSize * (Time.deltaTime / _upSpeed), 0);
                time -= Time.deltaTime / _upSpeed;
                yield return null;
            }

            // 待機
            yield return new WaitForSeconds(2.0f);
        }
    }

    /// <summary>
    /// 子分と同時突き上げ処理
    /// </summary>
    private IEnumerator ChildTotemPushUp()
    {
        // 子分の突き上げ処理
        for (int i = 0; i < _childTotemAmount; i++)
        {
            StartCoroutine(_childTotemList[i].PushUp(_upSpeed));
            yield return new WaitForSeconds(_upSpeed * 0.75f);
        }

        // 本体突き上げ処理
        transform.position = new Vector3(Random.Range(-10.0f, 10.0f), -_oneBlockSize * 3.0f, Random.Range(-10.0f, 10.0f));

        float time = 0.0f;
        while (time < 3.0f)
        {
            transform.position += new Vector3(0, _oneBlockSize * (Time.deltaTime / _upSpeed), 0);
            time += Time.deltaTime / _upSpeed;
            yield return null;
        }

        // TODO : 補正処理
        Vector3 pos = transform.position;
        pos.y = 0.0f;
        transform.position = pos;
    }

    /// <summary>
    /// 特殊攻撃処理
    /// </summary>
    private IEnumerator SpecialAtack()
    {
        // 子分を潜らせる
        for (int i = 0; i < _childTotemAmount; i++)
        {
            StartCoroutine(_childTotemList[i].Dive(_upSpeed));
        }

        // 本体も潜らせる
        float time = 0.0f;
        while (time < 3.0f)
        {
            transform.position -= new Vector3(0, _oneBlockSize * (Time.deltaTime / _upSpeed), 0);
            time += Time.deltaTime / _upSpeed;
            yield return null;
        }

        // 子分に特殊攻撃の実行を通知
        for (int i = 0; i < _childTotemAmount; i++)
        {
            StartCoroutine(_childTotemList[i].SpecialAtack(_upSpeed));
        }

        // 本体も特殊攻撃を実行
        Vector3 topPos = transform.position;
        topPos.y = 50.0f;
        Vector3 underPos = transform.position;

        // TODO : 続きここから
        while (time > 0.0f)
        {
            transform.position += new Vector3(0, _oneBlockSize * (Time.deltaTime / _upSpeed), 0);
            time -= Time.deltaTime / _upSpeed;
            yield return null;
        }
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