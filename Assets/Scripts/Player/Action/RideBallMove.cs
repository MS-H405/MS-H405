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

/// <summary>
/// TODO : 跳ね返り処理と速度に応じたダメージ処理
/// </summary>
  
public class RideBallMove : PlayerMove
{
    #region define

    readonly float AcceRate = 1.15f;        // 加速率 
    readonly float DeceRate = 0.98f;        // 減衰率 
    readonly float MinAcceleration = 0.05f; // 最低加速度（それ以下を0とみなして処理する）
    private float MaxAcceleration = 0.0f;   // 最大速度。通常の移動速度が静的でないため初期化時に指定
    private float InitAcceleration = 0.0f;  // 初期速度。通常の移動速度が静的でないため初期化時に指定

    #endregion

    #region variable

    // 演算用変数
    private float _nowAcceForward = 0.0f;    // 前方加速率
    private float _nowAcceBack    = 0.0f;    // 後方加速率
    private float _nowAcceRight   = 0.0f;    // 右方加速率
    private float _nowAcceLeft    = 0.0f;    // 左方加速率

    // 演出用変数
    private bool _isRigor = false;                  // 硬直判定
    private Rigidbody _rigidbody = null;            // 
    [SerializeField] GameObject _ballPrefab = null; // 玉乗りボールオブジェクト

    #endregion

    #region method

    /// <summary>
    /// 移動処理
    /// </summary>
    protected override void Move()
    {
        // 硬直時は処理しない
        if (_isRigor)
            return;

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
                    _nowAcceForward = InitAcceleration;
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
                    _nowAcceBack = InitAcceleration;
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
                    _nowAcceRight = InitAcceleration;
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
                    _nowAcceLeft = InitAcceleration;
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

    /// <summary>
    /// 加速リセット処理
    /// </summary>
    private void AcceReset()
    {
        _nowAcceForward = 0.0f;
        _nowAcceBack = 0.0f;
        _nowAcceRight = 0.0f;
        _nowAcceLeft = 0.0f;
    }

    /// <summary>  
    /// 終了処理  
    /// </summary>
    public void End()
    {
        StaticCoroutine.Instance.StartStaticCoroutine(RideOff());
    }

    #endregion

    #region Coroutine

    /// <summary>  
    /// 乗る時の演出処理  
    /// </summary>
    private IEnumerator RideOn()
    {
        // 行動停止
        _isRigor = true;

        // 上に飛ばし、玉より超えたら玉出現
        while (transform.position.y < 2.25f)
        {
            _rigidbody.AddForce(0.0f, 9.8f * 2.5f, 0.0f);
            //_rigidbody.AddForce(-transform.forward * 4.9f);
            yield return null;

        }
        Vector3 stopVelocity = _rigidbody.velocity;
        stopVelocity.x = stopVelocity.z = 0.0f;
        _rigidbody.velocity = stopVelocity;
        
        GameObject ball = Instantiate(_ballPrefab);
        ball.transform.SetParent(transform);
        ball.transform.localPosition = new Vector3(0.0f, -1.5f, 0.0f);

        // 乗るまで待つ
        while(_rigidbody.velocity.y != 0.0f)
        {
            yield return null;
        }

        // 行動再開
        _isRigor = false;
    }

    /// <summary>  
    /// 降りる時の演出処理  
    /// </summary>
    private IEnumerator RideOff()
    {
        // 行動停止
        _isRigor = true;

        // 降りる処理
        Destroy(transform.Find("RideBall(Clone)").gameObject);
        while (_isRigor)
        {
            yield return null;
        }
        
        GetComponent<PlayerMove>().enabled = true;
        this.enabled = false;
    }

    #endregion

    #region unity_event  

    /// <summary>  
    /// 初期化処理  
    /// </summary>  
    private void Awake()
    {
        InitAcceleration = _speed_Sec * 0.2f;
        MaxAcceleration = _speed_Sec * 1.7f;
        _rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>  
    /// アクティブ変更時の初期化処理  
    /// </summary>  
    private void OnEnable()
    {
        AcceReset();
        StaticCoroutine.Instance.StartStaticCoroutine(RideOn());
    }

    /// <summary>
    /// 地形との接地判定処理
    /// </summary>
    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Field")
        {
            _isRigor = false;
        }

        if(col.transform.tag == "Enemy")
        {
            // ダメージ処理がある場合はダメージ処理
            if (_nowAcceForward >= _speed_Sec)
            {
                GameObject obj = col.gameObject;
                while (obj.transform.parent)
                {
                    obj = obj.transform.parent.gameObject;
                }

                // 最高速の場合のみ3ダメージ
                obj.GetComponent<EnemyBase>().Damage(_nowAcceForward >= (MaxAcceleration * DeceRate - 0.1f) ? 3 : 1);
            }

            // 跳ね返り処理
            _isRigor = true;
            Vector3 reflectPower = -transform.forward * _nowAcceForward * 5.0f;
            reflectPower += new Vector3(0.0f, 200.0f, 0.0f);
            _rigidbody.AddForce(reflectPower);
            AcceReset();
        }
    }

    #endregion
}