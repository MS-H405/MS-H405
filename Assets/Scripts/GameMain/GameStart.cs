// ---------------------------------------------------------
// GameStart.cs
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
  
public class GameStart : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    #endregion

    #region method

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        Time.timeScale = 0.0f;
    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {
        float time = 0.0f;
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (MovieManager.Instance.GetisMovideFade())
                    return;

                time += Time.unscaledDeltaTime;

                if (time < 2.0f)
                    return;

                Time.timeScale = 1.0f;
                Destroy(gameObject);
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