// ---------------------------------------------------------
// JugglingAirEffect.cs
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
  
public class JugglingAirEffect : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    #endregion

    #region method

    /// <summary>
    /// 終了処理
    /// </summary>
    public void End()
    {
        StaticCoroutine.Instance.StartStaticCoroutine(EndRoutine());
    }

    private IEnumerator EndRoutine()
    {
        ParticleSystem particle = GetComponent<ParticleSystem>();
        transform.parent = null;
        particle.loop = false;

        ParticleSystem.Particle[] elements = null;

        while(particle.GetParticles(elements) > 0)
        {
            yield return null;
        }

        Destroy(gameObject);
    }

    #endregion
}  