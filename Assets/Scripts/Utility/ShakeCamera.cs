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

    void Update()
    {
        if (fShakeIntensity > 0)
        {
            // ランダムに位置と角度を移動
            transform.position = originPosition + Random.insideUnitSphere * fShakeIntensity;
            transform.rotation = new Quaternion(originRotation.x + Random.Range(-fShakeIntensity, fShakeIntensity),
                                                originRotation.y + Random.Range(-fShakeIntensity, fShakeIntensity),
                                                originRotation.z + Random.Range(-fShakeIntensity, fShakeIntensity),
                                                originRotation.w + Random.Range(-fShakeIntensity, fShakeIntensity));
            fShakeIntensity -= fShakeDecay;
        }
    }

    public void Shake()
    {
        originPosition = transform.position;
        originRotation = transform.rotation;
        fShakeIntensity = fCoefShakeIntensity;
    }  
}
