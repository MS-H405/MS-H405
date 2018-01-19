// ---------------------------------------------------------
// HermitCrab.cs
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
  
public class HermitCrab : EnemyBase
{
    #region define

    private enum eAction
    {
        Wait = 0,
        Assault,
        RightAttack,
        LeftAttack,
        RollAtack,
        ChargeFire,
        RollFire,
        Max,
    };

    readonly float NearRange = 7.5f;        // Playerを近くと判定する距離
    readonly float WeakPercentage = 0.25f;  //

    #endregion

    #region variable
    
    private eAction _nowAction;             // 現在の行動を保持
    private bool _isNext = false;           // 次の行動へ行くか
    private float _nearTime = 0.0f;         // Playerが近くにいる時の継続時間
    
    private SphereCollider _bodyAttackCollider = null;

    // 行動用変数
    private BoxCollider _leftScissors  = null;
    private BoxCollider _rightScissors = null;
    private List<GameObject> _pipeList = new List<GameObject>();
    private List<ParticleSystem> _pipeFireList = new List<ParticleSystem>();
    [SerializeField] GameObject _leftScissorsEffect = null;
    [SerializeField] GameObject _rightScissorsEffect = null;
    [SerializeField] GameObject _rollAttackEffect = null;
    [SerializeField] GameObject _chargeFireEffect = null;
    [SerializeField] GameObject _rollFireEffect = null;

    private EffekseerEmitter _defenseEffect = null;
    private EffekseerEmitter _invincibleEffect = null;

    #endregion

    #region method

    /// <summary>
    /// 行動ループ処理
    /// </summary>
    private IEnumerator Run()
    {
        _nowAction = eAction.LeftAttack;
        eAction oldAction = _nowAction;
        while (true)
        {
            // 行動ルーチン実行
            _nowAction = SelectAction();
            IEnumerator enumerator = RunAction();
            _isNext = false;

            // 終わるまで待機する
            while (!_isNext)
            {
                // スタン状態なら一時停止
                if (IsStan)
                {
                    StaticCoroutine.Instance.StopCoroutine(enumerator);

                    while (IsStan)
                    {
                        // スタン演出
                        yield return null;
                    }

                    yield return new WaitForSeconds(1.0f);
                }

                yield return null;
            }
        }
    }

    /// <summary>
    /// 行動選択処理
    /// </summary>
    private eAction SelectAction()
    {
        switch (_nowAction)
        {
            case eAction.Wait:

                // --- DEBUG ---
                if (_isDebug)
                {
                    if (Input.GetKey(KeyCode.H))
                    {
                        return eAction.Assault;
                    }
                    if (Input.GetKey(KeyCode.J))
                    {
                        return eAction.RollAtack;
                    }
                    if (Input.GetKey(KeyCode.K))
                    {
                        return eAction.ChargeFire;
                    }
                    if (Input.GetKey(KeyCode.L))
                    {
                        return eAction.RollFire;
                    }
                    if (Input.GetKey(KeyCode.N))
                    {
                        return eAction.RightAttack;
                    }
                    if (Input.GetKey(KeyCode.M))
                    {
                        return eAction.LeftAttack;
                    }
                    return eAction.Wait;
                }
               
                if (_nearTime > 5.0f)
                {
                    _nearTime = 0.0f;
                    return eAction.RollAtack;
                }

                int rand = Random.Range(0, 5);
                if (rand != 0)
                {
                    return eAction.Assault;
                }
                else if (rand < 3)
                {
                    return eAction.ChargeFire;
                }
                else
                {
                    return eAction.RollFire;
                }

            case eAction.Assault:
                return (eAction)Random.Range(2, 4); // 左or右攻撃か回転攻撃を出す

            case eAction.RightAttack:
                return eAction.Wait;

            case eAction.LeftAttack:
                return eAction.Wait;

            case eAction.RollAtack:
                return eAction.Wait;

            case eAction.ChargeFire:
                return eAction.Wait;

            case eAction.RollFire:
                return eAction.Wait;

            default:
                break;
        }
        return eAction.Wait;
    }

