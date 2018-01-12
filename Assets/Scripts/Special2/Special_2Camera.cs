using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;



public class Special_2Camera : MonoBehaviour
{
	public struct tSpecialCamera
	{
		public Vector3	vPos;
		public Vector3	vLook;
		public Vector3	vEuler;
		public float	fTime;
		public float	fWait;
	};

	#region 定数

	const float CON_BIGPIN_APPEAR = 0.2f;			// プレイヤー正面①から②への移動中で、デカピンが出現する割合
	readonly Vector3 CON_BACKJAMP_OFFSET = new Vector3(0.0f, 0.0f, 1.3f);	// 後方ジャンプ時の、注視点
	const float CON_BACKJAMP_FOLLOWRATE = 0.1f;		// 後方ジャンプ時の追従率
	const float CON_BALLRIDE_FOLLOWRATE = 0.1f;		// 玉に乗りに行く時の追従率
	const float CON_GOENEMY_FOLLOWRATE = 0.05f;		// 敵突撃時の注視点の追従率
	const float CON_SHOULDER_FOLLOWRATE = 0.1f;		// 敵の肩越しにいるときの追従率

	const float CON_TOTEMEFFCT_TIME = 0.4f;			// カメラがバビロンエフェクト表示用の位置まで行ってから、エフェクトが表示されるまでの時間

	#endregion

	#region 変数

	List<tSpecialCamera> CameraMoveList = new List<tSpecialCamera>();

	float fTime;	// タイマー
	float fWait;	// 待ち時間タイマー
	bool bInit;		// 初期化フラグ
	BezierCurve.tBez3 tbez3;	// プレイヤー正面②からデカピン投擲位置へ移動するときに使う

	[SerializeField] GameObject Special_1JugglingObj;
	bool bBigPinAppear;			// デカピンを出現させたかどうか
	SP_Jug cs_SP_Jug;

	[SerializeField] GameObject PlayerObj;
	[SerializeField] GameObject BallObj;

	Vector3 vLookAt;			// 注視点
	Vector3 vLookAt2;			// 注視点

	ShakeCamera cs_ShakeCamera;		// カメラ揺れ
	bool bShake = false;

	// Effekseer関係
	SetEffekseerObject cs_SetEffekseerObject;
	bool bSP_big_appear;
	bool bSP_player_land;

	BlurOptimized cs_BlurOptimized;
	bool bBlur = true;

	bool bTotemEffect = true;

	#endregion


