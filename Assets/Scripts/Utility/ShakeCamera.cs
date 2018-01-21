using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour 
{
    [SerializeField]
    float fCoefShakeDecay = 0.0006f;        // 揺れる力の減衰
    [SerializeField]
    float fCoefShakeIntensity = 0.03f;  // 揺れる幅の強度

    private Vector3 originPosition;     // 元の位置
    private Quaternion originRotation;  // 元の角度

    private float fShakeDecay;
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
			Debug.Log("fShakeIntensity : " + fShakeIntensity);
			Debug.Log("fShakeDecay : " + fShakeDecay);
        }
    }

	// カメラが動くときのの揺れ
    public void Shake(float intensity = 0.0f, float decay = 0.0f)
    {
		bMove = true;

        if (intensity == 0.0f)
        {
            fShakeIntensity = fCoefShakeIntensity;
        }
        else
        {
            fShakeIntensity = intensity;
        }

        if(decay == 0.0f)
        {
            fShakeDecay = fCoefShakeDecay;
        }
        else
        {
            fShakeDecay = decay;
        }
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
