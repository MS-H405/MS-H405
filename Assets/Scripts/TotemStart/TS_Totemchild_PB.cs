using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class TS_Totemchild_PB : PlayableBehaviour
{
	#region 定数

	float CON_APPEAR = 1.0f;				// ちびトーテムが出現し始める時間
	float CON_APPEAR_INTERVAL = 0.2f;		// ちびトーテムが出現する間隔
	float CON_LOOKAT_BOSS = 4.5f;			// ボスのほうを向き始める時間
	float CON_BACK = 10.0f;					// ひっこむ時間

	#endregion


	#region 変数

	private List<GameObject> _TotemChildObjList = new List<GameObject>();
	public GameObject TotemChildObj
	{
		set { _TotemChildObjList.Add(value); }
	}


	List<TS_Totemchild> TotemChildList = new List<TS_Totemchild>();	// スクリプトが長くなるからTransformも作っとく
	float fTime;

	#endregion


	// タイムライン開始時実行
	public override void OnGraphStart(Playable playable)
	{
		// スクリプト取得
		for(int i = 0 ; i < _TotemChildObjList.Count ; i ++)
			TotemChildList.Add(_TotemChildObjList[i].GetComponent<TS_Totemchild>());

		fTime = 0.0f;
	}

	// タイムライン停止時実行
	public override void OnGraphStop(Playable playable)
	{

	}

	// PlayableTrack再生時実行
	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{

	}

	// PlayableTrack停止時実行
	public override void OnBehaviourPause(Playable playable, FrameData info)
	{

	}

	// PlayableTrack再生時毎フレーム実行
	public override void PrepareFrame(Playable playable, FrameData info)
	{
		fTime += Time.deltaTime;

		if(fTime < CON_APPEAR)
			return;

		// ちびトーテム出現
		if (fTime >= CON_APPEAR_INTERVAL)
			TotemChildList[0].Appear();
		if (fTime >= CON_APPEAR_INTERVAL * 2 + 2)	// 2 = CON_APPEAR + カメラが変わるまでの1秒
			TotemChildList[1].Appear();
		if (fTime >= CON_APPEAR_INTERVAL * 3 + 2)
			TotemChildList[2].Appear();
		if (fTime >= CON_APPEAR_INTERVAL * 4 + 2)
			TotemChildList[3].Appear();
		if (fTime >= CON_APPEAR_INTERVAL * 5 + 2)
			TotemChildList[4].Appear();
		

		// ボスのほうを向く
		if (fTime >= CON_LOOKAT_BOSS)
		{
			for (int i = 0; i < TotemChildList.Count; i++)
				TotemChildList[i].LookAtBoss();
		}


		// ひっこむ
		if (fTime >= CON_BACK)
		{
			for (int i = 0; i < TotemChildList.Count; i++)
				TotemChildList[i].Back();
		}
	}
}
