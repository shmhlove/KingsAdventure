using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Firebase.Storage;

public class SHSceneManager : SHSingleton<SHSceneManager>
{
    #region Members
    private SHEvent m_pEventOfAddtiveScene = new SHEvent();
    #endregion


    #region Virtual Functions
    public override void OnInitialize()
    {
        SetDontDestroy();
    }
    
    public override void OnFinalize() { }
    #endregion
    

    #region Interface Functions
    public void Addtive(eSceneType eType, bool bIsUseFade = false, Action<eSceneType> pCallback = null)
    {
        if (true == IsLoadedScene(eType))
            return;

        if (null == pCallback)
            pCallback = (eSceneType e) => { };

        Action LoadScene = () =>
        {
            Single.Coroutine.CachingWait(()=> 
            {
                GetSceneBundleURL(eType, (strURL) =>
                {
                    Debug.LogWarningFormat("URL : {0}", strURL);
                    Single.Coroutine.WWW((pWWW) =>
                    {
                        if (null == pWWW.assetBundle)
                        {
                            Debug.LogErrorFormat("Scene bundle download error is {0}", pWWW.error);
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

                        if (true == string.IsNullOrEmpty(strLoadScenePath))
                        {
                            Debug.LogErrorFormat("Scene bundle Not matching of name");
                            pCallback(eType);
                            return;
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
    #endregion


    #region Utility Functions
    void GetSceneBundleURL(eSceneType eType, Action<string> pCallback)
    {
        // string strURL = string.Empty;

        // StreamingAssets 로컬 다운로드
        //#if UNITY_EDITOR || UNITY_STANDALONE
        //        strURL = string.Format("{0}{1}", "file://", SHPath.GetPathToStreamingAssets());
        //#elif UNITY_ANDROID
        //        strURL = string.Format("{0}{1}{2}", "jar:file://", SHPath.GetPathToAssets(), "!/assets");
        //#elif UNITY_IOS
        //        strURL = string.Format("{0}{1}{2}", "file://", SHPath.GetPathToAssets(), "/Raw");
        //#endif
        //        strURL = string.Format("{0}/AssetBundles/scene/{1}.scene", strURL, eType.ToString().ToLower());

        // NAS CDN 다운로드
        //strURL = "http://blueasa.synology.me/home/shmhlove/KOR/KingsAdventure/AOS";
        //strURL = string.Format("{0}/AssetBundles/scene/{1}.scene", strURL, eType.ToString().ToLower());
        //strURL = "http://blueasa.synology.me:5000/index.cgi?launchApp=SYNO.SDS.App.FileStation3.Instance&launchParam=openfile%3D%252Fhome%252Fshmhlove%252FKOR%252FKingsAdventure%252FAOS%252FAssetBundles%252Fscene%252F{0}.scene";
        //strURL = string.Format(strURL, eType.ToString().ToLower());
        //strURL = "https://drive.google.com/file/d/1NMgY5mSwCsg9gG06J-xfVQAAQ2Kewknn/view?usp=sharing";

        // Firebase에서 다운로드
        FirebaseStorage pStorage = FirebaseStorage.DefaultInstance;

        StorageReference pRootRef = pStorage.GetReferenceFromUrl("gs://kingsadventure-4e10d.appspot.com/");
        StorageReference pSceneRef = pRootRef.Child("/AssetBundles/scene/");
        StorageReference pBundleRef = pSceneRef.Child(string.Format("{0}.scene", eType.ToString().ToLower()));

        // URL 다운로드
        pBundleRef.GetDownloadUrlAsync().ContinueWith((Task<Uri> pTask) =>
        {
            if ((false == pTask.IsFaulted) && (false == pTask.IsCanceled))
                pCallback(pTask.Result.OriginalString);
            else
                pCallback(string.Empty);
        });
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
    #endregion
}