// ---------------------------------------------------------
// AutoSpriteFade.cs
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
  
public class AutoSpriteFade : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    [SerializeField] float _speed = 0.5f;

    #endregion

    #region method

    #endregion

    #region unity_event
    
    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {
        Image image = GetComponent<Image>();
        float time = 0.0f;
        var fadeDisposable = new SingleAssignmentDisposable();
        fadeDisposable.Disposable = this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                time += Time.deltaTime / _speed;
                image.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, time);

                if(time >= 1.0f)
                {
                    image.color = Color.white;
                    fadeDisposable.Dispose();
                    enabled = false;
                }
            });
    }

    #endregion
}  