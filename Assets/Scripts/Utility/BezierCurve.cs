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

	// 3次ベジエ曲線
	public struct tBez3
	{
		public float time;								// 時間
		public Vector3 start, middle1, middle2, end;	// 点
	}

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

<<<<<<< HEAD
	static public Vector3 CulcBez3(tBez3 bez)
	{
		float x = Mathf.Pow((1 - bez.time), 3) * bez.start.x +
					3 * Mathf.Pow((1 - bez.time), 2) * bez.time * bez.middle1.x +
					3 * (1 - bez.time) * Mathf.Pow(bez.time, 2) * bez.middle2.x +
					Mathf.Pow(bez.time, 3) * bez.end.x;
		float y = Mathf.Pow((1 - bez.time), 3) * bez.start.y +
					3 * Mathf.Pow((1 - bez.time), 2) * bez.time * bez.middle1.y +
					3 * (1 - bez.time) * Mathf.Pow(bez.time, 2) * bez.middle2.y +
					Mathf.Pow(bez.time, 3) * bez.end.y;
		float z = Mathf.Pow((1 - bez.time), 3) * bez.start.z +
					3 * Mathf.Pow((1 - bez.time), 2) * bez.time * bez.middle1.z +
					3 * (1 - bez.time) * Mathf.Pow(bez.time, 2) * bez.middle2.z +
					Mathf.Pow(bez.time, 3) * bez.end.z;
		return new Vector3 (x, y, z);
	}
=======

>>>>>>> 0f9d08e5520a519a59bafa99a94ecdad0b99173a
}
