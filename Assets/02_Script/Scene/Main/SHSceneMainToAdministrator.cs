using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Text;
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

    AssetBundle pAssetBundle;
    public void OnClickOfDownloadBundle()
    {
        StartCoroutine(DownloadBundle());
        //Single.Coroutine.CachingWait(() => 
        //{
        //    Single.Timer.StartDeltaTime("DownloadBundle");
        //    string strURL = "http://blueasa.synology.me/home/shmhlove/KOR/KingsAdventure/AOS";
        //    strURL = string.Format("{0}/AssetBundles/scene/{1}.scene", strURL, eSceneType.Intro.ToString().ToLower());
        //    Single.Coroutine.WWW((pWWW) =>
        //    {
        //        if (null == pWWW.assetBundle)
        //            return;

        //        var strScenes = pWWW.assetBundle.GetAllScenePaths();
        //        foreach (string strScene in strScenes)
        //        {
        //            if (true == strScene.Contains(eSceneType.Intro.ToString()))
        //            {
        //                strLoadScenePath = strScene;
        //                break;
        //            }
        //        }

        //        pAssetBundle = pWWW.assetBundle;
        //        Debug.LogErrorFormat("Download Bundle Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("DownloadBundle"));
        //    }, WWW.LoadFromCacheOrDownload(strURL, 0));

        //});
    }

    IEnumerator DownloadBundle()
    {
        Single.Timer.StartDeltaTime("DownloadBundle");
        //{
        //    while (false == Caching.ready)
        //        yield return null;

        //    string strURL = "http://blueasa.synology.me/home/shmhlove/KOR/KingsAdventure/AOS";
        //    strURL = string.Format("{0}/AssetBundles/scene/{1}.scene", strURL, eSceneType.Intro.ToString().ToLower());
        //    UnityWebRequest pRequest = UnityWebRequest.Get(strURL);
        //    pRequest.chunkedTransfer = true;
        //    yield return pRequest.Send();

        //    if (pRequest.isError)
        //    {
        //        Debug.LogErrorFormat("Download Bundle Error is {0}", pRequest.error);
        //    }
        //    else
        //    {
        //        //var Async = AssetBundle.LoadFromMemoryAsync(pRequest.downloadHandler.data);
        //        //while (false == Async.isDone)
        //        //    yield return null;

        //        // pAssetBundle = Async.assetBundle;
        //        // pAssetBundle = ((DownloadHandlerAssetBundle)pRequest.downloadHandler).assetBundle;
        //        pAssetBundle = DownloadHandlerAssetBundle.GetContent(pRequest);
        //    }
        //}

        string strURL = "https://blueasa.synology.me:5000/home/shmhlove/KOR/KingsAdventure/AOS/AssetBundles/scene/intro.scene";
        strURL = string.Format("{0}/AssetBundles/scene/{1}.scene", strURL, eSceneType.Intro.ToString().ToLower());
        UnityWebRequest request = UnityWebRequest.Get(strURL);
        yield return request.Send();

        SHUtils.CreateDirectory(SHPath.GetPathToPersistentAssetBundle());
        
        FileStream fs = new FileStream(string.Format("{0}/{1}.scene", SHPath.GetPathToPersistentAssetBundle(), eSceneType.Intro.ToString().ToLower()), System.IO.FileMode.Create);
        fs.Write(request.downloadHandler.data, 0, (int)request.downloadedBytes);
        fs.Close();
        
        Debug.LogErrorFormat("Download Bundle Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("DownloadBundle"));
    }

    public void OnClickOfLoadSceneByBundle()
    {
        Single.Timer.StartDeltaTime("LoadSceneByBundle");
        
        var myLoadedAssetBundle = AssetBundle.LoadFromFile(string.Format("{0}/{1}.scene", SHPath.GetPathToPersistentAssetBundle(), eSceneType.Intro.ToString().ToLower()));
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }
        else
        {
            Debug.Log("Successed to load AssetBundle!");
        }

        var strScenes = myLoadedAssetBundle.GetAllScenePaths();
        var strLoadScenePath = string.Empty;
        foreach (string strScene in strScenes)
        {
            if (true == strScene.Contains(eSceneType.Intro.ToString()))
            {
                strLoadScenePath = strScene;
                break;
            }
        }
        
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