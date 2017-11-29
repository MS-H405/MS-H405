using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MovieManager : MonoBehaviour
{
	public enum MOVIE_SCENE
	{
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
	private List<GameObject> ObjList = new List<GameObject>();
	private string MovieSceneName;


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
		//if (Input.GetKeyDown(KeyCode.P))
		//	MovieFinish();
	}


	// フェード中かどうかを返す
	public bool GetisMovideFade()
	{
		return isFading;
	}


	// ムービーシーンを呼び出す
	public void MovieStart(MOVIE_SCENE scene)
	{
		if(isFading)
			return;

		isFading = true;

		StartCoroutine("Col_MovieStart", scene);
	}

	// ムービーシーン終了されたら呼ばれる
	public void MovieFinish()
	{
		if (isFading)
			return;

		isFading = true;

		StartCoroutine("Col_MovieFinish");
	}

	private IEnumerator Col_MovieStart(MOVIE_SCENE scene)
	{
		// 最初にGameMain.sceneの全てのオブジェクトの更新を停止させる関数を呼ぶ


		// フェードイン
		bool bFadeIn = false;
		MovieFade.Instance.FadeIn(true, () =>
		{
			bFadeIn = true;
		});

		// フェードインが終わるまではここで処理ストップ
		while (!bFadeIn)
			yield return null;
	
		#region シーンを読み込む
		switch (scene)
		{
			case MOVIE_SCENE.TOTEM_START:
				SceneManager.LoadScene("a", LoadSceneMode.Additive);
				MovieSceneName = "a";
				break;

			case MOVIE_SCENE.TOTEM_DEATH:
				SceneManager.LoadScene("a", LoadSceneMode.Additive);
				MovieSceneName = "a";
				break;

			case MOVIE_SCENE.BAGPIPE_START:
				SceneManager.LoadScene("a", LoadSceneMode.Additive);
				MovieSceneName = "a";
				break;

			case MOVIE_SCENE.BAGPIPE_DEATH:
				SceneManager.LoadScene("a", LoadSceneMode.Additive);
				MovieSceneName = "a";
				break;

			case MOVIE_SCENE.MECHA_START:
				SceneManager.LoadScene("a", LoadSceneMode.Additive);
				MovieSceneName = "a";
				break;

			case MOVIE_SCENE.MECHA_DEATH:
				SceneManager.LoadScene("a", LoadSceneMode.Additive);
				MovieSceneName = "a";
				break;

			case MOVIE_SCENE.SPECIAL_1:
				SceneManager.LoadScene("Special_1", LoadSceneMode.Additive);
				MovieSceneName = "Special_1";
				break;

			case MOVIE_SCENE.SPECIAL_2:
				SceneManager.LoadScene("a", LoadSceneMode.Additive);
				MovieSceneName = "a";
				break;

			case MOVIE_SCENE.SPECIAL_3:
				SceneManager.LoadScene("a", LoadSceneMode.Additive);
				MovieSceneName = "a";
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
		yield return new WaitForSeconds(0.2f);

		// フェードアウト
		MovieFade.Instance.FadeOut(true, () =>
		{
			isFading = false;		// フェード終了
		});
	}

	private IEnumerator Col_MovieFinish()
	{
		// フェードイン
		bool bFadeIn = false;
		MovieFade.Instance.FadeIn(false, () =>
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
		yield return new WaitForSeconds(0.2f);

		// フェードアウト
		MovieFade.Instance.FadeOut(false, () =>
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
			foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
			{
				// DontDestroyOnLoadにあるオブジェクトはfalseにしないようにする
				GameObject RootObj = obj.transform.root.gameObject;

				Scene scene = SceneManager.GetActiveScene();
				foreach (var sceneRootObject in scene.GetRootGameObjects())
				{
					if (sceneRootObject == RootObj)
					{
						// audiolistenerが2つあると言われるので、GameMainシーンにあるaudiolistenerをfalseにしておく。
						//if(obj.name == "Main Camera")
						//	obj.GetComponent<AudioListener>().enabled = false;

						ObjList.Add(obj);
						obj.SetActive(false);
					}
				}
			}

			// テイルズみたいに秘奥義の行動が、秘奥義が終わった後に影響している(ジャンプして終了したら、上から降ってくるとか)
			// みたいにするんだったらここで少なくとも、プレイヤー、敵、カメラ、サウンドマネージャー、ライトとかはtrueに戻して、別スクリプトで操作する。
			// フェードを挟んで終了させるんだったら、このままで大丈夫
		}
		else
		{
			for(int i = 0 ; i < ObjList.Count ; i ++)
			{
				ObjList[i].SetActive(true);

				// GameMainシーンにあるaudiolistenerをtrueに戻しておく。
				//if (ObjList[i].name == "Main Camera")
				//	ObjList[i].GetComponent<AudioListener>().enabled = true;
			}
		}
	}
}
