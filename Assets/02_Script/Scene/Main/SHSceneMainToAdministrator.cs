﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Threading;
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

            // URL로 다운로드
            pIntroRef.GetDownloadUrlAsync().ContinueWith((Task<Uri> pTask) =>
            {
                Debug.Log("Done GetDownloadUrl");

                if ((false == pTask.IsFaulted) && (false == pTask.IsCanceled))
                {
                    Debug.LogFormat("Download Path is {0}", pTask.Result);

                    Single.Coroutine.WWW((pWWW) => 
                    {
                        Debug.Log("Intro Scene Bundle Download Complate");
                    }, WWW.LoadFromCacheOrDownload(pTask.Result.AbsoluteUri, 0));
                }
            });

            // Byte 배열로 다운로드
            //const long lMaxAllowedSize = 1 * 1024 * 1024;
            //pIntroRef.GetBytesAsync(lMaxAllowedSize).ContinueWith((Task<byte[]> pTask) =>
            //{
            //    if ((false == pTask.IsFaulted) && (false == pTask.IsCanceled))
            //    {
            //        byte[] byteFileContents = pTask.Result;
            //        Debug.Log("Finished downloading!");
            //    }
            //    else
            //    {
            //        Debug.Log(pTask.Exception.ToString());
            //    }
            //});

            // 스트리밍으로 다운로드
            //pIntroRef.GetStreamAsync((pStream) =>
            //{
            //    Debug.Log("Finished downloading!");
            //}, null, CancellationToken.None);
            
            // 로컬파일로 다운로드
            //string strLocalURL = "file:///local/scene/intro.scene";
            //pIntroRef.GetFileAsync(strLocalURL).ContinueWith(pTask =>
            //{
            //    if ((false == pTask.IsFaulted) && (false == pTask.IsCanceled))
            //    {
            //        Debug.Log("File downloaded.");
            //    }
            //});
        }
    }
}