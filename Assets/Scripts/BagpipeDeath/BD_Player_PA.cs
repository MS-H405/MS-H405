using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class BD_Player_PA : PlayableAsset
{
	[SerializeField]	ExposedReference<GameObject> PlayerObj;
	[SerializeField]	ExposedReference<GameObject> EffekseerObj;
	[SerializeField]	ExposedReference<GameObject> EffectObj_1;
	[SerializeField]	ExposedReference<GameObject> EffectObj_2;

	[SerializeField]	GameObject BossDeathObj;	// 技獲得UI

	// Factory method that generates a playable based on this asset
	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		var behaviour = new BD_Player_PB();

		behaviour.PlayerObj = PlayerObj.Resolve(graph.GetResolver());
		behaviour.EffekseerObj = EffekseerObj.Resolve(graph.GetResolver());
		behaviour.EffectObj_1 = EffectObj_1.Resolve(graph.GetResolver());
		behaviour.EffectObj_2 = EffectObj_2.Resolve(graph.GetResolver());
		behaviour.BossDeathObj = BossDeathObj;

		return ScriptPlayable<BD_Player_PB>.Create(graph, behaviour);
	}
}
