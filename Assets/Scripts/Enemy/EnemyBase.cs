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

    private int _mainHp = 0;        // 死亡までの敵HP

    private int _nowStanHp = 0;     // 気絶までの現在HP
    private int _stanHP = 0;        // 気絶までの初期HP
    protected bool _isStan = false; // 気絶フラグ

    #endregion

    #region method

    /// <summary>
    /// ダメージ処理
    /// </summary>
    public void Damage(int amount)
    {
        _nowStanHp -= amount;

        if (_nowStanHp > 0)
            return;

        // スタン状態にする
        _isStan = true;
        _nowStanHp = _stanHP;
    }

    /// <summary>
    /// 必殺技のダメージ処理
    /// </summary>
    public void SpecialDamage()
    {
        _mainHp--;
    }

    /// <summary>
    /// 死亡判定
    /// </summary>
    public bool Death()
    {
        return _mainHp <= 0;
    }

    #endregion
}