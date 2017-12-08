// ---------------------------------------------------------
// JugglingAtack.cs
// 概要 : 攻撃したら敵にまっすぐ飛んでく。あたったら跳ね返る（ランダム）。キャッチしたら速度上げる
// 作成者: Shota_Obora
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;

public class JugglingAtack : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    static private int _nowJugglingAmount = 0;
    static public int NowJugglingAmount { get { return _nowJugglingAmount; } }
    static private float _commonAtackSpeed = 1.0f;  // 共有する攻撃スピード
    private readonly float MaxAtackSpeed = 2.0f;    // 最大スピード

    private GameObject _targetObj = null;           // 
    private float _atackSpeed = 0.0f;               // 攻撃スピード

    private bool _isReflect = false;                // 反射判定
    private bool _isCatch = false;                  // このオブジェクトの生存判定

    [SerializeField] GameObject _dropPointEffect = null;

    #endregion

    #region method

    public void Run(GameObject target)
    {
        // 使用中の個数を増加
        _nowJugglingAmount++;

        // 初期化処理
        _atackSpeed = 10.0f * _commonAtackSpeed;
        Vector3 initPos = transform.position;
        initPos.y = 1.0f;
        transform.position = initPos;
        if (target)
        {
            _targetObj = target;
            Vector3 lookPos = _targetObj.transform.position;
            lookPos.y = 1.0f;
            transform.LookAt(lookPos);
        }

        // アクション実行
        StaticCoroutine.Instance.StartStaticCoroutine(ActionFlow());
    }

    /// <summary>
    /// 攻撃命令から地面に落ちるまでの処理フロー
    /// </summary>
    private IEnumerator ActionFlow()
    {
        Vector3 startPos = transform.position; // 投げてから当たるまでの距離を計算するため
        float atackTime = 0.0f;

        // 敵にあたって反射するか地形外に行くまで直進
        while (!_isReflect)
        {
            Vector3 moveAmount = transform.forward * _atackSpeed * Time.deltaTime;
            transform.position += moveAmount;

            atackTime += Time.deltaTime;
            yield return null;
        }

        // 反射したので適当な位置を落下地点として設定
        Vector3 dropPoint = startPos;   // TODO : ランダム算出を実装予定
        dropPoint.y = 0.0f;
        GameObject effect = Instantiate(_dropPointEffect, dropPoint, Quaternion.identity);

        // 跳ね返り処理
        BezierCurve.tBez bez = new BezierCurve.tBez();  // 曲線移動のためベジエ曲線を使用
        bez.start = transform.position;
        bez.middle = Vector3.Lerp(transform.position, dropPoint, 0.5f) + new Vector3(0,atackTime * 15.0f,0);
        bez.end = dropPoint;

        while (1.0f > bez.time && !_isCatch)
        {
            transform.position = BezierCurve.CulcBez(bez, true);
            bez.time += Time.deltaTime * (0.5f / atackTime);
            yield return null;
        }
        
        // キャッチ判定
        if (_isCatch)
        {
            // 速度アップ
            if (_commonAtackSpeed < MaxAtackSpeed)
            {
                _commonAtackSpeed += 0.1f;
            }
            // 次生成
            GameObject obj = Instantiate(gameObject, startPos, transform.rotation);
            obj.GetComponent<JugglingAtack>().Run(EnemyManager.Instance.BossEnemy);
            Debug.Log("キャッチ！");
        }
        else
        {
            // 速度初期化
            _commonAtackSpeed = 1.0f;
            Debug.Log("落とした！");
        }

        // 破棄処理
        Destroy(effect);
        PinDestroy(!_isCatch);
    }

    /// <summary>
    /// ピンのリロード処理
    /// </summary>
    private IEnumerator Reload()
    {
        float time = 0.0f;
        while(time < 4.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        _nowJugglingAmount--;
    }

    /// <summary>
    /// 破棄処理
    /// 引数 : すぐにピンを補充しない場合Trueを入れる
    /// </summary>
    private void PinDestroy(bool isReload)
    {
        if (isReload)
        {
            StaticCoroutine.Instance.StartStaticCoroutine(Reload());
        }
        else
        {
            _nowJugglingAmount--;
        }
        Destroy(gameObject);
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 当たり判定
    /// </summary>
    private void OnTriggerEnter (Collider col)
    {
        // 敵にあたった場合、ダメージ処理をして跳ね返す
        if (col.tag == "Enemy")
        {
            if (_isReflect)
                return;

            GameObject obj = col.gameObject;
            while(obj.transform.parent)
            {
                obj = obj.transform.parent.gameObject;
            }
            obj.GetComponent<EnemyBase>().Damage();
            _isReflect = true;
            return;
        }

        // Field外に行ってしまったら跳ね返す
        if (col.tag == "Field")
        {
            _isReflect = true;
            return;
        }

        // 跳ね返り中のピンにPlayerが触れたらキャッチ判定
        if (col.tag == "Player" && _isReflect)
        {
            _isCatch = true;
            transform.rotation = col.transform.rotation;
            return;
        }
    }

    #endregion
}  