// ---------------------------------------------------------
// NeedleManager.cs
// 概要 : 
// 作成者: Shota_Obora
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class MS_NeedleManager : MonoBehaviour
{
	#region define

	readonly int NeedleAmount = 20;
	const float CON_EFFECT_TIME = 1.0f;		// 棘がこの値ぶん進んだらエフェクト発生

	#endregion

	#region variable

	[SerializeField]
	GameObject _needlePrefab = null;
	private List<GameObject> _needleList = new List<GameObject>();

	bool bEffect = true;

	#endregion

	#region method

	#endregion

	#region unity_event

	// 棘が生える
	public IEnumerator NeedleAppear()
	{
		Vector3 InitPos = transform.position;

		float angle = 360.0f / NeedleAmount / 2.0f;
		for (int i = 0; i < NeedleAmount; i++)
		{
			GameObject obj = Instantiate(_needlePrefab, transform.position, _needlePrefab.transform.rotation);
			obj.transform.SetParent(transform);
			obj.transform.eulerAngles += new Vector3(0, 0, angle);
			angle += 360.0f / NeedleAmount;
			_needleList.Add(obj);
		}

		yield return null;

		float len = 0.0f;
		while(len < 1.85f)
		{
			len += 1.65f * Time.deltaTime;
			if(len >= 1.85f)
			{
				len = 1.85f;
			}

			if(len >= CON_EFFECT_TIME && bEffect)
			{
				foreach (GameObject needle in _needleList)
				{
					needle.GetComponent<EffekseerEmitter>().Play();
				}
				bEffect = false;
			}

			foreach (GameObject needle in _needleList)
			{
				needle.transform.position = InitPos + needle.transform.up * len;
			}
			yield return null;
		}
	}

	#endregion
}