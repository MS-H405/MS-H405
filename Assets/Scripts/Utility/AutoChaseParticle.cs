// ---------------------------------------------------------
// AutoChaseParticle.cs
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
  
public class AutoChaseParticle : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    [SerializeField] ParticleSystem _particle = null;

    #endregion

    #region method

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {

    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        if (!_particle)
            return;

        var particleElements = new ParticleSystem.Particle[_particle.particleCount];
        if (_particle.GetParticles(particleElements) <= 0)
            return;

        transform.position = particleElements[0].position;
    }

    #endregion
}  