using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Special_2Player : MonoBehaviour
{
	#region 定数

	const float CON_WAIT_BACKJAMP = 0.5f;		// 後方ジャンプするまでの待ち時間(モーションと併せる)
	const float CON_TIME_BACKJAMP = 2.0f;		// 後方ジャンプしている時間
	readonly Vector3 CON_BACKJAMP_SPEED = new Vector3(0.0f, 30.0f, -45.0f);	// 後方ジャンプ時のスピード
	const float CON_BACKJAMP_ACCELE_Y = -27.5f;	// 後方ジャンプの時の加速度Y

//	const float CON_RIDEBALL_TIME = 0.8f;		// 玉に乗るときの時間
//	readonly Vector3 CON_RIDEBALL_SPEED = new Vector3(0.0f, 0.0f, -10.0f);	// 玉に乗るときのスピード
//	const float CON_RIDEBALL_ACCELE_Y = -20.0f;	// 玉に乗るときの加速度Y

	const float CON_GOENEMY_SPEED = 70.0f;		// 突撃時のスピード(最初から最後までいっしょ)	86f
	const float CON_ENEMY_FLY = 130.0f;			// 敵を引きずる距離
	const float CON_END_DISTANCE = 200.0f;		// 必殺技終了の判定に使う移動した距離

	#endregion

	#region 変数

	[SerializeField] GameObject BallObj;
	[SerializeField] GameObject EnemyObj;

	bool bInit;
	float fTime;
	float fWait;

	Vector3 vSpeed;
	Vector3 vStartPos;
	Transform Parent;		// プレイヤー回転用のオブジェクト

	float fMovedDis;		// 突撃時に進んだ距離
	bool bHarf;				// カメラが切り替わる場所まで進んだかどうか
	bool bEnemyFly;			// 敵を吹き飛ばしたかどうか
	bool bEnd;				// 突撃が終わったかどうか

	// Effekseer関係
	[SerializeField] GameObject SetEffekseerObj;
	SetEffekseerObject cs_SetEffekseerObject;

	Special_2PlayerMotion cs_Special_1PlayerMotion;
	//Vector3 vStartEular;
	//Vector3 vEndEular;

	#endregion


	// Use this for initialization
	void Start ()
	{
		bInit = true;
		fTime = 0.0f;
		fWait = 0.0f;

		fMovedDis = 0.0f;
		bHarf = false;
		bEnemyFly = false;
		bEnd = false;

		// Effekseer関係
		cs_SetEffekseerObject = SetEffekseerObj.GetComponent<SetEffekseerObject>();

		cs_Special_1PlayerMotion = GetComponent<Special_2PlayerMotion>();
	}


	// 後方ジャンプ
	public bool BackJamp()
	{
		// 移動待ち
		fWait += Time.deltaTime;
		if (fWait < CON_WAIT_BACKJAMP)
			return false;

		// 初期化処理
		if (bInit)
		{
			// 一瞬だけ空のオブジェクトを作り、それを自分の足元に設定して親とし、その親の座標をいじる。
			Parent = new GameObject().GetComponent<Transform>();
			Parent.parent = transform.parent;
			Parent.position = new Vector3(transform.position.x, transform.position.y + 0.83f, transform.position.z);
			transform.parent = Parent;

			vStartPos = transform.position;
			vSpeed = CON_BACKJAMP_SPEED;

			// ジャンプエフェクト
			cs_SetEffekseerObject.NewEffect(5);
			cs_SetEffekseerObject.NewEffect(6);

			cs_Special_1PlayerMotion.Backjamp_Start();	// 後方ジャンプモーション
			//vStartEular = new Vector3(180.0f, 0.0f, 0.0f);
			//vEndEular = new Vector3(540.0f, 0.0f, 0.0f);

			SoundManager_Tatsuo.Instance.PlaySE(SoundManager_Tatsuo.eSeValue.SP_Jump);		// ジャンプ音

			bInit = false;
		}

		// 終了判定
		fTime += Time.deltaTime;
		if (fTime > CON_TIME_BACKJAMP)
		{
			fTime = 0.0f;
			fWait = 0.0f;
			bInit = true;

			transform.position = new Vector3(vStartPos.x, BallObj.transform.localScale.y, vStartPos.z + CON_BACKJAMP_SPEED.z * CON_TIME_BACKJAMP);
			Parent.eulerAngles = Vector3.zero;

			SoundManager_Tatsuo.Instance.PlaySE(SoundManager_Tatsuo.eSeValue.SP_RideOn);	// 着地音
			SoundManager_Tatsuo.Instance.PlaySE(SoundManager_Tatsuo.eSeValue.SP_RideOn2);	// キラキラ効果音

			return true;
		}

		// 移動
		vSpeed.y += CON_BACKJAMP_ACCELE_Y * Time.deltaTime;
		transform.position += vSpeed * Time.deltaTime;

		// 回転はモデルがモーションで動くせいでうまくできない
		//Parent.eulerAngles = Vector3.Lerp(vStartEular, vEndEular, fTime / CON_TIME_BACKJAMP);

		return false;
	}


	// 仮親を削除し、玉を子とする
	public void ParentConf()
	{
		transform.parent = null;										// 仮親との親子関係解除
		Destroy(Parent.gameObject);										// 仮親削除
		transform.position = new Vector3(transform.position.x, 5.0f, transform.position.z);
		BallObj.transform.parent = transform;	// 玉を子とする
	}


	// 敵に突撃
	public bool GoEnemy()
	{// カメラの位置変更のため、半分くらい行ったら1回trueを返す
		float temp = CON_GOENEMY_SPEED * Time.deltaTime;
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + temp);
		fMovedDis += temp;

		// 移動距離により、いろいろ判定
		if(fMovedDis >= 50.0f && !bHarf)
		{// カメラ敵背後へ移動
			bHarf = true;
			return true;
		}
		else if(100.0f <= fMovedDis && fMovedDis < CON_ENEMY_FLY)
		{// 敵引きずり
			EnemyObj.transform.position = new Vector3(EnemyObj.transform.position.x, EnemyObj.transform.position.y, transform.position.z + 2.5f);	// 玉の大きさ分ずらす
		}
		else if(CON_ENEMY_FLY <= fMovedDis && fMovedDis < CON_END_DISTANCE && !bEnemyFly)
		{// 敵吹き飛ばし
			EnemyObj.GetComponent<SP_Enemy>().StartFly();
			bEnemyFly = true;
		}
		else if (fMovedDis > CON_END_DISTANCE && !bEnd)
		{// 必殺技終了
			bEnd = true;
			return true;
		}

		return false;
	}
}
