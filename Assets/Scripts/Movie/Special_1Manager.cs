using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Special_1Manager : MonoBehaviour
{
	#region 定数

	#endregion

	#region 変数

	public enum State_Special1
	{
		JUG_EXPANSION,		// ピン展開

		FIN					// 終了
	};

	public State_Special1 State = State_Special1.JUG_EXPANSION;			// ステート変数
	bool bInitializ = true;											// 初期化フラグ(trueの時に初期化する)
	List<bool> bFlgs = new List<bool>();							// 各処理が終わったかどうかのフラグ


	SP_Jug cs_SP_Jug;

	#endregion


	// Use this for initialization
	void Start ()
	{
		cs_SP_Jug = GameObject.Find("Player").GetComponent<SP_Jug>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			cs_SP_Jug.Jug_ThrowExpansion();
		}

		switch(State)
		{
			case State_Special1.JUG_EXPANSION:
				Jug_Expansion();
				break;
		}
	}



	private void Jug_Expansion()
	{
		if (bInitializ)
		{
			InitializFlgs(1);	//	このステートで使うフラグの数に初期化しておく
			bInitializ = false;	// 初期化終了

			cs_SP_Jug.Jug_ThrowExpansion();		// ピン展開
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_SP_Jug.Update_Expansion();	// ピン展開移動

		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;						// 初期化可能状態にする
			State = State_Special1.FIN;				// 先頭キャラ移動待ち状態へ
		}
	}





	// bFlgsの要素数をnum個にして、falseで初期化する
	private void InitializFlgs(int num)
	{
		bFlgs.Clear();		// 中身をすべて削除

		for (int i = 0; i < num; i++)
			bFlgs.Add(false);
	}

	// bFlgsの中身が全部tureかどうかを判定する
	private bool CheckFlgs()
	{
		for (int i = 0; i < bFlgs.Count; i++)
		{
			if (!bFlgs[i])
				return false;
		}

		return true;
	}
}
