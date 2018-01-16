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

    #endregion

    #region method

    /// <summary>
    /// 実行処理
    /// </summary>
    public IEnumerator Run()
    {
        // 敵の方を向く
        // TODO : とりあえず一瞬で向く
        transform.LookAt(PlayerManager.Instance.Player.transform.position + new Vector3(0, 1.0f, 0));
        transform.localEulerAngles += new Vector3(0,0,90);

        float time = 0.0f;
        while(time < 0.5f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                transform.position += transform.forward * 20.0f * Time.deltaTime;
            });
    }

    /// <summary>
    /// 出現処理
    /// </summary>
    public IEnumerator Init()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        GameEffectManager.Instance.Play("KnifeStart", transform.position + (transform.forward * 2.0f));

        float time = 0.0f;
        while(time < 0.25f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        transform.GetChild(0).gameObject.SetActive(true);
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
        if (col.tag == "Player")
        {
            if (!col.GetComponent<Player>().Damage())
                return;

            GameEffectManager.Instance.Play("PinAttack", transform.position);
            Destroy(gameObject);
        }
        if (col.tag == "Field")
        {
            GameEffectManager.Instance.Play("KnifeLose", transform.position + transform.forward);
            Destroy(gameObject);
        }
    }

    #endregion
}  