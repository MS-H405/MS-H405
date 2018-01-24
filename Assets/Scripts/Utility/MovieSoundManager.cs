using UnityEngine;
using System;
using System.Collections;

public class MovieSoundManager : MonoBehaviour {

	/// <summary>
	/// 概要 : サウンド管理
	/// Author : 大洞祥太が作ったのをパクった
	/// </summary>
	/// 

	protected static MovieSoundManager instance;

	public static MovieSoundManager Instance {
		get {
			if (instance == null) {
				instance = (MovieSoundManager)FindObjectOfType(typeof(MovieSoundManager));

				if (instance == null) {
					Debug.LogError("MovieSoundManager Instance Error");
				}
			}

			return instance;
		}
	}

	public struct tSE
	{
		public float time;	// 時間
		public bool bDo;	// 処理中
		public bool bDone;	// 再生したか
	};

	[SerializeField]
	bool bUseSound = false;
	
	public enum eBgmValue {
		Special = 0,
		TotemStart,
		HermitCrabStart,
		MechaStart,
		MS_Thunder1,

        Max,
	};
	
	public enum eSeValue {
		SP_PinThrow = 0,
		SP_BigJug1,
		SP_BigJug2,
		SP_Slow,
		SP_SmallAttack,
		SP_Blast,			// 5
		SP_Jump,
		SP_RideOn,
		SP_StopBall,
		SP_Charge,
		SP_HitBall,			// 10
		SP_End,
		SP_RideOn2,
		SP_Babiron,
		SP_TotemAttack,
		TS_TotemChild,		// 15
		TS_TotemBoss,
		TS_TotemRoar,
		TS_TotemDive,
		TD_TotemSita,
		MS_LighUp,			// 20
		MS_Pause,
		MS_NeedleForm,
		MS_NeedleFormEffect,
		TS_ChildTurn,
		BS_Walk,			// 25
		BS_Jamp,
		BS_Volcano,
		BS_Cry,
		BD_CryLast,
		BS_SetUp,			// 30
		TS_Cry,
		TS_Special_Learning,
		TS_Win,
		TS_BossDeath,
		TS_BossFlush,		// 35
		MS_RollJamp,
		MS_Stroke,
		MS_Thunder2,
		MD_Bomb,
		MD_Cong,			// 40



		TS_GOGOGO,				// 未実装	ボストーテム登場前の揺れ

        Max,
	};

	// 音量
	public SoundVolume volume = new SoundVolume();

	// 最大同時再生数
	[SerializeField] int MaxSyncBgmPlay = 5;
	[SerializeField] int MaxSyncSePlay = 20;
	[SerializeField] int MaxSyncVoicePlay = 1;

	AudioSource[] BGMsource;	// BGM
	AudioSource[] SEsources;	// SE
	AudioSource[] VoiceSources;	// 音声

	[SerializeField] AudioClip[] BGM;		// BGM
	[SerializeField] AudioClip[] SE;		// SE
	[SerializeField] AudioClip[] Voice;		// 音声

	void Awake()
	{
		GameObject[] obj = GameObject.FindGameObjectsWithTag("MovieSoundManager");
		if (obj.Length > 1)
		{
			// 既に存在しているなら削除
			Destroy(gameObject);
		}
		else
		{
			// 音管理はシーン遷移では破棄させない
			DontDestroyOnLoad(gameObject);
		}

		// インスタンスの生成
		BGMsource		= new AudioSource[MaxSyncBgmPlay];
		SEsources		= new AudioSource[MaxSyncSePlay];
		VoiceSources	= new AudioSource[MaxSyncVoicePlay];
		volume			= new SoundVolume();

		//----- 全てのAudioSourceコンポーネントを追加する
		// BGM AudioSource
		for (int i = 0; i < BGMsource.Length; i++) {
			BGMsource[i] = gameObject.AddComponent<AudioSource> ();
			// BGMはループを有効にする
			BGMsource[i].loop = true;
		}

		// SE AudioSource
		for (int i = 0; i < SEsources.Length; i++) {
			SEsources[i] = gameObject.AddComponent<AudioSource>();
		}

		// 音声 AudioSource
		for (int i = 0; i < VoiceSources.Length; i++) {
			VoiceSources[i] = gameObject.AddComponent<AudioSource>();
		}

		// 音量の初期化
		volume.Init();
	}
		
