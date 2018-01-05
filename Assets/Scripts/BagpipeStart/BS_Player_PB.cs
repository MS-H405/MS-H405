using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class BS_Player_PB : PlayableBehaviour
{
	#region 定数

	readonly Vector3 CON_STARTPOS = new Vector3(0.0f, -0.07f, -15.0f);
	readonly Vector3 CON_ENDPOS = new Vector3(0.0f, -0.07f, -5.0f);
	const float CON_WAKLTIME = 5.0f;


	#endregion


	#region 変数

	private GameObject _PlayerObj;
	public GameObject PlayerObj { get; set; }

	Transform transform;
	Animator animator;

	float fTime = 0.0f;
	bool bEnd = false;

	#endregion


	public override void OnGraphStart(Playable playable)
	{
		transform = PlayerObj.GetComponent<Transform>();
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
		if(bEnd)
			return;

		fTime += Time.deltaTime;

		if(fTime < CON_WAKLTIME)
			transform.position = Vector3.Lerp(CON_STARTPOS, CON_ENDPOS, fTime / CON_WAKLTIME);
		else
		{
			animator.SetBool("bIdle", true);
			bEnd = true;
		}
	}
}
