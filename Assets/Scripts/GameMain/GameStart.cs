﻿// ---------------------------------------------------------
// GameStart.cs
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
  
public class GameStart : MonoBehaviour
{
    #region define

    private const float Rimit = 1.0f / 10.0f;
    public static float GameStartDeltaTime = 0.0f;

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
        Time.timeScale = 0.0f;
    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {
        // PlayerとEnemyを停止
        Time.timeScale = 1.0f;
        StaticCoroutine.Instance.AllStopCoroutine();
        MonoBehaviour[] playerMonos = PlayerManager.Instance.Player.GetComponentsInChildren<MonoBehaviour>();
        playerMonos = playerMonos.Where(_ => _.enabled).ToArray();
        foreach (MonoBehaviour playerMono in playerMonos)
        {
            playerMono.enabled = false;
        }
        MonoBehaviour[] enemyMonos = EnemyManager.Instance.BossEnemy.GetComponentsInChildren<MonoBehaviour>();
        enemyMonos = enemyMonos.Where(_ => _.enabled).ToArray();
        foreach (MonoBehaviour enemyMono in enemyMonos)
        {
            enemyMono.enabled = false;
        }
        MonoBehaviour[] cameraMonos = Camera.main.GetComponentsInChildren<MonoBehaviour>();
        cameraMonos = cameraMonos.Where(_ => _.enabled).ToArray();
        foreach (MonoBehaviour cameraMono in cameraMonos)
        {
            cameraMono.enabled = false;
        }

        this.ObserveEveryValueChanged(_ => MovieManager.Instance.GetisMovideFade() || Time.unscaledDeltaTime > Rimit)
            .Where(_ => !(MovieManager.Instance.GetisMovideFade() || Time.unscaledDeltaTime > Rimit))
            .Subscribe(_ =>
            {
                GetComponent<EffekseerEmitter>().Play();
            });

        float time = 0.0f;
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (MovieManager.Instance.GetisMovideFade() || Time.unscaledDeltaTime > Rimit)
                    return;

                time += Time.unscaledDeltaTime;
                GameStartDeltaTime = Time.unscaledDeltaTime;

                if (time < 5.0f)
                    return;
                
                StaticCoroutine.Instance.AllStartCoroutine();
                foreach (MonoBehaviour playerMono in playerMonos)
                {
                    playerMono.enabled = true;
                }
                foreach (MonoBehaviour enemyMono in enemyMonos)
                {
                    enemyMono.enabled = true;
                }
                foreach (MonoBehaviour cameraMono in cameraMonos)
                {
                    cameraMono.enabled = true;
                }
                Destroy(gameObject);
            });
    }

    #endregion
}  