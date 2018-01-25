using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class MD_Player_PB : PlayableBehaviour
{
	#region 定数

//	const float CON_WIN_TIME = 1.0f;		// 勝利モーションを始める時間	10
//	const float CON_WIN_EFFECT = 1.9f;		// プレイヤーの後ろに出てくる火みたいなエフェクトを発生させる時間	勝利モーション開始から0.8秒後
//	const float CON_KIRAKIRA_EFFECT = 2.4f;	// プレイヤーの後ろに出てくるキラキラしたエフェクト
//	const float CON_FADE_TIME = 10.0f;		// フェードし始めてもいい時間（シーン遷移は"キー入力されたら"だけど、この時間より前のキー入力は受け付けない）

	const float CON_WIN_TIME = 2.5f;		// 勝利モーションを始める時間	10
	const float CON_ARMPARTICLE_STOP_TIME = 4.0f;	// キラキラエフェクトを止める時間
	const float CON_WIN_EFFECT = 4.2f;		// プレイヤーの後ろに出てくる火みたいなエフェクトを発生させる時間	勝利モーション開始から0.8秒後
	const float CON_KIRAKIRA_EFFECT = 4.9f;	// プレイヤーの後ろに出てくるキラキラしたエフェクト
	const float CON_CONG_TIME = 5.5f;		// Congratulatinsを出す時間
	const float CON_FADE_TIME = 10.5f;		// フェードし始めてもいい時間（シーン遷移は"キー入力されたら"だけど、この時間より前のキー入力は受け付けない）

	const float CON_SE_SPLEARNING = 1.7f;	// 勝利モーションが始まってから、決めポーズSEを鳴らすまでの時間
	const float CON_SE_WIN = 0.5f;			// 勝利SE鳴らす時間(勝利モーションに切り替える時間の2秒前)
	
	#endregion


	#region 変数

	private GameObject _PlayerObj;
	public GameObject PlayerObj { get; set; }

	private GameObject _KamihubukiObj;
	public GameObject KamihubukiObj { get; set; }

	private GameObject _KirakiraObj;
	public GameObject KirakiraObj { get; set; }

	private GameObject _ArmParticleObj_1;
	public GameObject ArmParticleObj_1 { get; set; }
	private GameObject _ArmParticleObj_2;
	public GameObject ArmParticleObj_2 { get; set; }

	Animator animator;

	float fTime = 0.0f;
	bool bWin = true;
	bool bEffect = true;
	bool bKirakira = true;
	bool bFade = true;
	bool bKirakiraStop = true;
	bool bCong = true;


	// Effekseer
	private GameObject _EffekseerObj;
	public GameObject EffekseerObj { get; set; }
	SetEffekseerObject cs_SetEffekseerObject;

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

	}


	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		
	}


	public override void OnBehaviourPause(Playable playable, FrameData info)
	{
		
	}


	public override void PrepareFrame(Playable playable, FrameData info)
	{
        if(Input.GetKeyDown(KeyCode.T))
            MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.TS_Win, 2.0f);

        fTime += Time.deltaTime;

		if(fTime >= CON_WIN_TIME && bWin)
		{
			animator.SetBool("bWin", true);
			bWin = false;

			ArmParticleObj_1.GetComponent<ParticleSystem>().Play();
			ArmParticleObj_2.GetComponent<ParticleSystem>().Play();

			tSpecial_Learning.bDo = true;
		}
		else if (fTime >= CON_WIN_EFFECT && bEffect)
		{
			cs_SetEffekseerObject.NewEffect(0);						// 炎みたいなエフェクト
			KamihubukiObj.GetComponent<ParticleSystem>().Play();	// 紙吹雪
			bEffect = false;
		}
		else if (fTime >= CON_KIRAKIRA_EFFECT && bKirakira)
		{
			KirakiraObj.GetComponent<ParticleSystem>().Play();		// キラキラエフェクト
			bKirakira = false;
		}
		else if (fTime >= CON_ARMPARTICLE_STOP_TIME && bKirakiraStop)
		{
			ArmParticleObj_1.GetComponent<ParticleSystem>().Stop();	// 腕のパーティクルを止める
			ArmParticleObj_2.GetComponent<ParticleSystem>().Stop();
			bKirakiraStop = false;
		}
		else if (fTime >= CON_CONG_TIME && bCong)
		{
			cs_SetEffekseerObject.NewEffect(3);						// COngratulations
			MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.MD_Cong);
			bCong = false;
		}


		// 特定の時間を過ぎて(演出を全部見せたいから)、キー入力されたら、シーン遷移
		if (fTime >= CON_FADE_TIME && Input.GetKeyDown(KeyCode.Return) && bFade && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.TITLE);
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
				MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.MD_Win2);
				tWin.bDone = true;
			}
		}
	}
}
