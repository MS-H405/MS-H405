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
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.MechaPiero_KnifeForm);
        float time = 0.0f;
        while(time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        foreach (Knife knife in _knifeList)
        {
            knife.gameObject.SetActive(true);
        }

        time = 0.0f;
        while (time < 1.0f)
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

    /// <summary>
    /// 終了処理
    /// </summary>
    private IEnumerator End()
    {
        float time = 0.0f;
        while(time < 1.5f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // 終了処理
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.MechaPiero_KnifeForm);
        foreach (Knife knife in _knifeList)
        {
            StaticCoroutine.Instance.StartStaticCoroutine(knife.End());
        }
        
        time = 0.0f;
        while(time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
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

        // 終了判定監視
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (_knifeList.Where(knife => knife.IsEnd).Count() < _knifeList.Count())
                    return;

                StaticCoroutine.Instance.StartStaticCoroutine(End());
                disposable.Dispose();
            });
    }

    private void Start()
    {
        transform.eulerAngles = EnemyManager.Instance.BossEnemy.transform.eulerAngles + new Vector3(0, 180, 0);
    }

    #endregion
}  