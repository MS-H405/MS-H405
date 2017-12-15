// ---------------------------------------------------------
// EnemyManager.cs
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
  
public class EnemyManager : SingletonMonoBehaviour<EnemyManager>
{
    #region define

    #endregion

    #region variable

    private EnemyBase _bossEnemy = null;
    public EnemyBase BossEnemy
    {
        get
        {
            if (!_isActive && !_bossEnemy.IsStan)
                return null;

            return _bossEnemy;
        }
        private set { _bossEnemy = value; }
    }

    private bool _isActive = true;
    public bool Active { get { return _isActive; } set { _isActive = value; } }

    #endregion

    #region method

    /// <summary>
    /// 死亡判定
    /// </summary>
    public bool Death()
    {
        return _bossEnemy.Death();
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        if (_bossEnemy)
            return;

        // 登録が無ければ探索
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject obj in objs)
        {
            if (!obj.GetComponent<EnemyBase>())
                continue;

            _bossEnemy = obj.GetComponent<EnemyBase>();
            return;
        }

        // 警告文 : これが出てたらゲーム成立しない
        Debug.LogError("Bossが存在しません");
    }

    #endregion
}  
