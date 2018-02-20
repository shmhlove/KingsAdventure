using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Threading.Tasks;

using Firebase.Storage;

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

    public void OnClickOfFirebaseStorage()
    {
        //// 참조 얻기
        //{
        //    FirebaseStorage pStorage = FirebaseStorage.DefaultInstance;
        //    StorageReference pStorageRef = pStorage.GetReferenceFromUrl("gs://kingsadventure-c004a.appspot.com/");
        //    StorageReference pSceneBundleRef = pStorageRef.Child("AssetBundles/scene");
        //}

        //// 참조 만들기
        //{
        //    FirebaseStorage pStorage = FirebaseStorage.DefaultInstance;

        //    // Create a root reference
        //    StorageReference pStorageRef = pStorage.GetReference("gs://kingsadventure-c004a.appspot.com/");

        //    // Create a reference to "mountains.jpg"
        //    StorageReference pSceneBundleRef = pStorageRef.Child("AssetBundles/scene/intro.scene");
        //}

        //// 파일 업로드
        //{
        //    FirebaseStorage pStorage = FirebaseStorage.DefaultInstance;
        //    StorageReference pStorageRef = pStorage.GetReferenceFromUrl("gs://kingsadventure-c004a.appspot.com/");

        // string strURL = "http://blueasa.synology.me/home/shmhlove/KOR/KingsAdventure/AOS";
        // strURL = string.Format("{0}/AssetBundles/scene/{1}.scene", strURL, eSceneType.Intro.ToString().ToLower());
        // UnityWebRequest pRequest = UnityWebRequest.GetAssetBundle(strURL);
        // pRequest.chunkedTransfer = true;
        // yield return pRequest.Send();

        //    // Create a reference to the file you want to upload
        //    StorageReference RiversRef = pStorageRef.Child("AssetBundles/scene/intro.scene");

        //    // Upload the file to the path
        //    var pProgress = RiversRef.PutFileAsync(strLocalFile).ContinueWith((Task<StorageMetadata> pTask) =>
        //    {
        //        if (pTask.IsFaulted || pTask.IsCanceled)
        //        {
        //            Debug.Log(pTask.Exception.ToString());
        //            // Uh-oh, an error occurred!
        //        }
        //        else
        //        {
        //            // Metadata contains file metadata such as size, content-type, and download URL.
        //            StorageMetadata pMetadata = pTask.Result;
        //            string download_url = pMetadata.DownloadUrl.ToString();
        //            Debug.Log("Finished uploading...");
        //            Debug.Log("download url = " + download_url);
        //        }
        //    });

        //    // Progress
        //    pProgress.ContinueWith(pResultTask =>
        //    {
        //        if ((false == pResultTask.IsFaulted) && (false == pResultTask.IsCanceled))
        //        {
        //            Debug.Log("Upload finished.");
        //        }
        //    });
        //}

        // 파일 다운로드
        {
            FirebaseStorage pStorage = FirebaseStorage.DefaultInstance;

            StorageReference pRootRef = pStorage.GetReferenceFromUrl("gs://kingsadventure-4e10d.appspot.com/");
            StorageReference pSceneRef = pRootRef.Child("/AssetBundles/scene/");
            StorageReference pIntroRef = pSceneRef.Child("intro.scene");
            
            Debug.LogFormat("Root Path : {0}", pRootRef.Path);
            Debug.LogFormat("Root Name : {0}", pRootRef.Name);
            Debug.LogFormat("Root Bucket : {0}", pRootRef.Bucket);

            Debug.LogFormat("Scene Path : {0}", pSceneRef.Path);
            Debug.LogFormat("Scene Name : {0}", pSceneRef.Name);
            Debug.LogFormat("Scene Bucket : {0}", pSceneRef.Bucket);

            Debug.LogFormat("Intro Path : {0}", pIntroRef.Path);
            Debug.LogFormat("Intro Name : {0}", pIntroRef.Name);
            Debug.LogFormat("Intro Bucket : {0}", pIntroRef.Bucket);

            pIntroRef.GetDownloadUrlAsync().ContinueWith((Task<Uri> pTask) =>
            {
                Debug.Log("Done GetDownloadUrl");

                if ((false == pTask.IsFaulted) && (false == pTask.IsCanceled))
                {
                    Debug.LogFormat("Download Path is {0}", pTask.Result);

                    Single.Coroutine.WWW((pWWW) => 
                    {
                        Debug.Log("Intro Scene Bundle Download Complate");
                    }, WWW.LoadFromCacheOrDownload(pTask.Result.Host, 0));
                }
            });
        }
    }
}