// ---------------------------------------------------------
// LRpush.cs
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
  
public class LRpush : MonoBehaviour
{
    #region variable

    [SerializeField] bool _isRight = true;

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        Image image = GetComponent<Image>();
        Sprite onSprite = null;
        Sprite offSprite = image.sprite;

        if (_isRight)
        {
            onSprite = Resources.Load<Sprite>("Sprite/GameUI/RightButton2_On");

            this.ObserveEveryValueChanged(_ => Input.GetButton("Right"))
                .Subscribe(_ =>
                {
                    if (Input.GetButtonDown("Right"))
                    {
                        image.sprite = onSprite;
                    }
                    else
                    {
                        image.sprite = offSprite;
                    }
                });
        }
        else
        {
            onSprite = Resources.Load<Sprite>("Sprite/GameUI/LeftButton2_On");

            this.ObserveEveryValueChanged(_ => Input.GetButton("Left"))
                .Subscribe(_ =>
                {
                    if (Input.GetButtonDown("Left"))
                    {
                        image.sprite = onSprite;
                    }
                    else
                    {
                        image.sprite = offSprite;
                    }
                });
        }
    }

    #endregion
}  