using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_Enemy : MonoBehaviour
{
	#region 定数

	readonly Vector3 CON_FLY_SPEED = new Vector3(0.0f, 30.0f, 45.0f);	// ぶっ飛ばされる時のスピード
	const float CON_DELETE_TIME = 1.0f;				// 吹っ飛んでから消えるまでの時間

	#endregion

	#region 変数



	// Effekseer関係
	SetEffekseerObject cs_SetEffekseerObject;

	#endregion


	// Use this for initialization
	void Start ()
	{
		
	}
}
