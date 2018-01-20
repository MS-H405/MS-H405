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

	#endregion


	#region 変数

	private GameObject _PlayerObj;
	public GameObject PlayerObj { get; set; }
	Animator animator;

	float fTime = 0.0f;
	bool bWin = true;




	private GameObject _EffectObj_1;
	public GameObject EffectObj_1 { get; set; }
	private GameObject _EffectObj_2;
	public GameObject EffectObj_2 { get; set; }

	#endregion

	public override void OnGraphStart(Playable playable)
	{
		animator = PlayerObj.GetComponent<Animator>();
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
		if(bWin && fTime >= CON_WIN_TIME)
		{
			animator.SetBool("bWin", true);
			bWin = false;

			EffectObj_1.GetComponent<ParticleSystem>().Play();	// 腕からきらきらエフェクトを出す
			EffectObj_2.GetComponent<ParticleSystem>().Play();	// 腕からきらきらエフェクトを出す
		}
		else if(fTime >= CON_EFFECT_FIN_TIME)
		{
			EffectObj_1.GetComponent<ParticleSystem>().Stop();	// エフェクトを止める
			EffectObj_2.GetComponent<ParticleSystem>().Stop();	// エフェクトを止める
		}
	}
}
