// ---------------------------------------------------------
// GameOver.cs
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
  
public class GameOver : SingletonMonoBehaviour<GameOver>
{
    #region define

    #endregion

    #region variable

    #endregion

    #region method

    /// <summary>
    /// 実行処理
    /// </summary>
    public void Run()
    {
        StaticCoroutine.Instance.StartStaticCoroutine(RunCoroutine());
    }

    private IEnumerator RunCoroutine()
    {
        List<Image> _imageList = new List<Image>();
        for(int i = 0; i < transform.childCount; i++)
        {
            _imageList.Add(transform.GetChild(i).GetComponent<Image>());
        }

        float alpha = 0.0f;
        while(alpha < 1.0f)
        {
            alpha += Time.deltaTime / 3.0f;
            foreach(Image image in _imageList)
            {
                Color color = image.color;
                color.a = alpha;

                if(image.name.Contains("BackGround"))
                {
                    color.a *= 0.75f;
                }

                image.color = color;
            }
            yield return null;
        }
    }

    #endregion

    #region unity_event

    #endregion
}  