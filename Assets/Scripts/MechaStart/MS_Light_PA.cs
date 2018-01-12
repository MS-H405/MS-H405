using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class MS_Light_PA : PlayableAsset
{
	[SerializeField]	ExposedReference<GameObject> LightObj;

	// Factory method that generates a playable based on this asset
	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		var behaviour = new MS_Light_PB();

		behaviour.LightObj = LightObj.Resolve(graph.GetResolver());

		return ScriptPlayable<MS_Light_PB>.Create(graph, behaviour);
	}
}
