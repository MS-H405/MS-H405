using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class TS_Boss_PB : PlayableBehaviour
{
	#region 定数

	private enum _TSBOSSMODE
	{
		FIRSTWAIT,		// 待ち時間
		APPEAR,			// 生える
		SECONDWAIT,		// 待ち時間2(咆哮)
		BACK,			// ひっこむ

		FIN
	}

	const float CON_FIRSTSHAKE_TIME = 0.4f;	// ちびトーテム登場のために揺れるまでの待機時間

	const float CON_FIRSTWAIT = 8.0f;		// 待ち時間

	const float CON_APPEAR_TIME = 0.5f;		// 生える時間
	const float CON_START_POSY = -6.0f;		// 初期のY座標
	const float CON_END_POSY = 0.0f;		// 終わりのY座標

	const float CON_SECONDWAIT = 11.0f;		// 待ち時間2
	const float CON_ROAR_START = 6.5f;		// 口が開き始める時間
	const float CON_ROAR_END = 8.5f;		// 口が閉じ始める時間
	const float CON_MOUTHMAX_TIME = 0.3f;	// 口が全開になるのにかかる時間

	const float CON_BACK_TIME = 1.0f;		// 潜るのにかける時間
	const float CON_FADESTART_TIME = 0.6f;	// 潜るのが始まってから、フェードが開始される時間

	const float CON_WIND_TIME = 0.8f;		// APPEARになってから風効果音が鳴るまでの時間

	#endregion


	#region 変数

	_TSBOSSMODE BossMode = _TSBOSSMODE.FIRSTWAIT;
	bool bInit = true;


	private GameObject _BossObj;
	public GameObject BossObj { get; set; }

	private GameObject _BossAppearObj;
	public GameObject BossAppearObj { get; set; }

	private GameObject _BossRoarObj;
	public GameObject BossRoarObj { get; set; }

	private GameObject _BossDiveObj;
	public GameObject BossDiveObj { get; set; }

	private Material _mat;
	public Material mat{get;set;}

	private Animator animator;


	float fTime;				// タイマー
	Vector3 vStartPos;			// スタート位置
	Vector3 vEndPos;			// 終了位置
	bool bEffect;

	float fWait = 0.0f;	// ボスの出現と、エフェクトのタイミングを合わせるためのタイマー

	bool bFade = true;

	bool bRoarStart = true;
	bool bRoarEnd = true;

	// エフェクト関係
	float fEffectTime = 0.0f;
	bool bWind = true;

	#endregion


	public override void OnGraphStart(Playable playable)
	{
		fTime = 0.0f;
		BossObj.transform.position = new Vector3(BossObj.transform.position.x, CON_START_POSY, BossObj.transform.position.z);
		vStartPos = BossObj.transform.position;
		vEndPos = new Vector3(BossObj.transform.position.x, CON_END_POSY, BossObj.transform.position.z);
		bEffect = true;

		mat.color = Color.white;
		animator = BossObj.GetComponent<Animator>();
		animator.speed = 0.0f;		// 最初はアニメーションしない
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
		switch(BossMode)
		{
			case _TSBOSSMODE.FIRSTWAIT:
				FirstWait();
				break;

			case _TSBOSSMODE.APPEAR:
				Appera();
				break;

			case _TSBOSSMODE.SECONDWAIT:
				SecondWait();
				break;

			case _TSBOSSMODE.BACK:
				Back();
				break;

			case _TSBOSSMODE.FIN:
				break;
		}

		// スキップ処理
		if ((Input.GetKeyDown(KeyCode.Backspace) || (Input.GetButtonDown("Skip"))) && bFade && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.STAGE_1);
			bFade = false;
		}
	}




	// 最初の待ち時間
	private void FirstWait()
	{
		if(bInit)
		{
			fTime = 0.0f;
			bInit = false;
		}

		fTime += Time.deltaTime;
		if(fTime >= CON_FIRSTWAIT)
		{
			BossMode = _TSBOSSMODE.APPEAR;
			bInit = true;
		}
	}


	// 生えてくる
	private void Appera()
	{
		if(bInit)
		{
			fTime = 0.0f;
			fWait = 0.0f;
			bInit = false;
		}

		// 登場エフェクト発生
		if (bEffect)
		{
			BossAppearObj.GetComponent<EffekseerEmitter>().Play();
			bEffect = false;

			MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.TS_TotemBoss);
		}

		// 風効果音
		//fEffectTime += Time.deltaTime;
		//Debug.Log(fEffectTime);
		//if (fEffectTime > CON_WIND_TIME && bWind)
		//{
		//	MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.SP_TotemAttack);
		//	bWind = false;
		//}

		// ボスの出現と、エフェクトのタイミングを合わせる
		fWait += Time.deltaTime;
		if (fWait < 0.3f)
			return;

		fTime += Time.deltaTime / CON_APPEAR_TIME;

		// 終了判定
		if (fTime > 1.0f)
		{
			BossObj.transform.localPosition = Vector3.Lerp(vStartPos, vEndPos, 1.0f);

			bInit = true;
			BossMode = _TSBOSSMODE.SECONDWAIT;

			MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.SP_TotemAttack);
		}

		BossObj.transform.localPosition = Vector3.Lerp(vStartPos, vEndPos, fTime);
	}


	// 待ち時間2
	private void SecondWait()
	{
		if (bInit)
		{
			fTime = 0.0f;
			bInit = false;
		}

		fTime += Time.deltaTime;
		if(fTime >= CON_ROAR_START && bRoarStart)
		{
			BossRoarObj.GetComponent<EffekseerEmitter>().Play();
			animator.speed = 0.5f / CON_MOUTHMAX_TIME;			// 咆哮開始(口を開け始める)
			bRoarStart = false;

			//MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.TS_TotemRoar);		// 咆哮効果音
			MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.TS_Cry);		// 咆哮効果音
		}
		else if( (fTime >= CON_ROAR_START + CON_MOUTHMAX_TIME*1.8) && (fTime < CON_ROAR_END) )
		{
			animator.speed = 0.0f;								// アニメーションストップ(口が全開のときに止める)
		}
		if(fTime >=CON_ROAR_END)
		{
			animator.speed = 0.5f / CON_MOUTHMAX_TIME;			// 咆哮終了(口を閉じ始める)
		}


		if (fTime >= CON_SECONDWAIT)
		{
			BossMode = _TSBOSSMODE.BACK;
			bInit = true;
		}
	}


	// ひっこむ
	private void Back()
	{
		if(bInit)
		{
			fTime = 0.0f;
			fWait = 0.0f;

			vStartPos = BossObj.transform.localPosition;
			vEndPos = new Vector3(BossObj.transform.localPosition.x, CON_START_POSY, BossObj.transform.localPosition.z);

			BossDiveObj.GetComponent<EffekseerEmitter>().Play();	// 潜るエフェクト
			MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.TS_TotemDive);

			bEffect = true;
			bInit = false;
		}

		// 潜りエフェクト発生
		//if (bEffect)
		//{
		//	BossAppearObj.GetComponent<EffekseerEmitter>().Play();
		//	GameObject.Find("ShakeCameraObj").GetComponent<ShakeCamera>().Shake();		// Timelineで制御しているせいで、カメラが揺れない
		//	bEffect = false;
		//}

		// ボスの潜りと、エフェクトのタイミングを合わせる
		//fWait += Time.deltaTime;
		//if (fWait < 0.3f)
		//	return;


		fTime += Time.deltaTime / CON_BACK_TIME;

		// ゲームメインへシーン遷移
		if (fTime > CON_FADESTART_TIME && bFade && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.STAGE_1);
			bFade = false;
		}

		// 終了判定
		if (fTime > 1.0f)
		{
			BossObj.transform.localPosition = Vector3.Lerp(vStartPos, vEndPos, 1.0f);

			bInit = true;
			BossMode = _TSBOSSMODE.FIN;
		}

		BossObj.transform.localPosition = Vector3.Lerp(vStartPos, vEndPos, fTime);
	}
}
