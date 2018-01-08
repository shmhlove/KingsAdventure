using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public partial class SHLoader
{
    #region Utility Functions
    void CoroutineToLoadProcess()
    {
        LoadCall();

        if (false == IsRemainLoadFiles())
            return;

        Single.Coroutine.NextFrame(CoroutineToLoadProcess);
    }
    
    void CoroutineToLoadPrograssEvent()
    {
        CallEventToPrograss();

        if (true == IsLoadDone())
            return;

        Single.Coroutine.NextFrame(CoroutineToLoadPrograssEvent);
    }

    void LoadCall()
    {
        var pDataInfo = m_pPrograss.DequeueWaitingDataInfo();
        if (null == pDataInfo)
            return;

        if (false == pDataInfo.IsLoadTrigger())
        {
            m_pPrograss.EnqueueWaitingDataInfo(pDataInfo);
            return;
        }
        
        pDataInfo.LoadCall(OnEventToLoadStart, OnEventToLoadDone);
    }

    void AddLoadDatum(List<Dictionary<string, SHLoadData>> pLoadDatum)
    {
        SHUtils.ForToList<Dictionary<string, SHLoadData>>(
        pLoadDatum, (dicLoadData) => m_pPrograss.AddLoadDatum(dicLoadData) );
    }

    void AddLoadEvent(EventHandler pDone, EventHandler pProgress)
    {
        if (null != pDone)
            EventToDone.Add(pDone);

        if (null != pProgress)
            EventToProgress.Add(pProgress);
    }

    float GetLoadPrograss()
    {
        // 로드할 파일이 없으면 100프로지~
        if (false == IsRemainLoadFiles())
            return 100.0f;

        float iProgress = 0.0f;

        // 로드 중인 파일의 진행율
        SHUtils.ForToDic<string, SHLoadStartInfo>(m_pPrograss.LoadingFiles, (pKey, pValue) => 
        {
            if (true == m_pPrograss.IsDone(pKey))
                return;

            iProgress += pValue.GetPrograss();
        });

        // 로드 완료된 파일의 진행율
        var pCountInfo         = m_pPrograss.GetCountInfo();
        float fCountGap        = SHMath.Divide(100.0f, pCountInfo.Value1);
        float fDonePercent     = (fCountGap * pCountInfo.Value2);
        float fProgressPercent = (fCountGap * iProgress);

        // 로드 완료된 파일의 진행율 + 로드 중인 파일의 진행율
        return (fDonePercent + fProgressPercent);
    }

    void OnEventToLoadStart(string strFileName, SHLoadStartInfo pData)
    {
        m_pPrograss.AddLoadStartInfo(strFileName, pData);
    }

    void OnEventToLoadDone(string strFileName, SHLoadEndInfo pData)
    {
        // CallEventToPrograss(m_pPrograss.SetLoadFinish(strFileName, pData.m_bIsSuccess));
        m_pPrograss.SetLoadFinish(strFileName, pData.m_bIsSuccess);
        CallEventToDone();
    }
    
    void CallEventToPrograss(SHLoadData pData)
    {
        var pEvent            = new SHLoadingInfo();

        // 로드 중인 파일리스트 만들기
        // public eDataType            m_eType;
        // public string               m_strFileName;
        // public bool                 m_bIsDone;
        // public eErrorCode           m_eErrorCode;

        pEvent.m_eType        = pData.m_eDataType;
        pEvent.m_strFileName  = pData.m_strName;

        pEvent.m_pCount       = m_pPrograss.GetCountInfo();
        pEvent.m_pTime        = m_pPrograss.GetLoadTime(pData.m_strName);
        pEvent.m_bIsSuccess   = pData.m_bIsSuccess;
        pEvent.m_bIsFail      = m_pPrograss.m_bIsFail;
        pEvent.m_fLoadPercent = GetLoadPrograss();
        
        EventToProgress.Callback<SHLoadingInfo>(this, pEvent);
    }

    void CallEventToDone()
    {
        var pEvent                  = new SHLoadingInfo();
        pEvent.m_bIsFail            = m_pPrograss.m_bIsFail;
        pEvent.m_pCount             = m_pPrograss.GetCountInfo();
        pEvent.m_pTime              = new SHPair<float, float>(m_pPrograss.GetLoadTime(), 0.0f);
        EventToDone.Callback<SHLoadingInfo>(this, pEvent);
        EventToDone.Clear();
    }
    
    #endregion


    #region Interface Functions
    // 로드가 완료되었는가?(성공/실패유무가 아님)
    public bool IsLoadDone()
    {
        return m_pPrograss.m_bIsDone;
    }

    // 특정 파일이 로드완료되었는가?(성공/실패유무가 아님)
    public bool IsLoadDone(string strFileName)
    {
        return m_pPrograss.IsDone(strFileName);
    }

    // 특정 타입이 로드완료되었는가?(성공/실패유무가 아님)
    public bool IsLoadDone(eDataType eType)
    {
        return m_pPrograss.IsDone(eType);
    }

    // 로드할 파일이 있는가?
    public bool IsRemainLoadFiles()
    {
        SHPair<int, int> pCountInfo = m_pPrograss.GetCountInfo();

        // TotalCount가 0이면 로드할 파일이 없다
        if (0 == pCountInfo.Value1)
            return false;

        // TotalCount와 CurrentCount가 같으면 로드할 파일이 없다
        if (pCountInfo.Value1 == pCountInfo.Value2)
            return false;

        return true;
    }
    #endregion
}