	void Update() {
		// ミュート設定
		/*foreach (AudioSource source in BGMsource) {
			source.mute = volume.Mute;
		}
		foreach (AudioSource source in SEsources) {
			source.mute = volume.Mute;
		}
		foreach (AudioSource source in VoiceSources) {
			source.mute = volume.Mute;
		}

		// ボリューム設定
		foreach (AudioSource source in BGMsource) {
			source.volume = volume.SE;
		}
		foreach (AudioSource source in SEsources) {
			source.volume = volume.SE;
		}
		foreach (AudioSource source in VoiceSources) {
			source.volume = volume.Voice;
		}*/
	}

	public bool PlayBGM(eBgmValue i) {
		if (!bUseSound)
			return false;

		int index = (int)i; 
		if (0 > index || BGM.Length <= index) {
			return false;
		}

		// 同じBGMの場合は何もしない
		foreach (AudioSource source in BGMsource) {
			if (source.clip == BGM [index]) {
				return false;
			}
		}

		// 再生中で無いAudioSouceで鳴らす
		foreach (AudioSource source in BGMsource) {
			if (!source.clip) {
				source.clip = BGM[index];
				source.Play();
				return true;
			}
		}

		return false;
	}

	public void PauseBGM(bool bPause) {
		if (bPause) {
			foreach (AudioSource source in BGMsource) {
				source.Pause ();
			}
		} else {
			foreach (AudioSource source in BGMsource) {
				source.UnPause ();
			}
		}
	}

	// 指定したBGMがあれば再開or停止
	public bool PauseBGM(eBgmValue i, bool bPause) {
		int index = (int)i; 
		if (0 > index || BGM.Length <= index) {
			return false;
		}

		if (bPause) {
			foreach (AudioSource source in BGMsource) {
				if (source.clip != BGM [index])
					continue;
				
				source.Pause ();
                return true;
			}
		} else {
			foreach (AudioSource source in BGMsource) {
				if (source.clip != BGM [index])
					continue;
				
				source.UnPause ();
                return true;
			}
        }
        return false;
	}

	public bool StopBGM(eBgmValue i) {
		int index = (int)i; 
		if (0 > index || BGM.Length <= index) {
			return false;
		}

		// 再生中であれば止める
		foreach (AudioSource source in BGMsource) {
			if (source.clip == BGM[index]) {
				source.clip = null;
				source.Stop();
				return true;
			}
		}

		return false;
	}

	public void StopBGM() {
		// 全てのBGM用のAudioSouceを停止する
		foreach (AudioSource source in BGMsource) {
			source.Stop();
			source.clip = null;
		}
	}

	// 指定したBGMは再生中なのか 
	public bool NowOnBGM(eBgmValue i) {
		int index = (int)i; 
		if (0 > index || BGM.Length <= index) {
			return false;
		}
			
		foreach (AudioSource source in BGMsource) {
			if (source.clip != BGM [index])
				continue;
			
			return true;
		}

		return false;
	}

	public void FadeOutVolume(float interval) {
		StartCoroutine(FadeOut(interval));
	}

	public void FadeInVolume(float interval) {
		StartCoroutine(FadeIn(interval));
	}
		
	IEnumerator FadeOut(float interval) {
		float time = 0.0f;
		this.volume.OldBGM = this.volume.BGM;
		while (time <= interval) {
			//this.volume.BGM = Mathf.Lerp(this.volume.OldBGM, 0.0f, time / interval);
			this.volume.BGM = Mathf.Lerp(1.0f, 0.0f, time / interval);
			time += Time.deltaTime;
			yield return 0;
		}
	}
		
