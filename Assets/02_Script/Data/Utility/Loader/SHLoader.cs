using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public partial class SHLoader
{
    #region Interface Functions
    public void Process(SHLoadData pLoadInfo,
        EventHandler pDone = null, EventHandler pProgress = null)
    {
        Process(new Dictionary<string, SHLoadData>()
        {
            { pLoadInfo.m_strName, pLoadInfo}
        }, 
        pDone, pProgress);
    }
    public void Process(Dictionary<string, SHLoadData> pLoadList,
        EventHandler pDone = null, EventHandler pProgress = null)
    {
        Process(new List<Dictionary<string, SHLoadData>>()
        {
            pLoadList
        }, 
        pDone, pProgress);
    }
    public void Process(List<Dictionary<string, SHLoadData>> pLoadList,
        EventHandler pDone = null, EventHandler pProgress = null)
    {
        Initialize();
        
        AddLoadDatum(pLoadList);
        AddLoadEvent((pDone + OnEventToDone), (pProgress + OnEventToPrograss));
        
        m_pPrograss.StartLoadTime();
        
        if (false == IsRemainLoadFiles())
        {
            CallEventToDone();
            return;
        }
        
        CoroutineToLoadProcess();
        CoroutineToLoadPrograssEvent();
    }
    #endregion


    #region Event Handler
    void OnEventToPrograss(object pSender, EventArgs vArgs)
    {
        var pInfo = Single.Event.GetArgs<SHLoadingInfo>(vArgs);
        if (null == pInfo)
            return;

        // 어싱크 프로그래스
        if (true == pInfo.m_bIsAsyncPrograss)
        {
            Debug.Log(string.Format("로드 진행상황 어싱크 체커(" +
                       "Percent:<color=yellow>{0}</color>, " +
                       "Count:<color=yellow>{1}/{2}</color>)",
                       pInfo.m_fPercent,
                       pInfo.m_pCount.Value2,
                       pInfo.m_pCount.Value1));
            return;
        }
        
        // 싱크 프로그래스
        if (false == pInfo.m_bIsSuccess)
        {
            Debug.LogError(string.Format("<color=red>데이터 로드실패</color>(" +
                            "Type:<color=yellow>{0}</color>, " +
                            "Percent:<color=yellow>{2}%</color>, " +
                            "현재Time:<color=yellow>{3}sec</color>, " +
                            "전체Time:<color=yellow>{4}sec</color>" +
                            "Name:<color=yellow>{1}</color>)",
                            pInfo.m_eType, pInfo.m_strFileName,
                            pInfo.m_fPercent,
                            SHMath.Round(pInfo.m_pTime.Value2, 3),
                            SHMath.Round(pInfo.m_pTime.Value1, 2)));
        }
        else
        {
            Debug.Log(string.Format("데이터 로드성공(" +
                            "Type:<color=yellow>{0}</color>, " +
                            "Percent:<color=yellow>{2}%</color>, " +
                            "현재Time:<color=yellow>{3}sec</color>, " +
                            "전체Time:<color=yellow>{4}sec</color>" +
                            "Name:<color=yellow>{1}</color>)",
                            pInfo.m_eType, pInfo.m_strFileName,
                            pInfo.m_fPercent,
                            SHMath.Round(pInfo.m_pTime.Value2, 3),
                            SHMath.Round(pInfo.m_pTime.Value1, 2)));
        }
    }

    void OnEventToDone(object pSender, EventArgs vArgs)
    {
        var pInfo = Single.Event.GetArgs<SHLoadingInfo>(vArgs);
        if (null == pInfo)
            return;

        Debug.LogFormat("<color=blue>데이터 로드 완료("+
                        "성공여부 : </color><color=yellow>{0}</color><color=blue>, " +
                        "로드카운트 : </color><color=yellow>{1}</color><color=blue>, " +
                        "로드시간 : </color><color=yellow>{2}sec</color><color=blue>)!!</color>",
                        (false == pInfo.m_bIsFail),
                        pInfo.m_pCount.Value2,
                        pInfo.m_pTime.Value1);
    }
    #endregion
}