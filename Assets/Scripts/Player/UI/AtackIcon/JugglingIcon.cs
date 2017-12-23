// ---------------------------------------------------------
// JugglingIcon.cs
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
  
public class JugglingIcon : MonoBehaviour
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
        List<Sprite> jugglinSpriteList = new List<Sprite>();
        jugglinSpriteList.Add(Resources.Load<Sprite>("Sprite/GameUI/JugglingThree"));
        jugglinSpriteList.Add(Resources.Load<Sprite>("Sprite/GameUI/JugglingTwo"));
        jugglinSpriteList.Add(Resources.Load<Sprite>("Sprite/GameUI/JugglingOne"));
        jugglinSpriteList.Add(Resources.Load<Sprite>("Sprite/GameUI/JugglingZero"));

        Image image = GetComponent<Image>();
        this.ObserveEveryValueChanged(_ => JugglingAtack.NowJugglingAmount)
            .Subscribe(_ =>
            { 
                image.sprite = jugglinSpriteList[JugglingAtack.NowJugglingAmount];
            });
    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {

    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update ()
    {

    }

    #endregion
}  