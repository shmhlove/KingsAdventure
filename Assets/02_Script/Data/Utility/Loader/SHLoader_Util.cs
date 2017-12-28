using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public partial class SHLoader
{
    #region Utility Functions
    void CoroutineToLoad()
    {
        // 로드 실행(LoadCall()의 반환값 false의 의미는 Load함수 호출이 완료되었다는 의미)
        if (false == LoadCall())
            return;

        Single.Coroutine.WaitTime(CoroutineToLoad, 0.05f);
    }

    void ThreadToLoad()
    {
        while (true == LoadCall()) ;
    }

    void CoroutineToAsyncPrograss()
    {
        CallEventToAsyncPrograss();

        if (false == IsReMainLoadFiles())
            return;

        if (true == IsLoadDone())
            return;

        Single.Coroutine.WaitTime(CoroutineToAsyncPrograss, 0.2f);
    }

    bool LoadCall()
    {
        var pData = m_pPrograss.GetLoadData();
        if (null == pData)
            return false;

        if (false == pData.m_pTriggerLoadCall())
        {
            m_pPrograss.SetLoadData(pData);
            return true;
        }

        pData.m_pLoadFunc(pData, OnEventToLoadStart, OnEventToLoadDone);
        return true;
    }

    void AddLoadList(List<Dictionary<string, SHLoadData>> pLoadList)
    {
        SHUtils.ForToList<Dictionary<string, SHLoadData>>(
        pLoadList, (dicLoadList) =>
        {
            m_pPrograss.AddLoadInfo(dicLoadList);
        });
    }

    void AddLoadEvent(EventHandler pComplate, EventHandler pProgress, EventHandler pError)
    {
        if (null != pComplate)
            EventToComplate.Add(pComplate);

        if (null != pProgress)
            EventToProgress.Add(pProgress);

        if (null != pError)
            EventToError.Add(pError);
    }

    float GetLoadPrograss()
    {
        // 로드할 파일이 없으면 100프로지~
        if (false == IsReMainLoadFiles())
            return 100.0f;

        float iProgress = 0.0f;
        SHUtils.ForToDic<string, SHLoadStartInfo>(m_pPrograss.LoadingFiles, (pKey, pValue) => 
        {
            if (true == m_pPrograss.IsDone(pKey))
                return;

            iProgress += pValue.GetPrograss();
        });

        SHPair<int, int> pCountInfo = m_pPrograss.GetCountInfo();
        float fCountGap         = SHMath.Divide(100.0f, pCountInfo.Value1);
        float fComplatePercent  = (fCountGap * pCountInfo.Value2);
        float fProgressPercent  = (fCountGap * iProgress);

        return (fComplatePercent + fProgressPercent);
    }

    void OnEventToLoadStart(string strFileName, SHLoadStartInfo pData)
    {
        m_pPrograss.SetLoadStart(strFileName, pData);
    }

    void OnEventToLoadDone(string strFileName, SHLoadEndInfo pData)
    {
        CallEventToError(m_pPrograss.GetLoadDataInfo(strFileName), pData);
        CallEventToPrograss(m_pPrograss.SetLoadFinish(strFileName, pData.m_bIsSuccess));

        if (false == IsLoadDone())
            return;

        CallEventToComplate();
    }

    void CallEventToAsyncPrograss()
    {
        var pEvent                  = new SHLoadEvent();
        pEvent.m_pCount             = m_pPrograss.GetCountInfo();
        pEvent.m_fPercent           = GetLoadPrograss();
        pEvent.m_bIsAsyncPrograss   = true;
        EventToProgress.Callback<SHLoadEvent>(this, pEvent);
    }

    void CallEventToPrograss(SHLoadData pData)
    {
        if (null == pData)
            return;

        var pEvent                  = new SHLoadEvent();
        pEvent.m_eType              = pData.m_eDataType;
        pEvent.m_strFileName        = pData.m_strName;
        pEvent.m_pCount             = m_pPrograss.GetCountInfo();
        pEvent.m_pTime              = m_pPrograss.GetLoadTime(pData.m_strName);
        pEvent.m_bIsSuccess         = pData.m_bIsSuccess;
        pEvent.m_bIsFail            = m_pPrograss.m_bIsFail;
        pEvent.m_fPercent           = GetLoadPrograss();
        pEvent.m_bIsAsyncPrograss   = false;
        EventToProgress.Callback<SHLoadEvent>(this, pEvent);
    }

    void CallEventToComplate()
    {
        var pEvent                  = new SHLoadEvent();
        pEvent.m_bIsFail            = m_pPrograss.m_bIsFail;
        pEvent.m_pCount             = m_pPrograss.GetCountInfo();
        pEvent.m_pTime              = new SHPair<float, float>(m_pPrograss.GetLoadTime(), 0.0f);
        EventToComplate.Callback<SHLoadEvent>(this, pEvent);
        EventToComplate.Clear();
    }

    void CallEventToError(SHLoadData pData, SHLoadEndInfo pEndData)
    {
        if ((null == pData) || (null == pEndData))
            return;

        if (eLoadErrorCode.None == pEndData.m_eErrorCode)
            return;

        var pEvent                  = new SHLoadEvent();
        pEvent.m_eType              = pData.m_eDataType;
        pEvent.m_strFileName        = pData.m_strName;
        pEvent.m_pCount             = m_pPrograss.GetCountInfo();
        pEvent.m_pTime              = m_pPrograss.GetLoadTime(pData.m_strName);
        pEvent.m_bIsSuccess         = pData.m_bIsSuccess;
        pEvent.m_bIsFail            = m_pPrograss.m_bIsFail;
        pEvent.m_fPercent           = GetLoadPrograss();
        pEvent.m_eErrorCode         = pEndData.m_eErrorCode;
        pEvent.m_bIsAsyncPrograss   = false;
        EventToError.Callback<SHLoadEvent>(this, pEvent);
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
    public bool IsReMainLoadFiles()
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