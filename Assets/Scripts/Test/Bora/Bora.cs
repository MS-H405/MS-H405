// ---------------------------------------------------------  
// Bora.cs  
// 
// 作成者: Shota_Obora
// ---------------------------------------------------------  
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
  
// TODO : 名前は仮。一旦プレイヤーの機能を全て抽出し、抜き出しながら分解する
public class Bora : MonoBehaviour   
{
    #region define

    #endregion

    #region variable

    private ActionManager _actionManager = null;

    #endregion

    #region method  

    #endregion

    #region unity_event  

    /// <summary>  
    /// 初期化処理  
    /// </summary>  
    void Awake()
    {
        _actionManager = GetComponent<ActionManager>();
    }  
  
    /// <summary>  
    /// 更新前処理  
    /// </summary>  
    void Start ()   
    {  
      
    }  
      
    /// <summary>  
    /// 更新処理  
    /// </summary>  
    void Update ()   
    {
        // 攻撃選択＆選択時攻撃
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _actionManager.OnSelect();
        }
        // 行動キャンセル
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            _actionManager.Cancel();
        }
        // 武器スロット右回り
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _actionManager.ChangeSelect(true);
        }
        // 武器スロット左回り
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _actionManager.ChangeSelect(false);
        }
    }

    #endregion
}  