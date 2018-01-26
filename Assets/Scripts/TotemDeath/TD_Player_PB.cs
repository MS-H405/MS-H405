using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class TD_Player_PB : PlayableBehaviour
{
	#region 定数

	const float CON_WIN_TIME = 13.0f;			// 勝利モーションに切り替える時間
	const float CON_EFFECT_FIN_TIME = 14.5f;		// キラキラエフェクトを止める時間

	const float CON_SKILLUI_TIME = 16.0f;		// 技獲得UIを出す時間
	const float CON_FADE_TIME = 19.0f;			// シーン遷移入力の受付を開始する時間

	const float CON_SE_SPLEARNING = 2.2f;	// 勝利モーションが始まってから、決めポーズSEを鳴らすまでの時間
	const float CON_SE_WIN = 11.0f;			// 勝利SE鳴らす時間(勝利モーションに切り替える時間の2秒前)

	const float CON_LAST = 40.0f;			// 自動でシーン遷移する時間

	public struct tSE
	{
		public float time;	// 時間
		public bool bDo;	// 処理中
		public bool bDone;	// 再生したか
	};

	#endregion


	#region 変数

	private GameObject _PlayerObj;
	public GameObject PlayerObj { get; set; }
	Animator animator;

	float fTime = 0.0f;
	bool bWin = true;
	bool bSkillUI = true;

	bool bFade = true;



	private GameObject _EffectObj_1;
	public GameObject EffectObj_1 { get; set; }
	private GameObject _EffectObj_2;
	public GameObject EffectObj_2 { get; set; }


	public GameObject BossDeathObj;		// 技獲得UI

	tSE tSpecial_Learning;	// 決めポーズ
	tSE tWin;				// 勝利SE

	#endregion

	public override void OnGraphStart(Playable playable)
	{
		animator = PlayerObj.GetComponent<Animator>();

		tSpecial_Learning.time = 0.0f;
		tSpecial_Learning.bDo = false;
		tSpecial_Learning.bDone = false;
		tWin.time = 0.0f;
		tWin.bDo = true;
		tWin.bDone = false;
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
		// スキップ処理
		if (Input.GetKeyDown(KeyCode.Return) && bFade && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.TOTEM_TO_YADOKARI);
			bFade = false;
		}



		fTime += Time.deltaTime;
		if(bWin && fTime >= CON_WIN_TIME)
		{
			animator.SetBool("bWin", true);
			bWin = false;

			EffectObj_1.GetComponent<ParticleSystem>().Play();	// 腕からきらきらエフェクトを出す
			EffectObj_2.GetComponent<ParticleSystem>().Play();	// 腕からきらきらエフェクトを出す

			tSpecial_Learning.bDo = true;	// 決めポーズ処理開始
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
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.TOTEM_TO_YADOKARI);
			bFade = false;
		}


		SE();


		// 2400Frame(40秒)経ってもシーン遷移されないので、シーン遷移する
		if (fTime >= CON_LAST && bFade && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.TOTEM_TO_YADOKARI);
			bFade = false;
		}
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
