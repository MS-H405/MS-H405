// ---------------------------------------------------------
// Airplane.cs
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
  
public class Airplane : MonoBehaviour
{
    #region variable

    [SerializeField] Transform _target = null;
    [SerializeField] float _speed_sec = 1.0f;
    [SerializeField] MovieManager.MOVIE_SCENE nextScene;

    #endregion

    #region unity_event
    
    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {
        if(!_target)
        {
            Debug.LogError("行き先がありません");
            return;
        }

        SoundManager.Instance.PlaySE(SoundManager.eSeValue.UI_hikouki);

        Vector3 initPos = transform.position;
        float time = 0.0f;
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (MovieManager.Instance.GetisMovideFade())
                    return;

                time += Time.deltaTime / _speed_sec;
                transform.position = Vector3.Lerp(initPos, _target.position, time);

                if (time < 1.0f)
                    return;

                // 到着
                MovieManager.Instance.FadeStart(nextScene);
            });

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if(time < 0.5f)
                {
                    transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one, time * 2.0f);
                }
                else
                {
                    transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.5f, (time - 0.5f) * 2.0f);
                }
            });
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update ()
    {
        // スキップ
        if (Input.GetButtonDown("Cancel"))
        {
            MovieManager.Instance.FadeStart(nextScene);
        }
    }

    #endregion
}  