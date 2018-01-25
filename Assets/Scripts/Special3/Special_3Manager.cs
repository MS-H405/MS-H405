using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Special_3Manager : MonoBehaviour
{
	#region 定数

	#endregion

	#region 変数

	public enum State_Special3
	{
		JUG_EXPANSION,		// ピン展開
		CAMERAMOVE_1,		// ピン展開位置から、プレイヤー正面①へ
		CAMERAMOVE_2,		// プレイヤー正面①から、プレイヤー正面②へ
		CAMERAMOVE_3,		// プレイヤー正面②からデカピン投擲位置へ
		JUG_GOENEMY,		// ピン突撃
		CAMERAMOVE_4,		// 後方ジャンプ位置へ
		BACKJAMP,			// プレイヤー後方ジャンプ
		BAGPIPE,			// バグパイプ火の玉
		CENTERLINE,			// x=0のところで、炎の道ができるのを見る
		BALLIGNITION,		// プレイヤーの玉に着火
		TOTEMAPPEAREFFECT,	// バビロンエフェクト表示(SP2で追加)
		BALLRIDE,			// 玉に乗る(回転も)
		TOTEMPOKE,			// トーテム突き
		STOPCAMERA_1,		// 全てのオブジェクトが停止して、カメラが3カットくらいはいる	ズーム
		STOPCAMERA_2,		// 全てのオブジェクトが停止して、カメラが3カットくらいはいる	ズーム
		STOPCAMERA_3,		// 全てのオブジェクトが停止して、カメラが3カットくらいはいる	移動
		BALLMOVE,			// 玉が動き出す
		CAMERASHOULDER,		// カメラが敵の肩越しにワープ

		FIN					// 終了
	};

	public State_Special3 State;			// ステート変数
	bool bInitializ = true;					// 初期化フラグ(trueの時に初期化する)
	List<bool> bFlgs = new List<bool>();	// 各処理が終わったかどうかのフラグ

	[SerializeField] GameObject MainCameraObj;
	Special_3Camera cs_Camera;

	[SerializeField] GameObject Special_JugglingObj;
	SP_Jug cs_Jug;
	
	[SerializeField] GameObject PlayerObj;
	Special_3Player cs_Player;
	
	[SerializeField] GameObject BallObj;
	Special_2Ball	cs_Ball;

	[SerializeField] GameObject TotemPokeObj;
	Special_3Totem cs_Totem;

	[SerializeField] GameObject BagpipeFireObj;
	Special_3BagpipeFire cs_BagpipeFire;

	bool bSkip = true;

	#endregion


	// Use this for initialization
	void Start ()
	{
		State = State_Special3.JUG_EXPANSION;

		cs_Camera = MainCameraObj.GetComponent<Special_3Camera>();
		cs_Jug = Special_JugglingObj.GetComponent<SP_Jug>();
		cs_Player = PlayerObj.GetComponent<Special_3Player>();
		cs_Ball = BallObj.GetComponent<Special_2Ball>();
		cs_Totem = TotemPokeObj.GetComponent<Special_3Totem>();
		cs_BagpipeFire = BagpipeFireObj.GetComponent<Special_3BagpipeFire>();
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
			case State_Special3.JUG_EXPANSION:
				Jug_Expansion();
				break;

			case State_Special3.CAMERAMOVE_1:
				CameraMove1();
				break;

			case State_Special3.CAMERAMOVE_2:
				CameraMove2();
				break;

			case State_Special3.CAMERAMOVE_3:
				CameraMove3();
				break;

			case State_Special3.JUG_GOENEMY:
				Jug_GoEnemy();
				break;

			case State_Special3.CAMERAMOVE_4:
				CameraMove4();
				break;

			case State_Special3.BACKJAMP:
				BackJamp();
				break;

			case State_Special3.BAGPIPE:
				Bagpipe();
				break;

			case State_Special3.CENTERLINE:
				CenterLine();
				break;

			case State_Special3.BALLIGNITION:			// ここをとばす
				BallIgnition();
				break;

			case State_Special3.TOTEMAPPEAREFFECT:
				TotemAppearEffect();
				break;

			case State_Special3.BALLRIDE:
				BallRide();
				break;

			case State_Special3.TOTEMPOKE:
				TotemPoke();
				break;

			case State_Special3.STOPCAMERA_1:
				StopCamera_1();
				break;

			case State_Special3.STOPCAMERA_2:
				StopCamera_2();
				break;

			case State_Special3.STOPCAMERA_3:
				StopCamera_3();
				break;

			case State_Special3.BALLMOVE:
				BallMove();
				break;

			case State_Special3.CAMERASHOULDER:
				CameraShoulder();
				break;

			case State_Special3.FIN:
				Fin();
				break;
		}

		// スキップ処理
		if (Input.GetButtonDown("Atack") && bSkip && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.MovieFinish();
			bSkip = false;
		}
	}



	#region 必殺技3には関係なさそうなところ

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
			State = State_Special3.CAMERAMOVE_1;
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
			State = State_Special3.CAMERAMOVE_2;
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
			State = State_Special3.CAMERAMOVE_3;
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
			State = State_Special3.JUG_GOENEMY;
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
			State = State_Special3.CAMERAMOVE_4;
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
			State = State_Special3.BACKJAMP;
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
			cs_Player.ParentConf();				// 親子関係を設定する

			bInitializ = true;
			State = State_Special3.BAGPIPE;
		}
	}

	#endregion

	// バグパイプ
	private void Bagpipe()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;

			cs_BagpipeFire.Start_BagpipeFire();	// 火の玉発射
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Camera.LookAtFire();	// 火の玉を見る


		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			State = State_Special3.CENTERLINE;
		}
	}

	// 炎の道ができるのを見る
	private void CenterLine()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Camera.CenterLine();	// 炎の道ができるのを見る


		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			//State = State_Special3.BALLIGNITION;
			State = State_Special3.TOTEMAPPEAREFFECT;
		}
	}

	// プレイヤーの玉に着火
	private void BallIgnition()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Camera.BallIgnition();	// 玉に着火


		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
