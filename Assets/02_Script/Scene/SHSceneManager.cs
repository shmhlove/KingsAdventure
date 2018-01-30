﻿using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHSceneManager : SHSingleton<SHSceneManager>
{
    #region Members
    private SHEvent m_pEventOfAddtiveScene = new SHEvent();
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
    // 인터페이스 : 씬 추가
    public void Addtive(eSceneType eType, bool bIsUseFade = false, Action<eSceneType> pCallback = null)
    {
        if (true == IsLoadedScene(eType))
            return;

        if (null == pCallback)
            pCallback = (eSceneType e) => { };

        Action LoadScene = () =>
        {
            string strPath = string.Empty;

            // StreamingAssets 로컬 다운로드
#if UNITY_EDITOR || UNITY_STANDALONE
            strPath = string.Format("{0}{1}", "file://", SHPath.GetPathToStreamingAssets());
#elif UNITY_ANDROID
            strPath = string.Format("{0}{1}{2}", "jar:file://", SHPath.GetPathToAssets(), "!/assets");
#elif UNITY_IOS
            strPath = string.Format("{0}{1}{2}", "file://", SHPath.GetPathToAssets(), "/Raw");
#endif

            // CDN 다운로드
            strPath = "http://blueasa.synology.me/home/shmhlove/KOR/KingsAdventure/AOS";

            var strScenePath = string.Format("{0}/AssetBundles/scene/{1}.scene", strPath, eType.ToString().ToLower());
            Single.Coroutine.WWW((pWWW) =>
            {
                if (null == pWWW.assetBundle)
                {
                    pCallback(eType);
                    return;
                }
                
                var strScenes = pWWW.assetBundle.GetAllScenePaths();
                var strLoadScenePath = string.Empty;
                foreach (string strScene in strScenes)
                {
                    if (true == strScene.Contains(eType.ToString()))
                    {
                        strLoadScenePath = strScene;
                        break;
                    }
                }

                LoadProcess(SceneManager.LoadSceneAsync(strLoadScenePath, LoadSceneMode.Additive), (pAsyncOperation) =>
                {
                    pWWW.assetBundle.Unload(false);

                    if (true == bIsUseFade)
                        PlayFadeOut(() => pCallback(eType));
                    else
                        pCallback(eType);
                    
                    CallEventOfAddtiveScene(eType);
                });
            }, WWW.LoadFromCacheOrDownload(strScenePath, 0));
        };

        if (true == bIsUseFade)
            PlayFadeIn(() => LoadScene());
        else
            LoadScene();
    }

    // 인터페이스 : 씬 제거
    public void Remove(eSceneType eType)
    {
        if (false == IsLoadedScene(eType))
            return;

        SceneManager.UnloadSceneAsync(eType.ToString());
    }
    
    // 인터페이스 : 현재 로드되어 있는 씬 인가?
    public bool IsLoadedScene(eSceneType eType)
    {
        return SceneManager.GetSceneByName(eType.ToString()).isLoaded;
    }

    // 인터페이스 : 현재 활성화 되어 있는 씬
    public eSceneType GetActiveScene()
    {
        return SHHard.GetSceneTypeByString(SceneManager.GetActiveScene().name);
    }

    // 인터페이스 : 씬 추가 이벤트 등록
    public void AddEventOfAddtiveScene(EventHandler pCallback)
    {
        m_pEventOfAddtiveScene.Add(pCallback);
    }

    // 인터페이스 : 씬 추가 이벤트 해제
    public void RemoveEventOfAddtiveScene(EventHandler pCallback)
    {
        m_pEventOfAddtiveScene.Remove(pCallback);
    }
    #endregion


    #region Utility Functions
    // 유틸 : 씬 로드
    void LoadProcess(AsyncOperation pAsyncInfo, Action<AsyncOperation> pDone)
    {
        Single.Coroutine.Async(() => pDone(pAsyncInfo), pAsyncInfo);
    }
    
    // 유틸 : 씬 추가 이벤트 콜
    void CallEventOfAddtiveScene(eSceneType eType)
    {
        m_pEventOfAddtiveScene.Callback(this, eType);
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