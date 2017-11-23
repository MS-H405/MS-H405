using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Special_1Ball : MonoBehaviour
{
	#region 定数

	const float CON_ROTATION_WAIT = 0.5f;		// 回転を開始するまでの時間
	const float CON_ROTATION_TIME = 1.0f;		// 最大回転速度になるまでの時間
	readonly Vector3 CON_MAX_ROTATION = new Vector3(1800.0f, 0.0f, 0.0f);	// 最大回転速度　なんとなく秒間5回転

	#endregion

	#region 変数

	MeshRenderer meshrenderer;

	float fTime;
	float fWait;
	Vector3 vRotate;

	#endregion



	// Use this for initialization
	void Start ()
	{
		meshrenderer = GetComponent<MeshRenderer>();
		meshrenderer.enabled = false;

		fTime = 0.0f;
		fWait = 0.0f;
	}


	// 玉出現
	public void BallAppear()
	{
		meshrenderer.enabled = true;
	}

	// 玉回転開始
	public bool StartRotation()
	{
		// 待ち時間
		fWait += Time.deltaTime;
		if(fWait < CON_ROTATION_WAIT)
			return false;

		// 加速
		fTime += Time.deltaTime / CON_ROTATION_TIME;
		if(fTime >= 1.0f)
		{
			transform.Rotate(CON_MAX_ROTATION * Time.deltaTime);
			return true;
		}

		Vector3 temp = Vector3.Lerp(Vector3.zero, CON_MAX_ROTATION, fTime);
		transform.Rotate(temp * Time.deltaTime);

		return false;
	}

	// 玉回転
	public void Rotation()
	{
		transform.Rotate(CON_MAX_ROTATION * Time.deltaTime);
	}
}
