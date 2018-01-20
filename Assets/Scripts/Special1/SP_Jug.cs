using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class SP_Jug : MonoBehaviour
{
	#region 定数

	int MAX_PIN = 10;			// なんとなく10本投げる
	Vector3 THROW_OFFSET = new Vector3(0.0f, 1.0f, 0.7f);	// プレイヤーの座標(足元)から、展開ピンのオフセット値
	float WAIT_TIME = 0.4f;		// ピン展開までの待ち時間
	float MOVE_TIME = 1.0f;		// ピンの展開時間
	//float ENEMY_DIS = 8.0f;		// 展開されたピンが静止する、敵からの距離	5
	float GOENEMY_TIME = 0.6f;	// ピンの突撃時間

	int MAX_BIGPIN = 2;				// 大きいピンは2本
	float GOENEMY_TIME_BIG = 0.6f;	// デカピンの突撃時間
	float BIG_ROTATE_Z = 45.0f;		// デカピンの傾き(プラスとマイナス)

	float WAIT_EXPANSION = 0.8f;	// ピン展開までの待ち時間(プレイヤーのモーションと合わせる)
	float WAIT_GOENEMY = 0.2f;		// ピン突撃までの待ち時間
	float WAIT_GOENEMY_BIG = 0.2f;	// デカピン突撃までの待ち時間

	#endregion

	#region 変数

	[SerializeField] GameObject PinObj;		// 投げるピン
	[SerializeField] GameObject[] BigPinObjArray = new GameObject[2];	// 投げるデカピン
	SP_JugBig[] cs_SPJugBig = new SP_JugBig[2];
	[SerializeField] GameObject PlayerObj;
	[SerializeField] GameObject EnemyObj;
	[SerializeField] GameObject SetEffekseerObj;
	SetEffekseerObject cs_SetEffekseerObject;
	[SerializeField] GameObject MainCameraObj;

	List<SP_JugChild> PinList = new List<SP_JugChild>();	// 投げたピン

	BezierCurve.tBez[] tbezier;		// ベジエ曲線移動(fTime, vStart, vMiddle, vEnd)
	float fSecTime;					// 1秒あたりに増加するfTime
	float[] fRotate;				// 1秒あたりに回転する角度

	float fPinTime = 0.0f;
	float fBigPinTime = 0.0f;

	// Special_Manager関係
	float fWait = 0.0f;		// 待ち時間
	float fWaitBig = 0.0f;	// 待ち時間(デカピン)
	bool bInit = true;		// Time.deltaTimeが初回から2.3とかの値を突っ込むので、それを防ぐ処理のフラグ
	float fFirstWait;		// 最初にTime.deltaTimeから受け取った時間

	// Effekseer関係
	bool bSP_pin_hit;		// 展開ピンヒットエフェクト
	bool bSP_big_hit;		// デカピンヒットエフェクト

	// 音関係
	bool bSE_SmallAttack = true;

	[SerializeField] float ENEMY_DIS = 8.0f;		// 展開されたピンが静止する、敵からの距離	5

	#endregion

	// Use this for initialization
	void Start ()
	{
		// ----- 普通のピン -----
		tbezier = new BezierCurve.tBez[MAX_PIN];
		fRotate = new float[MAX_PIN];

		fSecTime = 1.0f / MOVE_TIME;

		// 初期情報を計算する
		Vector2 rot;	// end座標を計算するのに使う
		for(int i = 0 ; i < MAX_PIN ; i ++)
		{
			rot.x = (i * 30 - 45) * Mathf.Deg2Rad;		// この式だと、0~4までは右へ、5~9までは左へ飛んでいく。
			if(i % 2 == 0)
				rot.y = Mathf.PI / 6.0f;	// 30度(下にあるほう)
			else
				rot.y = Mathf.PI / 3.0f;	// 60度(上にあるほう)

			tbezier[i].time = 0.0f;
			tbezier[i].start = PlayerObj.transform.position + THROW_OFFSET;						// start座標も若干ずらしたほうがいいのかもしれない。
			tbezier[i].end = new Vector3(EnemyObj.transform.position.x + ENEMY_DIS * Mathf.Cos(rot.x) * Mathf.Cos(rot.y),
										 EnemyObj.transform.position.y + ENEMY_DIS * Mathf.Sin(rot.y),
										 EnemyObj.transform.position.z + ENEMY_DIS * Mathf.Sin(rot.x) * Mathf.Cos(rot.y));
			tbezier[i].end += new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));	// end座標を適当にずらす。
			tbezier[i].middle = new Vector3((tbezier[i].start.x + tbezier[i].end.x) / 2.0f,
											tbezier[i].start.y + (tbezier[i].end.y - tbezier[i].start.y) * 2.0f,
											(tbezier[i].start.z + tbezier[i].end.z) / 2.0f);
			fSecTime = 1.0f / MOVE_TIME;
			fRotate[i] = Random.Range(1000.0f, 2000.0f);
		}



		// ----- デカピン -----
		for(int i = 0 ; i < MAX_BIGPIN ; i ++)
		{
			cs_SPJugBig[i] = BigPinObjArray[i].GetComponent<SP_JugBig>();
		}


		// Effekseer関係
		bSP_pin_hit = true;
		bSP_big_hit = true;

		float a = Time.deltaTime;
	}



	// ピン展開
	public bool Jug_ThrowExpansion()
	{
		cs_SetEffekseerObject = SetEffekseerObj.GetComponent<SetEffekseerObject>();

		// すぐに投げると変なので、少し待つ
		fWait += Time.deltaTime;
		if(bInit)
		{
			fFirstWait = fWait;
			bInit = false;
		}

		if ((fWait - fFirstWait) < WAIT_EXPANSION)
			return false;

		for (int i = 0; i < MAX_PIN; i++)
		{
			GameObject obj = Instantiate(PinObj as GameObject, PlayerObj.transform.position + THROW_OFFSET, Quaternion.identity);
			obj.GetComponent<SP_JugChild>().SetParam(tbezier[i], fSecTime, fRotate[i]);
			PinList.Add(obj.GetComponent<SP_JugChild>());
		}

		fWait = 0.0f;
		StartCoroutine("Jug_Expansion");

		// ピン展開エフェクト
		cs_SetEffekseerObject.NewEffect(11);
		MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.SP_PinThrow);

		return true;
	}

	// ピン展開移動(完了を待つ必要はないのでコルーチン)
	private IEnumerator Jug_Expansion()
	{
		while(true)
		{
			fPinTime += fSecTime * Time.deltaTime;
			if (fPinTime > 1.0f)
			{// 移動終了
				fPinTime = 0.0f;
				for (int i = 0; i < MAX_PIN; i++)
					PinList[i].LookAtEnemy(EnemyObj.transform.position);		// 敵のほうを向く

				break;
			}

			for (int i = 0; i < MAX_PIN; i++)
				PinList[i].Update_Expansion(fPinTime);							// 展開移動

			yield return null;
		}
	}

	// ピン敵に突撃
	public bool GoEnemy()
	{
		fWait += Time.deltaTime;
		if(fWait < WAIT_GOENEMY)
			return false;

		if(bSE_SmallAttack)
		{
			MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.SP_SmallAttack);
			bSE_SmallAttack = false;
		}

		fPinTime += Time.deltaTime / GOENEMY_TIME;

		if(fPinTime > 1.0f)
		{
			fPinTime = 0.0f;
			for (int i = 0; i < MAX_PIN; i++)
				PinList[i].PinDestroy();		// ここで大きいのを1発かますか、PinDestroy()で個々に出すか

			fWait = 0.0f;

			return true;
		}

		for (int i = 0; i < MAX_PIN; i++)
			PinList[i].GoEnemy(fPinTime, EnemyObj.transform.position);

		if(bSP_pin_hit)
		{
			// 展開ピンヒットエフェクト
			cs_SetEffekseerObject.NewEffect(1);

			bSP_pin_hit = false;
		}

		return false;
	}


	// デカピン出現
	public void JugBig_Appear()
	{
		for (int i = 0; i < MAX_BIGPIN; i++)
		{
			cs_SPJugBig[i].JugBig_Appear();
		}
		MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.SP_BigJug2);	// デカピン出現SE
	}

	// デカピン投げる
	public void JugBig_Throw()
	{
		float rotateZ;

		for (int i = 0; i < MAX_BIGPIN; i++)
		{
			if(i == 0)
				rotateZ = BIG_ROTATE_Z;		// 左
			else
				rotateZ = -BIG_ROTATE_Z;	// 右
			cs_SPJugBig[i].JugBig_Throw(rotateZ);
		}

		// デカピン投擲エフェクト
		cs_SetEffekseerObject.NewEffect(0);
		MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.SP_Slow);
	}

	// デカピン移動
	public bool GoEnemy_Big()
	{
		fWaitBig += Time.deltaTime;
		if(fWaitBig < WAIT_GOENEMY_BIG)
			return false;

		fBigPinTime += Time.deltaTime / GOENEMY_TIME_BIG;

		if (fBigPinTime > 1.0f)
		{
			fBigPinTime = 0.0f;
			for (int i = 0; i < MAX_BIGPIN; i++)
				cs_SPJugBig[i].PinDestroy();

			fWaitBig = 0.0f;

			if (bSP_big_hit)
			{
				// デカピンヒットエフェクト
				cs_SetEffekseerObject.NewEffect(2);
				if (MainCameraObj.GetComponent<Special_1Camera>())
					MainCameraObj.GetComponent<Special_1Camera>().shakeCamera();
				else if (MainCameraObj.GetComponent<Special_2Camera>())
					MainCameraObj.GetComponent<Special_2Camera>().shakeCamera();
				else if(MainCameraObj.GetComponent<Special_3Camera>())
					MainCameraObj.GetComponent<Special_3Camera>().shakeCamera();

				// 敵スタン
				if (EnemyObj.GetComponent<SP_Enemy>())
					EnemyObj.GetComponent<SP_Enemy>().Stan();
				else if (EnemyObj.GetComponent<SP3_Enemy>())
					EnemyObj.GetComponent<SP3_Enemy>().Stan();

				MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.SP_Blast);
			
				bSP_big_hit = false;
			}

			return true;
		}

		for (int i = 0; i < MAX_BIGPIN; i++)
			cs_SPJugBig[i].GoEnemy_Big(fBigPinTime, EnemyObj.transform.position);

		return false;
	}
}