    /// <summary>
    /// 行動開始処理
    /// </summary>
    private IEnumerator RunAction()
    {
        switch (_nowAction)
        {
            case eAction.Wait:
                return StaticCoroutine.Instance.StartStaticCoroutine(Wait());

            case eAction.Assault:
                return StaticCoroutine.Instance.StartStaticCoroutine(Assault());

            case eAction.RightAttack:
            case eAction.LeftAttack:
                return StaticCoroutine.Instance.StartStaticCoroutine(ScissorAttack(_nowAction));

            case eAction.RollAtack:
                return StaticCoroutine.Instance.StartStaticCoroutine(RollAttack());

            case eAction.ChargeFire:
                return StaticCoroutine.Instance.StartStaticCoroutine(ChargeFire());

            case eAction.RollFire:
                return StaticCoroutine.Instance.StartStaticCoroutine(RollFire());

            default:
                break;
        }
        return null;
    }

    #endregion

    #region action_method

    /// <summary>
    /// 待機処理（Playerのターン）
    /// </summary>
    private IEnumerator Wait()
    {
        _animator.SetBool("Walk", false);

        float time = 0.0f;
        float waitTime = _isDebug ? 0.1f : Random.Range(1.0f, 5.0f);
        while (time < waitTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        _isNext = true;
    }

    /// <summary>
    /// 突進攻撃処理
    /// </summary>
    private IEnumerator Assault()
    {
        // 必要Player情報の取得
        float time = 0.0f;
        float speed = 0.0f;
        _animator.SetBool("Walk", true);
        _animator.speed = 1.5f;

        Vector3 startRot = transform.eulerAngles;
        transform.LookAt(PlayerManager.Instance.GetVerticalPos(transform.position));
        Vector3 targetRot = transform.eulerAngles;
        transform.eulerAngles = startRot;

        Vector3 startPos = transform.position;
        Vector3 targetPos = PlayerManager.Instance.Player.transform.position;

        // 無駄な回転量が出ないようにする
        if (targetRot.y - startRot.y > 180.0f)
        {
            targetRot.y -= 360.0f;
        }
        else if (targetRot.y - startRot.y < -180.0f)
        {
            targetRot.y += 360.0f;
        }

        speed = Mathf.Abs(startRot.y - targetRot.y) / 60.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime / speed;
            if (time > 1.0f) time = 1.0f;
            transform.eulerAngles = Vector3.Lerp(startRot, targetRot, time);
            yield return null;
        }
        
        time = 0.0f;
        _animator.speed = 3.0f;
        speed = Vector3.Distance(startPos, targetPos) / 20.0f;
        while (time < 0.6f)
        {
            time += Time.deltaTime / speed;
            if (time > 0.6f) time = 0.6f;
            transform.position = Vector3.Lerp(startPos, targetPos, time);
            yield return null;
        }

        _isNext = true;
        _animator.speed = 1.0f;
        _animator.SetBool("Walk", false);
    }

    /// <summary>
    /// はさみ攻撃処理
    /// </summary>
    private IEnumerator ScissorAttack(eAction action)
    {
        switch(action)
        {
            case eAction.RightAttack:
                _animator.SetTrigger("RightAttack");
                break;

            case eAction.LeftAttack:
                _animator.SetTrigger("LeftAttack");
                break;

            default:
                break;
        }
        _animator.speed = 1.25f;

        float time = 0.0f;
        while(time < (0.5f / _animator.speed))
        {
            time += Time.deltaTime;
            yield return null;
        }

        GameObject effect = null;
        switch (action)
        {
            case eAction.RightAttack:
                _rightScissors.enabled = true;
                effect = Instantiate(_rightScissorsEffect);
                effect.transform.SetParent(_rightScissors.transform.Find("RightS"));
                break;

            case eAction.LeftAttack:
                _leftScissors.enabled = true;
                effect = Instantiate(_leftScissorsEffect);
                effect.transform.SetParent(_leftScissors.transform.Find("LehtS"));
                break;

            default:
                break;
        }
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localEulerAngles = Vector3.zero;

        SoundManager.Instance.PlaySE(SoundManager.eSeValue.Bagpipe_Scissor);
        StaticCoroutine.Instance.StartStaticCoroutine(ActionEndWait());
        yield break;
    }

