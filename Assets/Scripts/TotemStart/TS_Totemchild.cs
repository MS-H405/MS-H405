using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// トーテム登場ムービーで使う子トーテム操作用
public class TS_Totemchild : MonoBehaviour
{
	#region 定数

	readonly Vector3 CON_SCALE = new Vector3(0.8f, 0.8f, 0.8f);		// 拡大率
	const float CON_APPEAR_TIME = 0.4f;		// 生える時間
	const float CON_START_POSY = -5.7f;		// 初期のY座標
	const float CON_END_POSY = 0.0f;		// 終わりのY座標

	const float CON_LOOKAT_TIME = 1.0f;		// ボスの方向を向く時間

	const float CON_BACK_TIME = 1.0f;		// ひっこむ時間

	#endregion


	#region 変数

	[SerializeField]	GameObject EffectObj;
	[SerializeField]	GameObject DiveObj;
	bool bEffect = true;		// 生えるエフェクトを再生するかどうか
	float fWait = 0.0f;			// 出現と、エフェクトのタイミングを合わせるためのタイマー

	float fTime;				// 生えるときに使うパラメーター
	Vector3 vStartPos;			// スタート位置
	Vector3 vEndPos;			// 終了位置

	float fRotateTime;			// 回転するときに使うパラメーター
	Vector3 vStartEular;		// 最初に向いてた角度
	Vector3 vEndEular;			// ボスの角度

	bool bInit = true;
	float fBack;

	#endregion


	// Use this for initialization
	void Start ()
	{
		// 初期の座標・拡大率
		SetStartPos();
		transform.position = new Vector3(transform.position.x, CON_START_POSY, transform.position.z);
		transform.localScale = CON_SCALE;

		// 生える時の情報を設定
		fTime = 0.0f;
		vStartPos = transform.position;
		vEndPos = new Vector3(transform.position.x, CON_END_POSY, transform.position.z);

		// 回転時の情報を設定
		fRotateTime = 0.0f;
		vStartEular = new Vector3(0.0f, 180.0f, 0.0f);
		//Vector2 vec = new Vector2(GameObject.Find("TotemBoss").transform.position.x, GameObject.Find("TotemBoss").transform.position.z);
		Vector2 vec = Vector2.zero;	// どうせBOSSはど真ん中にいるはず
		vec = new Vector2(vec.x - vEndPos.x, vec.y - vEndPos.z);
		vEndEular = new Vector3(0.0f, Mathf.Acos(Vector2.Dot(Vector2.down, vec.normalized)) * Mathf.Rad2Deg, 0.0f);
		if (transform.position.x < GameObject.Find("TS_TotemBoss").transform.position.x)
			vEndEular = new Vector3(vEndEular.x, vStartEular.y - vEndEular.y, vEndEular.z);
		else
			vEndEular = new Vector3(vEndEular.x, vStartEular.y + vEndEular.y, vEndEular.z);
	}



	// ちびトーテム生える
	public void Appear()
	{
		// エフェクト発生
		if (bEffect)
		{
			EffectObj.GetComponent<EffekseerEmitter>().Play();
			bEffect = false;

			MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.TS_TotemChild);
		}

		// 出現と、エフェクトのタイミングを合わせる
		fWait += Time.deltaTime;
		if (fWait < 0.1f)
			return;

		// 移動完了済み
		if(fTime >= 1.0f)
			return;

		fTime += Time.deltaTime / CON_APPEAR_TIME;
		if(fTime > 1.0f)
			fTime = 1.0f;

		transform.localPosition = Vector3.Lerp(vStartPos, vEndPos, fTime);
	}


	// ボスのほうを向く
	public void LookAtBoss()
	{
		// 回転完了済み
		if (fRotateTime >= 1.0f)
			return;

		fRotateTime += Time.deltaTime / CON_LOOKAT_TIME;
		if (fRotateTime > 1.0f)
			fRotateTime = 1.0f;

		Vector3 temp = Vector3.Lerp(vStartEular, vEndEular, fRotateTime);
		transform.eulerAngles = temp;
	}


	// ひっこむ
	public void Back()
	{
		if(bInit)
		{
			fBack = 0.0f;
			vStartPos = transform.position;
			vEndPos = new Vector3(transform.position.x, CON_START_POSY, transform.position.z);
			DiveObj.GetComponent<EffekseerEmitter>().Play();

			MovieSoundManager.Instance.PlaySE(MovieSoundManager.eSeValue.TS_TotemDive);

			bInit = false;
		}

		// 移動完了済み
		if (fBack >= 1.0f)
			return;

		fBack += Time.deltaTime / CON_BACK_TIME;
		if (fBack > 1.0f)
			fBack = 1.0f;

		transform.localPosition = Vector3.Lerp(vStartPos, vEndPos, fBack);
	}







	// Timelineのせいでよくバグって位置が(0.0f, 0.0f, 0.0f)になって再配置がめんどくさいので無理やり設定しておく
	private void SetStartPos()
	{
		switch(transform.name)
		{
			case "TS_TotemChild1":
				transform.position = new Vector3(-5.25f, 0.0f, -1.97f);
				transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
				break;

			case "TS_TotemChild2":
				transform.position = new Vector3(-2.97f, 0.0f, 2.78f);
				transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
				break;

			case "TS_TotemChild3":
				transform.position = new Vector3(1.58f, 0.0f, 4.6f);
				transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
				break;

			case "TS_TotemChild4":
				transform.position = new Vector3(3.93f, 0.0f, 1.21f);
				transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
				break;

			case "TS_TotemChild5":
				transform.position = new Vector3(5.62f, 0.0f, -1.69f);
				transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
				break;
		}
	}
}