//			State = State_Special3.TOTEMAPPEAREFFECT;
			State = State_Special3.BALLRIDE;
		}
	}

	// バビロンエフェクト表示(SP2で追加)
	private void TotemAppearEffect()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Camera.TotemAppearEffect();

		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			State = State_Special3.BALLRIDE;
//			State = State_Special3.BAGPIPE;
		}
	}


	#region 必殺技3には関係なさそうなところ


	// 玉に乗る(回転も)
	private void BallRide()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Ball.StartRotation();		// 玉回転開始

		// プレイヤーもここで玉乗りモーションをさせる
		//cs_Camera.Camera_BackJamp();				// 一応ここでもカメラを追従させておく。


		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			State = State_Special3.TOTEMPOKE;
		}
	}

	// トーテム突き
	private void TotemPoke()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Totem.Poke();				// トーテム突き

		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			State = State_Special3.STOPCAMERA_1;
		}
	}

	// 全てのオブジェクトが停止して、カメラが3カットくらいはいる	ズーム
	private void StopCamera_1()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;
			Time.timeScale=0;		// カメラ以外すべて停止
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Camera.StopCamera_1();				// カメラズーム1

		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			State = State_Special3.STOPCAMERA_2;
		}
	}

	// 全てのオブジェクトが停止して、カメラが3カットくらいはいる	ズーム
	private void StopCamera_2()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Camera.StopCamera_2();				// カメラズーム2

		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			State = State_Special3.STOPCAMERA_3;
		}
	}

	// 全てのオブジェクトが停止して、カメラが3カットくらいはいる	移動
	private void StopCamera_3()
	{
		if (bInitializ)
		{
			InitializFlgs(1);
			bInitializ = false;
		}

		if (!bFlgs[0])
			bFlgs[0] = cs_Camera.StopCamera_3();				// カメラ移動

		// 終了判定
		if (CheckFlgs())
		{
			Time.timeScale = 1.0f;		// 停止解除
			bInitializ = true;
			State = State_Special3.BALLMOVE;
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

		cs_Totem.Poke();							// トーテム突き
		cs_Camera.Camera_GoEnemy();					// カメラを追従
		cs_Ball.Rotation();							// 玉回転

		// 終了判定
		if (CheckFlgs())
		{
			bInitializ = true;
			State = State_Special3.CAMERASHOULDER;
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
			State = State_Special3.FIN;
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

	#endregion
}
