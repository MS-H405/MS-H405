// ---------------------------------------------------------
// FixHeightParticle.cs
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
  
public class FixHeightParticle : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    [SerializeField] ParticleSystem _particle = null;
    [SerializeField] Vector3 _eulerAngles = Vector3.zero;

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
    private void Start ()
    {

    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        if (!_particle)
        {
            transform.position = new Vector3(0, -10000, 0);
            return;
        }

        var particleElements = new ParticleSystem.Particle[_particle.particleCount];
        if (_particle.GetParticles(particleElements) <= 0)
        {
            transform.position = new Vector3(0, -10000, 0);
            return;
        }

        Vector3 pos = particleElements[0].position;
        pos.y = 0.0f;
        transform.position = pos;
        transform.eulerAngles = _eulerAngles;
    }

    #endregion
}  