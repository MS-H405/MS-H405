﻿using System.Collections;
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
		CAMERAMOVE_1,		// ピン展開位置から、プレイヤー正面①へ
		CAMERAMOVE_2,		// プレイヤー正面①から、プレイヤー正面②へ
		CAMERAMOVE_3,		// プレイヤー正面②からデカピン投擲位置へ
		JUG_GOENEMY,		// ピン突撃
		CAMERAMOVE_4,		// 後方ジャンプ位置へ
		BACKJAMP,			// プレイヤー後方ジャンプ
		BALLRIDE,			// 玉に乗る(回転も)
		BALLMOVE,			// 玉が動き出す
		CAMERASHOULDER,		// カメラが敵の肩越しにワープ

		FIN					// 終了
	};

	public State_Special1 State;			// ステート変数
	bool bInitializ = true;					// 初期化フラグ(trueの時に初期化する)
	List<bool> bFlgs = new List<bool>();	// 各処理が終わったかどうかのフラグ

	[SerializeField] GameObject MainCameraObj;
	Special_1Camera cs_Camera;

	[SerializeField] GameObject Special_JugglingObj;
	SP_Jug cs_Jug;
	
	[SerializeField] GameObject PlayerObj;
	Special_1Player cs_Player;
	
	[SerializeField] GameObject BallObj;
	Special_1Ball	cs_Ball;

	bool bSkip = true;

	#endregion


	// Use this for initialization
	void Start ()
	{
		State = State_Special1.JUG_EXPANSION;

		cs_Camera = MainCameraObj.GetComponent<Special_1Camera>();
		cs_Jug = Special_JugglingObj.GetComponent<SP_Jug>();
		cs_Player = PlayerObj.GetComponent<Special_1Player>();
		cs_Ball = BallObj.GetComponent<Special_1Ball>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		// ----- デバッグ -----

		if (Input.GetKeyDown(KeyCode.S))
		{
			// キャンバスにエフェクト出現
		}


		// --------------------

		switch(State)
		{
			case State_Special1.JUG_EXPANSION:
				Jug_Expansion();
				break;

			case State_Special1.CAMERAMOVE_1:
				CameraMove1();
				break;

			case State_Special1.CAMERAMOVE_2:
				CameraMove2();
				break;

			case State_Special1.CAMERAMOVE_3:
				CameraMove3();
				break;

			case State_Special1.JUG_GOENEMY:
				Jug_GoEnemy();
				break;

			case State_Special1.CAMERAMOVE_4:
				CameraMove4();
				break;

			case State_Special1.BACKJAMP:
				BackJamp();
				break;

			case State_Special1.BALLRIDE:
				BallRide();
				break;

			case State_Special1.BALLMOVE:
				BallMove();
				break;

			case State_Special1.CAMERASHOULDER:
				CameraShoulder();
				break;

			case State_Special1.FIN:
				Fin();
				break;
		}

		// スキップ処理
		if (Input.GetKeyDown(KeyCode.Return) && bSkip && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.MovieFinish();
			bSkip = false;
		}
	}





	// ピン展開
	private void Jug_Expansion()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Jug.Jug_ThrowExpansion();	// ピン展開

		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			State = State_Special1.CAMERAMOVE_1;
		}
	}

	// カメラ移動1
	private void CameraMove1()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Camera.CameraMove_1();	// カメラ移動

		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			State = State_Special1.CAMERAMOVE_2;
		}
	}

	// カメラ移動2
	private void CameraMove2()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Camera.CameraMove_2();

		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			State = State_Special1.CAMERAMOVE_3;
		}
	}

	// カメラ移動3
	private void CameraMove3()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Camera.CameraMove_3();

		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			State = State_Special1.JUG_GOENEMY;
		}
	}

	// ピン突撃
	private void Jug_GoEnemy()
	{
		if (bInitializ)
		{
			InitializFlgs(2);
			bInitializ = false;

			cs_Jug.JugBig_Throw();	// デカピン切り離し
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Jug.GoEnemy();			// ピン突撃
		if (!bFlgs[1])
			bFlgs[1] = cs_Jug.GoEnemy_Big();		// デカピン突撃

		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			State = State_Special1.CAMERAMOVE_4;
		}
	}

	// カメラ移動4
	private void CameraMove4()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Camera.CameraMove_4();

		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			State = State_Special1.BACKJAMP;
		}
	}

	// プレイヤー後方ジャンプ
	private void BackJamp()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;

			cs_Ball.BallAppear();		// ボール出現
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Player.BackJamp();

		cs_Camera.Camera_BackJamp();			// プレイヤーの後方ジャンプが終わるまで続ける


		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			State = State_Special1.BALLRIDE;
		}
	}

	// 玉に乗る(回転も)
	private void BallRide()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;

			cs_Player.ParentConf();					// 親子関係を設定する
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Ball.StartRotation();		// 玉回転開始

		// プレイヤーもここで玉乗りモーションをさせる
		cs_Camera.Camera_BackJamp();				// 一応ここでもカメラを追従させておく。


		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			State = State_Special1.BALLMOVE;
		}
	}

	// 玉動き出す
	private void BallMove()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Player.GoEnemy();			// プレイヤー(と玉)発射

		cs_Camera.Camera_GoEnemy();					// カメラを追従
		cs_Ball.Rotation();							// 玉回転

		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			State = State_Special1.CAMERASHOULDER;
		}
	}

	// カメラ敵の肩越しに移動
	private void CameraShoulder()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Player.GoEnemy();			// プレイヤー(と玉)発射

		cs_Camera.CameraShoulder();					// 敵の肩越しに移動
		cs_Ball.Rotation();							// 玉回転

		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			State = State_Special1.FIN;
		}
	}

	// カメラ敵の肩越しに移動
	private void Fin()
	{
		if (bInitializ && bSkip && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.MovieFinish();
			bInitializ = false;
		}

		cs_Player.GoEnemy();						// プレイヤー(と玉)発射
		cs_Camera.CameraShoulder();					// 敵の肩越しに移動
		cs_Ball.Rotation();							// 玉回転
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
