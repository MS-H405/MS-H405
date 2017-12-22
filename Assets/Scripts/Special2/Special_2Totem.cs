﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Special_2Totem : MonoBehaviour
{
	#region 定数

	const float CON_POKE_TIME = 0.5f;		// 突き出す時間
	readonly Vector3 CON_END_POS = new Vector3(0.0f, 2.5f, -112.5f);	// 突き終わりの座標(ローカル)
	readonly Vector3 CON_ROTATE = new Vector3(0.0f, 10.0f, 0.0f);		// 回転

	const float CON_FIN = 0.5f;	// 割合がこれだけいったら、月終わりを待たずに、トーテム突きフェイズ終了

	#endregion


	#region 変数

	float fParam = 0.0f;
	Vector3 vStartPos;

	GameObject PokeTotemObj;
	bool bInit = true;

	bool bFin = true;

	#endregion



	// Use this for initialization
	void Start ()
	{
		PokeTotemObj = transform.GetChild(0).gameObject;
		PokeTotemObj.SetActive(false);	// 最初は非表示

		vStartPos = transform.localPosition;
	}

	// 突き出す
	public bool Poke()
	{
		// トーテム出現＆エフェクト出現
		if(bInit)
		{
			PokeTotemObj.SetActive(true);

			bInit = false;
		}

		fParam += Time.deltaTime / CON_POKE_TIME;
		if (fParam >= 1.0f)
		{
			transform.localPosition = Vector3.Lerp(vStartPos, CON_END_POS, 1.0f);
		}

		transform.localPosition = Vector3.Lerp(vStartPos, CON_END_POS, fParam);
		PokeTotemObj.transform.localEulerAngles += CON_ROTATE;

		// 次のフェイズに渡す(まだ動き続けるけど...)
		if(fParam >= CON_FIN && bFin)
		{
			bFin = false;
			return true;
		}


		return false;
	}
}
