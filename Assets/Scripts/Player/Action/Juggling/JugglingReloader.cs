// ---------------------------------------------------------
// JugglingReloader.cs
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
  
public class JugglingReloader : SingletonMonoBehaviour<JugglingReloader>
{
    #region define

    #endregion

    #region variable

    private int _reloadAmount = 0;
    public float _reloadTime { get; private set; }

    [SerializeField] Slider[] _sliderArray = new Slider[2];

    #endregion

    #region method

    /// <summary>
    /// リロード依頼処理
    /// </summary>
    public void Reload()
    {
        _reloadAmount++;
    }

    /// <summary>
    /// リロードバー更新処理
    /// </summary>
    private void ReloadBarUpdate(float value)
    {
        foreach(Slider slider in _sliderArray)
        {
            slider.value = value;
        }
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {
        _reloadTime = 0.0f;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        if (_reloadAmount <= 0)
        {
            ReloadBarUpdate(4.0f);
            return;
        }

        _reloadTime += Time.deltaTime;
        ReloadBarUpdate(_reloadTime);

        if (_reloadTime < 4.0f)
            return;

        JugglingAtack.NowJugglingAmount--;
        _reloadAmount--;
        _reloadTime = 0.0f;
    }

    #endregion
}  