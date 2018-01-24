using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityStandardAssets.ImageEffects;

// A behaviour that is attached to a playable
public class TS_ShakeCamera_PB : PlayableBehaviour
{
	#region 定数

	const float CON_CHILDSHAKE_FIRST_TIME = 0.5f;		// 最初に1個生えてくる子トーテムの揺れを始める時間
	const float CON_CHILDSHAKE_SECOND_TIME = 2.1f;		// 4つ連続で生えてくる子トーテムの揺れを始める時間
	const float CON_SHILDSHAKE_STOP_TIME = 3.0f;		// 減衰率を入れる時間
	const float CON_BOSSSHAKE_TIME = 6.9f;				// ボストーテムの揺れを始める時間
	const float CON_DIVESHAKE_TIME = 15.0f;				// トーテムが潜るときの揺れを始める時間

	const float CON_BLUR_START_TIME = 15.5f;			// ボストーテム咆哮の時のブラーを始める時間
	const float CON_BLUR_END_TIME = 17.2f;				// ボストーテム咆哮の時のブラーをやめる時間

	#endregion


	#region 変数

	private GameObject _ShakeCameraObj;
	public GameObject ShakeCameraObj { get; set; }
	private ShakeCamera cs_ShakeCamera;

	private GameObject _MainCameraObj;
	public GameObject MainCameraObj { get; set; }
	BlurOptimized cs_BlurOptimized;

	float fTime = 0.0f;
	List<bool> bUse = new List<bool>();		// エフェクトを発生させたらfalseにする

	#endregion




	public override void OnGraphStart(Playable playable)
	{
		cs_ShakeCamera = ShakeCameraObj.GetComponent<ShakeCamera>();
		for(int i = 0 ; i < 7 ; i ++)
			bUse.Add(true);

		cs_BlurOptimized = MainCameraObj.GetComponent<BlurOptimized>();
		cs_BlurOptimized.enabled = false;
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
		}
		if (bUse[1] && fTime >= CON_CHILDSHAKE_SECOND_TIME)
		{
			cs_ShakeCamera.SetParam(0.02f, 0.0f);	// 最初は減衰率なし
			cs_ShakeCamera.DontMoveShake();
			bUse[1] = false;
		}
		if (bUse[2] && fTime >= CON_CHILDSHAKE_SECOND_TIME)
		{
			cs_ShakeCamera.SetParam(0.02f, 0.001f);	
			bUse[2] = false;
		}
		if (bUse[3] && fTime >= CON_BOSSSHAKE_TIME)
		{
			cs_ShakeCamera.SetParam(0.02f, 0.0007f);
			cs_ShakeCamera.DontMoveShake();
			bUse[3] = false;
		}
		if (bUse[4] && fTime >= CON_DIVESHAKE_TIME)
		{
			cs_ShakeCamera.SetParam(0.007f, 0.0002f);
			cs_ShakeCamera.DontMoveShake();
			bUse[4] = false;
		}

		// ブラー
		if (bUse[5] && fTime >= CON_BLUR_START_TIME)
		{
			cs_BlurOptimized.enabled = true;
			bUse[5] = false;
		}
		if (bUse[6] && fTime >= CON_BLUR_END_TIME)
		{
			cs_BlurOptimized.enabled = false;
			bUse[6] = false;
		}
	}
}
