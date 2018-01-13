// ---------------------------------------------------------
// FireAtack.cs
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
  
public class FireAtack : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    [SerializeField] bool _isPlayer = false;

    #endregion

    #region method

    #endregion

    #region unity_event

    /// <summary>
    /// 当たり判定処理
    /// </summary>
    private void OnTriggerEnter(Collider col)
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
                obj.GetComponent<EnemyBase>().Damage();
                transform.GetComponent<SphereCollider>().enabled = false;
            }
        }
        else
        {
            if (col.gameObject.tag == "Player")
            {
                col.gameObject.GetComponent<Player>().Damage();
                transform.GetComponent<SphereCollider>().enabled = false;
            }
        }
    }
    private void OnParticleCollision(GameObject obj)
    {
        if (_isPlayer)
        {
            if (obj.tag == "Enemy")
            {
                while (obj.transform.parent)
                {
                    obj = obj.transform.parent.gameObject;
                }
                obj.GetComponent<EnemyBase>().Damage();
                transform.GetComponent<SphereCollider>().enabled = false;
            }
        }
        else
        {
            if (obj.tag == "Player")
            {
                obj.GetComponent<Player>().Damage();

                var col = transform.GetComponent<SphereCollider>();
                if (!col)
                    return;

                col.enabled = false;
            }
        }
    }

    #endregion
}  