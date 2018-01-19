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
    #region variable

    [SerializeField] int _index = 0;

    #endregion

    #region unity_event

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {
        float meteorHeight = 15.0f;// - (2.0f * (2 - _index));
        float endHeight = -1.0f - (2.0f * (2 - _index));
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

                if (oldHeight > meteorHeight && transform.position.y <= meteorHeight)
                {
                    meteorEffect.Play();
                }

                if (oldHeight > endHeight && transform.position.y <= endHeight)
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

    /// <summary>
    /// 当たり判定
    /// </summary>
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            col.GetComponent<Player>().Damage();
        }
    }

    #endregion
}  