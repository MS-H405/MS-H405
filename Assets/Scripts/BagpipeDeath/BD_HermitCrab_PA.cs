using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class BD_HermitCrab_PA : PlayableAsset
{
	[SerializeField]	ExposedReference<GameObject> HermitCrabObj;
	[SerializeField]	ExposedReference<GameObject> EffekseerObj;
    [SerializeField]    ExposedReference<GameObject> ShakeCameraObj;
    [SerializeField]	Material mat;

	// Factory method that generates a playable based on this asset
	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		var behaviour = new BD_HermitCrab_PB();

		behaviour.HermitCrabObj = HermitCrabObj.Resolve(graph.GetResolver());
		behaviour.EffekseerObj = EffekseerObj.Resolve(graph.GetResolver());
        behaviour.ShakeCameraObj = ShakeCameraObj.Resolve(graph.GetResolver());
        behaviour.mat = mat;

		return ScriptPlayable<BD_HermitCrab_PB>.Create(graph, behaviour);
	}
}