    /// <summary>
    /// 回転攻撃処理
    /// </summary>
    private IEnumerator RollAttack()
    {
        PlayDefenseEffect();
        _animator.SetTrigger("RollAttack");

        float time = 0.0f;
        while (time < 2.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        _rightScissors.enabled = true;
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.Bagpipe_Roll);
        StaticCoroutine.Instance.StartStaticCoroutine(ActionEndWait());

        time = 0.0f;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        Instantiate(_rollAttackEffect, transform.position, _rollAttackEffect.transform.rotation);

        time = 0.0f;
        while (time < 0.75f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        _rightScissors.enabled = false;
    }

    /// <summary>
    /// チャージファイアー攻撃処理
    /// </summary>
    private IEnumerator ChargeFire()
    {
        PlayDefenseEffect();
        _animator.SetTrigger("ChargeFire");
        StaticCoroutine.Instance.StartStaticCoroutine(ActionEndWait());

        Vector3 startRot = transform.eulerAngles;
        Vector3 targetRot = transform.eulerAngles;
        targetRot.y = PlayerManager.Instance.Player.transform.eulerAngles.y;

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
        while (time < 2.33f)
        {
            time += Time.deltaTime;
            transform.eulerAngles = Vector3.Lerp(startRot, targetRot, time / 2.33f);
            yield return null;
        }

        foreach(GameObject pipe in _pipeList)
        {
            GameObject effect = Instantiate(_chargeFireEffect);
            effect.transform.SetParent(pipe.transform);
            effect.transform.localPosition = Vector3.zero;
            effect.transform.localEulerAngles = new Vector3(-90, 0, 0);
        }
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.Bagpipe_FireShot);
    }

