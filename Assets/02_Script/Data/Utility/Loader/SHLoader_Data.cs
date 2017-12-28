using UnityEngine;

using System;
using System.Threading; 
using System.Collections;
using System.Collections.Generic;

// enum : 로드 에러 코드
public enum eLoadErrorCode
{
    None,
    Load_Scene,         // 씬 로드 중 에러
    Load_Resource,      // 리소스 로드 중 에러
    Load_Table,         // 테이블 로드 중 에러
    Convert_Table,      // 테이블 패치 후 컨버팅 중 에러
    Patch_Table,        // 테이블 패치 중 에러
    Patch_Bundle,       // 번들 패치 중 에러
    Common_WWW,         // 일반적인 WWW 에러
}

// class : 로드 이벤트 데이터
public class SHLoadEvent
{
    public eDataType            m_eType;              // 현재 로드타입
    public string               m_strFileName;        // 현재 로드파일이름
    public bool                 m_bIsSuccess;         // 현재 파일 로드상태
    public bool                 m_bIsFail;            // 실패한 파일이 하나라도 있는가?
    public eLoadErrorCode       m_eErrorCode;         // 에러코드

    public SHPair<int, int>     m_pCount;             // 로드 카운트 정보<Total, Current>
    public SHPair<float, float> m_pTime;              // 로드 시간 정보<Total, Current>

    public float                m_fPercent;           // 진행도(0 ~ 100%)
    public bool                 m_bIsAsyncPrograss;   // 어싱크 프로그래스 정보인가? ( 어싱크는 로드 순서가 없기에 현재파일 정보는 보내줄 수가 없다. )
}

// class : 로드 데이터
public class SHLoadData
{
    public eDataType            m_eDataType;          // 로드 타입
    public string               m_strName;            // 파일명
    public Func<bool>           m_pTriggerLoadCall;   // 로트콜 조건있을 경우가 있다,, ( TableType이 모두 로드되었을때라거나 등등 )
    public Action                                     // 로드명령 콜백
    <
        SHLoadData, 
        Action<string, SHLoadStartInfo>,
        Action<string, SHLoadEndInfo>
    > m_pLoadFunc;
    
    public float                m_fLoadTime;          // 로드 시간
    public bool                 m_bIsSuccess;         // 로드 성공여부
    public bool                 m_bIsDone;            // 로드 완료여부
    
    public SHLoadData()
    {
        m_fLoadTime         = 0.0f;
        m_bIsSuccess        = false;
        m_bIsDone           = false;
        m_pTriggerLoadCall  = () => { return true; };
    }
}

// class : 로드 시작 이벤트 정보
public class SHLoadStartInfo
{
    public WWW              m_pPatch        = null;
    public AsyncOperation   m_pResource     = null;

    public SHLoadStartInfo() { }
    public SHLoadStartInfo(WWW pWWW)
    {
        m_pPatch = pWWW;
    }
    public SHLoadStartInfo(AsyncOperation pAsync)
    {
        m_pResource = pAsync;
    }

    public float GetPrograss()
    {
        if (null != m_pPatch)
            return m_pPatch.progress;

        if (null != m_pResource)
            return m_pResource.progress;

        return 0.0f;
    }
}

// class : 로드 종료 이벤트 정보
public class SHLoadEndInfo
{
    public bool             m_bIsSuccess;
    public eLoadErrorCode   m_eErrorCode;
    public SHLoadEndInfo() { }
    public SHLoadEndInfo(bool bIsSuccess, eLoadErrorCode eErrorCode)
    {
        m_bIsSuccess = bIsSuccess;
        m_eErrorCode = eErrorCode;
    }
}

public partial class SHLoader
{
    // 로드 정보
    public SHLoadPrograss m_pPrograss = new SHLoadPrograss();

    // 이벤트
    public SHEvent EventToComplate = new SHEvent();
    public SHEvent EventToProgress = new SHEvent();
    public SHEvent EventToError    = new SHEvent();

    public void Initialize()
    {
        m_pPrograss.Initialize();
        EventToComplate.Clear();
        EventToProgress.Clear();
        EventToError.Clear();
    }
}