using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovieFade : MonoBehaviour
{
	const float MOVIE_FADE_TIME = 0.3f;

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

	IEnumerator FadeoutCoroutine(System.Action action)
	{
		fAlpha = 1.0f;
		image.color = new Color(image.color.r, image.color.g, image.color.b, fAlpha);

		var endFrame = new WaitForEndOfFrame();

		while (fAlpha >= 0.0f)
		{
			fAlpha -= Time.deltaTime / MOVIE_FADE_TIME;
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

	IEnumerator FadeinCoroutine(System.Action action)
	{
		fAlpha = 0.0f;
		image.color = new Color(image.color.r, image.color.g, image.color.b, fAlpha);

		var endFrame = new WaitForEndOfFrame();

		while (fAlpha <= 1.0f)
		{
			fAlpha += Time.deltaTime / MOVIE_FADE_TIME;
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

	public Coroutine FadeOut(System.Action action)
	{
		StopAllCoroutines();
		return StartCoroutine(FadeoutCoroutine(action));
	}

	public Coroutine FadeOut()
	{
		return FadeOut(null);
	}

	public Coroutine FadeIn(System.Action action)
	{
		StopAllCoroutines();
		return StartCoroutine(FadeinCoroutine(action));
	}

	public Coroutine FadeIn()
	{
		return FadeIn(null);
	}
}
