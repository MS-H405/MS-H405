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

    [SerializeField] bool _isPlayer = false;
    [SerializeField] float _speed_sec = 30.0f;
    private float _life = 1.0f;

    #endregion

    #region method

    #endregion

    #region unity_event

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {
        // 位置調整
        Vector3 temp = transform.position;
        temp.y = 0.0f;
        transform.position = temp;
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.Bagpipe_FireExplosion);

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

        if (!_isPlayer)
        {
            Camera.main.GetComponent<ShakeCamera>().Shake(0.015f, 0.0012f);
        }
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    private void OnTriggerStay(Collider col)
    {
        if (_isPlayer)
        {
            if (col.tag == "Enemy")
            {
                GameObject obj = col.gameObject;
                while (obj.transform.parent)
                {
                    obj = obj.transform.parent.gameObject;
                }
                obj.GetComponent<EnemyBase>().Damage(1, "Bagpipe");
                _life = 0.25f;
                GetComponent<SphereCollider>().enabled = false;
            }
        }
        else
        {
            if (col.transform.position.y >= 1.0f)
                return;

            if (col.tag == "Player")
            {
                if (!col.GetComponent<Player>().Damage())
                    return;

                _life = 0.25f;
                GetComponent<SphereCollider>().enabled = false;
            }
        }
    }

    #endregion
}  