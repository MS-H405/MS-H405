// ---------------------------------------------------------
// LifeDanger.cs
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
  
public class LifeDanger : MonoBehaviour
{
    #region variable
    
    private Image _image = null;
    private bool _isAdd = true;
    private float _maxAlpha = 0.0f;
    private float _speed = 0.0f;

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update ()
    {
        // ここでライフに応じてスピードと最大透過度を調整
        float per = PlayerManager.Instance.Player.LifePercentage;

        if (per > 0.5f || per <= 0.0f)
            return;

        per *= 1.5f;
        _maxAlpha = 1.0f - per;
        _speed = per;

        if (_isAdd)
        {
            _image.color += new Color(0, 0, 0, _maxAlpha * (Time.deltaTime / _speed));

            if (_image.color.a >= _maxAlpha)
            {
                _isAdd = false;
                Color color = _image.color;
                color.a = _maxAlpha;
                _image.color = color;
            }
        }
        else
        {
            _image.color -= new Color(0, 0, 0, _maxAlpha * (Time.deltaTime / _speed));

            if (_image.color.a <= 0.0f)
            {
                _isAdd = true;
                Color color = _image.color;
                color.a = 0.0f;
                _image.color = color;
            }
        }
    }

    #endregion
}  