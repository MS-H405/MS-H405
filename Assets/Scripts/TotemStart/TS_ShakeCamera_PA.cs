using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class TS_ShakeCamera_PA : PlayableAsset
{
	[SerializeField]	ExposedReference<GameObject> ShakeCameraObj;

	// Factory method that generates a playable based on this asset
	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		var behaviour = new TS_ShakeCamera_PB();

		behaviour.ShakeCameraObj = ShakeCameraObj.Resolve(graph.GetResolver());

		return ScriptPlayable<TS_ShakeCamera_PB>.Create(graph, behaviour);
	}
}
