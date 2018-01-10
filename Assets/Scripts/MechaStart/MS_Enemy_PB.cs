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

		FIN
	}

	const float CON_STROKE_TIME = 1.0f;			// ストロークエフェクト表示開始時間
	const float CON_KAMINARI_TIME = 5.0f;		// 雷エフェクト表示開始時間
	const float CON_ENEMY_DRAW_TIME = 0.8f;		// 敵表示

	#endregion


	#region 変数

	STATE_MECHASTART State = STATE_MECHASTART.STROKE;

	#region Component, Script
	private GameObject _EnemyObj;
	public GameObject EnemyObj { get; set; }
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

	#endregion


	public override void OnGraphStart(Playable playable)
	{
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
