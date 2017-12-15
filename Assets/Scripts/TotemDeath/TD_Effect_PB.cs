using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class TD_Effect_PB : PlayableBehaviour
{
	#region 定数

	const float CON_EFFECT_ROAR = 0.2f;		// 咆哮を開始する時間
	const float CON_EFFECT_SITA = 3.6f;		// トーテム下用の土埃を発生させる時間
	const float CON_EFFECT_UE = 4.2f;		// トーテム上用の土埃を発生させる時間
	const float CON_EFFECT_NAKA = 4.8f;		// トーテム中用の土埃を発生させる時間
	const float CON_EFFECT_POWERUP = 6.3f;	// パワーアップのエフェクトを出す時間
	const float CON_EFFECT_FLUSH = 6.3f;	// フラッシュを出す時間
	const float CON_EFFECT_PLAYER_POWERUP = 14.3f;	// プレイヤーパワーアップエフェクト


	#endregion


	#region 変数

	private GameObject _SetEffekseerObj;
	public GameObject SetEffekseerObj { get; set; }
	private SetEffekseerObject cs_SetEffekseerObject;

	private List<GameObject> _TD_powerupList = new List<GameObject>();	// パワーアップエフェクト
	public GameObject TD_powerupList
	{
		set { _TD_powerupList.Add(value); }
	}

	float fTime = 0.0f;		// 処理開始からの経過秒数

	List<bool> bEffectList = new List<bool>();	// エフェクトを発生させるかどうかのフラグ(trueなら発生させてfalseに変える)

	#endregion


	public override void OnGraphStart(Playable playable)
	{
		cs_SetEffekseerObject = SetEffekseerObj.GetComponent<SetEffekseerObject>();

		// 今発生させるエフェクトの個数分用意する
		for(int i = 0 ; i < 7 ; i ++)
			bEffectList.Add(true);
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
		fTime += Time.deltaTime;

		if (bEffectList[0] && fTime >= CON_EFFECT_ROAR)		// 咆哮
		{
			cs_SetEffekseerObject.NewEffect(0);
			bEffectList[0] = false;
		}
		if (bEffectList[1] && fTime >= CON_EFFECT_SITA)		// トーテム下用土埃
		{
			cs_SetEffekseerObject.NewEffect(1);
			bEffectList[1] = false;
		}
		if (bEffectList[2] && fTime >= CON_EFFECT_UE)		// トーテム上用土埃
		{
			cs_SetEffekseerObject.NewEffect(2);
			bEffectList[2] = false;
		}
		if (bEffectList[3] && fTime >= CON_EFFECT_NAKA)		// トーテム中用土埃
		{
			cs_SetEffekseerObject.NewEffect(3);
			bEffectList[3] = false;
		}
		if (bEffectList[4] && fTime >= CON_EFFECT_POWERUP)	// パワーアップ
		{
			// 現在親になっているトーテムが消えたら、エフェクトまで消えてしまうので、そうならないために、発生直前に親子関係を消しておく。
			float scale = 0.4f;
			for (int i = 0; i < _TD_powerupList.Count; i++)
			{
				_TD_powerupList[i].transform.parent = null;
				_TD_powerupList[i].transform.localScale = new Vector3(scale, scale, scale);
			}

			cs_SetEffekseerObject.NewEffect(4);
			cs_SetEffekseerObject.NewEffect(5);
			cs_SetEffekseerObject.NewEffect(6);
			bEffectList[4] = false;
		}
		if (bEffectList[5] && fTime >= CON_EFFECT_FLUSH)		// フラッシュ
		{
			cs_SetEffekseerObject.NewEffect(7);
			bEffectList[5] = false;
		}
		if (bEffectList[6] && fTime >= CON_EFFECT_PLAYER_POWERUP)		// プレイヤーパワーアップ
		{
			cs_SetEffekseerObject.NewEffect(8);
			bEffectList[6] = false;
		}
	}
}
