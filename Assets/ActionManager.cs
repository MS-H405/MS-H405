// --------------------------------------------------------------------
// ActionManager.cs
// 概要 : 現在どの行動を選択しているか、行動中なのかどうかなどを管理
// 作成者: Shota_Obora
// --------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
  
public class ActionManager : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    private bool _isAction = false; // 現在何か行動中かどうか
    private int _nowAction = -1;    // 現在行動中のアクション番号

    private int _nowSelect = 0;     // 0.ジャグリング, 1.玉乗り, 2.トーテムジャンプ, 3.バグパイプ
    private int _maxSelect = 0;     // 初期化時にステージ番号を取得し、最大値を決定

    #endregion

    #region method

    /// <summary>
    /// 選択中の行動を状況に応じて処理する
    /// </summary>
    public void OnSelect()
    {
        // 行動中なら
        if (_isAction)
        {
            // 攻撃実行
            if (_nowAction == _nowSelect)
            {

            }
            // 行動変更
            else
            {
                _nowAction = _nowSelect;
            }
        }
        // なにも行動していないなら
        else
        {
            _isAction = true;
            _nowAction = _nowSelect;
        }
    }

    /// <summary>
    /// 行動中の行動をキャンセルする
    /// </summary>
    public void Cancle()
    {
        _isAction = false;
        _nowAction = -1;
    }

    /// <summary>
    /// 選択中の行動を変更
    /// </summary>
    public void Change(bool _isRight)
    {
        if(_isRight)
        {
            _nowSelect++;
            if(_nowSelect > _maxSelect)
            {
                _nowSelect = 0;
            }
        }
        else
        {
            _nowSelect--;
            if(_nowSelect < 0)
            {
                _nowSelect = _maxSelect;
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

    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {

    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update ()
    {

    }

    #endregion
}  