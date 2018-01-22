using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class MD_Player_PA : PlayableAsset
{
	[SerializeField]	ExposedReference<GameObject> PlayerObj;
	[SerializeField]	ExposedReference<GameObject> EffekseerObj;
	[SerializeField]	ExposedReference<GameObject> KamihubukiObj;
	[SerializeField]	ExposedReference<GameObject> KirakiraObj;
	[SerializeField]	ExposedReference<GameObject> ArmParticleObj_1;
	[SerializeField]	ExposedReference<GameObject> ArmParticleObj_2;

	// Factory method that generates a playable based on this asset
	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		var behaviour = new MD_Player_PB();

		behaviour.PlayerObj = PlayerObj.Resolve(graph.GetResolver());
		behaviour.EffekseerObj = EffekseerObj.Resolve(graph.GetResolver());
		behaviour.KamihubukiObj = KamihubukiObj.Resolve(graph.GetResolver());
		behaviour.KirakiraObj = KirakiraObj.Resolve(graph.GetResolver());
		behaviour.ArmParticleObj_1 = ArmParticleObj_1.Resolve(graph.GetResolver());
		behaviour.ArmParticleObj_2 = ArmParticleObj_2.Resolve(graph.GetResolver());

		return ScriptPlayable<MD_Player_PB>.Create(graph, behaviour);
	}
}
