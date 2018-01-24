// ---------------------------------------------------------  
// Player.cs  
// 
// 作成者: Shota_Obora
// ---------------------------------------------------------  
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class Player : MonoBehaviour
{
    #region define

    readonly int MaxHp = 6;

    #endregion

    #region variable

    [SerializeField] int _hp = 0;
    public int HP { get { return _hp; } }
    private ActionManager _actionManager = null;
    private PlayerMove _playerMove = null;
    private RideBallMove _rideBallMove = null;

    // ダメージ処理演出用変数
    private bool _isDamage = false;
    public bool IsDamage { get { return _isDamage; } }
    public bool IsInvincible { set { _isDamage = value; } }
    private Rigidbody _rigidBody = null;
    private Animator _animator = null;
    [SerializeField] float _backPower = 75.0f;
    [SerializeField] float _upPower = 200.0f;
    private ShakeCamera _shakeCamera = null;
    private bool _isReturn = false;

    #endregion

    #region method  

    /// <summary>  
    /// ダメージ処理  
    /// </summary>  
    public bool Damage(float power = 1.0f, bool isDebug = false)
    {
        if (_isDamage)
            return false;

        _hp--;
        StaticCoroutine.Instance.StartStaticCoroutine(DamageWait());
        DamageStan(power);
        //_animator.SetTrigger("Damage");
        _animator.Play("Down", 0, 0.0f);
        PlayerLifeManager.Instance.DamageEffect();
        _shakeCamera.Shake(0,035f);
        Debug.Log("Damage");

        if (isDebug)
        {
            int nowHp = _hp;
            while (_hp > 0)
            {
                _hp--;
                PlayerLifeManager.Instance.DamageEffect(nowHp - _hp);
            }
        }

        SoundManager.Instance.PlaySE(SoundManager.eSeValue.Player_Damage);
        DamageRed.Instance.Run();
        return true;
    }

    /// <summary>
    /// ダメージ時のスタン演出処理
    /// </summary>
    private void DamageStan(float power)
    {
        // 玉乗り中なら玉乗りを強制キャンセル
        if (_rideBallMove.enabled)
        {
            _actionManager.Cancel();
            _rideBallMove.Off();
        }
        else
        {
            _playerMove.OutGround();
        }

        if(_actionManager.NowAction == ActionManager.eActionType.Bagpipe)
        {
            _actionManager.Cancel();
        }

        Vector3 velocity = -transform.forward * _backPower * power;
        velocity.y = _upPower;
        _rigidBody.AddForce(velocity);
    }

    /// <summary>  
    /// ダメージ時の無敵時間処理  
    /// </summary>  
    private IEnumerator DamageWait()
    {
        _isDamage = true;
      
        // 遷移を待つ
        while (!PlayerManager.Instance.DamageAnimation())
        {
            yield return null;
        }

        // ダメージアニメーション終了を待つ
        while (PlayerManager.Instance.DamageAnimation())
        {
            yield return null;
        }

        _isDamage = false;
        _isReturn = false;
    }

    /// <summary>
    /// プレイヤーが地面についているかを判定
    /// </summary>
    public bool IsInput
    {
        get
        {
            AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
            if (!state.IsName("Base.Idle") && !state.IsName("Base.PipeIdle") &&
                !state.IsName("Base.Walk") && !state.IsName("Base.PipeWalk") && !state.IsName("Base.RideBallMove"))
            {
                return false;
            }

            if (_playerMove.enabled)
            {
                return _playerMove.IsInput;
            }
            else
            {
                return _rideBallMove.IsInput;
            }
        }
    }
    
    /// <summary>
    /// ライフの残り割合を返す(1.0fで100%)
    /// </summary>
    public float LifePercentage
    {
        get
        {
            return (float)_hp / MaxHp;
        }
    }

    #endregion

    #region unity_event  

    /// <summary> 
    /// 初期化処理  
    /// </summary>  
    private void Awake()
    {
        _hp = MaxHp;
        _animator = GetComponent<Animator>();
        _actionManager = GetComponent<ActionManager>();
        _playerMove = GetComponent<PlayerMove>();
        _rideBallMove = GetComponent<RideBallMove>();
        _rigidBody = GetComponent<Rigidbody>();
        _shakeCamera = Camera.main.GetComponent<ShakeCamera>();
    }

    /// <summary> 
    /// 更新前処理  
    /// </summary>  
    private void Start()
    {
        // PlayerInputのUpdate
        EnemyBase enemyBase = EnemyManager.Instance.BossEnemy.GetComponent<EnemyBase>();
        this.UpdateAsObservable()
            .Where(_ => _hp > 0)
            .Subscribe(_ =>
            {
                // 行動スロットの切り替えは常時可能
                if (!enemyBase.IsStan)
                {
                    // 武器スロット右回り
                    if (Input.GetButtonDown("Right") && AtackIconManager.Instance.IsChange)
                    {
                        _actionManager.ChangeSelect(true);
                        AtackIconManager.Instance.Rot(true);
                    }
                    // 武器スロット左回り
                    else if (Input.GetButtonDown("Left") && AtackIconManager.Instance.IsChange)
                    {
                        _actionManager.ChangeSelect(false);
                        AtackIconManager.Instance.Rot(false);
                    }
                }

                // 入力を受け付けてよいかを判定
                if (!IsInput)
                    return;

                // ダメージモーション中なら処理しない
                if (PlayerManager.Instance.DamageAnimation())
                    return;

                if (enemyBase.IsStan)
                {
                    // 必殺技実行
                    if(Input.GetButtonDown("Atack"))
                    {
                        enemyBase.SpecialDamage();
                        MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.SPECIAL_1);
                    }
                }
                else
                {
                    // 攻撃選択＆選択時攻撃
                    if (Input.GetButtonDown("Atack"))
                    {
                        _actionManager.OnSelect();
                        AtackIconManager.Instance.Select();
                    }
                    // 行動キャンセル
                    else if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetButtonDown("Cancel"))
                    {
                        if (_actionManager.NowAction == ActionManager.eActionType.RideBall)
                        {
                            _actionManager.Cancel();
                        }
                        if (_actionManager.NowAction == ActionManager.eActionType.Bagpipe)
                        {
                            _actionManager.Cancel();
                        }
                    }
                }
            });

        // 必殺打てる判定
        GameObject specialAura = transform.Find("SpecialAura").gameObject;
        specialAura.SetActive(false);
        this.ObserveEveryValueChanged(_ => enemyBase.IsStan)
            .Where(_ => enemyBase.IsStan)
            .Subscribe(_ =>
            {
                specialAura.SetActive(true);
                _actionManager.Cancel();
                PlayerManager.Instance.Player.IsInvincible = true;
            });
    }

    /// <summary> 
    /// 更新処理  
    /// </summary>  
    private void Update()
    {
        // DEBUG : デバッグコマンド
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Damage();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            Damage(1.0f, true);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            IsInvincible = !_isDamage;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(Time.timeScale >= 1.0f)
            {
                Time.timeScale = 0.0f;
                SoundManager.Instance.PauseSE(true);
            }
            else
            {
                Time.timeScale = 1.0f;
                SoundManager.Instance.PauseSE(false);
            }
        }
    }

    /// <summary>
    /// 地形との接地判定処理
    /// memo : Enterだと復帰しないバグが発生する恐れあり
    /// </summary>
    private void OnCollisionStay(Collision col)
    {
        if (col.transform.tag == "Field")
        {
            if (_isReturn)
                return;

            if (!_isDamage)
                return;

            if (transform.position.y > 0.0f)
                return;

            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName("Base.Down"))
                return;

            if (_hp <= 0)
            {
                // 死亡処理
                GameOver.Instance.Run();
                _rigidBody.isKinematic = true;
                _animator.ResetTrigger("Return");
                _isReturn = true;
                Debug.Log("Death");
                return;
            }

            _animator.SetTrigger("Return");
            _isReturn = true;
            Debug.Log("Return");
        }
    }

    #endregion
}  