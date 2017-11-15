using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_JugChild : MonoBehaviour
{

	#region 定数

	float DIR_SPEED = 10.0f;			// 1秒間に進む距離
	float ROTATE_TIME = 0.6f;		// ピンが1回転するのにかかる時間
	float LIFE = 1.0f;				// 生存時間

	#endregion

	[SerializeField] GameObject PinObj;			// 子のピン
	[SerializeField] GameObject HunnsyaObj;		// 子の噴射

	Vector3 vDir;					// 進行方向
	Vector3 vRotate;				// 1秒で回る回転角度
	float fLife = 0.0f;				// 生成されてからたった時間


	// Use this for initialization
	void Start ()
	{
		// ----- デバッグ -----



		// --------------------
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.position += vDir * DIR_SPEED * Time.deltaTime;
		PinObj.transform.Rotate(vRotate * Time.deltaTime);

		fLife += Time.deltaTime;
		if(fLife > LIFE)
			Destroy(this.gameObject);
	}

	// 進行方向と、回転軸を設定してもらう
	public void SetParam(Vector3 dir, bool side)
	{
		vDir = dir;
		
		// 角度
		if(side)
		{// Y軸回転(横投げ)
			PinObj.transform.eulerAngles = new Vector3(0.0f, Random.Range(0.0f, 360.0f), 90.0f);
			vRotate = new Vector3(360.0f / ROTATE_TIME, 0.0f, 0.0f);
		}
		else
		{// X軸回転(縦投げ)
			PinObj.transform.eulerAngles = new Vector3(Random.Range(0.0f, 360.0f), 0.0f, 0.0f);
			vRotate = new Vector3(360.0f / ROTATE_TIME, 0.0f, 0.0f);
		}
	}
}