    /// <summary>
    /// 回転ファイアー攻撃処理
    /// </summary>
    private IEnumerator RollFire()
    {
        PlayDefenseEffect();
        _animator.SetBool("RollFire", true);

        StaticCoroutine.Instance.StartStaticCoroutine(ActionEndWait());

        // 殻にこもるまで待機
        float time = 0.0f;
        while (time < 2.5f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        SoundManager.Instance.PlayBGM(SoundManager.eBgmValue.Bagpipe_Burst);
        Vector3 startPos = transform.position;
        Vector3 targetPos = Vector3.Lerp(startPos, PlayerManager.Instance.GetVerticalPos(startPos), 0.7f);

        // 回転
        time = 0.0f;
        _bodyAttackCollider.enabled = true;
        while (time < 3.0f)
        {
            if(PlayerManager.Instance.IsGround)
            {
                targetPos = Vector3.Lerp(startPos, PlayerManager.Instance.GetVerticalPos(startPos), 0.7f);
            }

            transform.position = Vector3.Lerp(startPos, targetPos, time / 3.0f);
            transform.eulerAngles += new Vector3(0, 360 * Time.deltaTime, 0);
            time += Time.deltaTime;

            foreach (ParticleSystem pipeFire in _pipeFireList)
            {
                string pipeName = pipeFire.transform.parent.name;
                if (!pipeName.Contains("B1") && !pipeName.Contains("B3") &&
                    !pipeName.Contains("B4") && !pipeName.Contains("B7"))
                {
                    continue;
                }

                pipeFire.Emit(Mathf.RoundToInt(100 * Time.deltaTime + 0.5f));
            }
            yield return null;
        }

        _bodyAttackCollider.enabled = false;
        SoundManager.Instance.StopBGM(SoundManager.eBgmValue.Bagpipe_Burst);
        _animator.SetBool("RollFire", false);
    }

    /// <summary>
    /// 行動終了待ち処理
    /// </summary>
    private IEnumerator ActionEndWait()
    {
        AnimatorStateInfo animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        while(animStateInfo.IsName("Base.Idle") || animStateInfo.IsName("Base.Walk"))
        {
            animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        while (!animStateInfo.IsName("Base.Idle"))
        {
            animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        _isNext = true;
        IsInvincible = false;
        _leftScissors.enabled = false;
        _rightScissors.enabled = false;
        _animator.speed = 1.0f;
    }

    /// <summary>
    /// 防御結界表示処理
    /// </summary>
    private void PlayDefenseEffect()
    {
        float time = 0.0f;
        _defenseEffect.Play();
        IsInvincible = true;

        var disposable = new SingleAssignmentDisposable();
        disposable.Disposable = this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (IsInvincible)
                {
                    time += Time.deltaTime;
                    if (time >= 0.5f)
                    {
                        _defenseEffect.paused = true;
                    }
                }
                else
                {
                    _defenseEffect.paused = false;
                    disposable.Dispose();
                }
            });
    }

    /// <summary>
    /// 無敵時エフェクト再生処理
    /// </summary>
    protected override void InvincibleEffect()
    {
        _invincibleEffect.Play();
    }

    /// <summary>
    /// 各自のスタンエフェクト再生処理
    /// </summary>
    protected override IEnumerator StanEffectUnique()
    {
        float time = 0.0f;
        while(time < 0.75f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        GameEffectManager.Instance.Play("HermitStan", transform.position);
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private new void Awake()
    {
        base.Awake();

        // はさみのColliderを取得
        List<GameObject> allChild = GetAllChildren.GetAll(gameObject);
        _leftScissors = allChild.Where(_ => _.name == "LeftScissors").FirstOrDefault().GetComponent<BoxCollider>();
        _leftScissors.enabled = false;
        _rightScissors = allChild.Where(_ => _.name == "RightScissors").FirstOrDefault().GetComponent<BoxCollider>();
        _rightScissors.enabled = false;
        _pipeList = allChild.Where(_ => _.name.Contains("pipe")).ToList();

        // 結界エフェクトを取得
        _defenseEffect = allChild.Where(_ => _.name == "DefenseEffect").FirstOrDefault().GetComponent<EffekseerEmitter>();
        _invincibleEffect = allChild.Where(_ => _.name == "InvincibleEffect").FirstOrDefault().GetComponent<EffekseerEmitter>();

        // パイプから出る炎を作成
        foreach (GameObject pipe in _pipeList)
        {
            GameObject effect = Instantiate(_rollFireEffect, pipe.transform.position, pipe.transform.rotation);
            effect.transform.SetParent(pipe.transform);
            effect.transform.localEulerAngles -= new Vector3(90, 0, 0);
            effect.name += " : " + pipe.name;
            _pipeFireList.Add(pipe.transform.GetChild(0).GetComponentInChildren<ParticleSystem>());
        }

        // ボディ攻撃判定の初期化
        _bodyAttackCollider = GetComponent<SphereCollider>();
        _bodyAttackCollider.enabled = false;
    }

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start ()
    {
        StaticCoroutine.Instance.StartStaticCoroutine(Run());
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private new void Update ()
    {
        base.Update();

        float distance = Vector3.Distance(transform.position, PlayerManager.Instance.Player.transform.position);
        if(distance <= NearRange)
        {
            _nearTime += Time.deltaTime;
        }
        else
        {
            _nearTime = 0.0f;
        }

        if (_animator.GetBool("Walk"))
        {
            SoundManager.Instance.PlayBGM(SoundManager.eBgmValue.Bagpipe_Walk);
        }
        else
        {
            SoundManager.Instance.StopBGM(SoundManager.eBgmValue.Bagpipe_Walk);
        }
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            col.GetComponent<Player>().Damage();
        }
    }

    #endregion
}