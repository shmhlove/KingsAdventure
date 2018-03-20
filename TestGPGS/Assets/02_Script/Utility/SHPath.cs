using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Net;
using System.Collections;

public static partial class SHPath
{
    // 경로 : 서버 URL
    public static string GetURLToServer()
    {
        return string.Empty;
        //return Single.Table.GetServerURL();
    }

    // 경로 : Configuration CDN 주소
    public static string GetURLToServerConfig()
    {
        return Single.Table.GetServerConfigURL();
    }
    
    // 경로 : 번들 CDN 주소
    public static string GetURLToBundleCDN()
    {
        return Single.Table.GetBundleCDN();
    }

    // 경로 : 번들 CDN/플랫폼식별자 주소
    public static string GetURLToBundleCDNWithPlatform()
    {
        return string.Empty;
        //return string.Format("{0}/{1}", GetURLToBundleCDN(), Single.AppInfo.GetStrToRuntimePlatform());
    }
    
    // 경로 : (Root)
    public static string GetPathToRoot()
    {
        return Application.dataPath.Substring(0, Application.dataPath.IndexOf("Assets") - 1);
    }

    // 경로 : (Root : Assets)
    public static string GetPathToAssets()
    {
        return Application.dataPath;
    }

    // 경로 : (Root : Assets/Resources)
    public static string GetPathToResources()
    {
        return string.Format("{0}{1}", GetPathToAssets(), "/Resources");
    }

    // 경로 : (Root : Assets/StreamingAssets)
    public static string GetPathToStreamingAssets()
    {
        return Application.streamingAssetsPath;
    }

    // 경로 : (Root : Build)
    public static string GetPathToBuild()
    {
        return string.Format("{0}{1}", GetPathToRoot(), "/Build");
    }

    // 경로 : (Root : Build/AssetBundles)
    public static string GetPathToExportAssetBundle()
    {
        return string.Format("{0}{1}", GetPathToBuild(), "/AssetBundles");
    }

#if UNITY_EDITOR
    // 경로 : (Root : Root/GetPathToAssetBundlesMakeInfo/플랫폼/)
    public static string GetPathToAssetBundlesMakeInfo(BuildTarget eTarget)
    {
        return string.Format("{0}/{1}/{2}", GetPathToRoot(), "AssetBundlesInfo", SHHard.GetPlatformStringByEnum(eTarget));
    }
#endif

    // 경로 : (Root : 사용자디렉토리/AppData/LocalLow/회사이름/프로덕트이름/플랫폼)
    public static string GetPathToPersistentData()
    {
#if UNITY_EDITOR
        return string.Format("{0}/{1}", Application.persistentDataPath, SHHard.GetPlatformStringByEnum(EditorUserBuildSettings.activeBuildTarget));
#else
        return string.Format("{0}/{1}", Application.persistentDataPath, SHHard.GetPlatformStringByEnum(Single.AppInfo.GetRuntimePlatform()));
#endif
    }

    // 경로 : (Root : 사용자디렉토리/AppData/LocalLow/회사이름/프로덕트이름/AssetBundle)
    public static string GetPathToPersistentAssetBundle()
    {
        return string.Format("{0}/{1}", SHPath.GetPathToPersistentData(), "AssetBundle");
    }

    // 경로 : (Root : 사용자디렉토리/AppData/LocalLow/회사이름/프로덕트이름/Byte)
    public static string GetPathToPersistentByte()
    {
        return string.Format("{0}/{1}", SHPath.GetPathToPersistentData(), "Byte");
    }

    // 경로 : (Root : 사용자디렉토리/AppData/LocalLow/회사이름/프로덕트이름/Json)
    public static string GetPathToPersistentJson()
    {
        return string.Format("{0}/{1}", SHPath.GetPathToPersistentData(), "Json");
    }

    // 경로 : (Root : 사용자디렉토리/AppData/LocalLow/회사이름/프로덕트이름/XML)
    public static string GetPathToPersistentXML()
    {
        return string.Format("{0}/{1}", SHPath.GetPathToPersistentData(), "XML");
    }

    // 경로 : (Root : 사용자디렉토리/AppData/LocalLow/회사이름/프로덕트이름/SQLite)
    public static string GetPathToPersistentSQLite()
    {
        return string.Format("{0}/{1}", SHPath.GetPathToPersistentData(), "SQLite");
    }

    // 경로 : (Root : Assets/Resources/Table/XML)
    public static string GetPathToXML()
    {
        return string.Format("{0}/{1}/{2}", SHPath.GetPathToResources(), "Table", "XML");
    }

    // 경로 : (Root : Assets/Resources/Table/Bytes)
    public static string GetPathToBytes()
    {
        return string.Format("{0}/{1}/{2}", SHPath.GetPathToResources(), "Table", "Bytes");
    }

    // 경로 : (Root : Assets/StreamingAssets/SQLite)
    public static string GetPathToSQLite()
    {
        return string.Format("{0}/{1}", SHPath.GetPathToStreamingAssets(), "SQLite");
    }

    // 경로 : (Root : Assets/StreamingAssets/JSons)
    public static string GetPathToJson()
    {
        return string.Format("{0}/{1}", SHPath.GetPathToStreamingAssets(), "JSons");
    }
}