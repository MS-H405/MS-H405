using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class BS_HermitCrab_PA : PlayableAsset
{
	[SerializeField]	ExposedReference<GameObject> HermitCrabObj;
	[SerializeField]	ExposedReference<GameObject> EffekseerObj;

	// Factory method that generates a playable based on this asset
	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		var behaviour = new BS_HermitCrab_PB();

		behaviour.HermitCrabObj = HermitCrabObj.Resolve(graph.GetResolver());
		behaviour.EffekseerObj = EffekseerObj.Resolve(graph.GetResolver());

		return ScriptPlayable<BS_HermitCrab_PB>.Create(graph, behaviour);
	}
}
