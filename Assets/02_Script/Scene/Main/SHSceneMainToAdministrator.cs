using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;

public class SHSceneMainToAdministrator : SHMonoWrapper
{
    #region System Functions
    public override void Start() 
    {
        base.Start();
        Single.AppInfo.CreateSingleton();
	}
    #endregion


    public void OnClickOfAddtiveIntro()
    {
        Single.Timer.StartDeltaTime("SceneLoadTime");
        Single.Scene.Addtive(eSceneType.Intro, false, (eType) =>
        {
            Debug.LogErrorFormat("Bundle Scene Load Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("SceneLoadTime"));
        });
    }

    public void OnClickOfRemoveIntro()
    {
        Single.Scene.Remove(eSceneType.Intro);
    }

    public void OnClickOfAddtivePatch()
    {
        Single.Timer.StartDeltaTime("SceneLoadTime");
        Single.Scene.Addtive(eSceneType.Patch, false, (eType) =>
        {
            Debug.LogErrorFormat("undle Scene Load Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("SceneLoadTime"));
        });
    }

    public void OnClickOfRemovePatch()
    {
        Single.Scene.Remove(eSceneType.Patch);
    }

    public void OnClickOfAddtiveLogin()
    {
        Single.Timer.StartDeltaTime("SceneLoadTime");
        Single.Scene.Addtive(eSceneType.Login, false, (eType) => 
        {
            Debug.LogErrorFormat("Bundle Scene Load Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("SceneLoadTime"));
        });
    }

    public void OnClickOfRemoveLogin()
    {
        Single.Scene.Remove(eSceneType.Login);
    }

    public void OnClickOfAddtiveLoading()
    {
        Single.Timer.StartDeltaTime("SceneLoadTime");
        Single.Scene.Addtive(eSceneType.Loading, false, (eType) =>
        {
            Debug.LogErrorFormat("Bundle Scene Load Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("SceneLoadTime"));
        });
    }

    public void OnClickOfRemoveLoading()
    {
        Single.Scene.Remove(eSceneType.Loading);
    }
    
    public void OnClickOfAddtiveInGame()
    {
        Single.Timer.StartDeltaTime("SceneLoadTime");
        Single.Scene.Addtive(eSceneType.InGame, false, (eType) =>
        {
            Debug.LogErrorFormat("Bundle Scene Load Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("SceneLoadTime"));
        });
    }

    public void OnClickOfRemoveInGame()
    {
        Single.Scene.Remove(eSceneType.InGame);
    }

    string strLoadScenePath;
    AssetBundle pAssetBundle;
    public void OnClickOfDownloadBundle()
    {
        Single.Coroutine.CachingWait(() => 
        {
            Single.Timer.StartDeltaTime("DownloadBundle");
            string strPath = "http://blueasa.synology.me/home/shmhlove/KOR/KingsAdventure/AOS";
            var strScenePath = string.Format("{0}/AssetBundles/scene/{1}.scene", strPath, eSceneType.Intro.ToString().ToLower());
            Single.Coroutine.WWW((pWWW) =>
            {
                if (null == pWWW.assetBundle)
                    return;

                var strScenes = pWWW.assetBundle.GetAllScenePaths();
                foreach (string strScene in strScenes)
                {
                    if (true == strScene.Contains(eSceneType.Intro.ToString()))
                    {
                        strLoadScenePath = strScene;
                        break;
                    }
                }

                pAssetBundle = pWWW.assetBundle;
                Debug.LogErrorFormat("Download Bundle Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("DownloadBundle"));
            }, WWW.LoadFromCacheOrDownload(strScenePath, 0));
        });
    }

    public void OnClickOfLoadSceneByBundle()
    {
        Single.Timer.StartDeltaTime("LoadSceneByBundle");
        Single.Coroutine.Async(() => 
        {
            Debug.LogErrorFormat("Load Scene By Bundle Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("LoadSceneByBundle"));
        }, SceneManager.LoadSceneAsync(strLoadScenePath, LoadSceneMode.Additive));
    }

    public void OnClickOfUnLoadBundle()
    {
        if (null == pAssetBundle)
            return;

        pAssetBundle.Unload(false);
        pAssetBundle = null;
    }
}