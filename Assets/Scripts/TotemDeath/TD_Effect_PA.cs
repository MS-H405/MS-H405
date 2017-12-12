using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class TD_Effect_PA : PlayableAsset
{
	[SerializeField]	ExposedReference<GameObject> SetEfekseerObj;	// SetEfekseerObj
	[SerializeField]	ExposedReference<GameObject> TD_powerup_ueObj;
	[SerializeField]	ExposedReference<GameObject> TD_powerup_nakaObj;
	[SerializeField]	ExposedReference<GameObject> TD_powerup_sitaObj;

	// Factory method that generates a playable based on this asset
	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		var behaviour = new TD_Effect_PB();

		behaviour.SetEffekseerObj = SetEfekseerObj.Resolve(graph.GetResolver());
		behaviour.TD_powerupList = TD_powerup_ueObj.Resolve(graph.GetResolver());
		behaviour.TD_powerupList = TD_powerup_nakaObj.Resolve(graph.GetResolver());
		behaviour.TD_powerupList = TD_powerup_sitaObj.Resolve(graph.GetResolver());

		return ScriptPlayable<TD_Effect_PB>.Create(graph, behaviour);
	}
}
