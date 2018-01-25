// ---------------------------------------------------------
// Needle.cs
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
  
public class Needle : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    private Vector3 _initLocalPos = Vector3.zero;
    private EffekseerEmitter _needleEffect = null;
    //private bool _isAttack = false;

    #endregion

    #region method

    /// <summary>
    /// トゲ飛ばし実行処理
    /// </summary>
    public IEnumerator Run(float life, bool isSound)
    {
        float time = 0.0f;
        GameEffectManager.Instance.Play("NeedleLight", transform.position + transform.up);
        while (time < 2.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        //_isAttack = true;

        time = 0.0f;
        _needleEffect.Play();
        if (isSound)
        {
            SoundManager.Instance.PlaySE(SoundManager.eSeValue.MechaPiero_Needle);
        }

        while (time < life - 2.0f)
        {
            time += Time.deltaTime;
            transform.position += transform.up * 20.0f * Time.deltaTime;
            yield return null;
        }

        //_isAttack = false;
        _needleEffect.Stop();
        transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// トゲ再生成処理
    /// </summary>
    public IEnumerator Reload()
    {
        float time = 0.0f;
        Vector3 nowPos = transform.localPosition;

        while (time < 1.0f)
        {
            time += Time.deltaTime * 1.666f;
            transform.localPosition = Vector3.Lerp(nowPos, _initLocalPos, time);
            yield return null;
        }
        transform.localPosition = _initLocalPos;
        GameEffectManager.Instance.Play("NeedleLight", transform.position + transform.up);
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {
        _initLocalPos = transform.localPosition;
        _needleEffect = GetComponentInChildren<EffekseerEmitter>();
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    private void OnTriggerEnter(Collider col)
    {
        //if (!_isAttack)
        //    return;

        if (col.tag == "Player")
        {
            if (!col.GetComponent<Player>().Damage())
                return;

            GameEffectManager.Instance.Play("NeedleHit", transform.position);
        }
    }

    #endregion
}  