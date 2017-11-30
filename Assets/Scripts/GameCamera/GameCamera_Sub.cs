using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera_Sub : MonoBehaviour
{
	#region 定数
	// readonlyも結局定数みたいなものなので　CON_　をつけている。

	const float			CON_fPlayerFollowRate = 0.6f;									// プレイヤーへの追従率(視点の移動)
	const float			CON_fDistance = 5.0f;											// プレイヤーとの距離
	readonly Vector2	CON_vFollowRate = new Vector2(1.0f, 0.05f);						// 敵・俯瞰プレイヤーの追従率(注視点の移動)(x : 遊び範囲外、y : 遊び範囲内)
	readonly Vector4	CON_vEnemyFollowRect = new Vector4(0.4f, 0.6f, 0.5f, 0.7f);		// 遊びの範囲(敵用)
	readonly Vector2	CON_RotY = new Vector2(Mathf.PI * 0.25f, 0.3853981f);			// 距離により変えるカメラの角度(Y)
	readonly Vector2	CON_RotYDistance = new Vector2(3.0f, 12.0f);					// この距離を判定に使ってカメラの角度(Y)を変える

	#endregion
	

	#region 変数

	GameObject PlayerObj = null;
	
	Camera camera = null;							// 自身のカメラ
	Vector2 rot;									// カメラの角度
	Vector3 vLookAtPos;								// 注視点

	#endregion

	
	// Update is called once per frame
	void Update ()
	{
		// 敵がいなければ処理しない
		if(!EnemyManager.Instance.BossEnemy)
			return;

		Vector3 vDiffPos;			// 遊びの範囲からのはみ出した距離
		float fFollowRate;			// プレイヤーへの追従率
		Vector3 vTargetPos;			// 注視点、角度の計算に使う

		vDiffPos = CheckPlaySpace(EnemyManager.Instance.BossEnemy.transform.position);		// 遊びの範囲からのはみ出した距離を計算
		if (vDiffPos == Vector3.zero)
		{// 範囲内
			fFollowRate = CON_vFollowRate.y;			// 追従率低め
			vTargetPos = EnemyManager.Instance.BossEnemy.transform.position;
		}
		else
		{// 範囲外
			float fPaternA = Vector3.Distance(Vector3.zero, vDiffPos) * CON_vFollowRate.x;									// vDiffPosを1.0fで追うパターン
			float fPaternB = Vector3.Distance(vLookAtPos, (EnemyManager.Instance.BossEnemy.transform.position - vDiffPos)) * CON_vFollowRate.y;	// 遊び範囲ギリギリを0.05fで追うパターン

			if (fPaternA > fPaternB)
			{
				fFollowRate = CON_vFollowRate.x;		// 追従率高め
				vTargetPos = vLookAtPos + vDiffPos;
			}
			else
			{
				fFollowRate = CON_vFollowRate.y;		// 追従率低め
				vTargetPos = EnemyManager.Instance.BossEnemy.transform.position;
			}
		}

		#region 注視点の計算
		vLookAtPos.x = Mathf.Lerp(vLookAtPos.x, vTargetPos.x, fFollowRate);		// 徐々に新しい座標に移動
		vLookAtPos.y = Mathf.Lerp(vLookAtPos.y, vTargetPos.y, fFollowRate);
		vLookAtPos.z = Mathf.Lerp(vLookAtPos.z, vTargetPos.z, fFollowRate);

		transform.LookAt(vLookAtPos);
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
			//vDiffPos = EnemyManager.Instance.BossEnemytransform.position - camera.ScreenToWorldPoint(work);
			vDiffPos = vPos - camera.ScreenToWorldPoint(work);
		}

		return vDiffPos;
	}


	// 初期化(俯瞰→通常モードになった瞬間呼ばれる)
	public void Init(GameObject PObj)
	{
		if(camera    == null)	camera    = GetComponent<Camera>();
		if(PlayerObj == null)	PlayerObj = PObj;

		vLookAtPos = EnemyManager.Instance.BossEnemy.transform.position;

		#region 角度の計算
		// スクリーン座標計算前に1度はやっておかないと
		Vector3 vTargetPos = vLookAtPos;
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
	}

	public Vector2 Getrot()
	{
		return rot;
	}

	public Vector3 GetLookAtPos()
	{
		return vLookAtPos;
	}
}
