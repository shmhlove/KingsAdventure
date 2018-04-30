using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using SH.Platform;

public class SHSceneManager : SHSingleton<SHSceneManager>
{
    private SHEvent m_pEventOfAddtiveScene = new SHEvent();

    public override void OnInitialize()
    {
        SetDontDestroy();
    }

    public void Addtive(eSceneType eType, bool bIsUseFade = false, Action<SHReply> pCallback = null)
    {
        if (true == IsLoadedScene(eType))
            return;

        if (null == pCallback)
            pCallback = (SHReply pReply) => { };

        Action LoadScene = () =>
        {
            Single.Coroutine.CachingWait(()=> 
            {
                Single.Firebase.Storage.GetFileURL(string.Format("AssetBundle/scene/{0}.scene", eType.ToString().ToLower()), (pReply) =>
                {
                    var pAsReply = pReply.GetAs<Firebase.SHReplyGetFileURL>();

                    Debug.LogErrorFormat("[SHSceneManager] BundleURL : {0}", pAsReply.m_strURL);
                    if (false == pAsReply.IsSucceed)
                    {

                    }

                    
                    Single.Coroutine.WWW((pWWW) =>
                    {
                        if (null == pWWW.assetBundle)
                        {
                            Debug.LogErrorFormat("[SHSceneManager] Scene bundle download error is {0}", pWWW.error);
                            pCallback(new SHReply(new SHError(eErrorCode.Failed, pWWW.error)));
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

                        if (true == string.IsNullOrEmpty(strLoadScenePath))
                        {
                            Debug.LogErrorFormat("[SHSceneManager] Scene bundle Not matching of name");
                            pCallback(new SHReply(new SHError(eErrorCode.Failed, "Scene bundle Not matching of name")));
                            return;
                        }

                        LoadProcess(SceneManager.LoadSceneAsync(strLoadScenePath, LoadSceneMode.Additive), (pAsyncOperation) =>
                        {
                            pWWW.assetBundle.Unload(false);
                            pCallback(new SHReply(new SHError(eErrorCode.Failed, "Scene bundle Not matching of name")));
                            if (true == bIsUseFade)
                                PlayFadeOut(() => pCallback(new SHReply()));
                            else
                                pCallback(new SHReply());

                            CallEventOfAddtiveScene(eType);
                        });
                    }, WWW.LoadFromCacheOrDownload(strURL, 0));
                });
            });
        };

        if (true == bIsUseFade)
            PlayFadeIn(() => LoadScene());
        else
            LoadScene();
    }
    
    public void Remove(eSceneType eType)
    {
        if (false == IsLoadedScene(eType))
            return;

        SceneManager.UnloadSceneAsync(eType.ToString());
    }
    
    public bool IsLoadedScene(eSceneType eType)
    {
        return SceneManager.GetSceneByName(eType.ToString()).isLoaded;
    }
    
    public eSceneType GetActiveScene()
    {
        return SHHard.GetSceneTypeByString(SceneManager.GetActiveScene().name);
    }
    
    public void AddEventOfAddtiveScene(EventHandler pCallback)
    {
        m_pEventOfAddtiveScene.Add(pCallback);
    }
    
    public void RemoveEventOfAddtiveScene(EventHandler pCallback)
    {
        m_pEventOfAddtiveScene.Remove(pCallback);
    }
    
    void LoadProcess(AsyncOperation pAsyncInfo, Action<AsyncOperation> pDone)
    {
        Single.Coroutine.Async(() => pDone(pAsyncInfo), pAsyncInfo);
    }
    
    void CallEventOfAddtiveScene(eSceneType eType)
    {
        m_pEventOfAddtiveScene.Callback(this, eType);
    }
    
    void PlayFadeIn(Action pCallback)
    {
        if (false == Single.UI.Show("Panel_FadeIn", pCallback))
        {
            if (null != pCallback)
                pCallback();
        }

        SHCoroutine.Instance.NextUpdate(() => Single.UI.Close("Panel_FadeOut"));
    }
    
    void PlayFadeOut(Action pCallback)
    {
        if (false == Single.UI.Show("Panel_FadeOut", pCallback))
        {
            if (null != pCallback)
                pCallback();
        }

        SHCoroutine.Instance.NextUpdate(() => Single.UI.Close("Panel_FadeIn"));
    }
}