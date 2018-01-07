// ---------------------------------------------------------
// FireTrap.cs
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
  
public class FireTrap : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    [SerializeField] float _speed_sec = 30.0f;
    private float _life = 1.0f;

    #endregion

    #region method

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake ()
    {

    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {
        ParticleSystem flame = transform.Find("Flame").GetComponent<ParticleSystem>();
        float flameInitSize = flame.startSize;
        ParticleSystem secondaryFlame = transform.Find("SecondaryFlame").GetComponent<ParticleSystem>();
        float secondaryFlameInitSize = secondaryFlame.startSize;

        SphereCollider collider = GetComponent<SphereCollider>();

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                _life -= Time.deltaTime / _speed_sec;
                flame.startSize = flameInitSize * _life;
                secondaryFlame.startSize = secondaryFlameInitSize * _life;
                collider.enabled = (_life > 0.25f);

                if (_life > 0.25f)
                    return;

                transform.position -= new Vector3(0.0f, 0.1f, 0.0f) * Time.deltaTime;

                if (_life > 0.0f)
                    return;

                 Destroy(gameObject);
            });
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {

    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && col.transform.position.y < 1.0f)
        {
            Debug.Log(col.name);
            col.GetComponent<Player>().Damage();
            _life = 0.25f;
        }
    }

    #endregion
}  