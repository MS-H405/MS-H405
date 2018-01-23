using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TatsuoTest_Effect : MonoBehaviour
{
	EffekseerEmitter em;

	// Use this for initialization
	void Start ()
	{
		em = GetComponent<EffekseerEmitter>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.O))
			em.Play();
		if (Input.GetKeyDown(KeyCode.P))
			em.StopRoot();
	}
}
