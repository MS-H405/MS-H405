// ---------------------------------------------------------
// PlayerMove.cs
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

public class PlayerMove : MonoBehaviour
{
    #region define

    protected enum eDirection
    {
        Forward = 0,
        Back,
        Right,
        Left
    };

    #endregion

    #region variable

    [SerializeField] protected float _speed_Sec = 7.5f;
    protected Vector3 _moveAmount = Vector3.zero;

    protected bool _isGround = false;    // 地面との接地判定
    public virtual bool IsInput { get { return _isGround; } }

    // animation用変数
    protected Animator _animator = null;
    private Vector3 _oldAngle = Vector3.zero;
    protected GameObject _runSmoke = null;

    #endregion

    #region method

    /// <summary>
    /// 移動処理
    /// </summary>
    protected virtual void Move()
    {
        // Animation更新
        _animator.SetBool("Walk", _moveAmount != Vector3.zero);
        _runSmoke.SetActive(_animator.GetBool("Walk"));
        _runSmoke.GetComponent<ParticleSystem>().loop = false;

        if (_animator.GetBool("Walk"))
        {
            SoundManager.Instance.PlayBGM(SoundManager.eBgmValue.Player_Run);
        }
        else
        {
            SoundManager.Instance.StopBGM(SoundManager.eBgmValue.Player_Run);
        }

        // 硬直時は処理しない
        if (!_isGround || IsAction)
            return;

        transform.position += _moveAmount * Time.deltaTime;
        transform.LookAt(transform.position + _moveAmount);
    }

    /// <summary>
    /// 加速処理
    /// </summary>
    protected virtual void Acceleration(eDirection dir)
    {
        // 硬直時は処理しない
        if (!_isGround || IsAction)
            return;

        switch (dir)
        {
            case eDirection.Forward:
                _moveAmount += transform.forward * _speed_Sec;
                break;
            case eDirection.Back:
                _moveAmount -= transform.forward * _speed_Sec;
                break;
            case eDirection.Right:
                _moveAmount += transform.right * _speed_Sec;
                break;
            case eDirection.Left:
                _moveAmount -= transform.right * _speed_Sec;
                break;
        }
    }

    /// <summary>
    /// 減速処理
    /// </summary>
    protected virtual void Deceleration()
    {
        _moveAmount = Vector3.zero; // 即減速
    }

    /// <summary>
    /// 地面離着処理
    /// </summary>
    public void OutGround()
    {
        _isGround = false;
    }

    /// <summary>
    /// 攻撃行動中かを返す
    /// </summary>
    private bool IsAction
    {
        get
        {
            AnimatorStateInfo animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            return !animStateInfo.IsName("Base.Idle") && !animStateInfo.IsName("Base.Walk");
        }
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    protected void Awake()
    {
        _animator = GetComponent<Animator>();
        _runSmoke = transform.Find("RunSmoke").gameObject;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update ()
    {
        // 敵を前方として移動するので、移動前に敵の方に向ける
        if (EnemyManager.Instance.BossEnemy)
        { 
            Vector3 enemyPos = EnemyManager.Instance.BossEnemy.transform.position;
            transform.LookAt(new Vector3(enemyPos.x, transform.position.y, enemyPos.z));
            _oldAngle = transform.eulerAngles;
        }
        else
        {
            transform.eulerAngles = _oldAngle;
        }

        // 入力判定に応じて加速
        if (Input.GetAxis("Vertical") > 0)
        {
            Acceleration(eDirection.Forward);
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            Acceleration(eDirection.Back);
        }
        if (Input.GetAxis("Horizontal") > 0)
        {
            Acceleration(eDirection.Right);
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            Acceleration(eDirection.Left);
        }

        // 移動・減速処理
        Move();
        Deceleration();
    }

    /// <summary>
    /// 地形との接地判定処理
    /// </summary>
    protected virtual void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Field")
        {
            _isGround = true;
        }
    }

    #endregion
}  