using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class TD_Player_PB : PlayableBehaviour
{
	#region 定数

	const float CON_WIN_TIME = 13.0f;		// 勝利モーションに切り替える時間
	const float CON_EFFECT_FIN_TIME = 14.5f;		// キラキラエフェクトを止める時間

	const float CON_SKILLUI_TIME = 16.0f;		// 技獲得UIを出す時間
	const float CON_FADE_TIME = 19.0f;			// シーン遷移入力の受付を開始する時間

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

	#endregion

	public override void OnGraphStart(Playable playable)
	{
		animator = PlayerObj.GetComponent<Animator>();
	}


	public override void OnGraphStop(Playable playable)
	{
		// 3000Frame(50秒)経ってもシーン遷移されないので、シーン遷移する
		if (bFade && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.TOTEM_TO_YADOKARI);
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
	}
}
