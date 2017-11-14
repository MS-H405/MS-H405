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

    [SerializeField] float _speed_Sec = 2.0f;
    [SerializeField] GameObject _enemy = null;  // TODO : Enemy取得どうする？
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
//#if !UNITY_EDITOR
        if (Input.GetKey(KeyCode.W))
        {                                          
            transform.position += transform.forward * Time.deltaTime * _speed_Sec;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * Time.deltaTime * _speed_Sec;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * Time.deltaTime * _speed_Sec;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * Time.deltaTime * _speed_Sec;
        }
//#endif
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
            _actionManager.Change(true);
        }
        // 武器スロット左回り
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _actionManager.Change(false);
        }

        // 敵の向きを常に見る
        transform.LookAt(new Vector3 (_enemy.transform.position.x, transform.position.y, _enemy.transform.position.z));
    }  
  
#endregion
}  