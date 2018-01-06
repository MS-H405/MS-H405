// ---------------------------------------------------------
// PushStart.cs
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
  
public class PushStart : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    private Image _image = null;
    private bool _isIn = true;
    [SerializeField] float _speed_Sec = 0.75f; 

    #endregion

    #region method

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
        if (_isIn)
        {
            _image.color -= new Color(0, 0, 0, 1.0f * (Time.deltaTime / _speed_Sec));

            if (_image.color.a <= 0.0f)
            {
                _isIn = false;
            }
        }
        else
        {
            _image.color += new Color(0, 0, 0, 1.0f * (Time.deltaTime / _speed_Sec));

            if (_image.color.a >= 1.0f)
            {
                _isIn = true;
            }
        }

        // TODO : ゲームパッドのインプットに対応する
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Atack"))
        {
            SoundManager.Instance.PlaySE(SoundManager.eSeValue.UI_PushButton);
            MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.INIT_TO_TOTEM);
            this.enabled = false;
        }
    }

    #endregion
}  