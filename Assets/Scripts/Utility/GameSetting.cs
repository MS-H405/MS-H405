// ---------------------------------------------------------  
// GameSetting.cs  
// 概要 : ゲームの初期設定を行う   
// 作成者: Shota_Obora
// ---------------------------------------------------------  
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
  
public class GameSetting : MonoBehaviour   
{	
  	#region unity_event  
  	
    /// <summary>  
    /// 初期化処理  
    /// </summary>  
    void Awake()  
    {

#if !UNITY_EDITOR
        // Editor以外でLogを表示しない
        Debug.logger.logEnabled = false;
#endif

        // 設定終了後、破棄
        Destroy(gameObject);
    }

    #endregion
}  