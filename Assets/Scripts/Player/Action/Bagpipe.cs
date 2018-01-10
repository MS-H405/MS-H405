// ---------------------------------------------------------
// Bagpipe.cs
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
  
public class Bagpipe : MonoBehaviour
{
    #region define

    private readonly int MaxFireAmount = 10;

    #endregion

    #region variable

    static public int _nowFireAmount = 0;

    [SerializeField] GameObject _pipeObj = null;
    [SerializeField] GameObject _bagpipeFirePrefab = null;
    private Animator _animator = null;

    #endregion

    #region method

    /// <summary>
    /// 行動実行処理
    /// </summary>
    public void Action()
    {
        if (_nowFireAmount >= MaxFireAmount)
            return;

        GameObject bag = Instantiate(_bagpipeFirePrefab, transform.position, transform.rotation);
        bag.transform.LookAt(transform.position + transform.up - (transform.forward / 5.0f));
        bag.transform.position = _pipeObj.transform.position + new Vector3(-0.2f, 0.7f, -0.3f);
        _nowFireAmount++;
    }

    /// <summary>
    /// 行動開始終了処理
    /// </summary>
    public void Run(bool isRun)
    {
        _pipeObj.SetActive(isRun);
        _animator.SetBool("Pipe", isRun);
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        _pipeObj.SetActive(false);
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {

    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update ()
    {

    }

    #endregion
}  