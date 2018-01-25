using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class BS_HermitCrab_PB : PlayableBehaviour
{
	#region 定数

	enum STATE_HERMITCRAB
	{
		WAIT,		// 待機
		WALK,		// 山の上を歩く
		ROTATE,		// プレイヤーのほうを見る
		JAMP,		// ジャンプ
		FALL,		// 落下
		ROAR,		// 咆哮

		FIN
	}

	const float CON_WAIT_TIME = 5.2f;			// 待機時間

	const float CON_WALK_TIME = 1.6f;			// 歩き時間
	readonly Vector3 CON_WALK_START_POS = new Vector3(42.92f, 22.31f, 39.4f);
	readonly Vector3 CON_WALK_END_POS = new Vector3(44.88f, 23.74f, 36.23f);

	const float CON_ROTATE_TIME = 1.5f;			// 回転時間
	const float CON_ROTATE_WAIT = 0.8f;			// 回転待機時間
	readonly Vector3 CON_ROTATE_START_EULAR = new Vector3(-20.3f, 148.0f, 0.0f);
	readonly Vector3 CON_ROTATE_END_EULAR = new Vector3(-0.4f, 230.0f, -13.4f);

	const float CON_JAMP_TIME = 2.0f;			// ジャンプ時間
	const float CON_JAMP_WAIT = 1.5f;			// 飛び跳ねるまでの時間
	const float CON_JAMP_SPEED = 90.0f;			// 1秒間に進む距離

	const float CON_FALL_TIME = 1.5f;			// 落下時間
	readonly Vector3 CON_FALL_START_POS = new Vector3(0.0f, 150.0f, 10.0f);
	readonly Vector3 CON_FALL_END_POS = new Vector3(0.0f, 0.0f, 10.0f);
	readonly Vector3 CON_FALL_START_EULAR = new Vector3(0.0f, 2000.0f, 0.0f);
	readonly Vector3 CON_FALL_END_EULAR = new Vector3(0.0f, 180.0f, 0.0f);

	const float CON_ROAR_TIME = 6.0f;			// 咆哮時間
	const float CON_ROAR_START = 3.0f;			// 咆哮開始時間
	const float CON_ROAR_STOP = 3.9f;			// 咆哮ストップ時間
	const float CON_ROAR_END = 5.4f;			// 咆哮終了時間
	const float CON_ROAR_EFFECT = 3.6f;			// 咆哮エフェクトを出す時間

	const float CON_FADE_TIME = 0.3f;			// シーン遷移し始める時間

	const float CON_SE_JAMP = 3.5f;		// ジャンプモーションに入ってからSEを鳴らすまでの時間

	#endregion


	#region 変数

	STATE_HERMITCRAB State = STATE_HERMITCRAB.WAIT;

	private GameObject _HermitCrabObj;
	public GameObject HermitCrabObj { get; set; }


	Transform transform;
	Animator animator;
	bool bInitializ = true;					// 初期化フラグ(trueの時に初期化する)

	float fTime = 0.0f;
	float fWait = 0.0f;
	bool bWait = true;

	bool bRoarStart, bRoarStop, bRoarEnd, bRoarEffect;
	bool bFade = true;

	// Effekseer
	private GameObject _EffekseerObj;
	public GameObject EffekseerObj { get; set; }
	SetEffekseerObject cs_SetEffekseerObject;

	bool bSE_Jamp = false;
	bool bSE_Jampd = false;
	float fJampTime = 0.0f;


	private GameObject _ShakeCameraObj;
	public GameObject ShakeCameraObj { get; set; }
	ShakeCamera cs_ShakeCamera;

	#endregion


	public override void OnGraphStart(Playable playable)
	{
		transform = HermitCrabObj.GetComponent<Transform>();
		animator = HermitCrabObj.GetComponent<Animator>();

		transform.position = CON_WALK_START_POS;
		transform.eulerAngles = CON_ROTATE_START_EULAR;

		cs_SetEffekseerObject = EffekseerObj.GetComponent<SetEffekseerObject>();
		cs_ShakeCamera = ShakeCameraObj.GetComponent<ShakeCamera>();
	}


	public override void OnGraphStop(Playable playable)
	{
		
	}


	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		
	}


	public override void OnBehaviourPause(Playable playable, FrameData info)
	{
		
	}


	public override void PrepareFrame(Playable playable, FrameData info)
	{
		// スキップ
		if (Input.GetKeyDown(KeyCode.Return) && bFade && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.STAGE_2);	// シーン遷移
			bFade = false;
		}





		switch(State)
		{
			case STATE_HERMITCRAB.WAIT:
				Wait();
				break;

			case STATE_HERMITCRAB.WALK:
				Walk();
				break;

			case STATE_HERMITCRAB.ROTATE:
				Rotate();
				break;

			case STATE_HERMITCRAB.JAMP:
				Jamp();
				break;

			case STATE_HERMITCRAB.FALL:
				Fall();
				break;

			case STATE_HERMITCRAB.ROAR:
				Roar();
				break;





			case STATE_HERMITCRAB.FIN:
				Fin();
				break;
		}

		SE_Jamp();
	}



	#region 関数

	// 待機
	private void Wait()
	{
		if (bInitializ)
		{
			fTime = 0.0f;
			bInitializ = false;
		}

		fTime += Time.deltaTime;

		// 処理を記述
		if(fTime >= CON_WAIT_TIME)
		{
			bInitializ = true;
			State = STATE_HERMITCRAB.WALK;
		}
	}


	// 歩く
	private void Walk()
	{
		if (bInitializ)
		{
			fTime = 0.0f;
			bInitializ = false;
		}

		fTime += Time.deltaTime;
		transform.position = Vector3.Lerp(CON_WALK_START_POS, CON_WALK_END_POS, Mathf.Clamp01(fTime / CON_WALK_TIME));


		if (fTime >= CON_WALK_TIME)
		{
			bInitializ = true;
			State = STATE_HERMITCRAB.ROTATE;

			animator.SetBool("bWait", true);
		}
	}


	// 回転
	private void Rotate()
	{
		if (bInitializ)
		{
			fTime = 0.0f;
			fWait = 0.0f;
			bWait = true;
			bInitializ = false;

			bSE_Jamp = true;
		}

		// 待機
		fWait += Time.deltaTime;
		if (fWait > CON_ROTATE_WAIT && bWait)
		{
			animator.SetBool("bWait", false);

			bWait = false;
		}
		if (bWait)
			return;

		// 処理を記述
		fTime += Time.deltaTime;
		transform.eulerAngles = Vector3.Lerp(CON_ROTATE_START_EULAR, CON_ROTATE_END_EULAR, Mathf.Clamp01(fTime / CON_ROTATE_TIME));


		if (fTime >= CON_ROTATE_TIME)
		{
			bInitializ = true;
			State = STATE_HERMITCRAB.JAMP;

			//animator.SetBool("bWait", true);
		}
	}


	// ジャンプ
	private void Jamp()
	{
		if (bInitializ)
		{
			fTime = 0.0f;
			fWait = 0.0f;
			bWait = true;
			bInitializ = false;

			animator.SetBool("bJamp", true);
		}

		// 処理を記述
		fTime += Time.deltaTime;

		// 待機
		fWait += Time.deltaTime;
		if (fWait > CON_JAMP_WAIT && bWait)
		{
			animator.SetBool("bWait", false);

			bWait = false;
		}
		if (bWait)
			return;
		transform.position += transform.up * CON_JAMP_SPEED * Time.deltaTime;


		if (fTime >= CON_JAMP_TIME)
		{
			bInitializ = true;
			State = STATE_HERMITCRAB.FALL;
		}
	}


	// 落下
	private void Fall()
	{
		if (bInitializ)
		{
			fTime = 0.0f;
			bInitializ = false;
		}

		fTime += Time.deltaTime;
		transform.position = Vector3.Lerp(CON_FALL_START_POS, CON_FALL_END_POS, Mathf.Clamp01(fTime / CON_FALL_TIME));
		transform.eulerAngles = Vector3.Lerp(CON_FALL_START_EULAR, CON_FALL_END_EULAR, Mathf.Clamp01(fTime / CON_FALL_TIME));

		if (fTime >= CON_FALL_TIME)
		{
			bInitializ = true;
			State = STATE_HERMITCRAB.ROAR;

			cs_SetEffekseerObject.NewEffect(0);
            MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.SP_Blast);

			// 画ぶれ
			cs_ShakeCamera.SetParam(0.06f, 0.002f);
			cs_ShakeCamera.DontMoveShake();
		}
	}


	// 咆哮
	private void Roar()
	{
		if (bInitializ)
		{
			fTime = 0.0f;
			bRoarStart = true;
			bRoarStop = true;
			bRoarEnd = true;
			bRoarEffect = true;
			bInitializ = false;
		}

		// 処理を記述
		fTime += Time.deltaTime;
		if (fTime > CON_ROAR_START && bRoarStart)			// 咆哮開始
		{
			animator.SetBool("bRoar", true);
			animator.speed = 0.4f;

			bRoarStart = false;
		}
		else if (fTime > CON_ROAR_STOP && bRoarStop)		// 咆哮ストップ
		{
			animator.speed = 0.1f;

			bRoarStop = false;
		}
		else if (fTime > CON_ROAR_END && bRoarEnd)			// 咆哮終了
		{
			animator.speed = 1.0f;

			bRoarEnd = false;
		}

		if(fTime > CON_ROAR_EFFECT && bRoarEffect)
		{
			cs_SetEffekseerObject.NewEffect(1);

			MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.BS_Volcano);
			MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.BS_Cry);

			bRoarEffect = false;
		}

		if (fTime >= CON_ROAR_TIME)
		{
			bInitializ = true;
			State = STATE_HERMITCRAB.FIN;
		}
	}


	// シーン遷移
	private void Fin()
	{
		if (bInitializ)
		{
			fTime = 0.0f;
			bInitializ = false;
		}

		fTime += Time.deltaTime;

		// 処理を記述
		if (fTime >= CON_FADE_TIME && bFade && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.STAGE_2);
			bFade = false;
		}
	}

	#endregion


	#region 効果音

	void SE_Jamp()
	{
		if(!bSE_Jamp || bSE_Jampd)
			return;

		fJampTime += Time.deltaTime;
		if(fJampTime >= CON_SE_JAMP)
		{
			MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.BS_Jamp);	// ヤドカリジャンプ音
			bSE_Jampd = true;
		}
	}

	#endregion
}
