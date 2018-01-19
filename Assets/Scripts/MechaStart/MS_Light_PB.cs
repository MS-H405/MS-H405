using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class MS_Light_PB : PlayableBehaviour
{
	#region 定数

	const float CON_LIGHT1 = 5.0f;		// ライト1がつく時間
	const float CON_LIGHT2 = 5.5f;		// ライト2がつく時間
	const float CON_LIGHT3 = 6.0f;		// ライト3がつく時間

	#endregion


	#region 変数

	#region Component, Script
	private GameObject _LightObj;
	public GameObject LightObj { get; set; }
	private Transform[] TransformArray;



	// 頻繁にfalseになっているので無理やりtrueに
	private GameObject _LightObj_1;
	public GameObject LightObj_1 { get; set; }
	private GameObject _LightObj_2;
	public GameObject LightObj_2 { get; set; }
	private GameObject _LightObj_3;
	public GameObject LightObj_3 { get; set; }
	#endregion

	float fTime = 0.0f;
	bool b1 = true, b2 = true, b3 = true;

	#endregion


	public override void OnGraphStart(Playable playable)
	{
		// 頻繁にfalseになっているので無理やりtrueに
		LightObj.SetActive(true);
		LightObj_1.SetActive(true);
		LightObj_2.SetActive(true);
		LightObj_3.SetActive(true);

		// ライト取得&非表示
		TransformArray = LightObj.GetComponentsInChildren<Transform>();
		foreach (Transform transform in TransformArray)
			transform.gameObject.SetActive(false);
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
		if (b1 && fTime >= CON_LIGHT1)
		{
			TransformArray[0].gameObject.SetActive(true);	// 親オブジェクト
			TransformArray[1].gameObject.SetActive(true);	// ライト1
			b1 = false;
		}
		else if (b2 && fTime >= CON_LIGHT2)
		{
			TransformArray[2].gameObject.SetActive(true);	// ライト2
			b2 = false;
		}
		else if (b3 && fTime >= CON_LIGHT3)
		{
			TransformArray[3].gameObject.SetActive(true);	// ライト3
			b3 = false;
		}
	}
}
