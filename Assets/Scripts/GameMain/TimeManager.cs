// ---------------------------------------------------------
// TimeManager.cs
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
  
public class TimeManager : SingletonMonoBehaviour<TimeManager>
{
    #region define

    #endregion

    #region variable

    [SerializeField] float _gameTime = 60.0f;

    #endregion

    #region method

    /// <summary>
    /// タイムアップ判定を返す
    /// </summary>
    public bool TimeUp
    {
        get
        {
            return _gameTime <= 0.0f;
        }
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        transform.Find("Center").GetComponent<ManualRotation>().Run(new Vector3(0,0,-360), _gameTime);
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update ()
    {
        _gameTime -= Time.deltaTime;
    }

    #endregion
}  