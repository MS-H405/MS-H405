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

    private Player _player = null;
    public Player Player { get { return _player; } private set { _player = value; } }
    public GameObject PlayerObj { get { return _player.gameObject; } }

    private Transform _playerRightHand = null;
    public Transform PlayerRightHand { get { return _playerRightHand; } }
    private Transform _playerLeftHand = null;
    public Transform PlayerLeftHand { get { return _playerLeftHand; } }

    private Animator _anim = null;
    public Animator Anim { get { return _anim; } private set { _anim = value; } }

    private ActionManager _actionManager = null;

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

    /// <summary>
    /// Playerの前方の座標を返す
    /// </summary>
    public Vector3 GetPlayerForward()
    {
        return _player.transform.position + _player.transform.forward;
    }

    /// <summary>
    /// 現在の行動が指定された行動中かを判定
    /// </summary>
    public bool CheckActionType(ActionManager.eActionType type)
    {
        return _actionManager.NowAction == type;
    }

    /// <summary>
    /// 現在ダメージアニメーション中かを返す
    /// </summary>
    public bool DamageAnimation()
    {
        AnimatorStateInfo animStateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        return animStateInfo.IsName("Base.Down") || animStateInfo.IsName("Base.Up");
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        // エラー回避
        if (!_player)
        {
            // 登録が無ければ探索
            GameObject obj = GameObject.FindGameObjectWithTag("Player");

            if (!obj)
            {
                // 警告文 : これが出てたらゲーム成立しない
                Debug.LogError("Playerが存在しません");
                return;
            }

            _player = obj.GetComponent<Player>();
        }

        // 初期化処理
        _anim = _player.GetComponent<Animator>();
        _actionManager = _player.GetComponent<ActionManager>();

        List<GameObject> allChild = GetAllChildren.GetAll(_player.gameObject);
        _playerLeftHand = allChild.Where(_ => _.name == "USER_20171210_1546:USER_20171210_1546:Character1_LeftForeArm").FirstOrDefault().transform;
        _playerRightHand = allChild.Where(_ => _.name == "USER_20171210_1546:USER_20171210_1546:Character1_RightForeArm").FirstOrDefault().transform;
    }

    #endregion
}  