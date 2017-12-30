// ---------------------------------------------------------
// EnemyBase.cs
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
  
public class EnemyBase : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    [SerializeField] int _mainHp = 0;                   // 死亡までの敵HP

    protected int _nowStanHp = 0;                       // 気絶までの現在HP
    [SerializeField] int _stanHP = 0;                   // 気絶までの初期HP
    public bool IsStan { get; protected set; }          // 気絶フラグ

    public bool IsInvincible { get; protected set; }    // 無敵フラグ

    protected Animator _animator = null;

    #endregion

    #region method

    /// <summary>
    /// ダメージ処理
    /// </summary>
    public void Damage(int amount = 1)
    {
        if (IsStan || IsInvincible)
            return;

        if(_nowStanHp <= 0)
        {
            _nowStanHp = _stanHP;
        }

        _nowStanHp -= amount;

        if (_nowStanHp > 0)
            return;

        // スタン状態にする
        IsStan = true;
        _animator.SetBool("Stan", true);
    }

    /// <summary>
    /// 必殺技のダメージ処理
    /// </summary>
    public void SpecialDamage()
    {
        _mainHp--;
        IsStan = false;
        _animator.SetBool("Stan", false);
    }

    /// <summary>
    /// 死亡判定
    /// </summary>
    public bool Death()
    {
        return _mainHp <= 0;
    }

    #endregion

    #region unity_event     
    
    /// <summary>
    /// 初期化処理
    /// </summary>
    protected void Awake()
    {
        IsStan = false;
        IsInvincible = false;

        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        // DEBUG : デバッグコマンド 
        if (Input.GetKeyDown(KeyCode.Q))
        {
            IsStan = true;
            _animator.SetBool("Stan", true);
        }
    }

    #endregion
}