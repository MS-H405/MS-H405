using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class TD_Totem_PA : PlayableAsset
{
	[SerializeField]	ExposedReference<GameObject> UeObj;		// トーテムの上
	[SerializeField]	ExposedReference<GameObject> NakaObj;	// トーテムの中
	[SerializeField]	ExposedReference<GameObject> SitaObj;	// トーテムの下

	[SerializeField]	ExposedReference<GameObject> SetEffekseerObject;

	// Factory method that generates a playable based on this asset
	public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
	{
		var behaviour = new TD_Totem_PB();

		behaviour.TotemObjList = UeObj.Resolve(graph.GetResolver());		// 上中下の順でセットする
		behaviour.TotemObjList = NakaObj.Resolve(graph.GetResolver());
		behaviour.TotemObjList = SitaObj.Resolve(graph.GetResolver());

		behaviour.SetEffekseerObject = SetEffekseerObject.Resolve(graph.GetResolver());

		return ScriptPlayable<TD_Totem_PB>.Create(graph, behaviour);
	}
}
