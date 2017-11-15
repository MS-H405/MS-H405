using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 全部テスト用　削除してもOK
public class TatsuoTest_Enemy : MonoBehaviour
{
	[SerializeField]	float fRadTime;		// 1回転にかかる時間

	float r;
	float rad;

	float speed;

	bool bDive = false;

	// Use this for initialization
	void Start ()
	{
		r = 5.0f;
		rad = 0.0f;
		speed = 10.0f;

		//transform.position = new Vector3(r * Mathf.Cos(rad), 0.0f,transform.position.z);
	}
	
	// Update is called once per frame
	void Update ()
	{
		//if(!Input.GetKey(KeyCode.Q))
		//	rad += 2 * Mathf.PI * Time.deltaTime  / fRadTime;
		//if (rad > 2 * Mathf.PI)		rad -= 2 * Mathf.PI;
		//if (rad < -2 * Mathf.PI)	rad += 2 * Mathf.PI;

		//transform.position = new Vector3(r * Mathf.Cos(rad), r * Mathf.Sin(rad), transform.position.z);		// XY円運動
		//transform.position = new Vector3(r * Mathf.Cos(rad), transform.position.y, r * Mathf.Sin(rad));		// XZ円運動
		//transform.position = new Vector3(transform.position.x, r * Mathf.Sin(rad), transform.position.z);	// 縦移動
		//transform.position = new Vector3(r * Mathf.Cos(rad), transform.position.y, transform.position.z);	// 横移動

		// 等速横
		//transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
		//if(Mathf.Abs(transform.position.x) >= 6.0f)
		//	speed *= -1;



		// 潜って出現
		if(Input.GetKeyDown(KeyCode.R))
		{
			if(!bDive)
			{
				transform.position = new Vector3(0.0f, 100.0f, 0.0f);	// 見えないところにワープ
				bDive = true;
			}
			else
			{
				transform.position = new Vector3(Random.Range(-7.5f, 7.5f), 0.5f, Random.Range(-7.5f, 7.5f));	// ワープ出現
				bDive = false;
			}

		}
	}
}
