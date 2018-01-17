// ---------------------------------------------------------
// AutoParticleDestoryer.cs
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
  
public class AutoParticleDestoryer : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    [SerializeField] GameObject _destroyObj = null;

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
        ParticleSystem particle = GetComponent<ParticleSystem>();
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (particle.isPlaying)
                    return;

                if (_destroyObj)
                {
                    Destroy(_destroyObj);
                }
                else
                {
                    Transform trans = transform;
                    while (trans.parent)
                    {
                        trans = trans.parent;
                    }
                    Destroy(trans.gameObject);
                }
            });
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update ()
    {

    }

    #endregion
}  