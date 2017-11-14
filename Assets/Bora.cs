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

    #endregion

    #region method  

    #endregion

    #region unity_event  

    /// <summary>  
    /// 初期化処理  
    /// </summary>  
    void Awake()
    {

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

#if !UNITY_EDITOR
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
#endif
        // 攻撃選択＆選択時攻撃
        if (Input.GetKeyDown(KeyCode.Return))
        {

        }
        // 行動キャンセル
        if (Input.GetKeyDown(KeyCode.Backspace))
        {

        }
        // 武器スロット右回り
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {

        }
        // 武器スロット左回り
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

        }

    }  
  
#endregion
}  