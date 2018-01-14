// ---------------------------------------------------------
// EnemyBase.cs
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
  
public class EnemyBase : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    [SerializeField] protected bool _isDebug = true;              // Debugモード管理フラグ

    [SerializeField] int _mainHp = 0;                   // 死亡までの敵HP

    protected int _nowStanHp = 0;                       // 気絶までの現在HP
    [SerializeField] int _stanHP = 0;                   // 気絶までの初期HP
    public bool IsStan { get; protected set; }          // 気絶フラグ

    public bool IsInvincible { get; protected set; }    // 無敵フラグ

    protected Animator _animator = null;
    private bool _isDamage = false;
    [SerializeField] List<Material> _matList = new List<Material>();
    //[SerializeField] Material _material = null;

    #endregion

    #region method

    /// <summary>
    /// ダメージ処理
    /// </summary>
    public void Damage(int amount = 1)
    {
        if (IsStan)
            return;

        if(IsInvincible)
        {
            InvincibleEffect();
            return;
        }

        if(_nowStanHp <= 0)
        {
            _nowStanHp = _stanHP;
        }

        _nowStanHp -= amount;
        StaticCoroutine.Instance.StartStaticCoroutine(DamageEffect());

        if (_nowStanHp > 0)
            return;

        // スタン状態にする
        IsStan = true;
        _animator.SetBool("Stan", true);
        StaticCoroutine.Instance.StartStaticCoroutine(StanEffect());
    }

    /// <summary>
    /// 必殺技のダメージ処理
    /// </summary>
    public void SpecialDamage()
    {
        _mainHp--;
        IsStan = false;
        _animator.SetBool("Stan", false);
    }

    /// <summary>
    /// 死亡判定
    /// </summary>
    public bool Death()
    {
        return _mainHp <= 0;
    }

    /// <summary>
    /// 無敵時エフェクト再生処理
    /// </summary>
    protected virtual void InvincibleEffect()
    {

    }

    /// <summary>
    /// ダメージエフェクト再生処理
    /// </summary>
    private IEnumerator DamageEffect()
    {
        if (_isDamage)
            yield break;

        _isDamage = true;
        float time = 0.0f;
        Color nowColor = Color.white;

        while (time < 1.0f)
        {
            time += Time.deltaTime * 5.0f;
            nowColor = Color.Lerp(Color.white, Color.red, time);

            foreach (Material mat in _matList)
            {
                mat.color = nowColor;
            }
            yield return null;
        }

        time = 0.0f;
        while(time < 1.0f)
        {
            time += Time.deltaTime * 5.0f;
            nowColor = Color.Lerp(Color.red, Color.white, time);

            foreach (Material mat in _matList)
            {
                mat.color = nowColor;
            }
            yield return null;
        }

        _isDamage = false;
    }

    /// <summary>
    /// スタンエフェクト再生処理
    /// </summary>
    private IEnumerator StanEffect()
    {
        while (_isDamage)
        {
            yield return null;
        }

        _isDamage = true;
        while (IsStan)
        {
            float time = 0.0f;
            Color nowColor = Color.white;

            while (time < 1.0f && IsStan)
            {
                time += Time.deltaTime * 5.0f;
                nowColor = Color.Lerp(Color.white, Color.red, time);

                foreach (Material mat in _matList)
                {
                    mat.color = nowColor;
                }
                yield return null;
            }

            time = 0.0f;
            while (time < 1.0f && IsStan)
            {
                time += Time.deltaTime * 5.0f;
                nowColor = Color.Lerp(Color.red, Color.white, time);

                foreach (Material mat in _matList)
                {
                    mat.color = nowColor;
                }
                yield return null;
            }
        }

        foreach (Material mat in _matList)
        {
            mat.color = Color.white;
        }

        _isDamage = false;
    }

    #endregion

    #region unity_event     

    /// <summary>
    /// 初期化処理
    /// </summary>
    protected void Awake()
    {
        IsStan = false;
        IsInvincible = false;
        _animator = GetComponent<Animator>();

        foreach (Material mat in _matList)
        {
            mat.color = Color.white;
        }
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    protected void Update()
    {
        // DEBUG : デバッグコマンド 
        if (Input.GetKeyDown(KeyCode.Q))
        {
            IsStan = true;
            _animator.SetBool("Stan", true);
            StaticCoroutine.Instance.StartStaticCoroutine(StanEffect());
        }
    }

    #endregion
}