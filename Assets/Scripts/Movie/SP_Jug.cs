using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_Jug : MonoBehaviour
{
	#region 定数
	float THROW_DIS = 1.0f;		// ピンの、プレイヤーからの出現距離
	#endregion

	[SerializeField] GameObject PinObj;
	[SerializeField] GameObject PlayerObj;

	Vector2[] vKakudoArray = new Vector2[6];	// なんとなく6本投げる


	// Use this for initialization
	void Start ()
	{
		vKakudoArray[0] = new Vector2(0.906308f, -0.422618f);	//一番左(下)	-25		「三角関数　計算」で検索した
		vKakudoArray[1] = new Vector2(0.965926f, -0.258819f);	//				-15
		vKakudoArray[2] = new Vector2(0.996195f, -0.087156f);	//				- 5
		vKakudoArray[3] = new Vector2(0.996195f,  0.087156f);	//				  5
		vKakudoArray[4] = new Vector2(0.965926f,  0.258819f);	//				 15
		vKakudoArray[5] = new Vector2(0.906308f, 0.422618f);	//一番右(上)	 25
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			Jug_ThrowSide();
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			Jug_ThrowVertical();
		}
	}


	// 横投げ
	public void Jug_ThrowSide()
	{
		Vector3 dir;

		for(int i = 0 ; i < vKakudoArray.GetLength(0) ; i ++)
		{
			dir = new Vector3(vKakudoArray[i].y, 0.0f, vKakudoArray[i].x);

			GameObject obj = Instantiate(PinObj as GameObject, PlayerObj.transform.position + dir * THROW_DIS, Quaternion.identity);
			obj.GetComponent<SP_JugChild>().SetParam(dir, true);
		}
	}

	// 縦投げ
	public void Jug_ThrowVertical()
	{
		Vector3 dir;

		for (int i = 0; i < vKakudoArray.GetLength(0); i++)
		{
			dir = new Vector3(0.0f, vKakudoArray[i].y, vKakudoArray[i].x);

			GameObject obj = Instantiate(PinObj as GameObject, PlayerObj.transform.position + dir * THROW_DIS, Quaternion.identity);
			obj.GetComponent<SP_JugChild>().SetParam(dir, false);
		}
	}
}
