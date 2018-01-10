// ---------------------------------------------------------
// BgmStarter.cs
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
  
public class BgmStarter_Tatsuo : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    [SerializeField] MovieSoundManager.eBgmValue _bgmValue;

    #endregion

    #region method

    #endregion

    #region unity_event

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {
        StartCoroutine(PlaySound());
    }

    private IEnumerator PlaySound()
    {
        yield return new WaitWhile(() => Time.timeScale <= 0.0f);
        MovieSoundManager.Instance.PlayBGM(_bgmValue);
    }

    /// <summary>
    /// 必殺後のBGM再生処理
    /// </summary>
    /*private void OnEnable()
    {
        SoundManager.Instance.PlayBGM(_bgmValue);
    }*/

    #endregion
}