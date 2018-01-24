using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class BD_HermitCrab_PB : PlayableBehaviour
{
	#region 定数

	enum STATE_HERMITCRAB_DEATH
	{
		WAIT,		// 待機
		ROAR,		// 咆哮エフェクト　＆　色を変える
		FADE,		// パワーアップエフェクト　＆　透明になる
		VANISH,		// 消える

		FIN
	}

	const float CON_WAIT_TIME = 0.7f;		// 待機時間（アニメーションは勝手に進行している）

	const float CON_ROAR_TIME = 1.8f;		// State.ROARに入ってから、色が変わりきるまでの時間
	readonly Color CON_ROAR_COLOR = new Color(90.0f / 255.0f, 90.0f / 255.0f, 90.0f / 255.0f, 1.0f);

	const float CON_FADE_WAIT = 1.0f;		// エフェクトを出すまでの待ち時間
	const float CON_FADE_TIME = 1.5f;		// 透明化が完了するまでの時間
	readonly Color CON_FADE_COLOR = new Color(90.0f / 255.0f, 90.0f / 255.0f, 90.0f / 255.0f, 0.7f);

	const float CON_VANISH_TIME = 2.2f;		// 消えるまでの時間

	#endregion


	#region 変数

	STATE_HERMITCRAB_DEATH State = STATE_HERMITCRAB_DEATH.WAIT;

	private GameObject _HermitCrabObj;
	public GameObject HermitCrabObj { get; set; }
	private Material _mat;
	public Material mat { get; set; }

	Transform transform;

	bool bInitializ = true;					// 初期化フラグ(trueの時に初期化する)
	float fTime = 0.0f;
	float fWait = 0.0f;
	bool bWait = true;


	// Effekseer
	private GameObject _EffekseerObj;
	public GameObject EffekseerObj { get; set; }
	SetEffekseerObject cs_SetEffekseerObject;

	#endregion


	public override void OnGraphStart(Playable playable)
	{
		transform = HermitCrabObj.GetComponent<Transform>();
		cs_SetEffekseerObject = EffekseerObj.GetComponent<SetEffekseerObject>();

		mat.color = Color.white;
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
		switch(State)
		{
			case STATE_HERMITCRAB_DEATH.WAIT:
				Wait();
				break;

			case STATE_HERMITCRAB_DEATH.ROAR:
				Roar();
				break;

			case STATE_HERMITCRAB_DEATH.FADE:
				Fade();
				break;

			case STATE_HERMITCRAB_DEATH.VANISH:
				Vanish();
				break;



			case STATE_HERMITCRAB_DEATH.FIN:
				break;
		}
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
		if (fTime >= CON_WAIT_TIME)
		{
			State = STATE_HERMITCRAB_DEATH.ROAR;
			bInitializ = true;
		}
	}

	// 咆哮エフェクト　＆　色変化
	private void Roar()
	{
		if (bInitializ)
		{
			fTime = 0.0f;
			bInitializ = false;

			cs_SetEffekseerObject.NewEffect(0);		// 咆哮エフェクト
			MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.BD_CryLast);
		}

		fTime += Time.deltaTime;
		mat.color = Color.Lerp(Color.white, CON_ROAR_COLOR, Mathf.Clamp01(fTime / CON_ROAR_TIME));

		// 処理を記述
		if (fTime >= CON_ROAR_TIME)
		{
			State = STATE_HERMITCRAB_DEATH.FADE;
			bInitializ = true;
		}
	}

	// パワーアップエフェクト　＆　透明になる
	private void Fade()
	{
		if (bInitializ)
		{
			fTime = 0.0f;
			fWait = 0.0f;
			bWait = true;
			bInitializ = false;
		}

		// 待機処理
		fWait += Time.deltaTime;
		if (fWait > CON_FADE_WAIT && bWait)
		{
			cs_SetEffekseerObject.NewEffect(1);		// パワーアップエフェクト
			MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.TS_BossDeath);	// 敵がパーティクルになる音

			bWait = false;
		}
		if (bWait)
			return;


		// 透明化処理
		fTime += Time.deltaTime;
		mat.color = Color.Lerp(CON_ROAR_COLOR, CON_FADE_COLOR, Mathf.Clamp01(fTime / CON_FADE_TIME));

		// 処理を記述
		if (fTime >= CON_FADE_TIME)
		{
			State = STATE_HERMITCRAB_DEATH.VANISH;
			bInitializ = true;
		}
	}

	// 消える
	private void Vanish()
	{
		if (bInitializ)
		{
			fTime = 0.0f;
			fWait = 0.0f;
			bWait = true;
			bInitializ = false;

			cs_SetEffekseerObject.NewEffect(2);	// 消えるエフェクト。時間的にちょうどいいので初期化処理で実行
		}

		fTime += Time.deltaTime;
		
		// 処理を記述
		if (fTime >= CON_VANISH_TIME)
		{
			mat.color = Color.clear;	// 完全に透明にする

			MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.TS_BossFlush);	// 敵が消える音

			State = STATE_HERMITCRAB_DEATH.FIN;
			bInitializ = true;
		}
	}

	#endregion
}
