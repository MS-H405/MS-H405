// ---------------------------------------------------------
// SpecialIconEffect.cs
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
  
public class SpecialIconEffect : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    [SerializeField] float _speed = 0.5f;

    #endregion

    #region method

    private IEnumerator Run()
    {
        float time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        Image image = GetComponent<Image>();
        bool isUp = true;

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (isUp)
                {
                    image.color += new Color(0, 0, 0, 1) * Time.deltaTime / _speed;

                    if (image.color.a >= 1.0f)
                    {
                        isUp = false;
                    }
                }
                else
                {
                    image.color -= new Color(0, 0, 0, 1) * Time.deltaTime / _speed;

                    if (image.color.a <= 0.0f)
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
        StaticCoroutine.Instance.StartStaticCoroutine(Run());
    }

    #endregion
}  