	// Use this for initialization
	void Start ()
	{

		cs_SP_Jug = Special_1JugglingObj.GetComponent<SP_Jug>();
		cs_ShakeCamera = GetComponent<ShakeCamera>();

		SetPosEulerTime();

		transform.position = CameraMoveList[0].vPos;
		transform.eulerAngles = CameraMoveList[0].vEuler;

		fTime = 0.0f;
		fWait = 0.0f;
		bInit = true;

		bBigPinAppear = false;

		// Effekseer関係
		cs_SetEffekseerObject = GameObject.Find("EffekseerObject").GetComponent<SetEffekseerObject>();
		bSP_big_appear = true;
		bSP_player_land = true;

		cs_BlurOptimized = GetComponent<BlurOptimized>();
		cs_BlurOptimized.enabled = false;		// 最初はブラーなし
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.M))
		{
			Debug.Log("視点    : " + transform.position);
			Debug.Log("注視点 : " + (transform.position + transform.forward * 10.0f));
			Debug.Log("角度    : " + transform.eulerAngles);
		}
	}


	// ピン展開位置から、プレイヤー正面①へ
	public bool CameraMove_1()
	{
		if (bInit)
		{
			tbez3.time = 0.0f;
			tbez3.start = CameraMoveList[0].vPos;
			tbez3.middle1 = new Vector3(9995.0f, 2.0f, -10.0f);
			tbez3.middle2 = new Vector3(9997.4f, 1.2f, -6.7f);
			tbez3.end = CameraMoveList[1].vPos;

			bInit = false;
		}

		fWait += Time.deltaTime;

		// デカピン出現エフェクト
		if(fWait > 1.0f && bSP_big_appear)
		{
			cs_SetEffekseerObject.NewEffect(3);
			cs_SetEffekseerObject.NewEffect(4);
			bSP_big_appear = false;
			SoundManager_Tatsuo.Instance.PlaySE(SoundManager_Tatsuo.eSeValue.SP_BigJug1);	// デカピン出現SE
		}

		if (fWait < CameraMoveList[1].fWait)
			return false;

		if(bBlur)
		{
			cs_BlurOptimized.enabled = true;
			bBlur = false;
		}

		tbez3.time += Time.deltaTime / CameraMoveList[1].fTime;
		if (tbez3.time > 1.0f)
		{
			tbez3.time = 1.0f;
			BezierMove(tbez3, CameraMoveList[0], CameraMoveList[1], 1.0f);

			cs_BlurOptimized.enabled = false;

			bInit = true;
			bBlur = true;

			return true;
		}

		BezierMove(tbez3, CameraMoveList[0], CameraMoveList[1], tbez3.time);

		return false;
	}

	// プレイヤー正面①から、プレイヤー正面②へ(LookAtMove()を使うとなぜかバグる)
	public bool CameraMove_2()
	{
		fWait += Time.deltaTime;
		if (fWait < CameraMoveList[2].fWait)
			return false;

		fTime += Time.deltaTime / CameraMoveList[2].fTime;
		if (fTime > 1.0f)
		{
			EulerMove(CameraMoveList[1], CameraMoveList[2], 1.0f);

			fTime = 0.0f;
			fWait = 0.0f;

			return true;
		}

		EulerMove(CameraMoveList[1], CameraMoveList[2], fTime);

		// デカピン出現
		if(fTime >= CON_BIGPIN_APPEAR && !bBigPinAppear)
		{
			cs_SP_Jug.JugBig_Appear();
			bBigPinAppear = true;
		}

		return false;
	}

	// プレイヤー正面②からデカピン投擲位置へ
	public bool CameraMove_3()
	{
		if (bInit)
		{
			tbez3.time = 0.0f;
			tbez3.start = CameraMoveList[2].vPos;
			tbez3.middle1 = new Vector3(10002.6f, 1.2f, -7.7f);
			tbez3.middle2 = new Vector3(10004.6f, 2.1f, -13.0f);
			tbez3.end = CameraMoveList[3].vPos;

			cs_BlurOptimized.enabled = true;

			bInit = false;
		}

		tbez3.time += Time.deltaTime / CameraMoveList[3].fTime;
		if (tbez3.time > 1.0f)
		{
			tbez3.time = 1.0f;
			BezierMove(tbez3, CameraMoveList[2], CameraMoveList[3], 1.0f);

			cs_BlurOptimized.enabled = false;

			bInit = true;

			return true;
		}

		BezierMove(tbez3, CameraMoveList[2], CameraMoveList[3], tbez3.time);

		return false;
	}

	// 後方ジャンプ位置へ
	public bool CameraMove_4()
	{
		fWait += Time.deltaTime;
		if(fWait < CameraMoveList[4].fWait)
			return false;
		
		transform.position = CameraMoveList[4].vPos;
		transform.eulerAngles = CameraMoveList[4].vEuler;
		vLookAt = PlayerObj.transform.position + CON_BACKJAMP_OFFSET;

		fTime = 0.0f;
		fWait = 0.0f;

		return true;
	}

	// 座標そのまんまで、プレイヤーちょい前を見る
	public void Camera_BackJamp()
	{// この関数の終了は、プレイヤーの後方ジャンプと同時なので、こっちでは終わりは指定しない

		fWait += Time.deltaTime;
		if(fWait < CameraMoveList[5].fWait)
		{// まだ動かない
			Vector3 temp = Vector3.Lerp(vLookAt, PlayerObj.transform.position + CON_BACKJAMP_OFFSET, CON_BACKJAMP_FOLLOWRATE);
			transform.LookAt(temp);
		}
		else
		{
			//fTime += Time.deltaTime / CameraMoveList[5].fTime;
			//
			//transform.position = Vector3.Lerp(transform.position, PlayerObj.transform.position - CameraMoveList[5].vPos, CON_BALLRIDE_FOLLOWRATE);
			//Vector3 temp = Vector3.Lerp(transform.position + transform.forward * 10.0f, BallObj.transform.position, CON_BALLRIDE_FOLLOWRATE);
			//transform.LookAt(temp);
			//vLookAt2 = temp;

			// 今の追従移動をやめて、ワープにする
			transform.position = BallObj.transform.position + new Vector3(0.0f, BallObj.transform.localScale.y / 2.0f, 0.0f) - CameraMoveList[5].vPos;
			vLookAt2 = BallObj.transform.position + new Vector3(0.0f, BallObj.transform.localScale.y / 2.0f, 0.0f);
			transform.LookAt(vLookAt2);

			// 着地エフェクト
			if (bSP_player_land)
			{
				cs_SetEffekseerObject.NewEffect(7);
				bSP_player_land = false;
			}
		}
	}

	// バビロンエフェクト表示位置へ
	public bool TotemAppearEffect()
	{
		if (bInit)
		{
			fTime = 0.0f;
			fWait = 0.0f;

			bInit = false;
		}

		fWait += Time.deltaTime;
		if (fWait < CameraMoveList[7].fWait)
			return false;

		fTime += Time.deltaTime;
		if (fTime >= CameraMoveList[7].fTime)
		{
			// 後方ジャンプ用の位置に戻しておく。
			transform.position = BallObj.transform.position + new Vector3(0.0f, BallObj.transform.localScale.y / 2.0f, 0.0f) - CameraMoveList[5].vPos;
			vLookAt2 = BallObj.transform.position + new Vector3(0.0f, BallObj.transform.localScale.y / 2.0f, 0.0f);
			transform.LookAt(vLookAt2);

			fTime = 0.0f;
			fWait = 0.0f;
			bInit = true;

			return true;
		}

		transform.position = CameraMoveList[7].vPos;
		transform.eulerAngles = CameraMoveList[7].vEuler;

		if(bTotemEffect && fTime >= CON_TOTEMEFFCT_TIME)
		{
			cs_SetEffekseerObject.NewEffect(13);
			SoundManager_Tatsuo.Instance.PlaySE(SoundManager_Tatsuo.eSeValue.SP_Babiron);	// バビロン効果音
			bTotemEffect = false;
		}

		return false;
	}

	// 停止中、ズーム１
	public bool StopCamera_1()
	{
		fTime += Time.unscaledDeltaTime / CameraMoveList[8].fTime;
		if (fTime > 1.0f)
		{
			EulerMove(CameraMoveList[8], CameraMoveList[9], 1.0f);

			fTime = 0.0f;
			fWait = 0.0f;

			return true;
		}

		EulerMove(CameraMoveList[8], CameraMoveList[9], fTime);

		return false;
	}

	// 玉発射
	public void Camera_GoEnemy()
	{// 視点は動かずに、注視点だけ追従
		vLookAt2 = Vector3.Lerp(vLookAt2, BallObj.transform.position + new Vector3(0.0f, BallObj.transform.localScale.y / 2.0f, 0.0f), CON_GOENEMY_FOLLOWRATE);
		transform.LookAt(vLookAt2);
	}

	// 敵の肩越し
	public void CameraShoulder()
	{
		if(bInit)
		{
			transform.position = CameraMoveList[6].vPos;
			transform.eulerAngles = CameraMoveList[6].vEuler;

			bInit = false;
		}

		//// カメラの前を通り過ぎたら追従
		//float cos_sita = Vector3.Dot(transform.forward, PlayerObj.transform.position - transform.position);
		//if(cos_sita > 0.0f)
		//{
		//	vLookAt = PlayerObj.transform.position - new Vector3(0.0f, -4.5f, 0.0f);
		//
		//	return;
		//}

		vLookAt = Vector3.Lerp(vLookAt, PlayerObj.transform.position - new Vector3(0.0f, 4.5f, 0.0f), CON_SHOULDER_FOLLOWRATE);
		transform.LookAt(vLookAt);

		if(transform.eulerAngles.x > CameraMoveList[6].vEuler.x)
			transform.eulerAngles = new Vector3(CameraMoveList[6].vEuler.x, transform.eulerAngles.y, transform.eulerAngles.z);
	}





	// カメラ揺れる
	public void shakeCamera()
	{
		cs_ShakeCamera.DontMoveShake();
	}





	// 回転にオイラー角を使った移動
	private void EulerMove(tSpecialCamera Start, tSpecialCamera End, float time)
	{
		transform.position = Vector3.Lerp(Start.vPos, End.vPos, time);
		transform.eulerAngles = Vector3.Lerp(Start.vEuler, End.vEuler, time);
	}

	// 回転に注視点を使った移動
	private void LookAtMove(tSpecialCamera Start, tSpecialCamera End, float time)
	{
		transform.position = Vector3.Lerp(Start.vPos, End.vPos, time);
		transform.LookAt(Vector3.Lerp(Start.vLook, End.vLook, time));
	}

	// 視点はベジエ曲線、回転はオイラー角で移動
	private void BezierMove(BezierCurve.tBez3 bez, tSpecialCamera Start, tSpecialCamera End, float time)
	{
		transform.position = BezierCurve.CulcBez3(bez);
		transform.eulerAngles = Vector3.Lerp(Start.vEuler, End.vEuler, time);
	}

	// 球面座標を使った視点座標計算
	private Vector3 ViewPoint(Vector3 center, float dis, Vector2 rot)
	{
		Vector3 ViewPoint = new Vector3(center.x + dis * Mathf.Cos(rot.x) * Mathf.Cos(rot.y),
										center.y + dis * Mathf.Sin(rot.y),
										center.z + dis * Mathf.Sin(rot.x) * Mathf.Cos(rot.y));

		return ViewPoint;
	}




	// カメラの座標・角度などを設定する(すごく長い)
	// 注視点は距離10で計測した
	private void SetPosEulerTime()
	{
		tSpecialCamera temp;

		//temp.vPos	= new Vector3(0.0f, 1.8f, -6.4f);		// プレイヤーを見る
		//temp.vLook	= new Vector3(0.0f, -2.7f, -15.4f);
		//temp.vEuler	= new Vector3(26.2f, 180.0f, 0.0f);
		//temp.fTime	= 0.0f;
		//temp.fWait	= 0.0f;
		//CameraMoveList.Add(temp);

		// 0
		temp.vPos	= new Vector3(-3.7f, 2.9f, -2.8f);		// ピン展開
		temp.vLook	= new Vector3(10000.4f, -0.1f, -4.7f);
		temp.vEuler	= new Vector3(6.29f, 32.2f, 0.0f);		// 17.5
		temp.fTime	= 0.0f;
		temp.fWait	= 0.0f;
		temp.vPos	+= PlayerObj.transform.position;// プレイヤーが(0.0f, 0.0f, -10.0f)にいなかった時のため
		CameraMoveList.Add(temp);

		// 1
		temp.vPos	= new Vector3(-1.3f, 1.2f, 3.3f);		// プレイヤーを見る
		temp.vLook	= new Vector3(10002.0f, -0.7f, -17.1f);
		temp.vEuler	= new Vector3(10.8f, 163.4f, 0.0f);
		temp.fTime	= 0.6f;
		temp.fWait	= 2.0f;
		temp.vPos	+= PlayerObj.transform.position;
		CameraMoveList.Add(temp);

		// 2
		temp.vPos	= new Vector3(1.3f, 1.2f, 3.3f);		// プレイヤーを見る
		temp.vLook	= new Vector3(10000.0f, 0.4f, -5.0f);
		temp.vEuler = new Vector3(10.8f, 196.6f, 0.0f);
		temp.fTime	= 1.0f;
		temp.fWait	= 0.0f;
		temp.vPos	+= PlayerObj.transform.position;
		CameraMoveList.Add(temp);

		// 3
		temp.vPos	= new Vector3(0.0f, 3.0f, -4.7f);		// デカピン投げる　→　ピン着弾
		temp.vLook	= new Vector3(9994.9f, 0.7f, -13.0f);		// 円を描くように素早く
		temp.vEuler = new Vector3(7.56f, 360.0f, 0.0f);		// 0度だとバグるので360度	15.1
		temp.fTime	= 0.4f;
		temp.fWait	= 0.0f;
		temp.vPos	+= PlayerObj.transform.position;
		CameraMoveList.Add(temp);

		// 4
		temp.vPos	= new Vector3(2.5f, 0.4f, 3.3f);		// 後方ジャンプ
		temp.vLook	= new Vector3(9994.7f, 1.6f, -12.8f);
		temp.vEuler = new Vector3(352.9f, 232.2f, 0.0f);
		temp.fTime	= 0.0f;
		temp.fWait	= 2.7f;
		temp.vPos	+= PlayerObj.transform.position;
		CameraMoveList.Add(temp);

		// 5
		//temp.vPos	= new Vector3(10.6f, -2.8f, -7.0f);		// 後方ジャンプ追従(玉が(0.0f, 0.0f, -100.0f)ならこの位置)
		temp.vPos	= new Vector3(10.6f, 0.0f, -7.0f);		// 後方ジャンプ追従(玉が(0.0f, 0.0f, -100.0f)ならこの位置)
		temp.vLook	= new Vector3(9997.5f, 5.2f, 2.5f);
		temp.vEuler	= new Vector3(21.4f, 119.0f, 0.0f);
		temp.fTime	= 1.0f;	// 角度だけ時間で移動
		temp.fWait	= 1.5f;
		CameraMoveList.Add(temp);

		// 6
		temp.vPos = new Vector3(10004.6f, 3.4f, 6.5f);		// 敵の肩越し
		temp.vLook = new Vector3(10001.0f, 1.1f, -2.6f);
		temp.vEuler = new Vector3(13.4f, 201.8f, 0.0f);
		temp.fTime = 0.0f;
		temp.fWait = 0.0f;
		CameraMoveList.Add(temp);

		// 7
		temp.vPos = new Vector3(10000f, 2.5f, -100.0f);		// バビロンエフェクト表示
		temp.vLook = new Vector3(0.0f, 0.0f, 0.0f);
		temp.vEuler = new Vector3(0.0f, 180.0f, 0.0f);
		temp.fTime = 1.5f;
		temp.fWait = 1.6f;
		CameraMoveList.Add(temp);

		// 8
		temp.vPos = new Vector3(10010.5f, 7.7f, -94.1f);		// ズーム１開始
		temp.vLook = new Vector3(10003f, 4.3f, -99.7f);
		temp.vEuler = new Vector3(20.0f, 233.2f, 0.0f);
		temp.fTime = 0.4f;
		temp.fWait = 0.0f;
		CameraMoveList.Add(temp);

		// 9
		temp.vPos = new Vector3(10006.2f, 5.8f, -97.3f);		// ズーム１終了
		temp.vLook = new Vector3(9998.7f, 2.4f, -102.9f);
		temp.vEuler = new Vector3(20.0f, 233.2f, 0.0f);
		temp.fTime = 0.0f;
		temp.fWait = 0.0f;
		CameraMoveList.Add(temp);
	}
}
