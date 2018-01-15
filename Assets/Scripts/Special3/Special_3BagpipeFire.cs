using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Special_3BagpipeFire : MonoBehaviour
{
	#region 定数

	const float CON_WAIT = 1.5f;
	readonly Vector3 CON_START_SPEED = new Vector3(0.0f, 5.0f, 5.0f);
	readonly Vector3 CON_ACCELE = new Vector3(0.0f, -5.0f, 0.0f);

	#endregion


	#region 変数

	[SerializeField]	Transform SP_BagpipeObj;
	[SerializeField]	GameObject SP_Bagpipe_HitObj;

	float fWait = 0.0f;
	bool bInitialize = true;
	bool bEnd = false;

	Vector3 vSpeed, vAccele;


	[SerializeField] GameObject SetEffekseerObj;
	SetEffekseerObject cs_SetEffekseerObject;

	#endregion

	// Use this for initialization
	void Start()
	{
		vSpeed = CON_START_SPEED;
		vAccele = CON_ACCELE;

		cs_SetEffekseerObject = SetEffekseerObj.GetComponent<SetEffekseerObject>();
	}

	public void Start_BagpipeFire()
	{
		StartCoroutine(BagpipeFire());
	}


	// プレイヤーのバグパイプから火が出て、着弾するまで
	private IEnumerator BagpipeFire()
	{
		while (true)
		{
			// 待機
			fWait += Time.deltaTime;
			if (fWait < CON_WAIT)
			{
				yield return null;
				continue;
			}

			if (bInitialize)
			{
				SP_BagpipeObj.gameObject.GetComponent<EffekseerEmitter>().Play();	// 火の玉エフェクト再生

				bInitialize = false;
			}

			vSpeed += vAccele * Time.deltaTime;
			SP_BagpipeObj.position += vSpeed * Time.deltaTime;

			// 終了判定
			if (SP_BagpipeObj.position.y <= 0.0f && !bEnd)
			{
				Vector3 temp = SP_BagpipeObj.transform.position;
				temp.y = 0.1f;		// エフェクト発生のY座標を、地面の少し上へ

				Instantiate(SP_Bagpipe_HitObj, temp, SP_Bagpipe_HitObj.transform.rotation);	// 着弾エフェクト＆ファイヤーロード　再生

				//Destroy(SP_BagpipeObj.gameObject);						// Destroyで消すと、消え方が不自然なので変更
				SP_BagpipeObj.gameObject.GetComponent<EffekseerEmitter>().StopRoot();	// 火の玉エフェクト停止
				break;
			}

			yield return null;
		}
	}
}
