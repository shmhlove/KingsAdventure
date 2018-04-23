using UnityEngine;
using UnityEngine.SocialPlatforms;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Firebase;
using Firebase.Storage;

public class SHFirebaseStorage
{
    public void OnInitialize()
    {
        Debug.LogErrorFormat("[SHFirebaseStorage] Call is OnInitialize");
    }

    public void OnFinalize()
    {
        Debug.LogErrorFormat("[SHFirebaseStorage] Call is OnFinalize");
    }

    public void GetFileURL(string strFilePath, Action<string> pCallback)
    {
        var pClientConfig = Single.Table.GetTable<JsonClientConfig>();
        FirebaseStorage pStorage = FirebaseStorage.DefaultInstance;

        StorageReference pRootRef = pStorage.GetReferenceFromUrl(pClientConfig.FB_StorageBaseURL);
        StorageReference pFileRef = pRootRef.Child(strFilePath);

        // URL 다운로드
        pFileRef.GetDownloadUrlAsync().ContinueWith((Task<Uri> pTask) =>
        {
            if ((false == pTask.IsFaulted) && (false == pTask.IsCanceled))
                pCallback(pTask.Result.OriginalString);
            else
                pCallback(string.Empty);
        });
    }

    public void Test()
    {
        // 파일 다운로드
        var pClientConfig = Single.Table.GetTable<JsonClientConfig>();
        var pStorage = FirebaseStorage.DefaultInstance;

        string strPlatform = SHHard.GetPlatformStringByEnum(Single.AppInfo.GetRuntimePlatform());
        StorageReference pRootRef = pStorage.GetReferenceFromUrl(pClientConfig.FB_StorageBaseURL);
        StorageReference pSceneRef = pRootRef.Child(string.Format("/{0}/AssetBundle/scene/", strPlatform));
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
                }, WWW.LoadFromCacheOrDownload(pTask.Result.OriginalString, 0));
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