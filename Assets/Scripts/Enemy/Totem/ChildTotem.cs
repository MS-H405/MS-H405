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

    private int HeadAmount = 5;

    #endregion

    #region variable

    [SerializeField] float _oneBlockSize = 1.0f;
    private Rigidbody _rigidbody = null;
    private bool _isAtack = false;  //  攻撃中かのフラグ

    // 演出用変数
    [SerializeField] string _appearEffectName = "TS_totem_appear";
    List<ManualRotation> _totemHeadList = new List<ManualRotation>();
    private List<GameObject> _dropEffectList = new List<GameObject>();
    private Totem _parentScript = null;

    #endregion

    #region method

    /// <summary>
    /// 通常攻撃処理
    /// </summary>
    public IEnumerator PushUp(float speed)
    {
        // ランダムな位置に移動
        transform.position = RandomPos(-_oneBlockSize * (HeadAmount + 1.0f)); 
        transform.LookAt(PlayerManager.Instance.GetVerticalPos(transform.position));

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
        TotemRot(true, speed);
        Vector3 initPos = transform.position;
        GameEffectManager.Instance.PlayOnHeightZero(_appearEffectName, transform.position);
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.Totem_Attack);
        while (time < HeadAmount)
        {
            transform.position = Vector3.Lerp(initPos, initPos + new Vector3(0, _oneBlockSize * (HeadAmount + 1.0f), 0), time / HeadAmount);
            time += Time.deltaTime / speed;
            yield return null;
        }
        _isAtack = false;
    }

    /// <summary>
    /// 出ているトーテムを潜らせる処理
    /// </summary>
    public IEnumerator Dive(float speed)
    {
        float time = 0.0f;
        GameEffectManager.Instance.PlayOnHeightZero(_appearEffectName, transform.position);
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.Totem_Attack);
        TotemRot(true, speed);
        while (time < HeadAmount + 1.0f)
        {
            transform.position -= new Vector3(0, _oneBlockSize * (Time.deltaTime / speed), 0);
            time += Time.deltaTime / speed;
            yield return null;
        }
    }

    /// <summary>
    /// 特殊攻撃処理
    /// </summary>
    public IEnumerator SpecialAtack(float speed, float fallHeight, bool isSound)
    {
        // 土煙を出す
        GameEffectManager.Instance.PlayOnHeightZero(_appearEffectName, transform.position);
        float time = 0.0f;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        if (isSound)
        {
            SoundManager.Instance.PlaySE(SoundManager.eSeValue.Totem_Fly);
        }

        // 上に飛び出る処理
        Vector3 topPos = transform.position;
        topPos.y = fallHeight;
        Vector3 underPos = transform.position;

        TotemRot(true, 1.0f / (float)HeadAmount);
        time = 0.0f;
        while (time < 1.0f)
        {
            transform.position = Vector3.Lerp(underPos, topPos, time);
            time += Time.deltaTime;
            yield return null;
        }

        // 落下位置
        Vector3[] initPos = new Vector3[HeadAmount];
        for(int i = 0; i < transform.childCount; i++)
        {
            // 位置を保存
            initPos[i] = transform.GetChild(i).localPosition;

            // ランダムな位置に移動
            transform.GetChild(i).position = RandomPos(fallHeight);
        }

        foreach(ManualRotation head in _totemHeadList)
        {
            head.GetComponent<CapsuleCollider>().isTrigger = true;
        }

        foreach (GameObject effect in _dropEffectList)
        {
            effect.SetActive(true);
        }

        // 一気に落下するのを防止
        time = 0.0f;
        float waitTime = Random.Range(0.0f, 2.0f);
        while (time < waitTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        SoundManager.Instance.PlaySE(SoundManager.eSeValue.Totem_Fall);

        // 落下を待つ
        _isAtack = true;
        bool isEnd = false;
        while (!isEnd)
        {
            _rigidbody.AddForce(0.0f, -9.8f, 0.0f);

            for (int i = 0; i < _totemHeadList.Count; i++)
            {
                if (i % 2 == 0)
                {
                    _totemHeadList[i].transform.eulerAngles += new Vector3(0, 720, 0) * Time.deltaTime;
                }
                else
                {
                    _totemHeadList[i].transform.eulerAngles -= new Vector3(0, 720, 0) * Time.deltaTime;
                }
            }

            isEnd = _totemHeadList.Where(_ => _.transform.position.y <= -1.0f).ToList().Count() == HeadAmount;
            yield return null;
        }
        _isAtack = false;

        foreach (GameObject effect in _dropEffectList)
        {
            effect.SetActive(false);
        }

        // 初期位置に戻す
        foreach (ManualRotation head in _totemHeadList)
        {
            head.GetComponent<CapsuleCollider>().isTrigger = false;
        }
        _rigidbody.useGravity = false;
        for (int i = 0; i < _totemHeadList.Count; i++)
        {
            _totemHeadList[i].transform.localPosition = initPos[i];
            _totemHeadList[i].transform.localEulerAngles = Vector3.zero;
        }
        _rigidbody.velocity = Vector3.zero;
        transform.position = new Vector3(1000,0,0);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// トーテムの回転処理
    /// </summary>
    private void TotemRot(bool isUp, float speed)
    {
        speed *= HeadAmount;
        Vector3 rotAmount = new Vector3(0, 360, 0) * HeadAmount;
        if (isUp)
        {
            rotAmount *= -1;
        }

        // 回転実行
        foreach (ManualRotation rot in _totemHeadList)
        {
            rot.Run(rotAmount, speed);
            rotAmount *= -1;
        }
    }

    /// <summary>
    /// Playerの真下の座標を返す
    /// </summary>
    private Vector3 PlayerPos(float y)
    {
        Vector3 pos = PlayerManager.Instance.PlayerObj.transform.position;
        pos.y = y;

        if (_parentScript.CheckNearTotem(gameObject, pos))
        {
            pos = RandomPos(y);
        }

        return pos;
    }

    /// <summary>
    /// 地形内のランダムな座標を返す
    /// </summary>
    private Vector3 RandomPos(float y)
    {
        float range = StageData.FieldSize;

        Vector3 pos = new Vector3(-StageData.FieldSize, y, -StageData.FieldSize);

        float t, f;
        t = Random.Range(0, 65536) / 65536.0f * 2.0f * Mathf.PI;
        f = Random.Range(0, 65536) / 65536.0f * 2.0f * Mathf.PI;

        pos.x = range * Mathf.Sin(t) * Mathf.Cos(f) + 1.0f;
        pos.z = range * Mathf.Cos(t) + 1.0f;

        while (_parentScript.CheckNearTotem(gameObject, pos))
        {
            pos = RandomPos(y);
        }

        return pos;
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        HeadAmount = transform.childCount;
        _rigidbody = GetComponent<Rigidbody>();
        _totemHeadList = GetComponentsInChildren<ManualRotation>().ToList();

        // 子の子に配置してあるパーティクルを取得
        _dropEffectList = gameObject.GetAll().Where(_ => _.name == "ChildTotemFall").ToList();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Init(Totem parent)
    {
        _parentScript = parent;
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player" && _isAtack)
        {
            col.GetComponent<Player>().Damage();
        }
    }

    #endregion
}  