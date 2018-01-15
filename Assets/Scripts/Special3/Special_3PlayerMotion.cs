using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Special_3PlayerMotion : MonoBehaviour
{
	#region 定数

	enum STATE_SPMOTION
	{
		JUG_SLOW,		// ゆっくりにする
		JUG_STOP,		// 一番振りかぶったところらへんで止める
		JUG_NORMAL,		// デカピン投げるために、普通のスピードにする
		BACKJAMP_START,	// 後方ジャンプ開始
		BACKJAMP_SETBOOL,	// 連続で後方ジャンプしないようにフラグを設定
		BALLWALK,		// 玉乗り歩きモーション

		FIN
	}


	const float CON_JUG_SLOW_TIME = 1.0f;		// ピン投げで、モーションがゆっくりになる時間
	const float CON_JUG_SLOW_SPEED = 0.28f;		// ピン投げで、モーションがゆっくりになった時のスピード
	const float CON_JUG_STOP_TIME = 2.0f;		// ピン投げで、モーションが止まる時間
	const float CON_JUG_NORMAL_TIME = 1.8f;		// ピン投げで、普通に動き出す時間

	const float CON_BALLWALK_TIME = 5.2f;		// 後方ジャンプモーションから玉乗りモーションに変えるまでの時間	2.1

	#endregion


	#region 変数

	[SerializeField]	GameObject NormalPlayerObj;
	[SerializeField]	GameObject BagpipePlayerObj;

	STATE_SPMOTION State_SPMotion = STATE_SPMOTION.JUG_SLOW; 
	Animator animator_Normal, animator_Bagpipe;
	float fTime = 0.0f;
	bool bInit = true;

	#endregion


	// Use this for initialization
	void Start ()
	{
		animator_Normal = NormalPlayerObj.GetComponent<Animator>();
		animator_Bagpipe = BagpipePlayerObj.GetComponent<Animator>();

		animator_Normal.speed = 1.0f;	// 最初は普通のスピード
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch(State_SPMotion)
		{
			case STATE_SPMOTION.JUG_SLOW:
				Jug_Slow();
				break;

			case STATE_SPMOTION.JUG_STOP:
				Jug_Stop();
				break;

			case STATE_SPMOTION.JUG_NORMAL:
				Jug_Normal();
				break;

			case STATE_SPMOTION.BACKJAMP_START:
				// このときはSpecial_1Playerから呼び出される
				break;

			case STATE_SPMOTION.BACKJAMP_SETBOOL:
				BackJamp_SetBool();
				break;

			case STATE_SPMOTION.BALLWALK:
				BallWalk();
				break;

			case STATE_SPMOTION.FIN:
				break;
		}
	}



	// ゆっくりにする
	private void Jug_Slow()
	{
		if (bInit)
		{
			fTime = 0.0f;
			bInit = false;
		}

		fTime += Time.deltaTime;
		if (fTime >= CON_JUG_SLOW_TIME)
		{
			animator_Normal.speed = CON_JUG_SLOW_SPEED;

			State_SPMotion = STATE_SPMOTION.JUG_STOP;
			bInit = true;
		}
	}

	// 一番振りかぶったところらへんで止める
	private void Jug_Stop()
	{
		if (bInit)
		{
			fTime = 0.0f;
			bInit = false;
		}

		fTime += Time.deltaTime;
		if (fTime >= CON_JUG_STOP_TIME)
		{
			animator_Normal.speed = 0.0f;

			State_SPMotion = STATE_SPMOTION.JUG_NORMAL;
			bInit = true;
		}
	}

	// デカピン投げるために、普通のスピードにする
	private void Jug_Normal()
	{
		if (bInit)
		{
			fTime = 0.0f;
			bInit = false;
		}

		fTime += Time.deltaTime;
		if (fTime >= CON_JUG_NORMAL_TIME)
		{
			animator_Normal.speed = 1.0f;

			State_SPMotion = STATE_SPMOTION.BACKJAMP_START;
			bInit = true;
		}
	}

	// 後方ジャンプを開始する時間
	public void Backjamp_Start()
	{
		animator_Normal.SetBool("bBackJamp", true);
		animator_Normal.speed = 0.35f;

		State_SPMotion = STATE_SPMOTION.BACKJAMP_SETBOOL;
	}

	// 連続で後方ジャンプしないようにフラグを設定
	private void BackJamp_SetBool()
	{
		if (bInit)
		{
			fTime = 0.0f;
			bInit = false;
		}

		fTime += Time.deltaTime;
		if (fTime >= 0.1f)
		{
			animator_Normal.SetBool("bBackJamp", false);

			State_SPMotion = STATE_SPMOTION.BALLWALK;
			bInit = true;
		}
	}

	// 連続で後方ジャンプしないようにフラグを設定
	private void BallWalk()
	{
		if (bInit)
		{
			fTime = 0.0f;
			bInit = false;
		}

		fTime += Time.deltaTime;
		if (fTime >= CON_BALLWALK_TIME)
		{
			animator_Normal.SetBool("bBallWalk", true);
			animator_Normal.speed = 2.0f;

			State_SPMotion = STATE_SPMOTION.FIN;
			bInit = true;
		}
	}
}
