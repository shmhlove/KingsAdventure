using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using HistoryList   = System.Collections.Generic.List<SHSceneHistory>;
using EventList     = System.Collections.Generic.List<System.Action<eSceneType, eSceneType>>;

public class SHSceneHistory
{
    public eSceneType m_eTo;
    public eSceneType m_eFrom;

    public SHSceneHistory(eSceneType eTo, eSceneType eFrom)
    {
        m_eTo   = eTo;
        m_eFrom = eFrom;
    }
}

public class SHSceneManager : SHSingleton<SHSceneManager>
{
    #region Members
    // 씬 상태
    private eSceneType      m_eCurrentScene   = eSceneType.None;
    private eSceneType      m_eBeforeScene    = eSceneType.None;

    // 히스토리
    public HistoryList      m_pHistory        = new HistoryList();

    // 이벤트
    private EventList       m_pEventToChangeScene = new EventList();

    public bool             m_bIsChanging     = false;
    #endregion


    #region Virtual Functions
    // 다양화 : 초기화
    public override void OnInitialize()
    {
        SetDontDestroy();
    }

    // 다양화 : 종료
    public override void OnFinalize() { }
    #endregion


    #region Interface Functions
    // 인터페이스 : 씬 이동
    public void GoTo(eSceneType eChange)
    {
        // 알리아싱
        eSceneType eCurrent = GetCurrentScene();

        // 씬 변경시 처리해야 할 여러가지 작업들
        PerformanceToChangeScene(eCurrent, eChange);

        // 로드명령
		ExcuteGoTo(eChange); 
    }

    // 인터페이스 : 현재 씬 얻기
    public eSceneType GetCurrentScene()
    {
        if (eSceneType.None == m_eCurrentScene)
            return GetActiveScene();
        
        return m_eCurrentScene;
    }

    // 인터페이스 : 현재 씬 얻기
    public eSceneType GetActiveScene()
    {
        return SHHard.GetSceneTypeToString(SceneManager.GetActiveScene().name);
    }

    // 인터페이스 : 이전 씬 얻기
    public eSceneType GetBeforeScene()
    {
        return m_eBeforeScene;
    }

    // 인터페이스 : 현재 씬 인가?
    public bool IsCurrentScene(eSceneType eType)
    {
        return (GetCurrentScene() == eType);
    }

    // 인터페이스 : 이전 씬 인가?
    public bool IsBeforeScene(eSceneType eType)
    {
        int iLastIndex = m_pHistory.Count - 1;
        if (0 > iLastIndex)
            return false;

        return (m_pHistory[iLastIndex].m_eTo == eType);
    }

    // 인터페이스 : X씬을 거친적이 있는가?
    public bool IsPassedScene(eSceneType eType)
    {
        foreach (var pHistory in m_pHistory)
        {
            if (eType == pHistory.m_eFrom)
                return true;
        }

        return false;
    }

    // 인터페이스 : 로딩이 필요한씬
    public bool IsNeedLoading(eSceneType eType)
    {
        return false;
    }

    // 인터페이스 : 콜백등록
    public void AddEventToChangeScene(Action<eSceneType, eSceneType> pAction)
    {
        if (null == pAction)
            return;

        if (true == IsAddEvent(pAction))
            return;

        m_pEventToChangeScene.Add(pAction);
    }

    // 인터페이스 : 콜백제거
    public void DelEventToChangeScene(Action<eSceneType, eSceneType> pAction)
    {
        if (false == IsAddEvent(pAction))
            return;

        m_pEventToChangeScene.Remove(pAction);
    }
    #endregion


    #region Utility Functions
    // 유틸 : 씬 이동 실행
    void ExcuteGoTo(eSceneType eType)
    {
        PlayFadeIn(() =>
        {
            if (true == IsNeedLoading(eType))
                LoadScene(eSceneType.Loading, (bIsSuccess) => PlayFadeOut(null));
            else
                LoadScene(eType,              (bIsSuccess) => PlayFadeOut(null));
        });
    }

    // 유틸 : 씬 로드 ( Change 방식 : GoTo 명령시 호출됨 )
    AsyncOperation LoadScene(eSceneType eType, Action<bool> pComplate)
    {
        return SetLoadPostProcess(pComplate,
            SceneManager.LoadSceneAsync(eType.ToString(), LoadSceneMode.Single));
    }
    
    // 유틸 : 씬 로드 ( Add 방식 : SceneData 클래스에서 호출됨 )
    AsyncOperation AddScene(string strSceneName, Action<bool> pComplate)
    {
        return SetLoadPostProcess(pComplate,
            SceneManager.LoadSceneAsync(strSceneName, LoadSceneMode.Additive));
    }

    // 유틸 : 씬 로드 후 처리를 위한 코루틴 등록
    AsyncOperation SetLoadPostProcess(Action<bool> pComplate, AsyncOperation pAsyncInfo)
    {
        if (null == pAsyncInfo)
        {
            Debug.LogError(string.Format("씬 로드 실패!!(SceneType : {0})", GetCurrentScene()));

            if (null != pComplate)
                pComplate(false);
        }
        else
        {
            Single.Coroutine.Async(() =>
            {
                if (null != pComplate)
                    pComplate(true);
            },
            pAsyncInfo);
        }

        return pAsyncInfo;
    }
    
    // 유틸 : 등록된 콜백인가?
    bool IsAddEvent(Action<eSceneType, eSceneType> pAction)
    {
        return m_pEventToChangeScene.Contains(pAction);
    }

    // 유틸 : 씬 변경이 시작될때 알려달라고 한 곳에 알려주자
    void SendCallback(eSceneType eCurrent, eSceneType eChange)
    {
        SHUtils.ForToList(m_pEventToChangeScene, (pAction) =>
        {
            if (null == pAction)
                return;

            pAction(eCurrent, eChange);
        });
    }

    // 유틸 : 씬 변경시 처리해야할 하드한 작업들
    void PerformanceToChangeScene(eSceneType eCurrent, eSceneType eChange)
    {
        // 씬 변경 이벤트 콜
        SendCallback(eCurrent, eChange);

        // 히스토리 남기기
        SetHistory(eChange);
    }

    // 유틸 : 씬 변경 히스토리 기록
    void SetHistory(eSceneType eType)
    {
        m_pHistory.Add(new SHSceneHistory(m_eCurrentScene, eType));
        m_eBeforeScene  = m_eCurrentScene;
        m_eCurrentScene = eType;
    }

    // 유틸 : 페이드 인
    void PlayFadeIn(Action pCallback)
    {
        if (false == Single.UI.Show("Panel_FadeIn", pCallback))
        {
            if (null != pCallback)
                pCallback();
        }

        SHCoroutine.Instance.NextUpdate(() => Single.UI.Close("Panel_FadeOut"));
    }

    // 유틸 : 페이드 아웃
    void PlayFadeOut(Action pCallback)
    {
        if (false == Single.UI.Show("Panel_FadeOut", pCallback))
        {
            if (null != pCallback)
                pCallback();
        }

        SHCoroutine.Instance.NextUpdate(() => Single.UI.Close("Panel_FadeIn"));
    }
    #endregion
}