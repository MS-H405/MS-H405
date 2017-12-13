// ---------------------------------------------------------
// PlayerManager.cs
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
  
public class PlayerManager : SingletonMonoBehaviour<PlayerManager>
{
    #region define

    #endregion

    #region variable

    [SerializeField] Player _player = null;
    public Player Player { get { return _player; } private set { _player = value; } }

    #endregion

    #region method

    /// <summary>
    /// Playerの方を垂直に見るときの変数
    /// 垂直になるよう対象のY座標に合わせる
    /// </summary>
    public Vector3 GetVerticalPos(Vector3 target)
    {
        Vector3 pos = _player.transform.position;
        pos.y = target.y;
        return pos;
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        if (_player)
            return;

        // 登録が無ければ探索
        GameObject obj = GameObject.FindGameObjectWithTag("Player");

        if(!obj)
        {
            // 警告文 : これが出てたらゲーム成立しない
            Debug.LogError("Playerが存在しません");
            return;
        }

        _player = obj.GetComponent<Player>();
    }

    #endregion
}  