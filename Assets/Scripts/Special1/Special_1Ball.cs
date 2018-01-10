using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Special_1Ball : MonoBehaviour
{
	#region 定数

	const float CON_ROTATION_WAIT = 0.8f;		// 回転を開始するまでの時間
	const float CON_ROTATION_TIME = 1.0f;		// 最大回転速度になるまでの時間
	//readonly Vector3 CON_MAX_ROTATION = new Vector3(1800.0f, 0.0f, 0.0f);	// 最大回転速度　なんとなく秒間5回転
	readonly Vector3 CON_MAX_ROTATION = new Vector3(360.0f, 0.0f, 0.0f);	// 最大回転速度　なんとなく秒間5回転

	#endregion

	#region 変数

	[SerializeField] GameObject BallObj;
	MeshRenderer meshrenderer;

	float fTime;
	float fWait;
	Vector3 vRotate;

	// Effekseer関係
	[SerializeField] GameObject SetEffekseerObj;
	SetEffekseerObject cs_SetEffekseerObject;
	bool bSP_ball_move;
	bool bSP_ball_speedup;

	#endregion



	// Use this for initialization
	void Start ()
	{
		meshrenderer = BallObj.GetComponent<MeshRenderer>();
		meshrenderer.enabled = false;

		fTime = 0.0f;
		fWait = 0.0f;

		// Effekseer関係
		cs_SetEffekseerObject = SetEffekseerObj.GetComponent<SetEffekseerObject>();
		bSP_ball_move = true;
		bSP_ball_speedup = true;
	}


	// 玉出現
	public void BallAppear()
	{
		meshrenderer.enabled = true;
	}

	// 玉回転開始
	public bool StartRotation()
	{
		// 待ち時間
		fWait += Time.deltaTime;
		if(fWait < CON_ROTATION_WAIT)
			return false;

		// 加速
		fTime += Time.deltaTime / CON_ROTATION_TIME;
		if(fTime >= 1.0f)
		{
			return true;
		}

		if(bSP_ball_speedup)
		{
			cs_SetEffekseerObject.NewEffect(9);
			bSP_ball_speedup = false;

			MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.SP_StopBall);		// 回転音
		}

		return false;
	}

	// 玉回転
	public void Rotation()
	{
		// 玉移動エフェクト
		if(bSP_ball_move)
		{
			cs_SetEffekseerObject.NewEffect(8);		// 玉移動
			cs_SetEffekseerObject.NewEffect(10);	// 玉発射
			MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.SP_Charge);	// 突進音
			bSP_ball_move = false;
		}

		BallObj.transform.eulerAngles += (CON_MAX_ROTATION * Time.deltaTime);
	}
}
