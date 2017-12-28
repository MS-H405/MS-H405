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

    private Image _image = null;
    private List<Sprite> _jugglingSpriteList = new List<Sprite>();

    #endregion

    #region method

    /// <summary>
    /// 通常のアイコンかを判定
    /// </summary>
    private bool CheckJugglingSprite()
    {
        foreach(Sprite sp in _jugglingSpriteList)
        {
            if (_image.sprite.name != sp.name)
                continue;

            return true;
        }

        return false;
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        _jugglingSpriteList.Add(Resources.Load<Sprite>("Sprite/GameUI/JugglingThree"));
        _jugglingSpriteList.Add(Resources.Load<Sprite>("Sprite/GameUI/JugglingTwo"));
        _jugglingSpriteList.Add(Resources.Load<Sprite>("Sprite/GameUI/JugglingOne"));
        _jugglingSpriteList.Add(Resources.Load<Sprite>("Sprite/GameUI/JugglingZero"));

        _image = GetComponent<Image>();
        this.ObserveEveryValueChanged(_ => JugglingAtack.NowJugglingAmount)
            .Subscribe(_ =>
            {
                if (!CheckJugglingSprite())
                    return;

                _image.sprite = _jugglingSpriteList[JugglingAtack.NowJugglingAmount];
            });

        this.ObserveEveryValueChanged(_ => CheckJugglingSprite())
            .Subscribe(_ =>
            {
                bool active = CheckJugglingSprite();
                for(int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(active);
                }
            });
    }

    #endregion
}  