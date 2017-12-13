using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class TS_Boss_PA : PlayableAsset
{
	[SerializeField]	ExposedReference<GameObject> BossObj;
	[SerializeField]	ExposedReference<GameObject> BossAppearObj;
	[SerializeField]	ExposedReference<GameObject> BossRoarObj;
	[SerializeField]	ExposedReference<GameObject> BossDiveObj;
	[SerializeField]	Material TotemMaterial;


	// Factory method that generates a playable based on this asset
	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		var behaviour = new TS_Boss_PB();

		behaviour.BossObj = BossObj.Resolve(graph.GetResolver());
		behaviour.BossAppearObj = BossAppearObj.Resolve(graph.GetResolver());
		behaviour.BossRoarObj = BossRoarObj.Resolve(graph.GetResolver());
		behaviour.BossDiveObj = BossDiveObj.Resolve(graph.GetResolver());
		behaviour.mat = TotemMaterial;

		return ScriptPlayable<TS_Boss_PB>.Create(graph, behaviour);
	}
}
