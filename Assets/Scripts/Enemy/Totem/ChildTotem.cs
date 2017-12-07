// ---------------------------------------------------------
// ChildTotem.cs
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
  
public class ChildTotem : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    [SerializeField] float _oneBlockSize = 1.0f;
    private Rigidbody _rigidbody = null;
    private bool _isAtack = false;  //  攻撃中かのフラグ

    // 演出用変数
    [SerializeField] string _appearEffectName = "TS_totem_appear";

    #endregion

    #region method

    /// <summary>
    /// 通常攻撃処理
    /// </summary>
    public IEnumerator PushUp(float speed)
    {
        // ランダムな位置に移動
        transform.position = new Vector3(Random.Range(-10.0f, 10.0f), -_oneBlockSize * 3.0f, Random.Range(-10.0f, 10.0f));

        // 土煙を出す
        GameEffectManager.Instance.PlayOnHeightZero(_appearEffectName, transform.position);
        float time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // 突き上げ処理
        time = 0.0f;
        _isAtack = true;
        GameEffectManager.Instance.PlayOnHeightZero(_appearEffectName, transform.position);
        while (time < 3.0f)
        {
            transform.position += new Vector3(0, _oneBlockSize * (Time.deltaTime / speed), 0);
            time += Time.deltaTime / speed;
            yield return null;
        }
        _isAtack = false;

        // TODO : 補正処理
        Vector3 pos = transform.position;
        pos.y = 0.0f;
        transform.position = pos;
    }

    /// <summary>
    /// 出ているトーテムを潜らせる処理
    /// </summary>
    public IEnumerator Dive(float speed)
    {
        float time = 0.0f;
        GameEffectManager.Instance.PlayOnHeightZero(_appearEffectName, transform.position);
        while (time < 3.0f)
        {
            transform.position -= new Vector3(0, _oneBlockSize * (Time.deltaTime / speed), 0);
            time += Time.deltaTime / speed;
            yield return null;
        }
    }

    /// <summary>
    /// 特殊攻撃処理
    /// </summary>
    public IEnumerator SpecialAtack(float speed, float fallHeight)
    {
        // 土煙を出す
        GameEffectManager.Instance.PlayOnHeightZero(_appearEffectName, transform.position);
        float time = 0.0f;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // 上に飛び出る処理
        Vector3 topPos = transform.position;
        topPos.y = fallHeight;
        Vector3 underPos = transform.position;

        time = 0.0f;
        while (time < 1.0f)
        {
            transform.position = Vector3.Lerp(underPos, topPos, time);
            time += Time.deltaTime;
            yield return null;
        }

        // 落下位置
        Vector3[] initPos = new Vector3[3];
        for(int i = 0; i < transform.childCount; i++)
        {
            // 位置を保存
            initPos[i] = transform.GetChild(i).localPosition;

            // ランダムな位置に移動
            transform.GetChild(i).position = new Vector3(Random.Range(-10.0f, 10.0f), fallHeight, Random.Range(-10.0f, 10.0f));
        }

        // TODO : 一気に落下するのを防止
        time = 0.0f;
        float waitTime = Random.Range(0.0f, 2.0f);
        while (time < waitTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // 落下を待つ
        _isAtack = true;
        while (transform.position.y > -10.0f)
        {
            _rigidbody.AddForce(0.0f, -9.8f, 0.0f);
            yield return null;
        }
        _isAtack = false;

        // 初期位置に戻す
        _rigidbody.useGravity = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localPosition = initPos[i];
        }
        _rigidbody.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Player" && _isAtack)
        {
            col.gameObject.GetComponent<Player>().Damage();
        }
    }

    #endregion
}  