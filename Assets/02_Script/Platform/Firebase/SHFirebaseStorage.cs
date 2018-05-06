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
    public const string BUNDLE_SCENE = "AssetBundle/scene/{0}.scene";

    public void OnInitialize()
    {
        Debug.LogErrorFormat("[SHFirebaseStorage] Call is OnInitialize");
    }

    public void OnFinalize()
    {
        Debug.LogErrorFormat("[SHFirebaseStorage] Call is OnFinalize");
    }
    
    public void GetFileURL(string strFilePath, Action<SHReply> pCallback)
    {
        var ePlatform = Single.AppInfo.GetRuntimePlatform();
        {
            if (RuntimePlatform.WindowsEditor == ePlatform)
                ePlatform = RuntimePlatform.Android;
            if (RuntimePlatform.OSXEditor == ePlatform)
                ePlatform = RuntimePlatform.IPhonePlayer;
        }
        var strPlatform = SHHard.GetPlatformStringByEnum(ePlatform);

        var pClientConfig = Single.Table.GetTable<JsonClientConfig>();
        var pStorage = FirebaseStorage.DefaultInstance;

        var pRootRef = pStorage.GetReferenceFromUrl(pClientConfig.FB_StorageBaseURL);
        var pFileRef = pRootRef.Child(string.Format("{0}/{1}", strPlatform, strFilePath));

        pFileRef.GetDownloadUrlAsync().ContinueWith((Task<Uri> pTask) =>
        {
            if ((false == pTask.IsFaulted) && (false == pTask.IsCanceled))
                pCallback(new Firebase.Storage.SHReplyGetFileURL(pTask.Result.OriginalString));
            else
                pCallback(new SHReply(new SHError(eErrorCode.Failed, "")));
        });
    }

    public void DownloadForBundle(eSceneType eType, Action<SHReply> pCallback)
    {
        DownloadForBundle(string.Format(SHFirebaseStorage.BUNDLE_SCENE, eType.ToString().ToLower()), pCallback);
    }
    public void DownloadForBundle(string strFilePath, Action<SHReply> pCallback)
    {
        GetFileURL(strFilePath, (pReply) =>
        {
            var pAsReply = pReply.GetAs<Firebase.Storage.SHReplyGetFileURL>();
            if (false == pAsReply.IsSucceed)
            {
                pCallback(new SHReply(pAsReply.Error));
                return;
            }

            Single.Coroutine.WWW((pWWW) =>
            {
                if (null == pWWW.assetBundle)
                {
                    pCallback(new SHReply(new SHError(eErrorCode.Failed, pWWW.error)));
                }
                else
                {
                    pCallback(new SHReplyDownloadForBundle(pWWW));
                }
            }, WWW.LoadFromCacheOrDownload(pAsReply.m_strURL, 0));
        });
    }
}

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