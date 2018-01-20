// ---------------------------------------------------------
// BagpipeIcon.cs
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
  
public class BagpipeIcon : MonoBehaviour
{
    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        Bagpipe bagpipe = PlayerManager.Instance.Player.GetComponent<Bagpipe>();
        Image image = GetComponent<Image>();
        Sprite onSprite = image.sprite;
        Sprite offSprite = Resources.Load<Sprite>("Sprite/GameUI/BagpipeFireBlack");

        this.ObserveEveryValueChanged(_ => bagpipe.IsOn)
            .Subscribe(_ =>
            {
                if(bagpipe.IsOn)
                {
                    image.sprite = onSprite;
                }
                else
                {
                    image.sprite = offSprite;
                }
            });
    }

    #endregion
}  