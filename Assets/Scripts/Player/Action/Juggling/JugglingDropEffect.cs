// ---------------------------------------------------------
// JugglingDropEffect.cs
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

public class JugglingDropEffect : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    #endregion

    #region method

    /// <summary>
    /// 終了処理
    /// </summary>
    public void End()
    {
        StaticCoroutine.Instance.StartStaticCoroutine(EndRoutine());
    }

    private IEnumerator EndRoutine()
    {
        Vector3 initScale = transform.localScale;
        while (transform.localScale.x > 0)
        {
            transform.localScale -= initScale * (Time.deltaTime / 0.75f);

            if(transform.localScale.x < 0)
            {
                transform.localScale = Vector3.zero;
            }

            yield return null;
        }
        Destroy(gameObject);
    }

    #endregion
}