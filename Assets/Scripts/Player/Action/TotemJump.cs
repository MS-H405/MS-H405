// ---------------------------------------------------------
// TotemJump.cs
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
  
public class TotemJump : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    // 地面との接地判定
    private bool _isGround = true;
    public bool IsGround { get { return _isGround; } } 

    // トーテム押し出し量と時間
    private GameObject _totemObj = null;
    [SerializeField] float _pushTime = 1.0f;
    [SerializeField] float _pushAmount = 3.0f;
    // ジャンプ力
    [SerializeField] float _addForwardPower = 20.0f;
    [SerializeField] float _addUpPower = 200.0f;

    private Animator _animator = null;
    private Rigidbody _rigidBody = null;
    private PlayerMove _playerMove = null;
    private CapsuleCollider _playerCollider = null;

    private ParticleSystem _totemJumpEffect = null;

    #endregion

    #region method

    /// <summary>
    /// 実行処理
    /// </summary>
    public IEnumerator Run()
    {
        // 既に飛んでたら破棄
        if(!_isGround)
        {
            yield break;
        }

        // 開始処理
        _isGround = false;
        _playerMove.RunSmoke(false);
        _playerMove.enabled = false;
        PlayerManager.Instance.Player.IsInvincible = true;  //  無敵状態にする
        _totemObj.SetActive(true);
        _totemObj.transform.position = new Vector3(transform.position.x, -1.5f, transform.position.z);
        _animator.SetTrigger("JumpStart");
        _rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        GameEffectManager.Instance.Play("TS_totem_appear", transform.position);

        float time = 0.0f;
        while(time < 1.0f)
        {
            time += Time.deltaTime / _pushTime;
            transform.position += new Vector3(0, _pushAmount * Time.deltaTime / _pushTime, 0);
            _totemObj.transform.position += new Vector3(0, _pushAmount * Time.deltaTime / _pushTime, 0);
            _animator.SetBool("Jump", transform.position.y > _pushAmount * 0.5f);
            yield return null;
        }

        // 上に飛ばす
        TotemJumpEffect(true);
        _playerMove.enabled = true;
        _playerMove.ChangeSpeed(1.5f);
        _rigidBody.AddForce(new Vector3(0, _addUpPower, 0) + (transform.forward * _addForwardPower));
        GameEffectManager.Instance.Play("PinAttack", transform.position);
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.Player_TotemJump);

        GameObject bossEnemy = EnemyManager.Instance.BossEnemy.gameObject;
        var enemyDistanceDisposable = new SingleAssignmentDisposable();
        enemyDistanceDisposable.Disposable = this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                Vector3 enemyPos = bossEnemy.transform.position;
                Vector3 playerPos = PlayerManager.Instance.GetVerticalPos(enemyPos);
                float distance = Vector3.Distance(enemyPos, playerPos);

                if (distance >= 8.0f)
                {
                    EnemyManager.Instance.Active = true;
                }
                else
                {
                    EnemyManager.Instance.Active = false;
                }
            });

        var invincibleDisposable = new SingleAssignmentDisposable();
        invincibleDisposable.Disposable = this.UpdateAsObservable()
            .Where(_ => transform.position.y <= 1.0f)
            .Subscribe(_ =>
            {
                PlayerManager.Instance.Player.IsInvincible = false;
                _playerCollider.enabled = false;
                invincibleDisposable.Dispose();
            });
        var jumpDisposable = new SingleAssignmentDisposable();
        jumpDisposable.Disposable = this.UpdateAsObservable()
            .Where(_ => transform.position.y <= 0.5f)
            .Subscribe(_ =>
            {
                _animator.SetBool("Jump", false);
                _playerCollider.enabled = true;
                SoundManager.Instance.PlaySE(SoundManager.eSeValue.Player_Landing);
                jumpDisposable.Dispose();
            });

        while (!_isGround)
        {
            _totemObj.transform.position -= new Vector3(0, _pushAmount * Time.deltaTime / _pushTime, 0) / 2.0f;
            yield return null;
        }

        TotemJumpEffect(false);
        _animator.speed = 1.0f;
        _playerMove.ChangeSpeed(1.0f);
        _totemObj.SetActive(false);
        EnemyManager.Instance.Active = true;
        enemyDistanceDisposable.Dispose();
        _rigidBody.constraints = RigidbodyConstraints.None;
        _rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    /// <summary>
    /// トーテムジャンプ時の滑空エフェクト処理
    /// </summary>
    private void TotemJumpEffect(bool isOn)
    {
        if (isOn)
        {
            bool old = _totemJumpEffect.loop;
            _totemJumpEffect.loop = true;
            
            if (_totemJumpEffect.loop == old)
                return;
            
            _totemJumpEffect.Play();
        }
        else
        {
            _totemJumpEffect.loop = false;
        }
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        _totemObj = transform.Find("TotemJump").gameObject;
        _totemObj.transform.SetParent(null);
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody>();
        _playerMove = GetComponent<PlayerMove>();
        _playerCollider = GetComponent<CapsuleCollider>();

        _totemJumpEffect = transform.Find("TotemJumpEffect").GetComponent<ParticleSystem>();
        _totemJumpEffect.Stop();
    }

    /// <summary>
    /// 地形との接地判定処理
    /// </summary>
    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Field")
        {
            _isGround = true;
        }
    }

    /// <summary>
    /// 地形との接地判定処理
    /// </summary>
    private void OnCollisionExit(Collision col)
    {
        if (col.transform.tag == "Field")
        {
            _isGround = false;
        }
    }

    #endregion
}  