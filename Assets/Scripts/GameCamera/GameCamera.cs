using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
	// カメラの挙動
	enum _CameraMode
	{
		LOCKON,				// ターゲットをロックオンしている状態
		TOP,				// ターゲットロストしている状態(俯瞰視点)
		BACKPLAYER,			// プレイヤーのもとに戻ってこようとしている状態
	};

	#region 定数
	// readonlyも結局定数みたいなものなので　CON_　をつけている。
	// 一部GameCamera_Sub.csも同じ値を使っているので、こっちを変更したらそっちも変えておく。
	
	[SerializeField] float	CON_fPlayerFollowRate = 0.15f;								// プレイヤーへの追従率(視点の移動)
	const float			CON_fDistance = 5.0f;											// プレイヤーとの距離
	readonly Vector2	CON_vFollowRate = new Vector2(1.0f, 0.05f);						// 敵・俯瞰プレイヤーの追従率(注視点の移動)(x : 遊び範囲外、y : 遊び範囲内)
	readonly Vector4	CON_vEnemyFollowRect = new Vector4(0.4f, 0.6f, 0.5f, 0.7f);		// 遊びの範囲(敵用)
	readonly Vector2	CON_RotY = new Vector2(0.6f, 0.3853981f);						// 距離により変えるカメラの角度(Y)  元→0.7853981
	readonly Vector2	CON_RotYDistance = new Vector2(3.0f, 12.0f);					// この距離を判定に使ってカメラの角度(Y)を変える

	const float CON_fTopTime = 1.0f;													// 俯瞰視点への移動にかかる時間(元に戻るときもこの時間で戻る)
	const float CON_fNormalTime = 1.0f;	// 3.0												// 通常視点への移動にかかる時間
	const float CON_fTopRotY = 0.7f;													// ターゲットロストして、俯瞰になった時の角度(Y)
	const float	CON_fTopDistance = 12.0f;												// ターゲットロストして、俯瞰になった時のプレイヤーからの距離

	const float CON_MIN_Y = 0.5f;	// Y座標がこの値以下にならないようにする

	#endregion
	

	#region 変数

	private GameObject PlayerObj;
	_CameraMode CameraMode;	// カメラの挙動
	
	Camera camera;									// 自身のカメラ
	Vector2 rot;									// カメラの角度
	public Vector3 vLookAtPos;								// 注視点


	float	fTopParameter;							// 俯瞰視点移動に使う移動割合
	float	fBeforeDis;								// 通常→俯瞰、俯瞰→通常への移動開始前のプレイヤーからの距離
	Vector3 vBeforePos;								// 通常→俯瞰、俯瞰→通常への移動開始前の座標
	Vector2 vBeforeRot;								// 通常→俯瞰、俯瞰→通常への移動開始前の角度
	Vector3 vBeforeLookAtPos;						// 通常→俯瞰、俯瞰→通常への移動開始前の注視点


	[SerializeField] GameObject SubCameraObj;		// 俯瞰→通常時のスクリーン座標計算用カメラ
	GameCamera_Sub cs_GameCamera_Sub;

	[SerializeField] Vector3 vLookPosOffset = Vector3.zero;	// 敵の座標から注視点までのズレ

	#endregion


	// Use this for initialization
	void Start()
	{
        PlayerObj = PlayerManager.Instance.Player.gameObject;
        camera = GetComponent<Camera>();

		CameraMode = _CameraMode.LOCKON;							// 最初はロックオン

		rot = new Vector2(Mathf.PI * 1.5f, Mathf.PI * 0.25f);		// カメラの初期角度
		vLookAtPos = EnemyManager.Instance.BossEnemy.transform.position + vLookPosOffset + vLookPosOffset + vLookPosOffset;
		
		cs_GameCamera_Sub = SubCameraObj.GetComponent<GameCamera_Sub>();	// 俯瞰→通常時のスクリーン座標計算用カメラ
		SubCameraObj.SetActive(false);										// 重さ軽減のため
	}

	// Update is called once per frame
	void Update()
	{
        //----- デバッグ機能 ---------------------------------------

        if (Input.GetKeyDown(KeyCode.O))
        {
            MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.SPECIAL_1);
            Debug.Log("MovieStart");
        }

		//---------------------------------------------------------


		if (!EnemyManager.Instance.BossEnemy)
		{// ターゲットロスト
			CameraChengeTop();
		}
		else
		{// ターゲット捕捉
			CameraChangeNormal();
		}


		switch(CameraMode)
		{
			case _CameraMode.LOCKON:		// 通常視点
				CameraLockOn();
				break;

			case _CameraMode.TOP:			// 通常　→　俯瞰、俯瞰視点
				CameraTopView();
				break;

			case _CameraMode.BACKPLAYER:	// 俯瞰　→　通常
				CameraBackPlayer();
				break;
		}

		// カメラが下に行き過ぎないようにする
		if(transform.position.y < CON_MIN_Y)
		{
			transform.position = new Vector3(transform.position.x, CON_MIN_Y, transform.position.z);
		}
	}



	// ロックオン
	private void CameraLockOn()
	{
		Vector3	vDiffPos;			// 遊びの範囲からのはみ出した距離
		float	fFollowRate;		// プレイヤーへの追従率
		Vector3	vTargetPos;			// 注視点、角度の計算に使う

		vDiffPos = CheckPlaySpace(EnemyManager.Instance.BossEnemy.transform.position + vLookPosOffset + vLookPosOffset + vLookPosOffset);		// 遊びの範囲からのはみ出した距離を計算
		if(vDiffPos == Vector3.zero)
		{// 範囲内
			fFollowRate = CON_vFollowRate.y;			// 追従率低め
			vTargetPos = EnemyManager.Instance.BossEnemy.transform.position + vLookPosOffset + vLookPosOffset;
		}
		else
		{// 範囲外
			//fFollowRate = CON_vFollowRate.x;			// 追従率高め
			//vTargetPos = vLookAtPos + vDiffPos;
			
			float fPaternA = Vector3.Distance(Vector3.zero, vDiffPos) * CON_vFollowRate.x;									// vDiffPosを1.0fで追うパターン
			float fPaternB = Vector3.Distance(vLookAtPos, (EnemyManager.Instance.BossEnemy.transform.position + vLookPosOffset + vLookPosOffset - vDiffPos)) * CON_vFollowRate.y;	// 遊び範囲ギリギリを0.05fで追うパターン

			if (fPaternA > fPaternB)
			{
				fFollowRate = CON_vFollowRate.x;		// 追従率高め
				vTargetPos = vLookAtPos + vDiffPos;
			}
			else
			{
				fFollowRate = CON_vFollowRate.y;		// 追従率低め
				vTargetPos = EnemyManager.Instance.BossEnemy.transform.position + vLookPosOffset + vLookPosOffset;	// - vDiffを追加
			}
		}

		// ここでvLookAtPos + vDiffPosを1.0fで追った方が早いか、
		// EnemyManager.Instance.BossEnemy.transform.positionを0.05fで追った方が早いかを計算し、数値が高いほうを適用する。
		// と考えたところまでしかやってない。

		// 範囲内ならパターンB
		// 範囲外なら大小比較して大きいほうを採用

		

		#region 注視点の計算
		vLookAtPos.x = Mathf.Lerp(vLookAtPos.x, vTargetPos.x, fFollowRate);		// 徐々に新しい座標に移動
		vLookAtPos.y = Mathf.Lerp(vLookAtPos.y, vTargetPos.y, fFollowRate);
		vLookAtPos.z = Mathf.Lerp(vLookAtPos.z, vTargetPos.z, fFollowRate);

		transform.LookAt(vLookAtPos);

		//Debug.Log(vLookAtPos);
		#endregion


		#region 角度の計算
		vTargetPos = vLookAtPos - vDiffPos;		// だいたいこれが一番いい感じ　たぶん対角線上にプレイヤーがいるからだと思う。
		float NewRotX, NewRotY;					// 新しい角度

		// ----- rot.xを計算 -----
		Vector2 PE = new Vector2(vTargetPos.x - PlayerObj.transform.position.x, vTargetPos.z - PlayerObj.transform.position.z);
		PE = PE.normalized;		// プレイヤーから敵へのベクトル正規化

		NewRotX = Mathf.Acos(Vector2.Dot(Vector2.left, PE));	// PEベクトルとleftベクトルから角度を計算
		if (PE.y > 0.0f)										// 角度の帳尻合わせ
			NewRotX += (Mathf.PI - NewRotX) * 2.0f;


		// ----- rot.yを計算 -----
		float fDis = Vector3.Distance(PlayerObj.transform.position, vTargetPos);	// プレイヤーと敵の距離を計算
		
		// XZの距離で角度計算
		if (fDis <= CON_RotYDistance.x)			// 距離が近いとき
			NewRotY = CON_RotY.x;
		else if (fDis >= CON_RotYDistance.y)	// 距離が遠いとき
			NewRotY = CON_RotY.y;
		else								// 距離が中途半端な時
		{
			float fRate = (fDis - CON_RotYDistance.x) / (CON_RotYDistance.y - CON_RotYDistance.x);
			NewRotY = (CON_RotY.y * fRate) + (CON_RotY.x * (1.0f - fRate));
		}

		// カメラの視点と注視点の高低差でrot.yに補正をかける
		Vector2 PEy = new Vector2(Mathf.Sqrt(Mathf.Pow(vTargetPos.x - PlayerObj.transform.position.x, 2) +
											 Mathf.Pow(vTargetPos.z - PlayerObj.transform.position.z, 2)),
								  vTargetPos.y - PlayerObj.transform.position.y);
		float fSita = Mathf.Acos(Vector2.Dot(PEy.normalized, Vector2.right));	// 補正分の角度を計算

		if (PlayerObj.transform.position.y > vTargetPos.y)						// プレイヤーの方が高ければ、角度を大きく
			NewRotY += fSita;
		else																	// 敵のほうが高ければ、角度を低く
			NewRotY -= fSita;

		if (NewRotY > Mathf.PI / 2.0f) NewRotY = Mathf.PI / 2.0f;	//角度補正
		if (NewRotY < -Mathf.PI / 2.0f) NewRotY = -Mathf.PI / 2.0f;	//角度補正

		rot = new Vector2(NewRotX, NewRotY);
		#endregion


		#region 視点の計算
		Vector3 NewPos = new Vector3(PlayerObj.transform.position.x + CON_fDistance * Mathf.Cos(rot.x) * Mathf.Cos(rot.y),		// 新しい座標を計算
									 PlayerObj.transform.position.y + CON_fDistance * Mathf.Sin(rot.y),
									 PlayerObj.transform.position.z + CON_fDistance * Mathf.Sin(rot.x) * Mathf.Cos(rot.y));
		float temp_x = Mathf.Lerp(transform.position.x, NewPos.x, CON_fPlayerFollowRate);									// 徐々に新しい座標に移動
		float temp_y = Mathf.Lerp(transform.position.y, NewPos.y, CON_fPlayerFollowRate);
		float temp_z = Mathf.Lerp(transform.position.z, NewPos.z, CON_fPlayerFollowRate);
		transform.position = new Vector3(temp_x, temp_y, temp_z);															// 移動

		#endregion
	}



	// 通常視点　→　俯瞰視点
	private void CameraTopView()
	{
		fTopParameter += Time.deltaTime / CON_fTopTime;
		if(fTopParameter > 1.0f)
			fTopParameter = 1.0f;

		#region 注視点の計算
		vLookAtPos.x = Mathf.Lerp(vLookAtPos.x, PlayerObj.transform.position.x, fTopParameter);		// 徐々に新しい座標に移動
		vLookAtPos.y = Mathf.Lerp(vLookAtPos.y, PlayerObj.transform.position.y, fTopParameter);
		vLookAtPos.z = Mathf.Lerp(vLookAtPos.z, PlayerObj.transform.position.z, fTopParameter);
		//vLookAtPos.x = Mathf.Lerp(vLookAtPos.x, 0.0f, 0.07f);		// 徐々に新しい座標に移動
		//vLookAtPos.y = Mathf.Lerp(vLookAtPos.y, 0.0f, 0.07f);
		//vLookAtPos.z = Mathf.Lerp(vLookAtPos.z, 0.0f, 0.07f);

		transform.LookAt(vLookAtPos);
		#endregion

		#region 角度の計算

		// 俯瞰視点の時にプレイヤーが向いてる方向を向く
		if (fTopParameter < 1.0f)
			rot.y = Mathf.Lerp(rot.y, CON_fTopRotY, fTopParameter);

		// 俯瞰視点の時にZプラス方向に向く
		//if (fTopParameter < 1.0f)
		//{
		//	rot.x = Mathf.Lerp(rot.x, Mathf.PI * 1.5f, fTopParameter);
		//	rot.y = Mathf.Lerp(rot.y, CON_fTopRotY, fTopParameter);
		//}

		//rot.x = Mathf.Lerp(vBeforeRot.x, Mathf.PI * 1.5f, Mathf.Clamp01(fTopParameter));		// ←これがあると結局Zプラス方向を向いてしまう
		rot.y = Mathf.Lerp(vBeforeRot.y, CON_fTopRotY, Mathf.Clamp01(fTopParameter));

		#endregion

		#region 視点の計算
		//float fDis = Mathf.Lerp(fBeforeDis, CON_fTopDistance, fTopParameter);	// プレイヤーから距離

		Vector3 NewPos = new Vector3(PlayerObj.transform.position.x + CON_fTopDistance * Mathf.Cos(rot.x) * Mathf.Cos(rot.y),		// 新しい座標を計算
									 PlayerObj.transform.position.y + CON_fTopDistance * Mathf.Sin(rot.y),
									 PlayerObj.transform.position.z + CON_fTopDistance * Mathf.Sin(rot.x) * Mathf.Cos(rot.y));
		//Vector3 NewPos = new Vector3(fDis * Mathf.Cos(rot.x) * Mathf.Cos(rot.y),		// 新しい座標を計算
		//							 fDis * Mathf.Sin(rot.y),
		//							 fDis * Mathf.Sin(rot.x) * Mathf.Cos(rot.y));
		float temp_x = Mathf.Lerp(transform.position.x, NewPos.x, fTopParameter);				// 徐々に新しい座標に移動
		float temp_y = Mathf.Lerp(transform.position.y, NewPos.y, fTopParameter);
		float temp_z = Mathf.Lerp(transform.position.z, NewPos.z, fTopParameter);//0.07
		transform.position = new Vector3(temp_x, temp_y, temp_z);
		#endregion
	}


	// 俯瞰視点　→　通常視点
	private void CameraBackPlayer()
	{
		bool bEnd = false;	// 俯瞰→通常終了判定

		fTopParameter += Time.deltaTime / CON_fNormalTime;
		if (fTopParameter > 1.0f)
		{// 終了判定
			fTopParameter = 1.0f;
			CameraMode = _CameraMode.LOCKON;	// 移動が完了したので、通常視点モードに戻す
			
			SubCameraObj.transform.localPosition = Vector3.zero;	// 一応座標を戻しておく

			bEnd = true;	// ここでSubCameraObj.SetActive(false)を実行したら関数が反応しなくなるので、この関数の最後にfalseにする。
		}

        SoundManager.Instance.PlaySE(SoundManager.eSeValue.UI_RockOn);

        #region 注視点の計算
        vLookAtPos.x = Mathf.Lerp(vLookAtPos.x, cs_GameCamera_Sub.GetLookAtPos().x, fTopParameter);		// 徐々に新しい座標に移動
		vLookAtPos.y = Mathf.Lerp(vLookAtPos.y, cs_GameCamera_Sub.GetLookAtPos().y, fTopParameter);
		vLookAtPos.z = Mathf.Lerp(vLookAtPos.z, cs_GameCamera_Sub.GetLookAtPos().z, fTopParameter);

		transform.LookAt(vLookAtPos);
		#endregion


		#region 角度の計算
		float temp_X = Mathf.Lerp(rot.x, cs_GameCamera_Sub.Getrot().x, fTopParameter);
		float temp_Y = Mathf.Lerp(rot.y, cs_GameCamera_Sub.Getrot().y, fTopParameter);

		rot = new Vector2(temp_X, temp_Y);
		#endregion


		#region 視点の計算
		float temp_x = Mathf.Lerp(transform.position.x, SubCameraObj.transform.position.x, fTopParameter);
		float temp_y = Mathf.Lerp(transform.position.y, SubCameraObj.transform.position.y, fTopParameter);
		float temp_z = Mathf.Lerp(transform.position.z, SubCameraObj.transform.position.z, fTopParameter);

		transform.position = new Vector3(temp_x, temp_y, temp_z);
		#endregion


		if(bEnd)
			SubCameraObj.SetActive(false);
	}


	// 遊びの範囲内からどれだけ飛び出しているかを計算する。
	// 飛び出していないなら戻り値はVector3.zero
	private Vector3 CheckPlaySpace(Vector3 vPos)
	{
		Vector3 vScreenPos;		// 対象のスクリーン座標
		Vector3 vDiffPos;		// 範囲内から飛び出した距離

		vScreenPos = camera.WorldToScreenPoint(vPos);
		vDiffPos = Vector3.zero;

		if (vScreenPos.x < Screen.width * CON_vEnemyFollowRect.x || Screen.width * CON_vEnemyFollowRect.y < vScreenPos.x ||		// 画面左下が(0, 0)で、横0.4~0.6、縦0.5~0.7の範囲
			vScreenPos.y < Screen.height * CON_vEnemyFollowRect.z || Screen.height * CON_vEnemyFollowRect.w < vScreenPos.y)		// CON_vEnemyFollowRectの x   y      z   w
		{// 対象が遊びの範囲外にいた
			// 遊びの範囲内から飛び出した距離を計算
			Vector3 work = vScreenPos;			// 計算用

			if (vScreenPos.x < Screen.width * CON_vEnemyFollowRect.x)				// 遊びの範囲内から、左に飛び出した
				work.x = Screen.width * CON_vEnemyFollowRect.x;
			else if (Screen.width * CON_vEnemyFollowRect.y < vScreenPos.x)			// 遊びの範囲内から、右に飛び出した
				work.x = Screen.width * CON_vEnemyFollowRect.y;

			if (vScreenPos.y < Screen.height * CON_vEnemyFollowRect.z)				// 遊びの範囲内から、下に飛び出した
				work.y = Screen.height * CON_vEnemyFollowRect.z;
			else if (Screen.height * CON_vEnemyFollowRect.w < vScreenPos.y)			// 遊びの範囲内から、上に飛び出した
				work.y = Screen.height * CON_vEnemyFollowRect.w;
			//vDiffPos = EnemyManager.Instance.BossEnemy.transform.position - camera.ScreenToWorldPoint(work);
			vDiffPos = vPos - camera.ScreenToWorldPoint(work);
		}

		return vDiffPos;
	}


	// 通常視点から俯瞰視点へ変更
	public void CameraChengeTop()
	{
		if(CameraMode == _CameraMode.TOP)
			return;

		CameraMode = _CameraMode.TOP;
		fTopParameter = 0.0f;				// 移動割合初期化

		// 俯瞰視点への移動開始前の情報を保存
		fBeforeDis = Vector3.Distance(PlayerObj.transform.position, transform.position);
		vBeforePos = transform.position;
		vBeforeRot = rot;
		vBeforeLookAtPos = vLookAtPos;
	}


	// 俯瞰視点から通常視点へ変更
	public void CameraChangeNormal()
	{
		// 俯瞰視点状態じゃないとここには来ない
		if(CameraMode != _CameraMode.TOP)
			return;

		SubCameraObj.SetActive(true);					// 俯瞰→通常時のみサブカメラのスクリーン座標が必要なのでアクティブに
		cs_GameCamera_Sub.Init(PlayerObj);				// 注視点・角度を初期化

		CameraMode = _CameraMode.BACKPLAYER;
		fTopParameter = 0.0f;				// 移動割合初期化

		// 通常視点への移動開始前の情報を保存
		fBeforeDis = Vector3.Distance(PlayerObj.transform.position, transform.position);
		vBeforePos = transform.position;
		vBeforeRot = rot;
		vBeforeLookAtPos = vLookAtPos;
	}
}
