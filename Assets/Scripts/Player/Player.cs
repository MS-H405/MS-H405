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
    private Rigidbody _rigidBody = null;
    [SerializeField] float _backPower = 75.0f;
    [SerializeField] float _upPower = 200.0f;

    #endregion

    #region method  

    /// <summary>  
    /// ダメージ処理  
    /// </summary>  
    public void Damage()
    {
        _hp--;
        DamageStan();
        PlayerLifeManager.Instance.DamageEffect();

        if (_hp > 0)
            return;

        // TODO : 死亡処理 
    }

    /// <summary>
    /// ダメージ時のスタン演出処理
    /// </summary>
    private void DamageStan()
    {
        // 玉乗り中なら玉乗りを強制キャンセル
        if(_rideBallMove.enabled)
        {
            _actionManager.Cancel();
            _rideBallMove.End();
        }
        else
        {
            _playerMove.OnRigor();
        }

        Vector3 velocity = -transform.forward * _backPower;
        velocity.y = _upPower;
        _rigidBody.AddForce(velocity);

    }

    #endregion

    #region unity_event  

    /// <summary> 
    /// 初期化処理  
    /// </summary>  
    private void Awake()
    {
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
                if(enemyBase.IsStan)
                {
                    // 必殺技実行
                    if(Input.GetKeyDown(KeyCode.Return))
                    {
                        enemyBase.SpecialDamage();
                        MovieManager.Instance.MovieStart(MovieManager.MOVIE_SCENE.SPECIAL_1);
                    }
                }
                else
                {
                    // 攻撃選択＆選択時攻撃
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        _actionManager.OnSelect();
                    }
                    // 行動キャンセル
                    if (Input.GetKeyDown(KeyCode.Backspace))
                    {
                        _actionManager.Cancel();
                    }
                    // 武器スロット右回り
                    if (Input.GetKeyDown(KeyCode.RightArrow) && AtackIconManager.Instance.IsChange)
                    {
                        _actionManager.ChangeSelect(true);
                        AtackIconManager.Instance.Rot(true);
                    }
                    // 武器スロット左回り
                    if (Input.GetKeyDown(KeyCode.LeftArrow)  && AtackIconManager.Instance.IsChange)
                    { 
                        _actionManager.ChangeSelect(false);
                        AtackIconManager.Instance.Rot(false);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    Damage();
                }
            });
    }

    #endregion
}  