using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BS_Fire : MonoBehaviour
{
	#region 定数

	const float CON_FIRE_START = 16.0f;
	const float CON_FIRE_EMD = 18.0f;

	#endregion


	#region 変数

	private List<GameObject> _pipeList = new List<GameObject>();
    private List<ParticleSystem> _pipeFireList = new List<ParticleSystem>();
    [SerializeField] GameObject _chargeFireEffect = null;
    [SerializeField] GameObject _rollFireEffect = null;

	float fTime = 0.0f;

	#endregion


	// Use this for initialization
	void Start ()
	{
		List<GameObject> allChild = GetAllChildren.GetAll(gameObject);
		_pipeList = allChild.Where(_ => _.name.Contains("pipe")).ToList();
		
		// パイプから出る炎を作成
		foreach (GameObject pipe in _pipeList)
		{
			GameObject effect = Instantiate(_rollFireEffect, pipe.transform.position, pipe.transform.rotation);
			effect.transform.SetParent(pipe.transform);
			effect.transform.localEulerAngles -= new Vector3(90, 0, 0);
			effect.name += " : " + pipe.name;
			_pipeFireList.Add(pipe.transform.GetChild(0).GetComponentInChildren<ParticleSystem>());
		}

	}
	
	// Update is called once per frame
	void Update ()
	{
		fTime += Time.deltaTime;

		if(CON_FIRE_START <= fTime && fTime <= CON_FIRE_EMD)
		{
			foreach (ParticleSystem pipeFire in _pipeFireList)
			{
				string pipeName = pipeFire.transform.parent.name;
				if (!pipeName.Contains("B1") && !pipeName.Contains("B3") &&
					!pipeName.Contains("B4") && !pipeName.Contains("B7"))
				{
					continue;
				}

				pipeFire.Emit(Mathf.RoundToInt(100 * Time.deltaTime + 0.5f));
			}
		}
	}
}
