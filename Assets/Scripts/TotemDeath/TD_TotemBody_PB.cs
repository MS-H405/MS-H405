using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityStandardAssets.ImageEffects;

// A behaviour that is attached to a playable
public class TD_TotemBody_PB : PlayableBehaviour
{
	#region 定数

	const float CON_MOUTHMAX_TIME = 0.4f;		// 口が全開になる時間

	const float CON_GRAY_TIME = 2.0f;			// 灰色になる時間
	const float CON_OUT_START = 6.3f;			// 消え始める時間
	const float CON_OUT_TIME = 2.5f;			// 消える時間
	const float CON_FADE_TIME = 16.0f;			// フェードを開始する時間

	const float CON_BLUR_END_TIME = 2.0f;		// ブラーを消す時間

	#endregion


	#region 変数

	private GameObject _TotemBodyObj;				// トーテム本体
	public GameObject TotemBodyObj { get; set; }
	private Material _TotemBodyMat;					// トーテム本体のマテリアル
	public Material TotemBodyMat { get; set; }
	private Material _MainCameraObj;				// カメラ
	public GameObject MainCameraObj { get; set; }
	BlurOptimized cs_BlurOptimized;


	Animator animator;

	float fTime = 0.0f;

	float fTotemColor = 1.0f;		// マテリアルのカラー
	float fTotemEmission = 0.0f;	// マテリアルのエミッション		α値を0.0にするのでは消えないので、こっちで白くして消す

	bool bFade = true;

	bool bBlurEnd = true;

	#endregion


	public override void OnGraphStart(Playable playable)
	{
		animator = TotemBodyObj.GetComponent<Animator>();

		TotemBodyMat.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		TotemBodyMat.EnableKeyword("_EMISSION");
		TotemBodyMat.SetColor("_EmissionColor", new Color(0.0f, 0.0f, 0.0f));

		cs_BlurOptimized = MainCameraObj.GetComponent<BlurOptimized>();
		cs_BlurOptimized.enabled = false;
	}


	public override void OnGraphStop(Playable playable)
	{
		
	}


	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		animator.speed = 0.5f / CON_MOUTHMAX_TIME;	// ちょうど真ん中で口が全開になるはずなので、1.0f / 2.0fを計算して0.5fにしてある
		cs_BlurOptimized.enabled = true;
	}


	public override void OnBehaviourPause(Playable playable, FrameData info)
	{
		
	}


	public override void PrepareFrame(Playable playable, FrameData info)
	{
		// スキップ処理
		if (Input.GetKeyDown(KeyCode.Return) && bFade && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.TOTEM_TO_YADOKARI);
			bFade = false;
		}


		fTime += Time.deltaTime;
		if(fTime > CON_MOUTHMAX_TIME)
		{
			animator.speed = 0.0f;
		}

		SetColor();


		// フェード
		if (fTime > CON_FADE_TIME && bFade && !MovieManager.Instance.GetisMovideFade())
		{
			MovieManager.Instance.FadeStart(MovieManager.MOVIE_SCENE.TOTEM_TO_YADOKARI);
			bFade = false;
		}

		// ブラーを消す
		if(bBlurEnd && fTime >= CON_BLUR_END_TIME)
		{
			cs_BlurOptimized.enabled = false;
			bBlurEnd = false;
		}
	}

	// マテリアルを操作する
	private void SetColor()
	{
		// 灰色化
		fTotemColor -= Time.deltaTime / CON_GRAY_TIME;
		if(fTotemColor <= -0.0f)
			fTotemColor = 0.0f;

		TotemBodyMat.color = new Color(fTotemColor, fTotemColor, fTotemColor);


		// 透明化
		if(fTime < CON_OUT_START)
			return;

		//fTotemAlpha -= Time.deltaTime / CON_OUT_TIME;
		//if (fTotemAlpha <= -0.0f)
		//	fTotemAlpha = 0.0f;
		//TotemBodyMat.color = new Color(TotemBodyMat.color.r, TotemBodyMat.color.g, TotemBodyMat.color.b, fTotemAlpha);
	}
}
