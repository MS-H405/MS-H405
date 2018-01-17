// ---------------------------------------------------------
// MeteorHit.cs
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
  
public class MeteorHit : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    #endregion

    #region method

    #endregion

    #region unity_event
   
    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {
        float oldHeight = transform.position.y;
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        EffekseerEmitter meteorEffect = GetComponentInChildren<EffekseerEmitter>();
        ShakeCamera shakeCamera = Camera.main.GetComponent<ShakeCamera>();
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (!col.isTrigger)
                {
                    oldHeight = transform.position.y;
                    return;
                }

                if (oldHeight > 15.0f && transform.position.y <= 15.0f)
                {
                    meteorEffect.Play();
                }

                if (oldHeight > -1.0f && transform.position.y <= -1.0f)
                {
                    Vector3 effectPos = transform.position;
                    effectPos.y = 0.01f;
                    meteorEffect.Stop();
                    GameEffectManager.Instance.Play("Totem_MeteorHit", effectPos);
                    SoundManager.Instance.PlaySE(SoundManager.eSeValue.Totem_Impact);
                    shakeCamera.Shake();
                }

                oldHeight = transform.position.y;
            });
    }

    #endregion
}  