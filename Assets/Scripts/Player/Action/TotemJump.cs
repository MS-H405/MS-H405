// ---------------------------------------------------------
// TotemJump.cs
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
  
public class TotemJump : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    // 地面との接地判定
    private bool _isGround = true;

    // トーテム押し出し量と時間
    private GameObject _totemObj = null;
    [SerializeField] float _pushTime = 1.0f;
    [SerializeField] float _pushAmount = 3.0f;
    // ジャンプ力
    [SerializeField] float _addForwardPower = 20.0f;
    [SerializeField] float _addUpPower = 200.0f;

    private PlayerMove _playerMove = null;
    private Rigidbody _rigidBody = null;

    #endregion

    #region method

    /// <summary>
    /// 実行処理
    /// </summary>
    public IEnumerator Run()
    {
        // 既に飛んでたら破棄
        if(!_isGround)
        {
            yield break;
        }

        _playerMove.enabled = false;
        _totemObj.SetActive(true);
        _totemObj.transform.position = new Vector3(transform.position.x, -1.5f, transform.position.z);

        float time = 0.0f;
        while(time < 1.0f)
        {
            time += Time.deltaTime / _pushTime;
            transform.position += new Vector3(0, _pushAmount * Time.deltaTime / _pushTime, 0);
            _totemObj.transform.position += new Vector3(0, _pushAmount * Time.deltaTime / _pushTime, 0);
            yield return null;
        }

        _rigidBody.AddForce(new Vector3(0, _addUpPower, 0) + (transform.forward * _addForwardPower));

        while (!_isGround)
        {
            _totemObj.transform.position -= new Vector3(0, _pushAmount * Time.deltaTime / _pushTime, 0) / 2.0f;
            yield return null;
        }

        _playerMove.enabled = true;
        _totemObj.SetActive(false);
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        _totemObj = transform.Find("TotemJump").gameObject;
        _totemObj.transform.SetParent(null);
        _playerMove = GetComponent<PlayerMove>();
        _rigidBody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 地形との接地判定処理
    /// </summary>
    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Field")
        {
            _isGround = true;
        }
    }

    /// <summary>
    /// 地形との接地判定処理
    /// </summary>
    private void OnCollisionExit(Collision col)
    {
        if (col.transform.tag == "Field")
        {
            _isGround = false;
        }
    }

    #endregion
}  