using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class TS_Boss_PA : PlayableAsset
{
	[SerializeField]	ExposedReference<GameObject> BossObj;
	[SerializeField]	ExposedReference<GameObject> BossAppearObj;


	// Factory method that generates a playable based on this asset
	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		var behaviour = new TS_Boss_PB();

		behaviour.BossObj = BossObj.Resolve(graph.GetResolver());
		behaviour.BossAppearObj = BossAppearObj.Resolve(graph.GetResolver());

		return ScriptPlayable<TS_Boss_PB>.Create(graph, behaviour);
	}
}
