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

	const float CON_WIN_TIME = 9.0f;		// 勝利モーションを始める時間	10
	const float CON_ARMPARTICLE_STOP_TIME = 10.5f;	// キラキラエフェクトを止める時間
	const float CON_WIN_EFFECT = 10.7f;		// プレイヤーの後ろに出てくる火みたいなエフェクトを発生させる時間	勝利モーション開始から0.8秒後
	const float CON_KIRAKIRA_EFFECT = 11.4f;	// プレイヤーの後ろに出てくるキラキラしたエフェクト
	const float CON_CONG_TIME = 12.0f;		// Congratulatinsを出す時間
	const float CON_FADE_TIME = 17.0f;		// フェードし始めてもいい時間（シーン遷移は"キー入力されたら"だけど、この時間より前のキー入力は受け付けない）
	
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

	#endregion


	public override void OnGraphStart(Playable playable)
	{
		animator = PlayerObj.GetComponent<Animator>();
		cs_SetEffekseerObject = EffekseerObj.GetComponent<SetEffekseerObject>();
	}


	public override void OnGraphStop(Playable playable)
	{
		// 1800Frame(30秒)経ってもシーン遷移されないので、シーン遷移する
		if (bFade && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.TITLE);
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

			ArmParticleObj_1.GetComponent<ParticleSystem>().Play();
			ArmParticleObj_2.GetComponent<ParticleSystem>().Play();
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
			bCong = false;
		}


		// 特定の時間を過ぎて(演出を全部見せたいから)、キー入力されたら、シーン遷移
		if (fTime >= CON_FADE_TIME && Input.GetKeyDown(KeyCode.Return) && bFade && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.TITLE);
			bFade = false;
		}
	}
}
