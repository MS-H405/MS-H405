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

    enum eActionType
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

    // TODO : 仮でここにプレハブを登録
    [SerializeField] GameObject _jugglingPrefab = null;

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
                GetComponent<PlayerMove>().enabled = true;
                GetComponent<RideBallMove>().enabled = false;
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
            _nowSelect++;
            if ((int)_nowSelect > _maxSelect)
            {
                _nowSelect = 0;
            }
        }
        else
        {
            _nowSelect--;
            if (_nowSelect < 0)
            {
                _nowSelect = (eActionType)_maxSelect;
            }
        }
    }

    /// <summary>
    /// 行動するアクションを変更
    /// </summary>
    public void ChangeAction()
    {
        _nowAction = _nowSelect;
        switch (_nowAction)
        {
            case eActionType.Juggling:

                break;

            case eActionType.RideBall:
                GetComponent<PlayerMove>().enabled = false;
                GetComponent<RideBallMove>().enabled = true;
                break;

            case eActionType.TotemJump:

                break;

            case eActionType.Bagpipe:

                break;

            default:
                break;
        }
    }

    /// <summary>
    /// 攻撃処理
    /// </summary> 
    private void OnAtack()
    {
        switch(_nowAction)
        {
            case eActionType.Juggling:
                GameObject obj = Instantiate(_jugglingPrefab, transform.position, transform.rotation);
                StartCoroutine(obj.GetComponent<JugglingAtack>().Run(EnemyManager.Instance.BossEnemy));
                break;

            case eActionType.RideBall:
                // 攻撃はない
                break;

            case eActionType.TotemJump:

                break;

            case eActionType.Bagpipe:

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
        Text selectText = GameObject.Find("SelectText").GetComponent<Text>();
        Text actionText = GameObject.Find("ActionText").GetComponent<Text>();
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                selectText.text = "選択してるの : " + _nowSelect;
                actionText.text = "行動してるの : " + _nowAction;
            });
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