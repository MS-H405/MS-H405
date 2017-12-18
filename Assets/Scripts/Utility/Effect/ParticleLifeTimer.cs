// ---------------------------------------------------------
// ParticleLifeTimer.cs
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
  
public class ParticleLifeTimer : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    private ParticleSystem _particle = null;
    private float _initLifeTime = 0.0f;

    #endregion

    #region method

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void ChangeLifeTime(float magnification)
    {
        _particle.startLifetime = _initLifeTime * magnification;
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        _particle = GetComponent<ParticleSystem>();
        _initLifeTime = _particle.startLifetime;
    }

    #endregion
}