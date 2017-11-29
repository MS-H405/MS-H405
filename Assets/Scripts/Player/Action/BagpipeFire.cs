// ---------------------------------------------------------
// BagpipeFire.cs
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
  
public class BagpipeFire : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    // 接地判定
    private bool _isGround = false;

    // 接地からの生存時間
    [SerializeField] float _life = 20.0f; 

    // 与える力
    [SerializeField] float _addBackPower = 20.0f;
    [SerializeField] float _addUpPower = 200.0f;

    #endregion

    #region method

    /// <summary>
    /// 実行処理
    /// </summary>
    public void Run(Vector3 forward)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(-forward * _addBackPower);
        rigidbody.AddForce(new Vector3(0, _addUpPower, 0));
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 地形との接地判定処理
    /// </summary>
    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Field" && !_isGround)
        {
            _isGround = true;
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    _life -= Time.deltaTime;

                    if (_life > 0.0f)
                        return;
                    
                    Destroy(gameObject);
                });
        }

        if (col.transform.tag == "Player")
        {
            // なんかある？
        }

        if (col.transform.tag == "Enemy")
        {
            // ダメージ処理
        }
    }

    /// <summary>
    /// 地形との接地判定処理
    /// </summary>
    /*private void OnCollisionExit(Collision col)
    {
        if (col.transform.tag == "Field")
        {
            _isGround = false;
        }
    }*/

    #endregion
}  