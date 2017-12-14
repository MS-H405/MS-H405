using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_JugChild : MonoBehaviour
{

	#region 定数

	

	#endregion

	#region 変数

	[SerializeField] GameObject PinObj;			// 子のピン

	BezierCurve.tBez tbezier;
	float fSecTime;
	float fRotate;
	Vector3 vStartPos = Vector3.zero;			// 突撃し始めた座標

	#endregion


	// ピン展開
	public void Update_Expansion(float time)
	{
		tbezier.time = time;
		transform.position = BezierCurve.CulcBez(tbezier, true);
		transform.Rotate(Vector3.right, fRotate * Time.deltaTime, Space.Self);
	}

	// 敵のほうを向く
	public void LookAtEnemy(Vector3 pos)
	{
		transform.LookAt(pos);
		transform.eulerAngles += new Vector3(90.0f, 0.0f, 0.0f);	// 頭を敵のほうにむける
	}

	// 進行方向と、回転軸を設定してもらう
	public void SetParam(BezierCurve.tBez bezier, float sectime, float rotate)
	{
		tbezier = bezier;
		fSecTime = sectime;
		fRotate = rotate;
		
		// 初期角度は、startとendのxとy座標の差から角度を計算して出現した時にz軸回転させておく。
		// そしてupdateではローカルx軸回転させたら、いい感じになる。
		Vector2 XY = tbezier.end - tbezier.start;
		XY = XY.normalized;
		float sita = Mathf.Acos(Vector2.Dot(Vector2.right, XY));
		sita = ((Mathf.PI / 2.0f) - sita) * -1 * Mathf.Rad2Deg;
		transform.eulerAngles = new Vector3(0.0f, 0.0f, sita);
	}

	// 敵に突撃する
	public void GoEnemy(float time, Vector3 targetpos)
	{
		// 移動開始座標を保存
		if(vStartPos == Vector3.zero)
			vStartPos = transform.position;

		transform.position = Vector3.Lerp(vStartPos, targetpos, time);
	}

	// 死ぬ
	public void PinDestroy()
	{
		Destroy(this.gameObject);
	}
}
