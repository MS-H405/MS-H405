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
    private bool _isRideAnim = false;
    public override bool IsInput { get { return _isGround && !_isRideAnim; } }
    private Rigidbody _rigidbody = null;            //
    private Vector3 _inputVec = Vector3.zero;       // 現在の入力している方向

    private GameObject _ballObj = null;             // 玉乗りボールインスタンス
    [SerializeField] GameObject _ballPrefab = null; // 玉乗りボールプレハブ
    private Vector3 _oldBallAngle = Vector3.zero;
    private ParticleSystem[] _rideEffects = new ParticleSystem[2];

    [SerializeField] GameObject _ballAttackEffect = null; // 玉乗り衝突エフェクト

    #endregion

    #region method

    /// <summary>
    /// 移動処理
    /// </summary>
    protected override void Move()
    {
        // 硬直時は処理しない
        if (!_isGround)
        {
            _animator.SetBool("BallWalk", false);
            RunSmoke(_animator.GetBool("BallWalk"));
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
        RunSmoke(_animator.GetBool("BallWalk"));

        if (_animator.GetBool("BallWalk"))
        {
            SoundManager.Instance.PlayBGM(SoundManager.eBgmValue.Player_BallWalk);
        }
        else
        {
            SoundManager.Instance.StopBGM(SoundManager.eBgmValue.Player_BallWalk);
        }

        // 移動と向きを更新
        transform.position += _moveAmount * Time.deltaTime;

        if (_inputVec == Vector3.zero)
        {
            transform.LookAt(transform.position + _moveAmount);
        }
        else
        {
            transform.LookAt(transform.position + _inputVec);
        }

        if (_moveAmount == Vector3.zero)
        {
            _animator.speed = 1.0f;
            RideEffect(false);
            return;
        }

        float speed = Mathf.Abs(_moveAmount.x) + Mathf.Abs(_moveAmount.z);

        // プレイヤーの歩行速度変更
        _animator.speed = speed / 10.0f;
        RideEffect(_animator.speed > 0.2f);

        // ボールの回転
        _ballObj.transform.eulerAngles = _oldBallAngle;
        _ballObj.transform.Rotate(transform.right * _moveAmount.x, 360 * (speed / 5.0f) * Time.deltaTime);
        _oldBallAngle = _ballObj.transform.eulerAngles;
        //_ballObj.transform.Rotate(transform.forward * _moveAmount.z, 360 * Time.deltaTime);
        //_ballObj.transform.eulerAngles += new Vector3(_moveAmount.z, 0.0f, -_moveAmount.x) * 5.0f * Time.deltaTime;
    }

    /// <summary>
    /// 加速処理
    /// </summary>
    protected override void Acceleration(eDirection dir)
    {
        // 硬直時は処理しない
        if (!_isGround)
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

                _inputVec += transform.forward;
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
                
                _inputVec -= transform.forward;
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

                _inputVec += transform.right;
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

                _inputVec -= transform.right;
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
                _rigidbody.velocity = Vector3.zero;
            }
        }
        if (_nowAcceBack > 0.0f)
        {
            _nowAcceBack *= DeceRate;
            if (_nowAcceBack < InitAcceleration)
            {
                _nowAcceBack = 0.0f;
                _rigidbody.velocity = Vector3.zero;
            }
        }
        if (_nowAcceRight > 0.0f)
        {
            _nowAcceRight *= DeceRate;
            if (_nowAcceRight < InitAcceleration)
            {
                _nowAcceRight = 0.0f;
                _rigidbody.velocity = Vector3.zero;
            }
        }
        if (_nowAcceLeft > 0.0f)
        {
            _nowAcceLeft *= DeceRate;
            if (_nowAcceLeft < InitAcceleration)
            {
                _nowAcceLeft = 0.0f;
                _rigidbody.velocity = Vector3.zero;
            }
        }

        _inputVec = Vector3.zero;
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
    /// 玉乗り加速エフェクト処理
    /// </summary>
    private void RideEffect(bool isOn)
    {
        if(isOn)
        {
            int count = 0;
            foreach (ParticleSystem effect in _rideEffects)
            {
                count++;
                bool old = effect.loop;
                effect.loop = true;
                effect.emissionRate = (3.0f * count) * (_animator.speed * _animator.speed);

                if (effect.loop == old)
                    continue;

                effect.Play();
            }
        }
        else
        {
            foreach (ParticleSystem effect in _rideEffects)
            {
                effect.loop = false;
            }
        }
    }

    /// <summary>  
    /// 開始処理  
    /// </summary>
    public void On()
    {
        AcceReset();
        StaticCoroutine.Instance.StartStaticCoroutine(RideOn());
    }

    /// <summary>  
    /// 終了処理  
    /// </summary>
    public void Off()
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
        if (_isRideAnim)
            yield break;

        // 行動停止
        _isRideAnim = true;
        _isGround = false;
        _animator.SetBool("Walk", false);
        _animator.SetTrigger("Roll");
        _rigidbody.useGravity = false;
        PlayerManager.Instance.Player.IsInvincible = true;

        // 上に飛ばし、玉より超えたら玉出現
        while (transform.position.y < 2.25f)
        {
            transform.position += new Vector3(0 , 5.0f * Time.deltaTime, 0);
            yield return null;
        }
        _rigidbody.useGravity = true;

        _ballObj = Instantiate(_ballPrefab);
        _ballObj.transform.SetParent(transform); //.Find("RideBall"));
        _ballObj.transform.localPosition = new Vector3(0.0f, -0.9f, 0.0f);
        _oldBallAngle = _ballObj.transform.eulerAngles;
        GameEffectManager.Instance.Play("Bohun", _ballObj.transform.position);
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.Player_Bofun);

        // 地面についてもアニメーションが終了していなければ待つ
        AnimatorStateInfo animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        while (!animStateInfo.IsName("Base.Idle"))
        {
            _isGround = false;
            _animator.speed = 1.0f;
            animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }
        _isRideAnim = false;
        _isGround = true;
        PlayerManager.Instance.Player.IsInvincible = false;
    }

    /// <summary>  
    /// 降りる時の演出処理  
    /// </summary>
    private IEnumerator RideOff()
    {
        if (_isRideAnim)
            yield break;

        // 行動停止
        _isGround = false;
        _isRideAnim = true;
        PlayerManager.Instance.Player.IsInvincible = true;
        RideEffect(false);

        // 
        if (!PlayerManager.Instance.Player.IsDamage)
        {
            _animator.speed = 1.0f;
            _animator.SetTrigger("Roll");
            _rigidbody.AddForce(new Vector3(0, 100, 0));
        }

        // 玉をエフェクトとともに消す
        _ballObj.SetActive(false);
        GameEffectManager.Instance.Play("Bohun", _ballObj.transform.position);
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.Player_Bofun);

        while (!_isGround)
        {
            yield return null;
        }

        // 玉乗り時に変更したアニメーションデータを初期化
        _isRideAnim = false;
        _animator.speed = 1.0f;
        _animator.SetBool("BallWalk", false);
        PlayerManager.Instance.Player.IsInvincible = false;
        RideEffect(false);
        SoundManager.Instance.StopBGM(SoundManager.eBgmValue.Player_BallWalk);

        // 玉の削除処理
        Destroy(_ballObj.gameObject);

        // コンポーネントを切り替え
        GetComponent<PlayerMove>().enabled = true;
        enabled = false;
    }

    #endregion

    #region unity_event  

    /// <summary>  
    /// 初期化処理  
    /// </summary>  
    private new void Awake()
    {
        base.Awake();
        InitAcceleration = _speed_Sec * 0.2f;
        MaxAcceleration = _speed_Sec * 1.7f;
        _rigidbody = GetComponent<Rigidbody>();

        _rideEffects[0] = transform.Find("TamanoriEffect").GetComponent<ParticleSystem>();
        _rideEffects[1] = transform.Find("TamanoriEffect").GetChild(0).GetComponent<ParticleSystem>();
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
            if (!_isGround || _nowAcceForward == 0.0f)
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

            ReflectBall(col.contacts[0].point, _nowAcceForward >= _speed_Sec);
        }

        if (col.transform.tag == "ChildTotem")
        {
            // 既に攻撃判定等が発生していたら処理しない
            if (!_isGround || _nowAcceForward == 0.0f)
                return;

            ReflectBall(col.contacts[0].point, false);
        }
    }

    private void ReflectBall(Vector3 point, bool isEffect)
    {
        // 跳ね返り処理
        _isGround = false;
        Vector3 reflectPower = -transform.forward * _nowAcceForward * 10.0f;
        reflectPower += new Vector3(0.0f, 300.0f, 0.0f);
        _rigidbody.AddForce(reflectPower);
        _animator.SetTrigger("Roll");
        _animator.speed = 1.0f;

        AcceReset();
        RideEffect(false);
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.Player_BallAttack);

        if (!isEffect)
            return;

        Instantiate(_ballAttackEffect, point + new Vector3(0, 0.5f, 0), Quaternion.identity);
        
    }

    #endregion
}