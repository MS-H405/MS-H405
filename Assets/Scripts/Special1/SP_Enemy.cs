using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_Enemy : MonoBehaviour
{
	#region 定数

	readonly Vector3 CON_FLY_SPEED = new Vector3(-10.0f, 35.0f, 30.0f);	// ぶっ飛ばされる時のスピード
	const float CON_FLY_ACCELE_Y = -20.0f;								// ぶっ飛ばされるときの加速度Y
	const float CON_DELETE_TIME = 1.0f;									// 吹っ飛んでから消えるまでの時間
	readonly Vector3 CON_FLY_ROTATE = new Vector3(-720.0f, 0.0f, 0.0f);	// 吹っ飛び時の回転速度

	#endregion

	#region 変数

	[SerializeField]	Material mat;

	// Effekseer関係
	SetEffekseerObject cs_SetEffekseerObject;

	#endregion


	// Use this for initialization
	void Start ()
	{
		mat.color = Color.white;
	}

	// 吹っ飛び開始
	public void StartFly()
	{
		StartCoroutine("Fly");
	}

	// 吹っ飛び
	private IEnumerator Fly()
	{
		Vector3 vStartScale = transform.localScale;
		Vector3 vSpeed = CON_FLY_SPEED;
	
		for(float fTime = 0.0f ; fTime < CON_DELETE_TIME ; fTime += Time.deltaTime)
		{
			vSpeed.y += CON_FLY_ACCELE_Y * Time.deltaTime;			// 移動
			transform.position += vSpeed * Time.deltaTime;

			transform.Rotate(CON_FLY_ROTATE * Time.deltaTime);		// 回転

			transform.localScale = Vector3.Lerp(vStartScale, Vector3.zero, fTime / CON_DELETE_TIME);		// 拡大率

			yield return null;
		}

		// 消滅エフェクトを出す
		SetEffekseerObject.Instance.NewEffect(12);
	}

	// スタン
	public void Stan()
	{
		Animator animator = GetComponent<Animator>();

		if(animator != null)
		{
			animator.SetBool("Stan", true);
		}
	}
}
