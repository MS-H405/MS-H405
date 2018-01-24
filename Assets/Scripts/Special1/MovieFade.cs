using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovieFade : MonoBehaviour
{
	#region 定数

	public enum _FADE_PATERN
	{
		NORMAL,		// 普通の黒フェード
		CUTIN,		// カットイン。必殺技を撃つとき
		WHITE		// ゆっくり白フェード。必殺技終わるとき
	}

	const float NORMAL_FADE_TIME = 1.0f;		// 普通の黒フェードの時間。始まりのイン・アウト、終わりのイン・アウトも全部この時間
	const float CUTIN_FADE_TIME = 0.4f;			// 必殺技始まりのフェード時間。フェードイン・アウト両方ともこの時間
	const float WHITE_FADE_TIME = 0.8f;			// 必殺技終わりの白フェード時間。フェードイン・アウト両方ともこの時間

	#endregion


	private static MovieFade instance;
	Image image;
	float fAlpha;

	bool bInit;
	float fFirstAlpha;

	_FADE_PATERN Fade_Patern;

	public static MovieFade Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (MovieFade)FindObjectOfType(typeof(MovieFade));

				if (instance == null)
				{
					GameObject obj = Instantiate(Resources.Load("Special/MovieFadeCanvas") as GameObject);
					instance = obj.GetComponent<MovieFade>();
				}
			}

			return instance;
		}
	}

	public void Awake()
	{
		if (this != Instance)
		{
			Destroy(this);
			return;
		}

		DontDestroyOnLoad(this.gameObject);

		image = gameObject.GetComponent<Image>();
		fAlpha = 0.0f;
	}



	// フェードイン(消える方 0 -> 1)
	IEnumerator SpecialFadein(float fFadeTime, System.Action action)
	{
		bInit = true;
		fAlpha = 0.0f;
		fFirstAlpha = 0.0f;
		image.color = new Color(image.color.r, image.color.g, image.color.b, 0.0f);

		//var endFrame = new WaitForEndOfFrame();

		while (fAlpha - fFirstAlpha < 1.0f)
		{
			fAlpha += Time.deltaTime / fFadeTime;
			if (bInit)
			{
				fFirstAlpha = fAlpha;
				bInit = false;
			}

			image.color = new Color(image.color.r, image.color.g, image.color.b, fAlpha - fFirstAlpha);


			#region BGM音量

			// 徐々に音量を下げる
			//if (Fade_Patern == _FADE_PATERN.WHITE)
			//	MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.Special, 1 - image.color.a);			// 必殺技
			//else if (MovieManager.Instance.GetOldScene() == MovieManager.MOVIE_SCENE.TOTEM_START)
			//	MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.TotemStart, 1 - image.color.a);		// トーテム登場
			//else if (MovieManager.Instance.GetOldScene() == MovieManager.MOVIE_SCENE.BAGPIPE_START)
			//	MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.HermitCrabStart, 1 - image.color.a);	// バグパイプ登場
			//else if (MovieManager.Instance.GetOldScene() == MovieManager.MOVIE_SCENE.MECHA_START)
			//	MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.MechaStart, 1 - image.color.a);		// メカ大道芸人登場


			ChangeBGMVolume(MovieManager.Instance.GetOldScene(), 1 - image.color.a);
			#endregion
			

			yield return null;
		}

		image.color = new Color(image.color.r, image.color.g, image.color.b, 1.0f);
        SoundManager.Instance.StopBGM();
        //SoundManager.Instance.StopSE();
        JugglingAtack.NowJugglingAmount = 0;

        if (MovieSoundManager.Instance)
        {
            MovieSoundManager.Instance.StopBGM();		// BGM停止
            //MovieSoundManager.Instance.StopSE();
        }

        if (action != null)
		{
			action();
		}
	}

	// フェードアウト(見えてくる方 1 -> 0)
	IEnumerator SpecialFadeout(float fFadeTime, System.Action action)
	{
		bInit = true;
		fAlpha = 0.0f;
		fFirstAlpha = 0.0f;
		image.color = new Color(image.color.r, image.color.g, image.color.b, 1.0f);

		//var endFrame = new WaitForEndOfFrame();

		#region BGM音量

		// 音量を0にして再生する
		ChangeBGMVolume(MovieManager.Instance.GetNowScene(), 0.0f);
		if (Fade_Patern == _FADE_PATERN.CUTIN)
		{
			//MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.Special, 0.0f);
			MovieSoundManager.Instance.PlayBGM(MovieSoundManager.eBgmValue.Special);						// 必殺技
		}
		else if (MovieManager.Instance.GetNowScene() == MovieManager.MOVIE_SCENE.TOTEM_START)
		{
			//MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.TotemStart, 0.0f);
			MovieSoundManager.Instance.PlayBGM(MovieSoundManager.eBgmValue.TotemStart);						// トーテム登場
		}
		else if (MovieManager.Instance.GetNowScene() == MovieManager.MOVIE_SCENE.BAGPIPE_START)
		{
			//MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.HermitCrabStart, 0.0f);
			MovieSoundManager.Instance.PlayBGM(MovieSoundManager.eBgmValue.HermitCrabStart);				// バグパイプ登場
		}
		else if (MovieManager.Instance.GetNowScene() == MovieManager.MOVIE_SCENE.MECHA_START)
		{
			//MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.MechaStart, 0.0f);
			MovieSoundManager.Instance.PlayBGM(MovieSoundManager.eBgmValue.MechaStart);						// メカ大道芸人登場
		}

		#endregion

		while (fAlpha - fFirstAlpha < 1.0)
		{
			//fAlpha += Time.deltaTime / fFadeTime;
			fAlpha += Time.unscaledDeltaTime / fFadeTime;
			if (bInit)
			{
				fFirstAlpha = fAlpha;
				bInit = false;
			}

			image.color = new Color(image.color.r, image.color.g, image.color.b, 1 - fAlpha - fFirstAlpha);

			#region BGM音量

			// 音量を徐々に上げる
			//if (Fade_Patern == _FADE_PATERN.CUTIN)
			//	MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.Special, fAlpha - fFirstAlpha);			// 必殺技
			//else if (MovieManager.Instance.GetNowScene() == MovieManager.MOVIE_SCENE.TOTEM_START)
			//	MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.TotemStart, fAlpha - fFirstAlpha);		// トーテム登場
			//else if (MovieManager.Instance.GetNowScene() == MovieManager.MOVIE_SCENE.BAGPIPE_START)
			//	MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.HermitCrabStart, fAlpha - fFirstAlpha);	// バグパイプ登場
			//else if (MovieManager.Instance.GetNowScene() == MovieManager.MOVIE_SCENE.MECHA_START)
			//	MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.MechaStart, fAlpha - fFirstAlpha);		// メカ大道芸人登場

			ChangeBGMVolume(MovieManager.Instance.GetNowScene(), fAlpha - fFirstAlpha);

			#endregion
			

			yield return null;
		}

		image.color = new Color(image.color.r, image.color.g, image.color.b, 0.0f);

		#region BGM音量

		// とりあえず最大に
		if (Fade_Patern == _FADE_PATERN.CUTIN)
			MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.Special, 1.0f);			// 必殺技
		else if (MovieManager.Instance.GetNowScene() == MovieManager.MOVIE_SCENE.TOTEM_START)
			MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.TotemStart, 1.0f);		// トーテム登場
		else if (MovieManager.Instance.GetNowScene() == MovieManager.MOVIE_SCENE.BAGPIPE_START)
			MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.HermitCrabStart, 1.0f);	// バグパイプ登場
		else if (MovieManager.Instance.GetNowScene() == MovieManager.MOVIE_SCENE.MECHA_START)
			MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.MechaStart, 1.0f);		// メカ大道芸人登場

		#endregion

		if (action != null)
		{
			action();
		}
	}


	// 必殺技開始か終わりかで、フェードの色と時間を変える
	public Coroutine FadeOut(_FADE_PATERN fade, System.Action action)
	{
		float fSPTime = 0.0f;

		// BGMの音量調整をするかどうかを決めるために、フェードパターンを記憶していく
		Fade_Patern = fade;

		if (fade == _FADE_PATERN.NORMAL)
		{// 普通の黒フェード
			image.color = Color.black;
			fSPTime = NORMAL_FADE_TIME;
		}
		else if (fade == _FADE_PATERN.CUTIN)
		{// カットイン
			image.color = Color.black;			// とりあえず黒にしているけど、カットインなら別のコルーチンを作る必要がある。
			fSPTime = CUTIN_FADE_TIME;
		}
		else if (fade == _FADE_PATERN.WHITE)
		{
			image.color = Color.white;
			fSPTime = WHITE_FADE_TIME;
		}

		StopAllCoroutines();
		return StartCoroutine(SpecialFadeout(fSPTime, action));
	}

	public Coroutine FadeIn(_FADE_PATERN fade, System.Action action)
	{
		float fSPTime = 0.0f;

		// BGMの音量調整をするかどうかを決めるために、フェードパターンを記憶していく
		Fade_Patern = fade;

		if (fade == _FADE_PATERN.NORMAL)
		{// 普通の黒フェード
			image.color = Color.black;
			fSPTime = NORMAL_FADE_TIME;
		}
		else if (fade == _FADE_PATERN.CUTIN)
		{// カットイン
			image.color = Color.black;			// とりあえず黒にしているけど、カットインなら別のコルーチンを作る必要がある。
			fSPTime = CUTIN_FADE_TIME;
		}
		else if (fade == _FADE_PATERN.WHITE)
		{
			image.color = Color.white;
			fSPTime = WHITE_FADE_TIME;
		}

		StopAllCoroutines();
		return StartCoroutine(SpecialFadein(fSPTime, action));
	}



	//public Coroutine FadeOut(bool bStartSP)
	//{
	//	return FadeOut(bStartSP, null);
	//}
	//
	//public Coroutine FadeIn(bool bStartSP)
	//{
	//	return FadeIn(bStartSP, null);
	//}

	// 音量を変更する
	void ChangeBGMVolume(MovieManager.MOVIE_SCENE scene, float volume)
	{
		if (Fade_Patern == _FADE_PATERN.WHITE)
			MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.Special, volume);			// 必殺技
		else if (scene == MovieManager.MOVIE_SCENE.TOTEM_START)
			MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.TotemStart, volume);			// トーテム登場
		else if (scene == MovieManager.MOVIE_SCENE.BAGPIPE_START)
			MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.HermitCrabStart, volume);	// バグパイプ登場
		else if (scene == MovieManager.MOVIE_SCENE.MECHA_START)
			MovieSoundManager.Instance.ChangeVolumeBGM(MovieSoundManager.eBgmValue.MechaStart, volume);			// メカ大道芸人登場
	}
}
