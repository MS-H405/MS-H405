﻿// ---------------------------------------------------------
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

    protected bool _isRigor = false;    // 硬直判定

    #endregion

    #region method

    /// <summary>
    /// 移動処理
    /// </summary>
    protected virtual void Move()
    {
        // 硬直時は処理しない
        if (_isRigor)
            return;

        transform.position += _moveAmount * Time.deltaTime;
    }

    /// <summary>
    /// 加速処理
    /// </summary>
    protected virtual void Acceleration(eDirection dir)
    {
        // 硬直時は処理しない
        if (_isRigor)
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
    /// 硬直処理
    /// </summary>
    public void OnRigor()
    {
        _isRigor = true;
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update ()
    {
        // 入力判定に応じて加速
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.W))
        {
            Acceleration(eDirection.Forward);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Acceleration(eDirection.Back);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Acceleration(eDirection.Right);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Acceleration(eDirection.Left);
        }
#endif

        Move();
        Deceleration();

        if (!EnemyManager.Instance.BossEnemy)
            return;

        Vector3 enemyPos = EnemyManager.Instance.BossEnemy.transform.position;
        transform.LookAt(new Vector3(enemyPos.x, transform.position.y, enemyPos.z));
    }

    /// <summary>
    /// 地形との接地判定処理
    /// </summary>
    protected virtual void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Field")
        {
            _isRigor = false;
        }
    }

    #endregion
}  