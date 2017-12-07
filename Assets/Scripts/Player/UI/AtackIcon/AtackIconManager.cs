// ---------------------------------------------------------
// AtackIconManager.cs
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
  
public class AtackIconManager : SingletonMonoBehaviour<AtackIconManager>
{
    #region define

    private const int IconAmount = 8;

    #endregion

    #region variable

    [SerializeField] float _rotSpeed = 0.5f;
    public bool IsChange { get; private set; }

    private ActionManager.eActionType _nowAction = 0;
    private AtackIcon[] _atackIconList = new AtackIcon[IconAmount];

    #endregion

    #region method

    /// <summary>
    /// UI回転処理
    /// </summary>
    public void Rot(bool isRightRot)
    {
        float rotAmount = 360.0f / IconAmount;
        if(isRightRot)
        {
            rotAmount *= -1;
        }

        StaticCoroutine.Instance.StartStaticCoroutine(RotRun(rotAmount));
    }

    private IEnumerator RotRun(float rotAmount)
    {
        // 選択アイコン変更
        ActionChange(rotAmount > 0);
        
        // 回転処理
        IsChange = false;
        float time = 0.0f;
        Vector3 startRot = transform.eulerAngles;
        Vector3 endRot = startRot + new Vector3(0, 0, rotAmount);

        while(time < 1.0f)
        {
            time += Time.deltaTime / _rotSpeed;
            transform.eulerAngles = Vector3.Lerp(startRot, endRot, time);
            yield return null;
        }
        IsChange = true;

        // まだ使えないアイコンなら飛ばす
        if((int)_nowAction > StageData.Instance.StageNumber)
        { 
            StaticCoroutine.Instance.StartStaticCoroutine(RotRun(rotAmount));
        }
    }

    /// <summary>
    /// 選択アイコン情報の変更処理
    /// </summary>
    private void ActionChange(bool isRight)
    {
        // 現在選択中のを縮小
        ScaleChange((int)_nowAction, false);

        // 選択アイコン変更処理
        if (isRight)
        {
            _nowAction++;

            if (_nowAction >= ActionManager.eActionType.Max)
            {
                _nowAction = 0;
            }
        }
        else
        {
            _nowAction--;

            if (_nowAction < 0)
            {
                _nowAction = ActionManager.eActionType.Max - 1;
            }
        }

        // 新しく選択したのを拡大
        ScaleChange((int)_nowAction, true);
    }

    /// <summary>
    /// アイコンの拡縮命令処理
    /// </summary>
    private void ScaleChange(int index, bool isAdd)
    {
        _atackIconList[index    ].ScaleChange(isAdd, _rotSpeed);
        _atackIconList[index + 4].ScaleChange(isAdd, _rotSpeed);
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        IsChange = true;

        for(int i = 0; i < IconAmount; i++)
        {
            _atackIconList[i] = transform.GetChild(i).GetComponent<AtackIcon>();
        }
    }

    #endregion
}  