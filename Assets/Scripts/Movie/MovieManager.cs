using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MovieManager : MonoBehaviour
{
	public enum MOVIE_SCENE
	{
		TITLE,			// タイトル
		STAGE_1,		// ステージ1
		STAGE_2,		// ステージ2
		STAGE_3,		// ステージ3
		RESULT,			// リザルト

		TOTEM_START,	// トーテムポールの登場
		TOTEM_DEATH,	// トーテムポールの死亡
		BAGPIPE_START,	// バグパイプの登場
		BAGPIPE_DEATH,	// バグパイプの死亡
		MECHA_START,	// メカ大道芸人の登場
		MECHA_DEATH,	// メカ大道芸人の死亡

		SPECIAL_1,		// 必殺技1
		SPECIAL_2,		// 必殺技2
		SPECIAL_3,		// 必殺技3
	};

	private static MovieManager instance;

	private bool isFading = false;  // フェード中かどうか
	private List<MonoBehaviour> _monoList = new List<MonoBehaviour>();
	private string MovieSceneName;

	private float fTime;
	private float fFirstTime;
	private bool bInit;
    private GameObject _mainCamera = null;

	MOVIE_SCENE NowSpecial;	// 今やっていた必殺技
	bool bGoDeath;			// 死んで死亡シーンにいくのか、敵がまだ死んでいなくてゲームメインに戻るのか


	public static MovieManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (MovieManager)FindObjectOfType(typeof(MovieManager));

				if (instance == null)
				{
					GameObject obj = new GameObject("MovieSceneManager");
					instance = obj.AddComponent<MovieManager>();
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
	}

	// デバッグ用
	void Update()
	{

	}


	// フェード中かどうかを返す
	public bool GetisMovideFade()
	{
		return isFading;
	}

	// ムービーシーンを呼び出す
	public void FadeStart(MOVIE_SCENE scene)
	{
		if(isFading)
			return;

		isFading = true;

		if(scene == MOVIE_SCENE.TITLE ||
		   scene == MOVIE_SCENE.STAGE_1 ||
		   scene == MOVIE_SCENE.STAGE_2 ||
		   scene == MOVIE_SCENE.STAGE_3 ||
		   scene == MOVIE_SCENE.RESULT ||
		   scene == MOVIE_SCENE.TOTEM_START ||
		   scene == MOVIE_SCENE.TOTEM_DEATH ||
		   scene == MOVIE_SCENE.BAGPIPE_DEATH ||
		   scene == MOVIE_SCENE.BAGPIPE_START ||
		   scene == MOVIE_SCENE.MECHA_DEATH ||
		   scene == MOVIE_SCENE.MECHA_START)
		{
			StartCoroutine("Col_SceneMove", scene);
		}
		else if(
			scene == MOVIE_SCENE.SPECIAL_1 ||
			scene == MOVIE_SCENE.SPECIAL_2 ||
			scene == MOVIE_SCENE.SPECIAL_3)
		{
			// TODO : 開始演出入れた際、要デバッグ
			_mainCamera = Camera.main.gameObject;

			bGoDeath = EnemyManager.Instance.BossEnemy.Death();		// 死んだかどうかを記録しておく
			NowSpecial = scene;										// 今撃った必殺技
			StartCoroutine("Col_SpecialStart", scene);
		}
		else
		{
			Debug.Log("そのシーン遷移作ってないです");
		}
	}

	// ムービーシーン終了されたら呼ばれる
	public void MovieFinish()
	{
		if (isFading)
			return;

		isFading = true;

		if (bGoDeath)
			StartCoroutine(Col_SpecialFinish_Next());	// 敵が死んだので、対応する死亡シーンへ
		else
			StartCoroutine(Col_SpecialFinish());		// 敵がまだ死んでいないので、もう一回ゲームメインへ
		}

    // 普通のシーン遷移
	// 遷移先が必殺技シーン以外の時
	private IEnumerator Col_SceneMove(MOVIE_SCENE scene)
	{
		// フェードイン
		bool bFadeIn = false;
		MovieFade.Instance.FadeIn(MovieFade._FADE_PATERN.NORMAL, () =>
		{
			bFadeIn = true;
		});

		// フェードインが終わるまではここで処理ストップ
		while (!bFadeIn)
			yield return null;

		#region シーンを読み込む
		switch (scene)
		{
			case MOVIE_SCENE.TITLE:
				SceneManager.LoadScene("Title");
				break;

			case MOVIE_SCENE.STAGE_1:
				SceneManager.LoadScene("TotemMain");
				break;

			case MOVIE_SCENE.STAGE_2:
				SceneManager.LoadScene("a");
				break;

			case MOVIE_SCENE.STAGE_3:
				SceneManager.LoadScene("a");
				break;

			case MOVIE_SCENE.RESULT:
				SceneManager.LoadScene("a");
				break;

			case MOVIE_SCENE.TOTEM_START:
				SceneManager.LoadScene("TotemStart");
				break;

			case MOVIE_SCENE.TOTEM_DEATH:
				SceneManager.LoadScene("TotemDeath");
				break;

			case MOVIE_SCENE.BAGPIPE_START:
				SceneManager.LoadScene("a");
				break;

			case MOVIE_SCENE.BAGPIPE_DEATH:
				SceneManager.LoadScene("a");
				break;

			case MOVIE_SCENE.MECHA_START:
				SceneManager.LoadScene("a");
				break;

			case MOVIE_SCENE.MECHA_DEATH:
				SceneManager.LoadScene("a");
				break;
		}
		#endregion

		// ロード時間とりあえず今回は時間で判定
		bInit = true;
		fTime = 0.0f;
		fFirstTime = 0.0f;
		while (fTime - fFirstTime < 0.2f)
		{
			fTime += Time.unscaledDeltaTime;
			if (bInit)
			{
				fFirstTime = fTime;
				bInit = false;
			}
			yield return null;
		}

		// フェードアウト
		MovieFade.Instance.FadeOut(MovieFade._FADE_PATERN.NORMAL, () =>
		{
			isFading = false;		// フェード終了
		});
	}


	// 必殺技始まり
	private IEnumerator Col_SpecialStart(MOVIE_SCENE scene)
	{
		// 最初にGameMain.sceneの全てのオブジェクトの更新を停止させる関数を呼ぶ



		// フェードイン
		bool bFadeIn = false;
		MovieFade.Instance.FadeIn(MovieFade._FADE_PATERN.CUTIN, () =>
		{
			bFadeIn = true;
		});

		// フェードインが終わるまではここで処理ストップ
		while (!bFadeIn)
			yield return null;

		#region シーンを読み込む
		switch (scene)
		{
			case MOVIE_SCENE.SPECIAL_1:
				SceneManager.LoadScene("Special_1", LoadSceneMode.Additive);
				MovieSceneName = "Special_1";
				break;

			case MOVIE_SCENE.SPECIAL_2:
				SceneManager.LoadScene("Special_2", LoadSceneMode.Additive);
				MovieSceneName = "Special_2";
				break;

			case MOVIE_SCENE.SPECIAL_3:
				SceneManager.LoadScene("Special_3", LoadSceneMode.Additive);
				MovieSceneName = "Special_3";
				break;
		}
		#endregion
		
		//yield return new WaitForEndOfFrame();	// こっちにすると、1フレームの間カメラが無いって言われる
		yield return null;						// こっちにすると、1フレームの間audiolistenerが2つあるって言われる

		// GameMainシーンの全てのオブジェクトに対して、SetActive(false)をやる
		AllObjectControl(false);

		// アクティブなシーンを切り替える(このままではGameMainシーンにオブジェクトが追加されてしまう)
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(MovieSceneName));

		// ロード時間とりあえず今回は時間で判定
		bInit = true;
		fTime = 0.0f;
		fFirstTime = 0.0f;
		while(fTime - fFirstTime < 0.2f)
		{
			fTime += Time.unscaledDeltaTime;
			if(bInit)
			{
				fFirstTime = fTime;
				bInit = false;
			}
			yield return null;
		}
		//yield return new WaitForSeconds(0.2f);		// こっち使うと、timescaleを0にした場合止まってしまう。

		// フェードアウト
		MovieFade.Instance.FadeOut(MovieFade._FADE_PATERN.CUTIN, () =>
		{
			isFading = false;		// フェード終了
		});
	}


	// 必殺技シーン終わり
	private IEnumerator Col_SpecialFinish()
	{
		// フェードイン
		bool bFadeIn = false;
		MovieFade.Instance.FadeIn(MovieFade._FADE_PATERN.WHITE, () =>
		{
			bFadeIn = true;
		});

		// フェードインが終わるまではここで処理ストップ
		while (!bFadeIn)
			yield return null;

		// 使ったムービーシーンを破棄
		SceneManager.UnloadSceneAsync(MovieSceneName);

		// audiolistenerが2個あるって言われないように、1フレーム待つ
		yield return null;

		// GameMainシーンの全てのオブジェクトに対して、SetActive(true)をやる
		AllObjectControl(true);

		// ロード時間とりあえず今回は時間で判定
		bInit = true;
		fTime = 0.0f;
		fFirstTime = 0.0f;
		while (fTime - fFirstTime < 0.2f)
		{
			fTime += Time.unscaledDeltaTime;
			if (bInit)
			{
				fFirstTime = fTime;
				bInit = false;
			}
			yield return null;
		}
		//yield return new WaitForSeconds(0.2f);		// こっち使うと、timescaleを0にした場合止まってしまう。

		// フェードアウト
		MovieFade.Instance.FadeOut(MovieFade._FADE_PATERN.WHITE, () =>
		{
			isFading = false;		// フェード終了
		});

		// 最後にGameMain.sceneの全てのオブジェクトの更新を再開させる関数を呼ぶ
	}

	// 必殺技が終わって、敵が死に、次のシーンに行く時
	private IEnumerator Col_SpecialFinish_Next()
	{
		// フェードイン
		bool bFadeIn = false;
		MovieFade.Instance.FadeIn(MovieFade._FADE_PATERN.WHITE, () =>
		{
			bFadeIn = true;
		});

		// フェードインが終わるまではここで処理ストップ
		while (!bFadeIn)
			yield return null;

		#region シーンを読み込む
		switch (NowSpecial)
		{
			case MOVIE_SCENE.SPECIAL_1:
				SceneManager.LoadScene("TotemDeath");
				break;

			case MOVIE_SCENE.SPECIAL_2:
				SceneManager.LoadScene("a");
				break;

			case MOVIE_SCENE.SPECIAL_3:
				SceneManager.LoadScene("a");
				break;
		}
		#endregion

		// audiolistenerが2個あるって言われないように、1フレーム待つ
		//yield return null;

		// ロード時間とりあえず今回は時間で判定
		bInit = true;
		fTime = 0.0f;
		fFirstTime = 0.0f;
		while (fTime - fFirstTime < 0.2f)
		{
			fTime += Time.unscaledDeltaTime;
			if (bInit)
			{
				fFirstTime = fTime;
				bInit = false;
			}
			yield return null;
		}
		//yield return new WaitForSeconds(0.2f);		// こっち使うと、timescaleを0にした場合止まってしまう。

		// フェードアウト
		MovieFade.Instance.FadeOut(MovieFade._FADE_PATERN.WHITE, () =>
		{
			isFading = false;		// フェード終了
		});

		// 最後にGameMain.sceneの全てのオブジェクトの更新を再開させる関数を呼ぶ
	}




	// 引数true  : 現在Activeになっているオブジェクトを全て取得し、falseにする(DontDestroyOnLoadにあるオブジェクトを除く)
	// 引数false : ↑でfalseにしたすべてのオブジェクトをActiveにする。
	private void AllObjectControl(bool flg)
	{
		if(!flg)
		{
			foreach (MonoBehaviour mono in UnityEngine.Object.FindObjectsOfType(typeof(MonoBehaviour)))
			{
                if (!mono.enabled)
                    continue;

				// DontDestroyOnLoadにあるオブジェクトはfalseにしないようにする
				GameObject RootObj = mono.transform.root.gameObject;

				Scene scene = SceneManager.GetActiveScene();
				foreach (var sceneRootObject in scene.GetRootGameObjects())
				{
					if (sceneRootObject == RootObj)
					{
						// audiolistenerが2つあると言われるので、GameMainシーンにあるaudiolistenerをfalseにしておく。
						//if(obj.name == "Main Camera")
						//	obj.GetComponent<AudioListener>().enabled = false;

						_monoList.Add(mono);
						mono.enabled = false;
					}
				}
			}

			// テイルズみたいに秘奥義の行動が、秘奥義が終わった後に影響している(ジャンプして終了したら、上から降ってくるとか)
			// みたいにするんだったらここで少なくとも、プレイヤー、敵、カメラ、サウンドマネージャー、ライトとかはtrueに戻して、別スクリプトで操作する。
			// フェードを挟んで終了させるんだったら、このままで大丈夫
		}
		else
		{
			for(int i = 0 ; i < _monoList.Count ; i ++)
			{
                if (_monoList[i] == null)
                    continue;

				_monoList[i].enabled = true;

				// GameMainシーンにあるaudiolistenerをtrueに戻しておく。
				//if (ObjList[i].name == "Main Camera")
				//	ObjList[i].GetComponent<AudioListener>().enabled = true;
			}
		}

        // Camera停止
        _mainCamera.SetActive(flg);

    }
}
