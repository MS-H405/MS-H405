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

    [SerializeField] GameObject _bossEnemy = null;
    public GameObject BossEnemy
    {
        get
        {
            if (!_isActive)
                return null;

            return _bossEnemy;
        }
        private set { _bossEnemy = value; }
    }

    private bool _isActive = true;
    public bool Active { get { return _isActive; } set { _isActive = value; } }

    #endregion

    #region method

    #endregion
}  