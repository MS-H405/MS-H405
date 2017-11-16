using UnityEngine;
using System.Collections;

public class BezierCurve
{
	/// <summary>
	/// 概要 : ベジエ処理の計算
	/// Author : 大洞祥太
	/// </summary>

	public struct tBez
    {
		public float time;				    // 時間
		public Vector3 start,middle,end;	// 点
	};

	static public Vector3 CulcBez(tBez bez)
    {

		float x = (1 - bez.time) * (1 - bez.time) * bez.start.x + 2 * (1 - bez.time) * bez.time * bez.middle.x + bez.time * bez.time * bez.end.x;
		float y = (1 - bez.time) * (1 - bez.time) * bez.start.y + 2 * (1 - bez.time) * bez.time * bez.middle.y + bez.time * bez.time * bez.end.y;
		return new Vector3 (x, y, 0.0f);
	}

	static public Vector3 CulcBez(tBez bez, bool bZ) 
	{
		float x = (1 - bez.time) * (1 - bez.time) * bez.start.x + 2 * (1 - bez.time) * bez.time * bez.middle.x + bez.time * bez.time * bez.end.x;
		float y = (1 - bez.time) * (1 - bez.time) * bez.start.y + 2 * (1 - bez.time) * bez.time * bez.middle.y + bez.time * bez.time * bez.end.y;
		float z = (1 - bez.time) * (1 - bez.time) * bez.start.z + 2 * (1 - bez.time) * bez.time * bez.middle.z + bez.time * bez.time * bez.end.z;
		return new Vector3 (x, y, z);
	}
}
