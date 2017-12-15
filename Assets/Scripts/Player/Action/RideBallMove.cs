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

    readonly float AcceRate = 1.1f;         // 加速率 
    readonly float DeceRate = 0.98f;        // 減衰率 
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
    private Rigidbody _rigidbody = null;            // 
    private GameObject _ballObj = null;             // 玉乗りボールインスタンス
    [SerializeField] GameObject _ballPrefab = null; // 玉乗りボールプレハブ

    #endregion

    #region method

    /// <summary>
    /// 移動処理
    /// </summary>
    protected override void Move()
    {
        // 硬直時は処理しない
        if (_isRigor)
        {
            _animator.SetBool("BallWalk", false);
            return;
        }

        // 全方位の加速度を元に移動量を算出
        _moveAmount = Vector3.zero;
        _moveAmount += transform.forward * _nowAcceForward;
        _moveAmount -= transform.forward * _nowAcceBack;
        _moveAmount += transform.right   * _nowAcceRight;
        _moveAmount -= transform.right   * _nowAcceLeft;

        // Animation更新
        _animator.SetBool("BallWalk", _moveAmount != Vector3.zero);

        // 向き更新し、移動させる
        Vector3 oldAngle = _ballObj.transform.eulerAngles;
        transform.LookAt(transform.position + _moveAmount);
        transform.position += _moveAmount * Time.deltaTime;

        if (_moveAmount == Vector3.zero)
        {
            _animator.speed = 1.0f;
            return;
        }

        float speed = Mathf.Abs(_moveAmount.x) + Mathf.Abs(_moveAmount.z);

        // プレイヤーの歩行速度変更
        _animator.speed = speed / 10.0f;

        // ボールの回転
        // _ballObj.transform.eulerAngles = oldAngle;
        _ballObj.transform.Rotate(transform.right * _moveAmount.x, 360 * (speed / 5.0f) * Time.deltaTime);
        //_ballObj.transform.Rotate(transform.forward * _moveAmount.z, 360 * Time.deltaTime);
        //_ballObj.transform.eulerAngles += new Vector3(_moveAmount.z, 0.0f, -_moveAmount.x) * 5.0f * Time.deltaTime;
    }

    /// <summary>
    /// 加速処理
    /// </summary>
    protected override void Acceleration(eDirection dir)
    {
        // 硬直時は処理しない
        if (_isRigor)
            return;

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
            if (_nowAcceForward < InitAcceleration)
            {
                _nowAcceForward = 0.0f;
            }
        }
        if (_nowAcceBack > 0.0f)
        {
            _nowAcceBack *= DeceRate;
            if (_nowAcceBack < InitAcceleration)
            {
                _nowAcceBack = 0.0f;
            }
        }
        if (_nowAcceRight > 0.0f)
        {
            _nowAcceRight *= DeceRate;
            if (_nowAcceRight < InitAcceleration)
            {
                _nowAcceRight = 0.0f;
            }
        }
        if (_nowAcceLeft > 0.0f)
        {
            _nowAcceLeft *= DeceRate;
            if (_nowAcceLeft < InitAcceleration)
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
        _animator.SetBool("Walk", false);
        _animator.SetTrigger("RideOn");
        _rigidbody.useGravity = false;

        // 上に飛ばし、玉より超えたら玉出現
        while (transform.position.y < 2.25f)
        {
            transform.position += new Vector3(0 , 5.0f * Time.deltaTime, 0);
            yield return null;
        }
        _rigidbody.useGravity = true;

        _ballObj = Instantiate(_ballPrefab);
        _ballObj.transform.SetParent(transform);
        _ballObj.transform.localPosition = new Vector3(0.0f, -1.0f, 0.0f);

        // 地面につくまでまで待つ
        while(_isRigor)
        {
            yield return null;
        }
    }

    /// <summary>  
    /// 降りる時の演出処理  
    /// </summary>
    private IEnumerator RideOff()
    {
        // 行動停止
        _isRigor = true;

        // 降りる処理
        if (_ballObj)
            _ballObj.SetActive(false);
        
        while (_isRigor)
        {
            yield return null;
        }

        // 玉を削除
        Destroy(_ballObj.gameObject);

        // 玉乗り時に変更したアニメーションデータを初期化
        _animator.SetBool("BallWalk", false);
        _animator.speed = 1.0f;

        // コンポーネントを切り替え
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
        base.Awake();
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
    protected override void OnCollisionEnter(Collision col)
    {
        base.OnCollisionEnter(col);

        if (col.transform.tag == "Field")
        {
            _rigidbody.velocity = Vector3.zero;
        }

        if (col.transform.tag == "Enemy")
        {
            // 既に攻撃判定等が発生していたら処理しない
            if (_isRigor)
                return;

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
            Vector3 reflectPower = -transform.forward * _nowAcceForward * 10.0f;
            reflectPower += new Vector3(0.0f, 250.0f, 0.0f);
            _rigidbody.AddForce(reflectPower);
            AcceReset();
        }
    }

    #endregion
}