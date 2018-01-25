using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class BD_Player_PB : PlayableBehaviour
{
	#region 定数

	const float CON_WIN_TIME = 10.0f;		// 勝利モーションを始める時間
	const float CON_EFFECT_FIN_TIME = 11.5f;// パーティクルを止める時間
	const float CON_WIN_EFFECT = 11.8f;		// プレイヤーの後ろに出てくる火みたいなエフェクトを発生させる時間

	const float CON_SKILLUI_TIME = 14.0f;		// 技獲得UIを出す時間
	const float CON_FADE_TIME = 17.0f;			// シーン遷移入力の受付を開始する時間

	const float CON_SE_SPLEARNING = 2.2f;	// 勝利モーションが始まってから、決めポーズSEを鳴らすまでの時間
	const float CON_SE_WIN = 9.0f;			// 勝利SE鳴らす時間(勝利モーションに切り替える時間の2秒前)

	#endregion


	#region 変数

	private GameObject _PlayerObj;
	public GameObject PlayerObj { get; set; }

	Animator animator;

	float fTime = 0.0f;
	bool bWin = true;
	bool bEffect = true;
	bool bFade = true;


	// Effekseer
	private GameObject _EffekseerObj;
	public GameObject EffekseerObj { get; set; }
	SetEffekseerObject cs_SetEffekseerObject;


	private GameObject _EffectObj_1;
	public GameObject EffectObj_1 { get; set; }
	private GameObject _EffectObj_2;
	public GameObject EffectObj_2 { get; set; }

	public GameObject BossDeathObj;		// 技獲得UI
	bool bSkillUI = true;


	MovieSoundManager.tSE tSpecial_Learning;	// 決めポーズ
	MovieSoundManager.tSE tWin;					// 勝利SE

	#endregion


	public override void OnGraphStart(Playable playable)
	{
		animator = PlayerObj.GetComponent<Animator>();
		cs_SetEffekseerObject = EffekseerObj.GetComponent<SetEffekseerObject>();

		tSpecial_Learning.time = 0.0f;
		tSpecial_Learning.bDo = false;
		tSpecial_Learning.bDone = false;
		tWin.time = 0.0f;
		tWin.bDo = true;
		tWin.bDone = false;
	}


	public override void OnGraphStop(Playable playable)
	{
		// 2400Frame(40秒)経ってもシーン遷移されないので、シーン遷移する
		if (bFade && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.YADOKARI_TO_MECHA);
			bFade = false;
		}
	}


	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		
	}


	public override void OnBehaviourPause(Playable playable, FrameData info)
	{
		
	}


	public override void PrepareFrame(Playable playable, FrameData info)
	{
		fTime += Time.deltaTime;

		if(fTime >= CON_WIN_TIME && bWin)
		{
			animator.SetBool("bWin", true);
			bWin = false;

			EffectObj_1.GetComponent<ParticleSystem>().Play();	// 腕からきらきらエフェクトを出す
			EffectObj_2.GetComponent<ParticleSystem>().Play();	// 腕からきらきらエフェクトを出す

			tSpecial_Learning.bDo = true;	// 決めポーズ処理開始
		}
		else if (fTime >= CON_WIN_EFFECT && bEffect)
		{
			cs_SetEffekseerObject.NewEffect(3);		// 炎みたいなエフェクト
			bEffect = false;
		}
		else if (fTime >= CON_EFFECT_FIN_TIME)
		{
			EffectObj_1.GetComponent<ParticleSystem>().Stop();	// エフェクトを止める
			EffectObj_2.GetComponent<ParticleSystem>().Stop();	// エフェクトを止める
		}


		// 技UI
		if (fTime >= CON_SKILLUI_TIME && bSkillUI)
		{
			MonoBehaviour.Instantiate(BossDeathObj);
			bSkillUI = false;
		}

		// フェード
		if (Input.GetButtonDown("Atack") && fTime > CON_FADE_TIME && bFade && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.YADOKARI_TO_MECHA);
			bFade = false;
		}



		// スキップ
		if (Input.GetKeyDown(KeyCode.Return) && bFade && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.YADOKARI_TO_MECHA);	// シーン遷移
			bFade = false;
		}
		SE();
	}

	// SEを鳴らす
	private void SE()
	{
		// 決めポーズ
		if (tSpecial_Learning.bDo && !tSpecial_Learning.bDone)
		{
			tSpecial_Learning.time += Time.deltaTime;
			if (tSpecial_Learning.time >= CON_SE_SPLEARNING)
			{
				MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.TS_Special_Learning);
				tSpecial_Learning.bDone = true;
			}
		}

		// 勝利SE
		if (tWin.bDo && !tWin.bDone)
		{
			tWin.time += Time.deltaTime;
			if (tWin.time >= CON_SE_WIN)
			{
				MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.TS_Win);
				tWin.bDone = true;
			}
		}
	}
}
