using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class MS_Light_PA : PlayableAsset
{
	[SerializeField]	ExposedReference<GameObject> LightObj;


	// 頻繁にfalseになっているので無理やりtrueに
	[SerializeField]	ExposedReference<GameObject> LightObj_1;
	[SerializeField]	ExposedReference<GameObject> LightObj_2;
	[SerializeField]	ExposedReference<GameObject> LightObj_3;

	// Factory method that generates a playable based on this asset
	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		var behaviour = new MS_Light_PB();

		behaviour.LightObj = LightObj.Resolve(graph.GetResolver());
		behaviour.LightObj_1 = LightObj_1.Resolve(graph.GetResolver());
		behaviour.LightObj_2 = LightObj_2.Resolve(graph.GetResolver());
		behaviour.LightObj_3 = LightObj_3.Resolve(graph.GetResolver());

		return ScriptPlayable<MS_Light_PB>.Create(graph, behaviour);
	}
}
