using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovieFade : MonoBehaviour
{
	const float MOVIE_FADE_TIME_START = 0.3f;	// 必殺技始まりのフェード時間
	const float MOVIE_FADE_TIME_END	  = 0.8f;	// 必殺技終わりのフェード時間

	private static MovieFade instance;
	Image image;
	float fAlpha;

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

	IEnumerator FadeoutCoroutine(float fFadeTime, System.Action action)
	{
		fAlpha = 1.0f;
		image.color = new Color(image.color.r, image.color.g, image.color.b, fAlpha);

		var endFrame = new WaitForEndOfFrame();

		while (fAlpha >= 0.0f)
		{
			fAlpha -= Time.unscaledDeltaTime / fFadeTime;
			if (fAlpha < 0.0f)
			{
				image.color = new Color(image.color.r, image.color.g, image.color.b, 0.0f);
				break;
			}

			image.color = new Color(image.color.r, image.color.g, image.color.b, fAlpha);
			yield return endFrame;
		}

		if (action != null)
		{
			action();
		}
	}

	IEnumerator FadeinCoroutine(float fFadeTime, System.Action action)
	{
		fAlpha = 0.0f;
		image.color = new Color(image.color.r, image.color.g, image.color.b, fAlpha);

		var endFrame = new WaitForEndOfFrame();

		while (fAlpha <= 1.0f)
		{
			fAlpha += Time.unscaledDeltaTime / fFadeTime;
			if(fAlpha > 1.0f)
			{
				image.color = new Color(image.color.r, image.color.g, image.color.b, 1.0f);
				break;
			}

			image.color = new Color(image.color.r, image.color.g, image.color.b, fAlpha);
			yield return endFrame;
		}

		if (action != null)
		{
			action();
		}
	}


	// 必殺技開始か終わりかで、フェードの色と時間を変える
	public Coroutine FadeOut(bool bStartSP, System.Action action)
	{
		float fSPTime;

		if(bStartSP)
		{// 必殺技始まり
			image.color = Color.black;
			fSPTime = MOVIE_FADE_TIME_START;
		}
		else
		{// 必殺技終わり
			image.color = Color.white;
			fSPTime = MOVIE_FADE_TIME_END;
		}

		StopAllCoroutines();
		return StartCoroutine(FadeoutCoroutine(fSPTime, action));
	}

	public Coroutine FadeOut(bool bStartSP)
	{
		return FadeOut(bStartSP, null);
	}

	public Coroutine FadeIn(bool bStartSP, System.Action action)
	{
		float fSPTime;

		if (bStartSP)
		{// 必殺技始まり
			image.color = Color.black;
			fSPTime = MOVIE_FADE_TIME_START;
		}
		else
		{// 必殺技終わり
			image.color = Color.white;
			fSPTime = MOVIE_FADE_TIME_END;
		}

		StopAllCoroutines();
		return StartCoroutine(FadeinCoroutine(fSPTime, action));
	}

	public Coroutine FadeIn(bool bStartSP)
	{
		return FadeIn(bStartSP, null);
	}
}
