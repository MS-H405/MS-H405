// ---------------------------------------------------------
// GetAciton.cs
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
  
public class GetAction : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    [SerializeField] Sprite _changeSprite = null;
    [SerializeField] float _upHeight = 0.0f;
    [SerializeField] float _rotAmount = 1.0f;
    [SerializeField] float _speed = 1.0f;
    [SerializeField] float _windowWait = 1.0f;

    #endregion

    #region method

    /// <summary>
    /// 実行処理
    /// </summary>
    private IEnumerator Run()
    {
        Image image = GetComponent<Image>();

        float time = 0.0f;
        image.color = new Color(1,1,1,0);
        while(time < 1.0f)
        {
            image.color += new Color(0, 0, 0, 1) * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }

        time = 0.0f;
        Vector3 min = transform.position;
        Vector3 max = transform.position + new Vector3(0, _upHeight, 0);
        SoundManager.Instance.PlaySE(SoundManager.eSeValue.UI_ChangeBurst);

        // 上にくるくると回転
        while (time < 1.0f)
        {
            transform.position = Vector3.Lerp(min, max, time);
            transform.eulerAngles += new Vector3(0, (360 * _rotAmount) * (Time.deltaTime / _speed), 0);
            time += Time.deltaTime / _speed;
            yield return null;
        }
        transform.eulerAngles = new Vector3(0, 0, 0);

        // 頂点到達でテクスチャ差し替え
        image.sprite = _changeSprite;

        // 下にくるくると回転
        time = 0.0f;
        while (time < 1.0f)
        {
            transform.position = Vector3.Lerp(max, min, time);
            transform.eulerAngles += new Vector3(0, (360 * _rotAmount) * (Time.deltaTime / _speed), 0);
            time += Time.deltaTime / _speed;
            yield return null;
        }
        transform.eulerAngles = new Vector3(0, 0, 0);
        transform.position = min;

        // Window表示
        yield return new WaitForSeconds(_windowWait);
        transform.parent.Find("Window").gameObject.SetActive(true);
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {
        StartCoroutine(Run());
    }

    #endregion
}  