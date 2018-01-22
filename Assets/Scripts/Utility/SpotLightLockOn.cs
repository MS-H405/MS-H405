using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightLockOn : MonoBehaviour
{
	/// <summary>
	/// スポットライト制御
	/// 
	/// 制作者 : 原田達夫
	/// </summary>
	
	[SerializeField]	Transform Target;	// スポットライトで照らし続けるオブジェクト
	private Transform[] SpotLightArray;

	// Use this for initialization
	void Start ()
	{
		SpotLightArray = GetComponentsInChildren<Transform>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		foreach(Transform trans in SpotLightArray)
		{
			if (trans.gameObject.activeSelf && trans != transform)		// このスクリプトがアタッチされているオブジェクトも入っっているので、それだけは処理しない
				trans.LookAt(Target);
		}
	}
}
