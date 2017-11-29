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

    #endregion

    #region method

    /// <summary>
    /// 通常攻撃処理
    /// </summary>
    public IEnumerator NormalAtack(float speed)
    {
        // ランダムな位置に移動
        transform.position = new Vector3(Random.Range(-10.0f, 10.0f), -5.0f, Random.Range(-10.0f, 10.0f));

        int amount = 0;
        while(amount < 3)
        {
            amount++;

            float time = 0.0f;
            while(time < amount * speed)
            {
                transform.position += new Vector3(0, _oneBlockSize * (Time.deltaTime / speed), 0);
                time += Time.deltaTime;
                yield return null;
            }

            while (time > 0.0f && amount < 3)
            {
                transform.position -= new Vector3(0, _oneBlockSize * (Time.deltaTime / speed), 0);
                time -= Time.deltaTime;
                yield return null;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 特殊攻撃処理
    /// </summary>
    public IEnumerator SpecialAtack()
    {
        // TODO : 一気に落下するのを防止
        yield return new WaitForSeconds(Random.Range(0.0f, 2.0f));

        _rigidbody.useGravity = true;
        Vector3[] initPos = new Vector3[3];
        for(int i = 0; i < transform.childCount; i++)
        {
            // 位置を保存
            initPos[i] = transform.localPosition;

            // ランダムな位置に移動
            transform.GetChild(i).position = new Vector3(Random.Range(-10.0f, 10.0f), 50.0f, Random.Range(-10.0f, 10.0f));
        }

        // 落下を待つ
        while(transform.GetChild(0).position.y > 0.0f)
        {
            yield return null;
        }

        // 初期位置に戻す
        _rigidbody.useGravity = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).position = initPos[i];
        }

    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
    }

    #endregion
}  