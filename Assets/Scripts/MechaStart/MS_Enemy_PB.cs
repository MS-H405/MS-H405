using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class MS_Enemy_PB : PlayableBehaviour
{
	#region 定数

	enum STATE_MECHASTART
	{
		STROKE,		// ストローク　　ﾋﾟｰって感じ
		KAMINARI,	// カミナリドッカーン
		DRAW,		// 敵出現
		WAIT,		// 待機		カメラが動いてると思う
		BACKJAMP,	// バク宙で玉に乗る	
		//BALLRIDE,

		FIN
	}

	const float CON_STROKE_TIME = 7.0f;			// ストロークエフェクト表示開始時間	4.9
	const float CON_KAMINARI_TIME = 5.0f;		// 雷エフェクト表示開始時間
	const float CON_ENEMY_DRAW_TIME = 0.8f;		// 敵表示
	const float CON_WAIT_TIME = 4.0f;			// 待機時間

	const float CON_BACKJAMP_TIME = 1.0f;		// 後方ジャンプモーションを開始してから、玉に着地するまでの時間
	readonly Vector3 CON_BACKJAMP_START_POS = new Vector3(0.0f, 0.0f, 0.0f);	// 開始点
	readonly Vector3 CON_BACKJAMP_MIDDLE_POS = new Vector3(0.0f, 10.0f, 3.5f);	// ベジエ曲線の制御点
	readonly Vector3 CON_BACKJAMP_END_POS = new Vector3(0.0f, 4.19f, 7.0f);		// 終了点

	#endregion


	#region 変数

	STATE_MECHASTART State = STATE_MECHASTART.STROKE;

	#region Component, Script
	private GameObject _EnemyObj;
	public GameObject EnemyObj { get; set; }
	private Transform transform;
	private Component[] SkinnedMeshRendererArray;
	private Component[] MeshRendererArray;

	Animator animator;

	// Effekseer
	private GameObject _EffekseerObj;
	public GameObject EffekseerObj { get; set; }
	SetEffekseerObject cs_SetEffekseerObject;
	#endregion

	float fTime = 0.0f;
	float fWait = 0.0f;
	bool bInitialize = true;

	BezierCurve.tBez tBez;

	#endregion


	public override void OnGraphStart(Playable playable)
	{
		transform = EnemyObj.GetComponent<Transform>();
		animator = EnemyObj.GetComponent<Animator>();
		cs_SetEffekseerObject = EffekseerObj.GetComponent<SetEffekseerObject>();

		// SkinnedMeshRenderer, MeshRenderer取得&非表示
		SkinnedMeshRendererArray = EnemyObj.GetComponentsInChildren<SkinnedMeshRenderer>();
		MeshRendererArray = EnemyObj.GetComponentsInChildren<MeshRenderer>();
		DrawEnemy(false);
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
		switch(State)
		{
			case STATE_MECHASTART.STROKE:
				Stroke();
				break;

			case STATE_MECHASTART.KAMINARI:
				Kaminari();
				break;

			case STATE_MECHASTART.DRAW:
				Draw();
				break;

			case STATE_MECHASTART.WAIT:
				Wait();
				break;

			case STATE_MECHASTART.BACKJAMP:
				BackJamp();
				break;
		}
	}



	#region　関数

	// ストロークアニメーション開始
	private void Stroke()
	{
		if (bInitialize)
		{
			fTime = 0.0f;
			bInitialize = false;
		}

		fTime += Time.deltaTime;

		if (fTime >= CON_STROKE_TIME)
		{
			cs_SetEffekseerObject.NewEffect(0);		// ストロークエフェクト再生

			State = STATE_MECHASTART.KAMINARI;
			bInitialize = true;
		}
	}

	// 雷再生
	private void Kaminari()
	{
		if (bInitialize)
		{
			fTime = 0.0f;
			bInitialize = false;
		}

		fTime += Time.deltaTime;
		if (fTime > CON_KAMINARI_TIME)
		{
			cs_SetEffekseerObject.NewEffect(1);		// 雷再生

			State = STATE_MECHASTART.DRAW;
			bInitialize = true;
		}
	}

	// 敵表示
	private void Draw()
	{
		if (bInitialize)
		{
			fTime = 0.0f;
			bInitialize = false;
		}

		fTime += Time.deltaTime;
		if (fTime > CON_ENEMY_DRAW_TIME)
		{
			cs_SetEffekseerObject.DeleteEffect(0);		// ストロークエフェクト削除
			DrawEnemy(true);							// 敵表示

			State = STATE_MECHASTART.WAIT;
			bInitialize = true;
		}
	}

	// 後方ジャンプまでの待機（カメラが動いてると思う）
	private void Wait()
	{
		if (bInitialize)
		{
			fTime = 0.0f;
			bInitialize = false;
		}

		fTime += Time.deltaTime;
		if (fTime > CON_WAIT_TIME)
		{
			State = STATE_MECHASTART.BACKJAMP;
			bInitialize = true;
		}
	}

	// バク宙で玉に乗る
	private void BackJamp()
	{
		if (bInitialize)
		{
			fTime = 0.0f;
			bInitialize = false;
			animator.SetBool("bBackJamp", true);	// 後方ジャンプモーション開始

			tBez.time = 0.0f;						// ベジエ曲線移動用意
			tBez.start = CON_BACKJAMP_START_POS;
			tBez.middle = CON_BACKJAMP_MIDDLE_POS;
			tBez.end = CON_BACKJAMP_END_POS;
		}

		tBez.time += Mathf.Clamp01(Time.deltaTime / CON_BACKJAMP_TIME);
		transform.position = BezierCurve.CulcBez(tBez, true);

		if (tBez.time >= 1.0f)
		{
			transform.position = CON_BACKJAMP_END_POS;

			State = STATE_MECHASTART.FIN;
			bInitialize = true;
		}
	}





	// 敵の表示(true)・非表示(false)
	private void DrawEnemy(bool bDraw)
	{
		foreach (SkinnedMeshRenderer skinnedmeshrenderer in SkinnedMeshRendererArray)
			skinnedmeshrenderer.enabled = bDraw;
		foreach (MeshRenderer meshrenderer in MeshRendererArray)
			meshrenderer.enabled = bDraw;
	}

	#endregion
}
