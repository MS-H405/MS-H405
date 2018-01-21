// ---------------------------------------------------------
// LockOn.cs
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
  
public class LockOn : MonoBehaviour
{
    #region variable

    [SerializeField] float _lookAdjustment = 1.0f;
    [SerializeField] float _lookPosHeight = 0.0f;

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        float distance = 0.0f;
        Vector3 initScale = transform.localScale;
        SpriteRenderer spRend = GetComponent<SpriteRenderer>(); 
        GameCamera gameCamera = Camera.main.GetComponent<GameCamera>();

        this.UpdateAsObservable()
            .Where(_ => EnemyManager.Instance.BossEnemy)
            .Subscribe(_ =>
            {
                transform.position = EnemyManager.Instance.BossEnemy.transform.position + gameCamera.LookPosOffset + new Vector3(0,_lookPosHeight,0);
                distance = Vector3.Distance(transform.position, gameCamera.transform.position);
                transform.LookAt(gameCamera.transform.position);
                transform.position += transform.forward * _lookAdjustment;
                transform.localScale = initScale * distance / 3.0f;
            });

        this.ObserveEveryValueChanged(_ => EnemyManager.Instance.BossEnemy)
            .Subscribe(_ =>
            {
                spRend.enabled = EnemyManager.Instance.BossEnemy;
            });
    }

    #endregion
}  