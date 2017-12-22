//================================
//   Effekseerの制御スクリプト
//--------------------------------
// 制作者:和仁裕介
// エフェクトを再生させたい
// オブジェクトに付ける
//================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEffekseerObject : SingletonMonoBehaviour<SetEffekseerObject> 
{

    [SerializeField, Header("Effekseerのプレハブをセット\n")]
    List<GameObject> m_SetEffekseerObj = new List<GameObject>();

    // ゲットコンポ―ネーション用配列
    EffekseerEmitter[] m_Effect;

	// Use this for initialization
	void Start () 
    {
    	// リストにあるEffekseerのプレハブを全部取得
        List<GameObject> Effectlist = m_SetEffekseerObj;
        
        // リストにある数だけEffekseerEmitterを配列宣言
        m_Effect = new EffekseerEmitter[Effectlist.Count];
        for (int i = 0; i < Effectlist.Count; i++ )
        {
        	// プレハブのエフェクトの数だけ
        	// EffekseerEmitterをゲットコンポーネント
        	// ここで呼び出す番号を配列順に決める
            m_Effect[i] = Effectlist[i].GetComponent<EffekseerEmitter>();
        }
	}


    //********************************************************
    // 下の関数は連続して出すと最新のエフェクトしか作用しない
    //--------------------------------------------------------
    // 引数:no 「エフェクトの番号を渡す」
    // (1番目のエフェクトを再生したいならNewEffect(1)と宣言する)
    // updateの中身を参考にすること
    //********************************************************
    
    // セットしたエフェクトを生成
    // Newの理由は関数がPlayだが中身は生成の為
    // 自分がイメージしやすいためNewと記述
    public void NewEffect(int no)
    {
        m_Effect[no].Play();
    }
    
    // セットしたエフェクトを消去
    // Deleteの理由は関数がStopだが中身は消去の為
    // 自分がイメージしやすいためDeleteと記述
	public void DeleteEffect(int no)
    {
        m_Effect[no].Stop();
    }
    
    // セットしたエフェクトを再生
    // 停止している場合のみ有効
	public void StartEffect(int no)
    {
        m_Effect[no].paused = false;
    }
    
    // セットしたエフェクトを停止
    // 再生している場合のみ有効
	public void StopEffect(int no)
    {
        m_Effect[no].paused = true;
    }
}
