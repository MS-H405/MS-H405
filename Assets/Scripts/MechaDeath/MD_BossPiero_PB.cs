using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class MD_BossPiero_PB : PlayableBehaviour
{
	#region 定数

	const float CON_SPARK_TIME = 0.8f;		// スパークエフェクトを出す時間
	const float CON_BOMB_TIME = 4.8f;		// 爆発エフェクトを出す時間
	const float CON_DELETE_TIME = 5.2f;		// 消える時間

	#endregion


	#region 変数

	private GameObject _EnemyObj;
	public GameObject EnemyObj { get; set; }

	float fTime = 0.0f;
	bool[] bFlgs = new bool[5];


	// Effekseer
	private GameObject _EffekseerObj;
	public GameObject EffekseerObj { get; set; }
	SetEffekseerObject cs_SetEffekseerObject;

	private GameObject _ShakeCameraObj;
	public GameObject ShakeCameraObj { get; set; }
	ShakeCamera cs_ShakeCamera;

	#endregion


	public override void OnGraphStart(Playable playable)
	{
		cs_SetEffekseerObject = EffekseerObj.GetComponent<SetEffekseerObject>();

		for(int i = 0 ; i < bFlgs.GetLength(0) ; i ++)
			bFlgs[i] = true;

		cs_SetEffekseerObject = EffekseerObj.GetComponent<SetEffekseerObject>();
		cs_ShakeCamera = ShakeCameraObj.GetComponent<ShakeCamera>();
	}


	public override void OnGraphStop(Playable playable)
	{
		
	}


	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		
	}


	public override void OnBehaviourPause(Playable playable, FrameData info)
	{
		
	}


	public override void PrepareFrame(Playable playable, FrameData info)
	{
		fTime += Time.deltaTime;

		if (fTime >= CON_SPARK_TIME && bFlgs[0])				// スパーク
		{
			cs_SetEffekseerObject.NewEffect(1);
			bFlgs[0] = false;
		}
		else if (fTime >= CON_BOMB_TIME && bFlgs[1])			// 爆発
		{
			cs_SetEffekseerObject.NewEffect(2);
			MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.MD_Bomb);

			// 画ぶれ
			cs_ShakeCamera.SetParam(0.03f, 0.002f);
			cs_ShakeCamera.DontMoveShake();

			bFlgs[1] = false;
		}
		else if (fTime >= CON_DELETE_TIME && bFlgs[2])			// 消す
		{
			MonoBehaviour.Destroy(EnemyObj);
			bFlgs[2] = false;
		}
	}
}
