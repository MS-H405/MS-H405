using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class MS_Enemy_PA : PlayableAsset
{
	[SerializeField]	ExposedReference<GameObject> EnemyObj;
	[SerializeField]	ExposedReference<GameObject> BallObj;
	[SerializeField]	ExposedReference<GameObject> group1Obj;		// MS_NeedleManagerがついているオブジェクト
	[SerializeField]	ExposedReference<GameObject> EffekseerObj;

	// Factory method that generates a playable based on this asset
	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		var behaviour = new MS_Enemy_PB();

		behaviour.EnemyObj = EnemyObj.Resolve(graph.GetResolver());
		behaviour.BallObj = BallObj.Resolve(graph.GetResolver());
		behaviour.group1Obj = group1Obj.Resolve(graph.GetResolver());
		behaviour.EffekseerObj = EffekseerObj.Resolve(graph.GetResolver());

		return ScriptPlayable<MS_Enemy_PB>.Create(graph, behaviour);
	}
}