	IEnumerator FadeIn(float interval) {
		float time = 0.0f;
		float VolumeB = this.volume.BGM;
		while (time <= interval) {
			//this.volume.BGM = Mathf.Lerp(VolumeB, this.volume.OldBGM, time / interval);
			this.volume.BGM = Mathf.Lerp(0.0f, 1.0f, time / interval);
			time += Time.deltaTime;
			yield return 0;
		}
	}

    public bool PlaySE(eSeValue i)
    {
        if (!bUseSound)
            return false;

        int index = (int)i;
        if (0 > index || SE.Length <= index)
        {
            return false;
        }

        // 再生中で無いAudioSouceで鳴らす
        foreach (AudioSource source in SEsources)
        {
            if (!source.isPlaying)
            {
                source.clip = SE[index];
                source.Play();
                return true;
            }
        }

        return false;
    }

	public void PauseSE(bool bPause) {
		if (bPause) {
			foreach (AudioSource source in SEsources) {
                source.Pause(); 
			}
		} else {
			foreach (AudioSource source in SEsources) {
                source.UnPause(); 
			}
		}
	}

	// 指定したSEがあれば再開or停止
	public void PauseSE(eSeValue i, bool bPause) {
		int index = (int)i; 
		if (0 > index || SE.Length <= index) {
			return;
		}

		if (bPause) {
			foreach (AudioSource source in SEsources) {
				if (source.clip != SE [index])
					continue;
				
				source.Pause ();
			}
		} else {
			foreach (AudioSource source in SEsources) {
				if (source.clip != SE [index])
					continue;
				
				source.UnPause ();
			}
		}
	}

    public void StopSE()
    {
        // 全てのSE用のAudioSouceを停止する
        foreach (AudioSource source in SEsources)
        {
            source.Stop();
            source.clip = null;
        }
    }

    public void StopSE(eSeValue i)
    {
        int index = (int)i;
        if (0 > index || SE.Length <= index)
        {
            return;
        }

        // 全てのSE用のAudioSouceを停止する
        foreach (AudioSource source in SEsources)
        {
			if (source.clip != SE [index])
				continue;
            
            source.Stop();
            source.clip = null;
        }
    }

	public void PlayVoice(int index) {
		if (0 > index || Voice.Length <= index) {
			return;
		}
		// 再生中で無いAudioSouceで鳴らす
		foreach (AudioSource source in VoiceSources) {
			if (false == source.isPlaying) {
				source.clip = Voice[index];
				source.Play();
				return;
			}
		}
	}
		
	public void StopVoice() {
		// 全ての音声用のAudioSouceを停止する
		foreach (AudioSource source in VoiceSources) {
			source.Stop();
			source.clip = null;
		}
	}

	public void SaveVolume() {
		PlayerPrefs.SetFloat("BGM", volume.BGM);
		PlayerPrefs.SetFloat("SE", volume.SE);
	}

	[System.Serializable]
	public class SoundVolume {
		public float BGM = 1.0f;
		public float Voice = 1.0f;
		public float SE = 1.0f;
		public bool Mute = false;

		public float OldBGM = 1.0f;
		public float OldVoice = 1.0f;
		public float OldSE = 1.0f;

		public void Init() {
			OldBGM		= BGM	= PlayerPrefs.GetFloat("BGM", 1.0f);
			OldSE		= SE	= PlayerPrefs.GetFloat("SE", 1.0f);
			OldVoice = Voice = 1.0f;
			Mute = false;
		}
	}



	// BGMの音量を変える
	public void  ChangeVolumeBGM(eBgmValue i, float volume)
	{
		int index = (int)i;
		if(0 > index || BGM.Length <= index)
			return;

		foreach(AudioSource source in BGMsource)
		{
			if(source.clip != BGM[index])
				continue;

			source.volume = volume;
		}
	}
}
