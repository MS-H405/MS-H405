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

    #endregion

    #region variable

    [SerializeField] ActionManager.eActionType _actionType;

    #endregion

    #region method

    /// <summary>
    /// 拡縮処理
    /// </summary>
    public void ScaleChange(bool isAdd, float speed)
    {
        StaticCoroutine.Instance.StartStaticCoroutine(ScaleChangeRun(isAdd, speed));
    }

    private IEnumerator ScaleChangeRun(bool isAdd, float speed)
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

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        if (StageData.Instance.StageNumber >= (int)_actionType)
            return;

        // TODO : 使えないアイコンの処理
        GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update ()
    {
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    #endregion
}  