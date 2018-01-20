using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class TD_Player_PA : PlayableAsset
{
	[SerializeField]	ExposedReference<GameObject> PlayerObj;
	[SerializeField]	ExposedReference<GameObject> EffectObj_1;
	[SerializeField]	ExposedReference<GameObject> EffectObj_2;

	// Factory method that generates a playable based on this asset
	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{

		var behaviour = new TD_Player_PB();

		behaviour.PlayerObj = PlayerObj.Resolve(graph.GetResolver());
		behaviour.EffectObj_1 = EffectObj_1.Resolve(graph.GetResolver());
		behaviour.EffectObj_2 = EffectObj_2.Resolve(graph.GetResolver());

		return ScriptPlayable<TD_Player_PB>.Create(graph, behaviour);
	}
}
