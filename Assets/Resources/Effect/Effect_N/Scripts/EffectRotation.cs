using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRotation : MonoBehaviour 
{
    [SerializeField]
    private float fRotationSpeedX;       // X回転スピード
    [SerializeField]
    private float fRotationSpeedY;       // Y回転スピード
    [SerializeField]
    private float fRotationSpeedZ;       // Z回転スピード

    // ===== 更新 =====
    void Update()
    {
        // オブジェクトを回転させる
        this.transform.Rotate(fRotationSpeedX, fRotationSpeedY, fRotationSpeedZ);
    }
}
