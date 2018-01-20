using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class Special_3BossTama : MonoBehaviour
{
	#region define

	readonly int NeedleAmount = 20;

	#endregion

	#region variable

	[SerializeField]
	GameObject _needlePrefab = null;

	#endregion

	#region method

	#endregion

	#region unity_event

	/// <summary>
	/// 更新前処理
	/// </summary>
	private void Start()
	{
		float angle = 360.0f / NeedleAmount / 2.0f;
		for (int i = 0; i < NeedleAmount; i++)
		{
			GameObject obj = Instantiate(_needlePrefab, transform.position, _needlePrefab.transform.rotation);
			obj.transform.SetParent(transform);
			obj.transform.eulerAngles += new Vector3(0, 0, angle);
			obj.transform.position += obj.transform.up * 1.65f;
			angle += 360.0f / NeedleAmount;
		}
	}

	#endregion
}  