// ---------------------------------------------------------
// GameStartAutoScaler.cs
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
  
public class GameStartAutoScaler : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    [SerializeField] bool _isParent = false;
    [SerializeField] float _speed_sec = 0.5f;

    private Vector3 _minScale = Vector3.zero;
    private Vector3 _maxScale = Vector3.zero;

    #endregion

    #region method

    #endregion

    #region unity_event

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    { 
        float time = 0.0f;

        bool isAlpha = false;
        Image image = GetComponent<Image>();

        _maxScale = transform.localScale;
        transform.localScale = Vector3.zero;

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (!isAlpha)
                {
                    time += GameStart.GameStartDeltaTime / _speed_sec;
                    isAlpha = time > 1.0f;
                    transform.localScale += (_maxScale - _minScale) * (GameStart.GameStartDeltaTime / _speed_sec);

                    if(isAlpha)
                    {
                        SoundManager.Instance.PlaySE(SoundManager.eSeValue.UI_ShowTime);
                    }
                    return;
                }
                else if (time < 3.0f)
                {
                    time += GameStart.GameStartDeltaTime / _speed_sec;
                    return;
                }

                //transform.localScale += (_maxScale - _minScale) * (GameStart.GameStartDeltaTime / _speed_sec);
                image.color -= new Color(0.0f, 0.0f, 0.0f, 1.0f * (GameStart.GameStartDeltaTime / _speed_sec));

                if (image.color.a <= 0.0f)
                    enabled = false;
            });

        // 親じゃなければ終了
        if (!_isParent)
            return;

        List<Animator> animatorList = new List<Animator>();

        for (int i = 1; i < transform.parent.childCount; i++)
        {
            animatorList.Add(transform.parent.GetChild(i).GetComponent<Animator>());
        }

        this.ObserveEveryValueChanged(_ => time < 1.0f)
            .Subscribe(_ =>
            {
                if (time < 1.0f)
                    return;

                foreach (Animator anim in animatorList)
                {
                    anim.SetTrigger("Run");
                }
            });
    }

    #endregion
}  