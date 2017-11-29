// ---------------------------------------------------------
// RideBallMove.cs
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
  
public class RideBallMove : PlayerMove
{
    #region define

    readonly float AcceRate = 1.05f;        // 加速率 
    readonly float DeceRate = 0.98f;        // 減衰率 
    readonly float MinAcceleration = 0.05f; // 最低加速度（それ以下を0とみなして処理する）
    private  float MaxAcceleration = 0.0f;  // 最大速度。通常の移動速度が静的でないため初期化時に指定

    #endregion

    #region variable

    private float _nowAcceForward = 0.0f;    // 前方加速率
    private float _nowAcceBack    = 0.0f;    // 後方加速率
    private float _nowAcceRight   = 0.0f;    // 右方加速率
    private float _nowAcceLeft    = 0.0f;    // 左方加速率

    #endregion

    #region method

    /// <summary>
    /// 移動処理
    /// </summary>
    protected override void Move()
    {
        // 全方位の加速度を元に移動量を算出
        _moveAmount = Vector3.zero;
        _moveAmount += transform.forward * _nowAcceForward;
        _moveAmount -= transform.forward * _nowAcceBack;
        _moveAmount += transform.right   * _nowAcceRight;
        _moveAmount -= transform.right   * _nowAcceLeft;

        transform.position += _moveAmount * Time.deltaTime;
    }

    /// <summary>
    /// 加速処理
    /// </summary>
    protected override void Acceleration(eDirection dir)
    {
        switch (dir)
        {
            case eDirection.Forward:
                if(_nowAcceForward == 0.0f)
                {
                    _nowAcceForward = AcceRate;
                }

                _nowAcceForward *= AcceRate;

                if(_nowAcceForward > MaxAcceleration)
                {
                    _nowAcceForward = MaxAcceleration;
                }
                break;

            case eDirection.Back:
                if (_nowAcceBack == 0.0f)
                {
                    _nowAcceBack = AcceRate;
                }

                _nowAcceBack *= AcceRate;

                if (_nowAcceBack > MaxAcceleration)
                {
                    _nowAcceBack = MaxAcceleration;
                }
                break;

            case eDirection.Right:
                if (_nowAcceRight == 0.0f)
                {
                    _nowAcceRight = AcceRate;
                }

                _nowAcceRight *= AcceRate;

                if (_nowAcceRight > MaxAcceleration)
                {
                    _nowAcceRight = MaxAcceleration;
                }
                break;

            case eDirection.Left:
                if (_nowAcceLeft == 0.0f)
                {
                    _nowAcceLeft = AcceRate;
                }

                _nowAcceLeft *= AcceRate;

                if (_nowAcceLeft > MaxAcceleration)
                {
                    _nowAcceLeft = MaxAcceleration;
                }
                break;
        }
    }

    /// <summary>
    /// 減速処理
    /// </summary>
    protected override void Deceleration()
    {
        if (_nowAcceForward > 0.0f)
        {
            _nowAcceForward *= DeceRate;
            if (_nowAcceForward < MinAcceleration)
            {
                _nowAcceForward = 0.0f;
            }
        }
        if (_nowAcceBack > 0.0f)
        {
            _nowAcceBack *= DeceRate;
            if (_nowAcceBack < MinAcceleration)
            {
                _nowAcceBack = 0.0f;
            }
        }
        if (_nowAcceRight > 0.0f)
        {
            _nowAcceRight *= DeceRate;
            if (_nowAcceRight < MinAcceleration)
            {
                _nowAcceRight = 0.0f;
            }
        }
        if (_nowAcceLeft > 0.0f)
        {
            _nowAcceLeft *= DeceRate;
            if (_nowAcceLeft < MinAcceleration)
            {
                _nowAcceLeft = 0.0f;
            }
        }
    }

    #endregion

    #region unity_event  

    /// <summary>  
    /// 初期化処理  
    /// </summary>  
    void Awake()
    {
        MaxAcceleration = _speed_Sec * 2.0f;
    }

    /// <summary>  
    /// アクティブ変更時の初期化処理  
    /// </summary>  
    void OnEnable()
    {
        _nowAcceForward = 0.0f;
        _nowAcceBack = 0.0f;
        _nowAcceRight = 0.0f;
        _nowAcceLeft = 0.0f;
    }

    #endregion
}