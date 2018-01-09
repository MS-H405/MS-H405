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

    #endregion

    #region variable

    [SerializeField] int _hp = 6;
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

    #endregion

    #region method  

    /// <summary>  
    /// ダメージ処理  
    /// </summary>  
    public void Damage(bool isDebug = false)
    {
        if (_isDamage)
            return;

        _hp--;
        DamageStan();
        _animator.SetTrigger("Damage");
        PlayerLifeManager.Instance.DamageEffect();

        if (isDebug)
        {
            int nowHp = _hp;
            while (_hp > 0)
            {
                _hp--;
                PlayerLifeManager.Instance.DamageEffect(nowHp - _hp);
            }
        }

        StaticCoroutine.Instance.StartStaticCoroutine(DamageWait());
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.Player_Damage);
        DamageRed.Instance.Run();
    }

    /// <summary>
    /// ダメージ時のスタン演出処理
    /// </summary>
    private void DamageStan()
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

        Vector3 velocity = -transform.forward * _backPower;
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
    }

    /// <summary>
    /// プレイヤーが地面についているかを判定
    /// </summary>
    public bool IsInput
    {
        get
        {
            AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
            if(!state.IsName("Base.Idle") && !state.IsName("Base.Walk") && !state.IsName("Base.RideBallMove"))
            {
                return false;
            }

            if(_playerMove.enabled)
            {
                return _playerMove.IsInput;
            }
            else
            {
                return _rideBallMove.IsInput;
            }
        }
    }

    #endregion

    #region unity_event  

    /// <summary> 
    /// 初期化処理  
    /// </summary>  
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _actionManager = GetComponent<ActionManager>();
        _playerMove = GetComponent<PlayerMove>();
        _rideBallMove = GetComponent<RideBallMove>();
        _rigidBody = GetComponent<Rigidbody>();
    }

    /// <summary> 
    /// 更新前処理  
    /// </summary>  
    private void Start()
    {
        // PlayerInputのUpdate
        EnemyBase enemyBase = EnemyManager.Instance.BossEnemy.GetComponent<EnemyBase>();
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
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
                    }
                    // 行動キャンセル
                    else if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetButtonDown("Cancel"))
                    {
                        _actionManager.Cancel();
                    }
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
            Damage(true);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _isDamage = !_isDamage;
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
    /// </summary>
    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Field")
        {
            if(_hp <= 0)
            {
                // 死亡処理
                GameOver.Instance.Run();
                _rigidBody.isKinematic = true;
                return;
            }

            if (!_isDamage)
                return;

            _animator.SetTrigger("Return");
        }
    }

    #endregion
}  