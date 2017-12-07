// ---------------------------------------------------------
// GameEffectManager.cs
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
  
public class GameEffectManager : SingletonMonoBehaviour<GameEffectManager>
{
    #region define

    #endregion

    #region variable

    // Game中に再生するEffekseerEmitterのList
    private List<EffekseerEmitter> _gameEffectList = new List<EffekseerEmitter>();

    #endregion

    #region method

    /// <summary>
    /// エフェクト再生処理
    /// </summary>
    public void Play(string name, Vector3 pos)
    {
        _gameEffectList.Where(_ => name == _.name).FirstOrDefault().Play(pos);
    }

    /// <summary>
    /// エフェクト再生処理
    /// 座標を自分で指定するが、高さは0とする　
    /// 地面から発生するエフェクトで使用
    /// </summary>
    public void PlayOnHeightZero(string name, Vector3 pos)
    {
        pos.y = 0.0f;
        _gameEffectList.Where(_ => name == _.name).FirstOrDefault().Play(pos);

    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        // Effectを取得
        for(int i = 0; i < transform.childCount; i++)
        {
            _gameEffectList.Add(transform.GetChild(i).GetComponent<EffekseerEmitter>());
        }
    }

    #endregion
}  