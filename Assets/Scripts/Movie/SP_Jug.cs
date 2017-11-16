using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class SP_Jug : MonoBehaviour
{
	#region 定数
	int MAX_PIN = 10;			// なんとなく10本投げる
	float THROW_DIS = 1.0f;		// ピンの、プレイヤーからの出現距離
	float MOVE_TIME = 1.0f;		// ピンの移動時間
	float ENEMY_DIS = 5.0f;		// 展開されたピンが静止する、敵からの距離
	#endregion

	[SerializeField] GameObject PinObj;		// 投げるピン
	[SerializeField] GameObject PlayerObj;
	GameObject EnemyObj;

	List<SP_JugChild> PinList = new List<SP_JugChild>();	// 投げたピン

	BezierCurve.tBez[] tbezier;		// ベジエ曲線移動(fTime, vStart, vMiddle, vEnd)
	float fSecTime;					// 1秒あたりに増加するfTime
	float[] fRotate;				// 1秒あたりに回転する角度

	float time = 0.0f;

	// Use this for initialization
	void Start ()
	{
		EnemyObj = GameObject.Find("Enemy");


		tbezier = new BezierCurve.tBez[MAX_PIN];
		fRotate = new float[MAX_PIN];

		fSecTime = 1.0f / MOVE_TIME;

		// 初期情報を計算する
		Vector2 rot;	// end座標を計算するのに使う
		for(int i = 0 ; i < MAX_PIN ; i ++)
		{
			rot.x = (i * 30 - 45) * Mathf.Deg2Rad;		// この式だと、0~4までは右へ、5~9までは左へ飛んでいく。
			if(i % 2 == 0)
				rot.y = Mathf.PI / 6.0f;	// 30度(下にあるほう)
			else
				rot.y = Mathf.PI / 3.0f;	// 60度(上にあるほう)

			tbezier[i].time = 0.0f;
			tbezier[i].start = PlayerObj.transform.position + PlayerObj.transform.forward * THROW_DIS;						// start座標も若干ずらしたほうがいいのかもしれない。
			tbezier[i].end = new Vector3(EnemyObj.transform.position.x + ENEMY_DIS * Mathf.Cos(rot.x) * Mathf.Cos(rot.y),
										 EnemyObj.transform.position.y + ENEMY_DIS * Mathf.Sin(rot.y),
										 EnemyObj.transform.position.z + ENEMY_DIS * Mathf.Sin(rot.x) * Mathf.Cos(rot.y));
			tbezier[i].end += new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));	// end座標を適当にずらす。
			tbezier[i].middle = new Vector3((tbezier[i].start.x + tbezier[i].end.x) / 2.0f,
											tbezier[i].start.y + (tbezier[i].end.y - tbezier[i].start.y) * 2.0f,
											(tbezier[i].start.z + tbezier[i].end.z) / 2.0f);
			fSecTime = 1.0f / MOVE_TIME;
			fRotate[i] = Random.Range(1000.0f, 2000.0f);
		}
	}
	
	
	public bool Update_Expansion ()
	{
		time += fSecTime * Time.deltaTime;
		if(time > 1.0f)
		{// 移動終了
			time = 1.0f;
			for(int i = 0 ; i < PinList.Count ; i ++)
				PinList[i].LookAtEnemy(EnemyObj.transform.position);		// 敵のほうを向く

			return true;
		}

		for (int i = 0; i < PinList.Count; i++)
			PinList[i].Update_Expansion(time);								// 展開移動

		return false;
	}


	// ピン展開
	public void Jug_ThrowExpansion()
	{
		for(int i = 0 ; i < MAX_PIN ; i ++)
		{
			GameObject obj = Instantiate(PinObj as GameObject, PlayerObj.transform.position + PlayerObj.transform.forward * THROW_DIS, Quaternion.identity);
			obj.GetComponent<SP_JugChild>().SetParam(tbezier[i], fSecTime, fRotate[i]);
			PinList.Add(obj.GetComponent<SP_JugChild>());
		}
	}
}
