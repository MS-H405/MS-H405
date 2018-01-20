// ---------------------------------------------------------
// HermitCrabRunSmoke.cs
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
  
public class HermitCrabRunSmoke : MonoBehaviour
{
    #region define

    #endregion

    #region variable

    private List<ParticleSystem> _runSmokes = new List<ParticleSystem>();

    #endregion

    #region method

    /// <summary>
    /// 土煙エフェクト生成処理
    /// </summary>
    private void RunSmoke(bool isWalk)
    {
        if (!isWalk)
        {
            foreach (ParticleSystem runSmoke in _runSmokes)
            {
                runSmoke.loop = false;
            }
            return;
        }

        foreach (ParticleSystem runSmoke in _runSmokes)
        {
            bool old = runSmoke.loop;
            runSmoke.loop = true;

            if (runSmoke.loop == old)
                return;

            runSmoke.Play();
        }
    }

    #endregion

    #region unity_event

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            ParticleSystem runSmoke = transform.GetChild(i).GetComponent<ParticleSystem>();
            _runSmokes.Add(runSmoke);
            runSmoke.Stop();
        }

        Vector3 oldPos = transform.position;
        Vector3 oldAngle = transform.eulerAngles;
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                bool isWalk = oldPos != transform.position || oldAngle != transform.eulerAngles;
                RunSmoke(isWalk);
                oldPos = transform.position;
                oldAngle = transform.eulerAngles;
            });
    }

    #endregion
}  