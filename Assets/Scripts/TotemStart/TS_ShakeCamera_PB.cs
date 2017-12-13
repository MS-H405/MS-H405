using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class TS_ShakeCamera_PB : PlayableBehaviour
{
	#region 定数

	const float CON_CHILDSHAKE_FIRST_TIME = 0.5f;		// 最初に1個生えてくる子トーテムの揺れを始める時間
	const float CON_CHILDSHAKE_SECOND_TIME = 2.1f;		// 4つ連続で生えてくる子トーテムの揺れを始める時間
	const float CON_SHILDSHAKE_STOP_TIME = 3.0f;		// 減衰率を入れる時間
	const float CON_BOSSSHAKE_TIME = 5.9f;				// ボストーテムの揺れを始める時間

	#endregion


	#region 変数

	private GameObject _ShakeCameraObj;
	public GameObject ShakeCameraObj { get; set; }
	private ShakeCamera cs_ShakeCamera;

	float fTime = 0.0f;
	List<bool> bUse = new List<bool>();		// エフェクトを発生させたらfalseにする

	#endregion




	public override void OnGraphStart(Playable playable)
	{
		cs_ShakeCamera = ShakeCameraObj.GetComponent<ShakeCamera>();
		for(int i = 0 ; i < 4 ; i ++)
			bUse.Add(true);
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
		if (bUse[0] && fTime >= CON_CHILDSHAKE_FIRST_TIME)
		{
			cs_ShakeCamera.SetParam(0.02f, 0.001f);
			cs_ShakeCamera.DontMoveShake();			// ShakeCameraObjは動いていないのでこっちの関数
			bUse[0] = false;
			Debug.Log(123);
		}
		if (bUse[1] && fTime >= CON_CHILDSHAKE_SECOND_TIME)
		{
			cs_ShakeCamera.SetParam(0.02f, 0.0f);	// 最初は減衰率なし
			cs_ShakeCamera.DontMoveShake();
			bUse[1] = false;
		}
		if (bUse[2] && fTime >= CON_CHILDSHAKE_SECOND_TIME)
		{
			cs_ShakeCamera.SetParam(0.02f, 0.0006f);	
			bUse[2] = false;
		}
		if (bUse[3] && fTime >= CON_BOSSSHAKE_TIME)
		{
			cs_ShakeCamera.SetParam(0.03f, 0.0008f);
			cs_ShakeCamera.DontMoveShake();
			bUse[3] = false;
		}
	}
}
