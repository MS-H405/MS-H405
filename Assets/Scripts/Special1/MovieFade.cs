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

	const float NORMAL_FADE_TIME = 0.4f;		// 普通の黒フェードの時間。始まりのイン・アウト、終わりのイン・アウトも全部この時間
	const float CUTIN_FADE_TIME = 0.4f;			// 必殺技始まりのフェード時間。フェードイン・アウト両方ともこの時間
	const float WHITE_FADE_TIME = 0.8f;			// 必殺技終わりの白フェード時間。フェードイン・アウト両方ともこの時間

	#endregion


	private static MovieFade instance;
	Image image;
	float fAlpha;

	bool bInit;
	float fFirstAlpha;

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
			yield return null;
		}

		image.color = new Color(image.color.r, image.color.g, image.color.b, 1.0f);
        SoundManager.Instance.StopBGM();
        //SoundManager.Instance.StopSE();
        JugglingAtack.NowJugglingAmount = 0;

        if (MovieSoundManager.Instance)
        {
            MovieSoundManager.Instance.StopBGM();
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
			yield return null;
		}

		image.color = new Color(image.color.r, image.color.g, image.color.b, 0.0f);

		if (action != null)
		{
			action();
		}
	}


	// 必殺技開始か終わりかで、フェードの色と時間を変える
	public Coroutine FadeOut(_FADE_PATERN fade, System.Action action)
	{
		float fSPTime = 0.0f;

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
}
