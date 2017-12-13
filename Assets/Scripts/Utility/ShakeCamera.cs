using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour 
{
    [SerializeField]
    float fShakeDecay = 0.0006f;        // 揺れる力の減衰
    [SerializeField]
    float fCoefShakeIntensity = 0.03f;  // 揺れる幅の強度

    private Vector3 originPosition;     // 元の位置
    private Quaternion originRotation;  // 元の角度
    private float fShakeIntensity;

	bool bMove = true;

    void Update()
    {
        if (fShakeIntensity > 0)
        {
			if(!bMove)
			{// カメラが動かない時の揺れ
				// ランダムに位置と角度を移動
				transform.position = originPosition + Random.insideUnitSphere * fShakeIntensity;
				transform.rotation = new Quaternion(originRotation.x + Random.Range(-fShakeIntensity, fShakeIntensity),
				                                    originRotation.y + Random.Range(-fShakeIntensity, fShakeIntensity),
				                                    originRotation.z + Random.Range(-fShakeIntensity, fShakeIntensity),
				                                    originRotation.w + Random.Range(-fShakeIntensity, fShakeIntensity));
			}
			else
			{// カメラが動くときの揺れ
				transform.position = transform.position + Random.insideUnitSphere * fShakeIntensity;
				transform.rotation = new Quaternion(transform.rotation.x + Random.Range(-fShakeIntensity, fShakeIntensity),
													transform.rotation.y + Random.Range(-fShakeIntensity, fShakeIntensity),
													transform.rotation.z + Random.Range(-fShakeIntensity, fShakeIntensity),
													transform.rotation.w + Random.Range(-fShakeIntensity, fShakeIntensity));
			}

            fShakeIntensity -= fShakeDecay;
        }
    }

	// カメラが動くときのの揺れ
    public void Shake()
    {
		bMove = true;
        fShakeIntensity = fCoefShakeIntensity;
    }

	// カメラが動かない時の揺れ
	public void DontMoveShake()
	{
		bMove = false;
        originPosition = transform.position;
        originRotation = transform.rotation;
        fShakeIntensity = fCoefShakeIntensity;
	}

	// 揺れの強さを設定する
	public void SetParam(float fIntensity, float fDecay)
	{
		fCoefShakeIntensity = fIntensity;
		fShakeDecay = fDecay;
	}
}
