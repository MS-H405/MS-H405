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

    // TODO : 変更したい
    static public GameObject instance = null;

    #endregion

    #region method  

    /// <summary>  
    /// ダメージ処理  
    /// </summary>  
    public void Damage()
    {
        _hp--;
        PlayerLifeManager.Instance.DamageEffect();

        if (_hp > 0)
            return;

        // TODO : 死亡処理 
    }

    #endregion

    #region unity_event  

    /// <summary> 
    /// 初期化処理  
    /// </summary>  
    private void Awake()
    {
        _actionManager = GetComponent<ActionManager>();
        instance = gameObject;

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
                    PlayerLifeManager.Instance.DamageEffect();
                }
            });
    }

    #endregion
}  