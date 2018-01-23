﻿// ---------------------------------------------------------
// IconEffect.cs
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
  
public class IconEffect : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    private Image _image = null;
    [SerializeField] float _speed = 0.5f;

    private Sprite _normalSprite = null;
    private Sprite _specialSprite = null;

    #endregion

    #region method

    /// <summary>
    /// 実行処理
    /// </summary>
    public void Run(bool isSpecial)
    {
        if (isSpecial)
        {
            StaticCoroutine.Instance.StartStaticCoroutine(RunSpecial());
        }
        else
        {
            StaticCoroutine.Instance.StartStaticCoroutine(RunNormal());
        }
    }

    /// <summary>
    /// 通常エフェクト実行処理
    /// </summary>
    private IEnumerator RunNormal()
    {
        bool isUp = true;
        _image.sprite = _normalSprite;
        float amount = 0.2f;

        while (true)
        {
            if (isUp)
            {
                _image.color += new Color(0, 0, 0, 1) * Time.deltaTime / _speed;
                transform.eulerAngles += new Vector3(0, 0, 180 * Time.deltaTime / _speed);
                transform.localScale += new Vector3(amount, amount, 0.0f) * Time.deltaTime / _speed;

                if (_image.color.a >= 1.0f)
                {
                    isUp = false;
                }

                yield return null;
            }
            else
            {
                _image.color -= new Color(0, 0, 0, 1) * Time.deltaTime / _speed;
                transform.eulerAngles += new Vector3(0, 0, 180 * Time.deltaTime / _speed);
                transform.localScale -= new Vector3(amount, amount, 0.0f) * Time.deltaTime / _speed;

                if (_image.color.a <= 0.0f)
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    transform.localScale = new Vector3(2.25f, 2.25f, 0.0f);
                    yield break;
                }

                yield return null;
            }
        }
    }

    /// <summary>
    /// 特殊エフェクト実行処理
    /// </summary>
    private IEnumerator RunSpecial()
    {
        float time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        
        bool isUp = true;
        _image.sprite = _specialSprite;

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (isUp)
                {
                    _image.color += new Color(0, 0, 0, 1) * Time.deltaTime / _speed;

                    if (_image.color.a >= 1.0f)
                    {
                        isUp = false;
                    }
                }
                else
                {
                    _image.color -= new Color(0, 0, 0, 1) * Time.deltaTime / _speed;

                    if (_image.color.a <= 0.0f)
                    {
                        isUp = true;
                    }

                }
            });
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        _image = GetComponent<Image>();
        _normalSprite = Resources.Load<Sprite>("Sprite/GameUI/Icon_Select");
        _specialSprite = Resources.Load<Sprite>("Sprite/GameUI/Special_Gold_Effect");
    }

    #endregion
}