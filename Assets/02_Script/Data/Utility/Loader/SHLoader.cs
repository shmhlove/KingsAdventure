using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public partial class SHLoader
{
    #region Interface Functions
    public void LoadStart(SHLoadData pLoadInfo,
        EventHandler pComplate = null, EventHandler pProgress = null, EventHandler pError = null)
    {
        var dicInfo = new Dictionary<string, SHLoadData>();
        dicInfo.Add(pLoadInfo.m_strName, pLoadInfo);
        LoadStart(dicInfo, pComplate, pProgress, pError);
    }
    public void LoadStart(Dictionary<string, SHLoadData> pLoadList,
        EventHandler pComplate = null, EventHandler pProgress = null, EventHandler pError = null)
    {
        LoadStart(new List<Dictionary<string, SHLoadData>>() { pLoadList }, pComplate, pProgress, pError);
    }
    public void LoadStart(List<Dictionary<string, SHLoadData>> pLoadList,
        EventHandler pComplate = null, EventHandler pProgress = null, EventHandler pError = null)
    {
        // 로더 초기화
        Initialize();
        
        // 로드 리스트 추가
        AddLoadList(pLoadList);

        // 로드 이벤트 추가
        pComplate = pComplate + OnEventToComplate;
        pProgress = pProgress + OnEventToPrograss;
        AddLoadEvent(pComplate, pProgress, pError);

        // 로드 타이머 시작
        m_pPrograss.StartLoadTime();

        // 예외처리 : 로드할 리스트가 있는가?
        if (false == IsRemainLoadFiles())
        {
            CallEventToComplate();
            return;
        }
        
        // 어싱크 프로그래스 체크시작
        CoroutineToAsyncPrograss();

        // 로드명령시작
        Single.Coroutine.NextFrame(CoroutineToLoad);

        // 쓰레드로 로드 : 생성한 쓰레드에서 유니티 함수가 콜되면 Error발생
        //Thread pThread = new Thread(new ThreadStart(ThreadToLoad));
        //pThread.Start();
    }
    #endregion


    #region Event Handler
    void OnEventToPrograss(object pSender, EventArgs vArgs)
    {
        var pInfo = Single.Event.GetArgs<SHLoadEvent>(vArgs);
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

    void OnEventToComplate(object pSender, EventArgs vArgs)
    {
        var pInfo = Single.Event.GetArgs<SHLoadEvent>(vArgs);
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