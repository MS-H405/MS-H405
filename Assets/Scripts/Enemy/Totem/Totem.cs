﻿// ---------------------------------------------------------
// Totem.cs
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
  
public class Totem : EnemyBase
{
    #region define

    private enum eAction
    {
        TotemPushUp = 0,
        ChildTotemPushUp,
        SpecialAtack,
        Max,
    };

    #endregion

    #region variable

    // トーテムの現在の状態を保持
    private eAction _action = eAction.TotemPushUp;
    private int _headAmount = 3;
    private bool _isAtack = false;  //  攻撃中かのフラグ

    [SerializeField] float _oneBlockSize = 1.0f;
    [SerializeField] float _oneBlockUpSpeed = 0.25f;         // 頭ひとつぶんの飛び出る

    [SerializeField] int _childTotemAmount = 5;              // 子分トーテムの数
    [SerializeField] GameObject _childTotemPrefab = null;    // 子分トーテムのプレハブ

    // 子分トーテムのリスト
    private List<ChildTotem> _childTotemList = new List<ChildTotem>();

    private Rigidbody _rigidbody = null;
    private float _fallHeight = 50.0f;

    // 演出用変数
    private ShakeCamera _shakeCamera = null;
    private Animator _animator = null;
    [SerializeField] string _appearEffectName = "TS_boss_appear";
    List<ManualRotation> _totemHeadList = new List<ManualRotation>();

    #endregion

    #region method

    /// <summary>
    /// 行動ループ処理
    /// </summary>
    private IEnumerator Run()
    {
        _action = eAction.TotemPushUp;
        eAction oldAction = _action;
        while (true)
        {
            // 行動ルーチン実行
            IEnumerator enumerator = RunAction();

            // 終わるまで待機する
            oldAction = _action;
            while (_action == oldAction)
            {
                // スタン状態なら一時停止
                if (IsStan)
                {
                    _animator.SetBool("IsStan", true);
                    StaticCoroutine.Instance.StopCoroutine(enumerator);

                    while (IsStan)
                    {
                        // スタン演出
                        yield return null;
                    }

                    _animator.SetBool("IsStan", false);
                    yield return new WaitForSeconds(1.0f);
                }

                yield return null;
            }

            // 一周してたらループさせる
            if(_action >= eAction.Max)
            {
                _action = eAction.TotemPushUp;
            }
        }
    }
    
    /// <summary>
    /// 行動開始処理
    /// </summary>
    private IEnumerator RunAction()
    {
        switch (_action)
        {
            case eAction.TotemPushUp:
                return StaticCoroutine.Instance.StartStaticCoroutine(TotemPushUp());

            case eAction.ChildTotemPushUp:
                return StaticCoroutine.Instance.StartStaticCoroutine(ChildTotemPushUp());

            case eAction.SpecialAtack:
                return StaticCoroutine.Instance.StartStaticCoroutine(SpecialAtack());

            default:
                break;
        }
        return null;
    }

    /// <summary>
    /// 突き上げ攻撃処理
    /// </summary>
    private IEnumerator TotemPushUp()
    {
        int amount = 1;
        float time = 0.0f;
        while (amount < _headAmount)
        {
            amount++;
            transform.position = RandomPos();
            transform.LookAt(PlayerManager.Instance.GetVerticalPos(transform.position));

            // 土煙を出す
            AppearEffect();
            time = 0.0f;
            while (time < 2.0f)
            {
                time += Time.deltaTime;
                yield return null;
            }

            time = 0.0f;
            _isAtack = true;
            AppearEffect();
            TotemRot(true, amount);
            Vector3 initPos = transform.position;
            while (time < amount)
            {
                transform.position = Vector3.Lerp(initPos, initPos + new Vector3(0, _oneBlockSize * amount, 0), time / amount);
                time += Time.deltaTime / _oneBlockUpSpeed;

                // 半分出たところで通常視点に戻す
                if (time >= amount / 2.0f)
                {
                    EnemyManager.Instance.Active = true;
                }
                
                yield return null;
            }
            _isAtack = false;

            // 待機
            float waitTime = 0.0f;
            while (waitTime < 2.0f)
            {
                waitTime += Time.deltaTime;
                yield return null;
            }
            EnemyManager.Instance.Active = false;

            // 潜る処理
            AppearEffect();
            TotemRot(false, amount);
            while (time > 0.0f)
            {
                transform.position -= new Vector3(0, _oneBlockSize * (Time.deltaTime / _oneBlockUpSpeed), 0);
                time -= Time.deltaTime / _oneBlockUpSpeed;
                yield return null;
            }
            
            // 待機処理
            time = 0.0f;
            while(time < 2.0f)
            {
                time += Time.deltaTime;
                yield return null;
            }
        }

        // 次の行動へ
        _action += 1;
    }

