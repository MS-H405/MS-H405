using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class TD_TotemBody_PA : PlayableAsset
{
	[SerializeField]	ExposedReference<GameObject> TotemBodyObj;	// トーテム親
	[SerializeField]	Material TotemMat;		// トーテムのマテリアル
	[SerializeField]	ExposedReference<GameObject> MainCameraObj;	// カメラ


	// Factory method that generates a playable based on this asset
	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		var behaviour = new TD_TotemBody_PB();

		behaviour.TotemBodyObj = TotemBodyObj.Resolve(graph.GetResolver());
		behaviour.TotemBodyMat = TotemMat;
		behaviour.MainCameraObj = MainCameraObj.Resolve(graph.GetResolver());

		return ScriptPlayable<TD_TotemBody_PB>.Create(graph, behaviour);
	}
}
