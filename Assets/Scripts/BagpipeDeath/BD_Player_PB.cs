using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class BD_Player_PB : PlayableBehaviour
{
	#region 定数



	#endregion


	#region 変数

	private GameObject _PlayerObj;
	public GameObject PlayerObj { get; set; }

	Animator animator;

	float fTime = 0.0f;
	bool bEnd = false;

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
		if (bEnd)
			return;

		fTime += Time.deltaTime;

		if(fTime > 5.0f)
		{
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.YADOKARI_TO_MECHA);
			bEnd = true;
		}
	}
}