    /// <summary>
    /// 子分と同時突き上げ処理
    /// </summary>
    private IEnumerator ChildTotemPushUp()
    {
        float time = 0.0f;

        // 子分の突き上げ処理
        for (int i = 0; i < _childTotemAmount; i++)
        {
            _childTotemList[i].gameObject.SetActive(true);
            StaticCoroutine.Instance.StartStaticCoroutine(_childTotemList[i].PushUp(_oneBlockUpSpeed));

            time = 0.0f;
            _shakeCamera.Shake();
            while (time < (_oneBlockUpSpeed * 3.0f) * 0.75f)
            {
                time += Time.deltaTime;
                yield return null;
            }
        }

        // 本体突き上げ処理
        transform.position = RandomPos();
        transform.LookAt(PlayerManager.Instance.GetVerticalPos(transform.position));

        // 土煙を出す
        AppearEffect();
        time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        time = 0.0f;
        _isAtack = true;
        AppearEffect();
        TotemRot(true, _headAmount);
        Vector3 initPos = transform.position;
        while (time < _headAmount)
        {
            transform.position = Vector3.Lerp(initPos, initPos + new Vector3(0, _oneBlockSize * _headAmount, 0), time / _headAmount);
            time += Time.deltaTime / _oneBlockUpSpeed;

            // 半分出たところで通常視点に戻す
            if(time >= _headAmount / 2.0f)
            {
                EnemyManager.Instance.Active = true;
            }

            yield return null;
        }
        _isAtack = false;

        // 次の行動へ
        _action += 1;
    }

    /// <summary>
    /// 特殊攻撃処理
    /// </summary>
    private IEnumerator SpecialAtack()
    {
        // 待機
        float time = 0.0f;
        while (time < 2.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // 子分を潜らせる
        _shakeCamera.Shake();
        for (int i = 0; i < _childTotemAmount; i++)
        {
            StaticCoroutine.Instance.StartStaticCoroutine(_childTotemList[i].Dive(_oneBlockUpSpeed));
        }

        // 待機
        time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // 子分に特殊攻撃の実行を通知
        _shakeCamera.Shake();
        for (int i = 0; i < _childTotemAmount; i++)
        {
            StaticCoroutine.Instance.StartStaticCoroutine(_childTotemList[i].SpecialAtack(_oneBlockUpSpeed, _fallHeight));
        }

        // 待機
        while(true)
        {
            int cnt = 0;
            for(int i = 0; i < _childTotemAmount; i++)
            {
                if (_childTotemList[i].gameObject.active)
                    continue;

                cnt++;
            }

            if (cnt >= _childTotemAmount)
            {
                break;
            }
            yield return null;
        }

        // 本体も潜らせる
        time = 0.0f;
        AppearEffect();
        TotemRot(false, _headAmount);
        while (time < _headAmount)
        {
            transform.position -= new Vector3(0, _oneBlockSize * (Time.deltaTime / _oneBlockUpSpeed), 0);
            time += Time.deltaTime / _oneBlockUpSpeed;
            yield return null;
        }
        EnemyManager.Instance.Active = false;

        // 待機
        time = 0.0f;
        while (time < 2.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        // 次の行動へ
        _action += 1;
    }

    /// <summary>
    /// 突き上げエフェクト処理
    /// </summary>
    private void AppearEffect()
    {
        _shakeCamera.Shake();
        GameEffectManager.Instance.PlayOnHeightZero(_appearEffectName, transform.position);
    }

    /// <summary>
    /// トーテムの回転処理
    /// </summary>
    private void TotemRot(bool isUp, int headAmount)
    {
        float speed = _oneBlockUpSpeed * headAmount;
        Vector3 rotAmount = new Vector3(0, 360, 0) * headAmount;
        if(isUp)
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
    /// ランダムな座標を返す
    /// </summary>
    private Vector3 RandomPos()
    {
        float x = Random.Range(-StageData.FieldSize, StageData.FieldSize);
        float y = -_oneBlockSize * _headAmount;
        float z = Random.Range(-StageData.FieldSize, StageData.FieldSize);
        return new Vector3(x,y,z);
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        // トーテムの先端が見えてしまうので頭を一つ増やした状態で処理
        _headAmount += 1;

        // 必要コンポーネントの取得
        _rigidbody = GetComponent<Rigidbody>();
        _shakeCamera = Camera.main.GetComponent<ShakeCamera>();
        _animator = GetComponent<Animator>();

        // 子分トーテムの生成処理
        for (int i = 0; i < _childTotemAmount; i++)
        {
            ChildTotem instance = Instantiate(_childTotemPrefab).GetComponent<ChildTotem>();
            instance.gameObject.SetActive(false);
            _childTotemList.Add(instance);
        }

        // トーテムの顔ごとの回転処理用コンポーネントを取得
        for(int i = 0; i < transform.childCount; i++)
        {
            _totemHeadList.Add(transform.GetChild(i).GetComponent<ManualRotation>());
        }
    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {
        // 行動開始
        StaticCoroutine.Instance.StartStaticCoroutine(Run());
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player" && _isAtack)
        {
            col.gameObject.GetComponent<Player>().Damage();
        }
    }

    #endregion
}  