// ---------------------------------------------------------
// PlayerLifeManager.cs
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
  
public class PlayerLifeManager : SingletonMonoBehaviour<PlayerLifeManager>
{
    #region define

    public const int LifeAnimAmount = 6;

    #endregion

    #region variable
    
    private Sprite[] _lifeSpriteArray = new Sprite[LifeAnimAmount];

    // 演出調整用変数
    [SerializeField] float _effectTime = 0.5f;

    #endregion

    #region method

    /// <summary>
    /// Life減少処理 (外部呼出し用)
    /// </summary>
    public void DamageEffect(int index = 0)
    {
        StaticCoroutine.Instance.StartStaticCoroutine(DamageEffectCoroutine(index));
    }

    /// <summary>
    /// life減少コルーチン
    /// </summary>
    private IEnumerator DamageEffectCoroutine(int index)
    {
        if(!transform.GetChild(index))
        {
            yield break;
        }

        float oneAnimTime = _effectTime / LifeAnimAmount;
        Image lifeImage = transform.GetChild(index).GetComponent<Image>();
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.UI_DamageBalloon);

        for (int i = 0; i < LifeAnimAmount; i++)
        {
            float time = 0.0f;
            while (time < 1.0f)
            {
                time += Time.deltaTime / oneAnimTime;

                if (time >= 1.0f)
                {
                    // TODO : MissingError
                    lifeImage.sprite = _lifeSpriteArray[i];
                }

                yield return null;
            }
        }

        // 演出後破棄
        if (index > 0)
        {
            if (transform.gameObject)
            {
                Destroy(transform.gameObject);
            }
        }
        else
        {
            Destroy(transform.GetChild(0).gameObject);
        }
        yield return null;
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        // Lifeのアニメーション用SpriteをLoad
        _lifeSpriteArray = Resources.LoadAll<Sprite>("Sprite/GameUI/LifeAnim");
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