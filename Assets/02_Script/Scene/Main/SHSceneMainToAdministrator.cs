using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Text;
using System.Collections;
//using System.Threading.Tasks;

//using Firebase.Storage;

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
        // {
        //     // Get a reference to the storage service, using the default Firebase App
        //     FirebaseStorage pStorage = FirebaseStorage.DefaultInstance;
            
        //     // This is equivalent to creating the full reference
        //     StorageReference pSpaceRef = pStorage.GetReferenceFromUrl(
        //         "gs://kingsadventure-c004a.appspot.com/AssetBundles/scene/intro.scene");
            
        //     // Create a reference from an HTTPS URL
        //     // Note that in the URL, characters are URL escaped!
        //     StorageReference pHttpsRef = pStorage.GetReferenceFromUrl(
        //         "https://firebasestorage.googleapis.com/b/bucket/o/AssetBundles%20scene%20intro.scene");

        //     // Fetch the download URL
        //     pHttpsRef.GetDownloadUrlAsync().ContinueWith((Task<Uri> pTask) =>
        //     {
        //         if (!pTask.IsFaulted && !pTask.IsCanceled)
        //         {
        //             Debug.Log("Download URL: " + pTask.Result);

        //             // ... now download the file via WWW or UnityWebRequest.
        //             Single.Coroutine.WWW((pWWW) => 
        //             {
        //                 Debug.Log("Download Complate");
        //             }, WWW.LoadFromCacheOrDownload(pTask.Result.Host, 0));
                    
        //         }
        //     });
        // }
    }
}