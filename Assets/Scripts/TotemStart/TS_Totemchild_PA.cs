using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class TS_Totemchild_PA : PlayableAsset
{
	[SerializeField]	ExposedReference<GameObject> TotemChildObj1;		// ちびトーテム
	[SerializeField]	ExposedReference<GameObject> TotemChildObj2;		// ちびトーテム
	[SerializeField]	ExposedReference<GameObject> TotemChildObj3;		// ちびトーテム
	[SerializeField]	ExposedReference<GameObject> TotemChildObj4;		// ちびトーテム
	[SerializeField]	ExposedReference<GameObject> TotemChildObj5;		// ちびトーテム

	// Factory method that generates a playable based on this asset
	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		var behaviour = new TS_Totemchild_PB();

		behaviour.TotemChildObj = TotemChildObj1.Resolve(graph.GetResolver());
		behaviour.TotemChildObj = TotemChildObj2.Resolve(graph.GetResolver());
		behaviour.TotemChildObj = TotemChildObj3.Resolve(graph.GetResolver());
		behaviour.TotemChildObj = TotemChildObj4.Resolve(graph.GetResolver());
		behaviour.TotemChildObj = TotemChildObj5.Resolve(graph.GetResolver());

		return ScriptPlayable<TS_Totemchild_PB>.Create(graph, behaviour);
	}
}
