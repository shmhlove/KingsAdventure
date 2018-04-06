﻿using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public partial class SHLoader
{
    void CoroutineToLoadProcess()
    {
        LoadCall();

        if (false == IsRemainLoadFiles())
            return;

        Single.Coroutine.NextFrame(CoroutineToLoadProcess);
    }
    
    void CoroutineToLoadProgressEvent()
    {
        CallEventToProgress();

        if (true == IsLoadDone())
            return;

        Single.Coroutine.NextFrame(CoroutineToLoadProgressEvent);
    }

    void LoadCall()
    {
        var pDataInfo = m_pProgress.DequeueWaitingDataInfo();
        if (null == pDataInfo)
            return;

        if (false == pDataInfo.IsLoadOkayByTrigger())
        {
            m_pProgress.EnqueueWaitingDataInfo(pDataInfo);
            return;
        }
        
        pDataInfo.LoadCall(OnEventToLoadStart, OnEventToLoadDone);
    }

    void AddLoadDatum(List<Dictionary<string, SHLoadData>> pLoadDatum)
    {
        SHUtils.ForToList<Dictionary<string, SHLoadData>>(pLoadDatum, (dicLoadData) =>
        {
            m_pProgress.AddLoadDatum(dicLoadData);
        });
    }

    void AddLoadEvent(EventHandler pDone, EventHandler pProgress)
    {
        if (null != pDone)
            EventToDone.Add(pDone);

        if (null != pProgress)
            EventToProgress.Add(pProgress);
    }

    void OnEventToLoadStart(string strFileName, SHLoadStartInfo pData)
    {
        m_pProgress.SetLoadStartInfo(strFileName, pData);
    }

    void OnEventToLoadDone(string strFileName, SHLoadEndInfo pData)
    {
        m_pProgress.SetLoadDoneInfo(strFileName, pData);
        CallEventToDone();
    }
    
    void CallEventToProgress()
    {
        EventToProgress.Callback<SHLoadingInfo>(this, m_pProgress.GetLoadingInfo());
    }

    void CallEventToDone()
    {
        EventToDone.Callback<SHLoadingInfo>(this, m_pProgress.GetLoadingInfo());
    }
    
    // 로드가 완료되었는가?(성공/실패유무가 아님)
    public bool IsLoadDone()
    {
        return m_pProgress.IsDone();
    }

    // 특정 파일이 로드완료되었는가?(성공/실패유무가 아님)
    public bool IsLoadDone(string strFileName)
    {
        return m_pProgress.IsDone(strFileName);
    }

    // 특정 타입이 로드완료되었는가?(성공/실패유무가 아님)
    public bool IsLoadDone(eDataType eType)
    {
        return m_pProgress.IsDone(eType);
    }

    // 로드할 파일이 있는가?
    public bool IsRemainLoadFiles()
    {
        return m_pProgress.IsRemainLoadWaitQueue();
    }
}