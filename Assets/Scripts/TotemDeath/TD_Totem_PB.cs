using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class TD_Totem_PB : PlayableBehaviour
{
	#region 定数

	readonly Vector3 CON_FORCE_UE = new Vector3(900.0f, -300.0f, -100.0f);			// トーテム上にかかる力
	readonly Vector3 CON_FORCE_NAKA = new Vector3(-1200.0f, -300.0f, -200.0f);		// トーテム中にかかる力
	readonly Vector3 CON_FORCE_SITA = new Vector3(600.0f, 0.0f, 200.0f);			// トーテム下にかかる力

	//const float CON_EFFECT_SITA = 0.5f;		// トーテム下用のエフェクトが発生する時間
	//const float CON_EFFECT_UE = 0.8f;		// トーテム上用のエフェクトが発生する時間
	//const float CON_EFFECT_NAKA = 1.2f;		// トーテム中用のエフェクトが発生する時間

	const float CON_DESTROY_TIME = 6.0f;		// 消える時間

	#endregion


	#region 変数

	private List<GameObject> _TotemObjList = new List<GameObject>();		// 0:上、1:中、2:下の順でセットしてもらう
	public GameObject TotemObjList
	{
		set { _TotemObjList.Add(value); }
	}

	private GameObject _SetEffekseerObject;
	public GameObject SetEffekseerObject { get; set; }
	
	private List<Rigidbody> RigitList = new List<Rigidbody>();

	float fTime = 0.0f;
	bool bDestroy = true;

	#endregion



	public override void OnGraphStart(Playable playable)
	{
		// Rigidbody取得、物理挙動無効
		for(int i = 0 ; i < _TotemObjList.Count ; i ++)
		{
			RigitList.Add(_TotemObjList[i].GetComponent<Rigidbody>());
			RigitList[i].isKinematic = true;
		}
	}


	public override void OnGraphStop(Playable playable)
	{
		//// オブジェクト消去
		//for (int i = 0; i < _TotemObjList.Count; i++)
		//	GameObject.Destroy(_TotemObjList[i]);
	}


	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		// 物理挙動開始　＆　一瞬力をかけて、トーテムを崩す
		for (int i = 0; i < _TotemObjList.Count; i++)
		{
			RigitList[i].isKinematic = false;
		}
		RigitList[0].AddForce(CON_FORCE_UE, ForceMode.Impulse);
		RigitList[1].AddForce(CON_FORCE_NAKA, ForceMode.Impulse);
		RigitList[2].AddForce(CON_FORCE_SITA, ForceMode.Impulse);
	}


	public override void OnBehaviourPause(Playable playable, FrameData info)
	{

	}


	public override void PrepareFrame(Playable playable, FrameData info)
	{
		// 消えてたら処理しない
		if(!bDestroy)
			return;

		for (int i = 0; i < _TotemObjList.Count; i++)
		{
			RigitList[i].AddForce(new Vector3(0.0f, -10000.0f, 0.0f));	// 下方向に力をかけて重そうにｽﾞﾄﾞﾝとさせる
		}

		// オブジェクト消去
		fTime += Time.deltaTime;
		if (bDestroy && fTime > CON_DESTROY_TIME)
		{
			for (int i = 0; i < _TotemObjList.Count; i++)
				GameObject.Destroy(_TotemObjList[i]);
			bDestroy = false;
		}
	}
}
