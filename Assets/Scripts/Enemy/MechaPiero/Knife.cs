// ---------------------------------------------------------
// Knife.cs
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
  
public class Knife : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    private bool _isEnd = false;
    public bool IsEnd { get { return _isEnd; } } 

    #endregion

    #region method

    /// <summary>
    /// 実行処理
    /// </summary>
    public IEnumerator Run()
    {
        // 敵の方を向く
        Vector3 startRot = transform.eulerAngles;
        transform.LookAt(PlayerManager.Instance.Player.transform.position + new Vector3(0, 1.0f, 0));
        Vector3 targetRot = transform.eulerAngles + new Vector3(0, 0, 90);
        transform.eulerAngles = startRot;

        // 無駄な回転量が出ないようにする
        if (targetRot.y - startRot.y > 180.0f)
        {
            targetRot.y -= 360.0f;
        }
        else if (targetRot.y - startRot.y < -180.0f)
        {
            targetRot.y += 360.0f;
        }

        float time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime * 4.0f;
            transform.eulerAngles = Vector3.Lerp(startRot, targetRot, time);
            yield return null;
        }

        GetComponentInChildren<EffekseerEmitter>().Play();
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.MechaPiero_KnifeShot);

        time = 0.0f;
        while(time < 0.5f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        time = 2.5f;
        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                transform.position += transform.forward * 20.0f * Time.deltaTime;
                time -= Time.deltaTime;

                if (!_isEnd && time > 0.0f)
                    return;

                _isEnd = true;
                transform.position += transform.forward * 75.0f * Time.deltaTime;
                disposable.Dispose();
            });
    }

    /// <summary>
    /// 出現処理
    /// </summary>
    public IEnumerator Init()
    {
        transform.GetChild(0).gameObject.SetActive(false);

        //if (name.Contains("Center") || name.Contains("L3") || name.Contains("R3"))
        //{
        GameEffectManager.Instance.Play("KnifeStart", transform.position + (transform.forward * 2.0f));
        //}

        float time = 0.0f;
        while(time < 0.75f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        transform.GetChild(0).gameObject.SetActive(true);
    }

    /// <summary>
    /// 終了処理
    /// </summary>
    public IEnumerator End()
    {
        GameEffectManager.Instance.Play("KnifeLose", transform.position + (transform.forward * 2.0f));

        float time = 0.0f;
        while (time < 0.75f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        StaticCoroutine.Instance.StartStaticCoroutine(Init());
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    private void OnTriggerEnter(Collider col)
    {
        if (_isEnd)
            return;

        if (col.tag == "Player")
        {
            if (!col.GetComponent<Player>().Damage())
                return;

            GameEffectManager.Instance.Play("NeedleLight", transform.position);
            SoundManager.Instance.PlaySE(SoundManager.eSeValue.MechaPiero_KnifeHit);
        }
        if (col.tag == "Field")
        {
            GameEffectManager.Instance.Play("KnifeStick", transform.position + transform.forward * 3.0f);
            SoundManager.Instance.PlaySE(SoundManager.eSeValue.MechaPiero_KnifeHit);
            _isEnd = true;
        }
    }

    #endregion
}  