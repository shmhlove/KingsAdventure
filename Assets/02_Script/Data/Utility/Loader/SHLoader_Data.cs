﻿using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;


// public class : 로드 데이터
/* Summary
 * --------------------------------------------------------------------------------------
 * SHLoadData는 로드할 데이터에 대한 정보.
 * --------------------------------------------------------------------------------------
 * 데이터 로드부에서 SHLoadData를 구성하여 로더에 Push 해두면, 
 * 로드 타이밍이 왔을때 로더가 데이터 로드부에 SHLoadData를 전달해준다.
 * 이때 데이터 로드부에서는 자신이 구성한 SHLoadData를 바탕으로 데이터를 로드하면 된다.
 * --------------------------------------------------------------------------------------
 * 데이터로드부 -SHLoadData-> Push -> 로더
 * 로더 -SHLoadData-> Load -> 데이터로드부
 * --------------------------------------------------------------------------------------
 */
public class SHLoadData
{
    public eDataType            m_eDataType;          // 로드할 데이터 타입
    public string               m_strName;            // 로드할 데이터 이름
    public Func<bool>           m_pTriggerLoadCall;   // 트리거 람다 : 로드 타이밍을 데이터 로드부에서 결정할 수 있도록 트리거 람다를 등록할 수 있다.
    public Action                                     // 로드 콜백 : 로드 타이밍이 왔을때 콜이 될 람다
    <
        SHLoadData, 
        Action<string, SHLoadStartInfo>,
        Action<string, SHLoadEndInfo>
    > m_pLoadFunc;
    
    public SHLoadData()
    {
        m_pTriggerLoadCall  = () => { return true; };
    }
}

// public class : 로드 시작 정보
/* Summary
 * --------------------------------------------------------------------------------------
 * 데이터 로드가 시작될때 로더에 알려줘야 할 정보.
 * --------------------------------------------------------------------------------------
 * 만약 로드 방식이 Async 라면 
 * 데이터 로드부에서 AsyncOperation를 로더에 전달하여
 * 로더의 프로그래스 정보를 갱신시켜줄 수 있다.
 * --------------------------------------------------------------------------------------
 */
public class SHLoadStartInfo
{
    public WWW              m_pWWW   = null;
    public AsyncOperation   m_pAsync = null;

    public SHLoadStartInfo() { }
    public SHLoadStartInfo(WWW pWWW)
    {
        m_pWWW = pWWW;
    }
    public SHLoadStartInfo(AsyncOperation pAsync)
    {
        m_pAsync = pAsync;
    }

    public float GetPrograss()
    {
        if (null != m_pWWW)
            return m_pWWW.progress;

        if (null != m_pAsync)
            return m_pAsync.progress;

        return 0.0f;
    }
}

// public class : 로드 종료 정보
/* Summary
 * --------------------------------------------------------------------------------------
 * 데이터 로드가 종료 되었을때 로더에 알려줘야 할 정보.
 * --------------------------------------------------------------------------------------
 * 데이터를 성공적으로 로드하였는지에 대한 정보를 로더에게 알려주면
 * 로더는 모든 데이터의 결과를 종합하여 최종적으로 데이터 로드가 성공했는지를 기록한다.
 * 이 기록으로 데이터 로드를 재시작 할 것인지 다음 스텝으로 게임을 진행 시킬 것인지 결정할 수 있다.
 * --------------------------------------------------------------------------------------
 */
public class SHLoadEndInfo
{
    public bool         m_bIsSuccess;
    public bool         m_bIsDone;
    public eErrorCode   m_eErrorCode;

    public SHLoadEndInfo()
    {
        m_bIsDone    = false;
    }
    public SHLoadEndInfo(eErrorCode errorCode) : this()
    {
        m_bIsSuccess = eErrorCode.Succeed == errorCode;
        m_eErrorCode = errorCode;
    }
}

// public class : 로드 데이터 상태
/* Summary
 * --------------------------------------------------------------------------------------
 * 로드할 데이터에 대한 상태 정보로 로드 시작부터 종료까지 모든 상태를 기록한다.
 * --------------------------------------------------------------------------------------
 */
public class SHLoadDataStateInfo
{
    public SHLoadData      m_pLoadDataInfo;
    public SHLoadStartInfo m_pStartInfo;
    public SHLoadEndInfo   m_pEndInfo;

    public DateTime        m_pLoadStartTime;

    public SHLoadDataStateInfo()
    {
        m_pLoadDataInfo = null;
        m_pStartInfo = null;
        m_pEndInfo = null;
    }
    public SHLoadDataStateInfo(SHLoadData pData) : this()
    {
        m_pLoadDataInfo = pData;
    }
    
    public bool IsLoadTrigger()
    {
        if (null == m_pLoadDataInfo)
            return false;

        return m_pLoadDataInfo.m_pTriggerLoadCall();
    }

    public void LoadCall(Action<string, SHLoadStartInfo> OnEventStart, Action<string, SHLoadEndInfo> OnEventDone)
    {
        if (null == m_pLoadDataInfo)
            return;

        m_pLoadStartTime = DateTime.Now;
        m_pLoadDataInfo.m_pLoadFunc(m_pLoadDataInfo, OnEventStart, OnEventDone);
    }

    public bool IsDone()
    {
        if (null == m_pEndInfo)
            return true;

        return m_pEndInfo.m_bIsDone;
    }
}

// public class : 데이터 로딩 중 정보
/* Summary
 * --------------------------------------------------------------------------------------
 * 로딩 중인 현재 데이터와 진행상태 정보입니다.
 * --------------------------------------------------------------------------------------
 * 유저 클래스에서는 이 정보데이터를 Prograss 혹은 Done 이벤트 콜백으로 프레임마다 받을 수 있습니다.
 * --------------------------------------------------------------------------------------
 */
public class SHLoadingInfo
{
    // 로드 중인 데이터 정보들
    public List<SHLoadDataStateInfo> m_pLoadingDatum = new List<SHLoadDataStateInfo>();

    // 로딩 카운트 정보
    public int                       m_iLoadDoneCount;  // 로드 완료된 데이터 카운트
    public int                       m_iTotalDataCount; // 전체 데이터 카운트
    public int                       m_fElapsedTime;    // 현재 경과된 시간
    public float                     m_fLoadPercent;    // 로드 진행도(0 ~ 100%)
}

// public class : 로더
/* Summary
 * --------------------------------------------------------------------------------------
 * 로딩 플로우를 전체적으로 관리하는 클래스
 * --------------------------------------------------------------------------------------
 * Loader를 통해 SHLoadData 리스트를 등록하면 
 * 순차적으로 로드콜을 받을 수 있고, 프로그래스 정보를 이벤트로 받을 수 있습니다.
 * --------------------------------------------------------------------------------------
 */
public partial class SHLoader
{
    // 로드 진행 관리
    public SHLoadPrograss m_pPrograss = new SHLoadPrograss();

    // 이벤트
    public SHEvent EventToDone        = new SHEvent();
    public SHEvent EventToProgress    = new SHEvent();

    public void Initialize()
    {
        m_pPrograss.Initialize();
        EventToDone.Clear();
        EventToProgress.Clear();
    }
}