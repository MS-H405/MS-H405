using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class TS_Totemchild_PB : PlayableBehaviour
{
	private List<GameObject> _TotemChildObjList = new List<GameObject>();
	public GameObject TotemChildObj
	{
		get { return _TotemChildObjList[0]; }
		set { _TotemChildObjList.Add(value); }
	}

	List<Transform> TotemTransList = new List<Transform>();	// スクリプトが長くなるからTransformも作っとく


	// タイムライン開始時実行
	public override void OnGraphStart(Playable playable)
	{
		for(int i = 0 ; i < _TotemChildObjList.Count ; i ++)
			TotemTransList.Add(_TotemChildObjList[i].transform);
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
		for (int i = 0; i < TotemTransList.Count; i++)
			TotemTransList[i].position = new Vector3(TotemTransList[i].position.x, TotemTransList[i].position.y + 0.05f, TotemTransList[i].position.z);
	}
}
