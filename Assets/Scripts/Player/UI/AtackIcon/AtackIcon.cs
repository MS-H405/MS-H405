// ---------------------------------------------------------
// AtackIcon.cs
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
  
public class AtackIcon : MonoBehaviour
{
    #region define

    private readonly Vector3 MinSize = new Vector3(1.0f, 1.0f, 1.0f);
    private readonly Vector3 MaxSize = new Vector3(2.0f, 2.0f, 2.0f);

    private readonly float SpecialTime = 0.5f;
    private readonly float SpecialUpHeight = 210.0f;
    private readonly int SpecialRotAmount = 2;

    #endregion

    #region variable

    [SerializeField] ActionManager.eActionType _actionType;

    // 演出用
    private Image _image = null;
    private Sprite _defaultSprite = null;

    #endregion

    #region method

    /// <summary>
    /// 拡縮処理
    /// </summary>
    public void ChangeScale(bool isAdd, float speed)
    {
        StaticCoroutine.Instance.StartStaticCoroutine(ChangeScaleRun(isAdd, speed));
    }

    private IEnumerator ChangeScaleRun(bool isAdd, float speed)
    {
        float time = 0.0f;
        Vector3 startSize = transform.localScale;
        Vector3 endSize = isAdd ? MaxSize : MinSize;

        while(time < 1.0f)
        {
            time += Time.deltaTime / speed;
            transform.localScale = Vector3.Lerp(startSize, endSize, time);
            yield return null;
        }
    }

    /// <summary>
    /// 特殊アイコン変更処理
    /// </summary>
    public void ChangeSpecialIcon(bool isStan)
    {
        // コルーチン実行
        StaticCoroutine.Instance.StartStaticCoroutine(ChangeSpecialIconRun(isStan));
    }

    private IEnumerator ChangeSpecialIconRun(bool isStan)
    {
        if(isStan)
        { 
            float time = 0.0f;
            Vector3 min = transform.position;
            Vector3 max = transform.position + new Vector3(0, SpecialUpHeight, 0);

            // 上にくるくると回転
            while (time < 1.0f)
            {
                transform.position = Vector3.Lerp(min, max, time);
                transform.eulerAngles += new Vector3(0, (360 * SpecialRotAmount) * (Time.deltaTime / SpecialTime), 0);
                time += Time.deltaTime / SpecialTime;
                yield return null;
            }
            transform.eulerAngles = new Vector3(0, 0, 0);

            // 頂点到達でテクスチャ差し替え
            _image.sprite = AtackIconManager.Instance.SpecialIconSprite;

            // 下にくるくると回転
            time = 0.0f;
            while (time < 1.0f)
            {
                transform.position = Vector3.Lerp(max, min, time);
                transform.eulerAngles += new Vector3(0, (360 * SpecialRotAmount) * (Time.deltaTime / SpecialTime), 0);
                time += Time.deltaTime / SpecialTime;
                yield return null;
            }
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            // コルーチン再開時に差し替えるため
            yield return new WaitForSeconds(100.0f);

            // 戻すときはムービー中にそのまま差し替え
            _image.sprite = _defaultSprite;
        }
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        // SpriteやColorの変更があるため初期化時に取得
        _image = GetComponent<Image>();
        _defaultSprite = _image.sprite;

        if (StageData.Instance.StageNumber >= (int)_actionType)
            return;

        // TODO : 使えないアイコンの処理
        _image.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update ()
    {
        Vector3 angle = transform.eulerAngles;
        angle.z = 0.0f;
        transform.eulerAngles = angle;
    }

    #endregion
}  