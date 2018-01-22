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

    private float FireInterval = 6.0f;

    #endregion

    #region variable

    private float _nowFireInterval = 0.0f;
    public bool IsOn { get { return _nowFireInterval <= 0.0f; } }

    [SerializeField] GameObject _pipeObj = null;
    [SerializeField] GameObject _bagpipeSmokePrefab = null;
    [SerializeField] GameObject _bagpipeFirePrefab = null;
    private Animator _animator = null;

    private EffekseerEmitter _fireEffect = null;

    #endregion

    #region method

    /// <summary>
    /// 行動実行処理
    /// </summary>
    public void Action()
    {
        if (_nowFireInterval > 0.0f)
        {
            GameObject smoke = Instantiate(_bagpipeSmokePrefab, transform.position, transform.rotation);
            smoke.transform.LookAt(transform.position + transform.up - (transform.forward / 5.0f));
            smoke.transform.position = _pipeObj.transform.position + new Vector3(-0.2f, 0.6f, -0.2f);
            smoke.transform.SetParent(_pipeObj.transform);
            SoundManager.Instance.PlaySE(SoundManager.eSeValue.Player_BagpipeSmoke);
        }
        else
        {
            GameObject fire = Instantiate(_bagpipeFirePrefab, transform.position, transform.rotation);
            fire.transform.LookAt(transform.position + transform.up - (transform.forward / 5.0f));
            fire.transform.position = _pipeObj.transform.position + new Vector3(-0.2f, 0.7f, -0.3f);

            fire = Instantiate(_bagpipeFirePrefab, transform.position, transform.rotation);
            fire.transform.LookAt(transform.position + transform.up + (transform.forward / 5.0f));
            fire.transform.position = _pipeObj.transform.position + new Vector3(-0.2f, 0.7f, -0.3f);

            _nowFireInterval = FireInterval;
            SoundManager.Instance.PlaySE(SoundManager.eSeValue.Bagpipe_FireShot);
        }
    }

    /// <summary>
    /// 行動開始終了処理
    /// </summary>
    public void Run(bool isRun)
    {
        _fireEffect.Play();
        _animator.SetBool("Pipe", isRun);
        StaticCoroutine.Instance.StartStaticCoroutine(RunActive(isRun));
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.Player_BagpipeAppearance);
    }

    private IEnumerator RunActive(bool isRun)
    {
        if(isRun)
        {
            float time = 0.0f;
            while(time < 0.2f)
            {
                time += Time.deltaTime;
                yield return null;
            }
            _pipeObj.SetActive(true);
        }
        else
        {
            _pipeObj.SetActive(false);
        }
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
        _fireEffect = _pipeObj.transform.parent.Find("PipeFire").GetComponent<EffekseerEmitter>();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        if (_nowFireInterval <= 0.0f)
            return;

        _nowFireInterval -= Time.deltaTime;
    }

    #endregion
}  