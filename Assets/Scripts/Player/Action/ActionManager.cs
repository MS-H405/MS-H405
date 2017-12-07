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

    public enum eActionType
    {
        Juggling = 0,
        RideBall,
        TotemJump,
        Bagpipe,
        Max,
    };

    #endregion

    #region variable

    private bool _isAction = false;                     // 現在何か行動中かどうか
    private eActionType _nowAction = eActionType.Max;   // 現在行動中のアクション番号

    private eActionType _nowSelect = (eActionType)0;    // 0.ジャグリング, 1.玉乗り, 2.トーテムジャンプ, 3.バグパイプ
    private int _maxSelect = 3;                         // 初期化時にステージ番号を取得し、最大値を決定

    // ここに生成するプレハブなどを登録
    [SerializeField] GameObject _jugglingPrefab = null;
    private PlayerMove _playerMove = null;
    private RideBallMove _rideBallMove = null;
    private TotemJump _totemJump = null;
    [SerializeField] GameObject _bagpipeFirePrefab = null;

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
                OnAtack();
            }
            // 行動変更
            else
            {
                Cancel();   // 先にキャンセル
                ChangeAction();
            }
        }
        // なにも行動していないなら
        else
        {
            ChangeAction();
        }
    }

    /// <summary>
    /// 行動中の行動をキャンセルする
    /// </summary>
    public void Cancel()
    {
        _isAction = false;

        switch(_nowAction)
        {
            case eActionType.Juggling:

                break;

            case eActionType.RideBall:
                _rideBallMove.End();
                break;

            case eActionType.TotemJump:

                break;

            case eActionType.Bagpipe:

                break;

            default:
                break;
        }

        _nowAction = eActionType.Max;
    }

    /// <summary>
    /// 選択中の行動を変更
    /// </summary>
    public void ChangeSelect(bool _isRight)
    {
        if (_isRight)
        {
            _nowSelect--;
            if (_nowSelect < 0)
            {
                _nowSelect = (eActionType)_maxSelect;
            }
        }
        else
        {
            _nowSelect++;
            if ((int)_nowSelect > _maxSelect)
            {
                _nowSelect = 0;
            }
        }
        //Debug.Log(_nowSelect);
    }

    /// <summary>
    /// 行動するアクションを変更
    /// </summary>
    public void ChangeAction()
    {
        _isAction = true;
        _nowAction = _nowSelect;
        switch (_nowAction)
        {
            case eActionType.Juggling:
                break;

            case eActionType.RideBall:
                _playerMove.enabled = false;
                _rideBallMove.enabled = true;
                break;

            case eActionType.TotemJump:
                break;

            case eActionType.Bagpipe:
                break;

            default:
                break;
        }

        // 選択した瞬間に攻撃する?
        //OnAtack();
    }

    /// <summary>
    /// 攻撃処理
    /// </summary> 
    private void OnAtack()
    {
        switch(_nowAction)
        {
            case eActionType.Juggling:
                if (JugglingAtack.NowJugglingAmount >= 3)
                    return;

                GameObject jug = Instantiate(_jugglingPrefab, transform.position, transform.rotation);
                jug.GetComponent<JugglingAtack>().Run(EnemyManager.Instance.BossEnemy);
                break;

            case eActionType.RideBall:
                // 攻撃はない
                break;

            case eActionType.TotemJump:
                StaticCoroutine.Instance.StartStaticCoroutine(_totemJump.Run());
                break;

            case eActionType.Bagpipe:
                GameObject bag = Instantiate(_bagpipeFirePrefab, transform.position + new Vector3(0,1,0), transform.rotation);
                bag.GetComponent<BagpipeFire>().Run(transform.forward);
                break;

            default:
                break;
        }
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        _playerMove = GetComponent<PlayerMove>();
        _rideBallMove = GetComponent<RideBallMove>();
        _totemJump = GetComponent<TotemJump>();

        // ステージ番号を取得
        _maxSelect = StageData.Instance.StageNumber;
    }

    #endregion
}  