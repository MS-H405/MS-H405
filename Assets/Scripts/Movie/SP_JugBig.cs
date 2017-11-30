using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_JugBig : MonoBehaviour
{
	#region 定数

	float ROTATE = 1000.0f;		// 1秒間で回転する角度

	#endregion


	#region 変数

	[SerializeField] GameObject MyPinObj;
	Vector3 vStartPos = Vector3.zero;			// 突撃し始めた座標

	#endregion


	// Use this for initialization
	void Start ()
	{
		MyPinObj.SetActive(false);
	}

	// デカピン出現
	public void JugBig_Appear()
	{
		MyPinObj.SetActive(true);
		GetComponent<EffekseerEmitter>().Play();
	}
	
	// デカピンを投げる
	public void JugBig_Throw(float rotateZ)
	{
		transform.parent = null;		// プレイヤーの手との親子関係を切り離す。
		transform.eulerAngles = new Vector3(0.0f, 0.0f, rotateZ);
	}

	// 敵に突撃
	public void GoEnemy_Big(float time, Vector3 targetpos)
	{
		// 移動開始座標を保存
		if (vStartPos == Vector3.zero)
			vStartPos = transform.position;

		transform.position = Vector3.Lerp(vStartPos, targetpos, time);
		transform.Rotate(Vector3.right, ROTATE * Time.unscaledDeltaTime, Space.Self);	
	}

	// 死ぬ
	public void PinDestroy()
	{
		Destroy(this.gameObject);
	}
}
