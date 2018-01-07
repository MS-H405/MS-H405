using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class BD_Player_PB : PlayableBehaviour
{
	#region 定数

	const float CON_WIN_TIME = 10.0f;		// 勝利モーションを始める時間
	const float CON_WIN_EFFECT = 11.8f;		// プレイヤーの後ろに出てくる火みたいなエフェクトを発生させる時間
	const float CON_FADE_TIME = 14.0f;		// フェードを始める時間

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

	#endregion


	public override void OnGraphStart(Playable playable)
	{
		animator = PlayerObj.GetComponent<Animator>();
		cs_SetEffekseerObject = EffekseerObj.GetComponent<SetEffekseerObject>();
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
		fTime += Time.deltaTime;

		if(fTime >= CON_WIN_TIME && bWin)
		{
			animator.SetBool("bWin", true);
			bWin = false;
		}
		else if (fTime >= CON_WIN_EFFECT && bEffect)
		{
			cs_SetEffekseerObject.NewEffect(3);		// 炎みたいなエフェクト
			bEffect = false;
		}
		else if (fTime >= CON_FADE_TIME && bFade)
		{
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.YADOKARI_TO_MECHA);
			bFade = false;
		}

		//if (!bFade)
		//    return;

		//fTime += Time.deltaTime;

		//if(fTime > 5.0f)
		//{
		//    MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.YADOKARI_TO_MECHA);
		//    bFade = false;
		//}
	}
}
