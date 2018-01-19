// ---------------------------------------------------------
// FreezeRotation.cs
// 概要 : 
// 作成者: Shota_Obora
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
  
public class FreezeRotation : MonoBehaviour
{ 
    #region variable

    [SerializeField] bool x = false;
    [SerializeField] bool y = false;
    [SerializeField] bool z = false;

    private Vector3 _initEulerAngles = Vector3.zero;

    #endregion

    #region method
    
    /// <summary>
    /// 回転初期化処理
    /// </summary>
    public void InitAngles()
    {
        transform.eulerAngles = _initEulerAngles;
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        Vector3 oldEulerAngles = transform.eulerAngles;
        this.LateUpdateAsObservable()
            .Subscribe(_ =>
            {
                if (oldEulerAngles == transform.eulerAngles)
                    return;

                Vector3 setEulerAngles = oldEulerAngles;
                if (!x)
                {
                    setEulerAngles.x = transform.eulerAngles.x;
                }
                if (!y)
                {
                    setEulerAngles.y = transform.eulerAngles.y;
                }
                if (!z)
                {
                    setEulerAngles.z = transform.eulerAngles.z;
                }
                transform.eulerAngles = oldEulerAngles;
                oldEulerAngles = transform.eulerAngles;
            });

        _initEulerAngles = transform.eulerAngles;
    }

    #endregion
}  