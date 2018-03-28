using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

class SHBuildScript
{
    #region Members
    static string[] SCENES    = FindEnabledEditorScenes();
    #endregion


    #region Android Build
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [MenuItem("SHTools/CI/Android AppBuild For Korea")]
	static void KOR_AndroidAppBuild()
    { AppBuild(eNationType.Korea, BuildTarget.Android, eServiceMode.QA, BuildOptions.None);   }
    
    [MenuItem("SHTools/CI/Android AssetBundles Packing")]
	static void AndroidAssetBundlesPacking()
    { AssetBundlesPacking(BuildTarget.Android, eBundlePackType.All);  }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #endregion


    #region iOS Build
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [MenuItem("SHTools/CI/iOS AppBuild For Korea")]
	static void KOR_iOSAppBuild()
    { AppBuild(eNationType.Korea, BuildTarget.iOS, eServiceMode.QA, BuildOptions.AcceptExternalModificationsToPlayer);   }
    
    [MenuItem("SHTools/CI/iOS AssetBundles Packing")]
	static void iOSAssetBundlesPacking()
    { AssetBundlesPacking(BuildTarget.iOS, eBundlePackType.All);  }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #endregion
    

    #region Utility Functions
    static void AppBuild(eNationType eNation, BuildTarget eTarget, eServiceMode eMode, BuildOptions eOption)
    {
        SetNationInfo(eNation, eMode);
        SetBuildTargetInfo(eTarget);
		BuildApplication(SCENES, eTarget, eOption);
        PostProcessor(eTarget);
    }
    
    static void AssetBundlesPacking(BuildTarget eTarget, eBundlePackType ePackType)
    {
        PackingAssetBundles(eTarget, ePackType, true);
        PostProcessor(eTarget);
    }
    
    static void SetNationInfo(eNationType eNation, eServiceMode eMode)
    {
        UpdateClientConfig(GetServerConfigPath(eNation), eMode);
    }
    
    static void SetBuildTargetInfo(BuildTarget eTarget)
    {
        switch(eTarget)
        {
            case BuildTarget.Android:
                PlayerSettings.Android.keystoreName = Path.GetFullPath(Path.Combine(Application.dataPath, "../GoogleKeyStore/kingsadventure.keystore"));
                PlayerSettings.Android.keystorePass = "lee35235";
                PlayerSettings.Android.keyaliasName = "KingsAdventure";
                PlayerSettings.Android.keyaliasPass = "lee35235";
                PlayerSettings.Android.bundleVersionCode = 1;
                EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ETC;
                break;
            case BuildTarget.iOS:
                EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.PVRTC;
                break;
        }

        PlayerSettings.bundleIdentifier = "com.mangogames.kingsadventure.dev";
    }
    
	static void BuildApplication(string[] strScenes, BuildTarget eTarget, BuildOptions eOptions)
    {
        string strBuildName = GetBuildName(eTarget, Single.AppInfo.GetAppName(), Single.Table.GetClientVersion());
        Debug.LogFormat("** Build Start({0}) -> {1}", strBuildName, DateTime.Now.ToString("yyyy-MM-dd [ HH:mm:ss ]"));
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(eTarget);

            string strFileName = string.Format("{0}/{1}", SHPath.GetPathToBuild(), strBuildName);

			if (false == SHUtils.IsExistsDirectory(strFileName))
				eOptions = BuildOptions.None;

            SHUtils.CreateDirectory(strFileName);

			string strResult = BuildPipeline.BuildPlayer(strScenes, strFileName, eTarget, eOptions);
            if (0 < strResult.Length)
                throw new Exception("BuildPlayer failure: " + strResult);
        }
        Debug.LogFormat("** Build End({0}) -> {1}", strBuildName, DateTime.Now.ToString("yyyy-MM-dd [ HH:mm:ss ]"));
    }
    
    static void PackingAssetBundles(BuildTarget eTarget, eBundlePackType eType, bool bIsDelOriginal)
    {
        Debug.LogFormat("** AssetBundles Packing Start({0}) -> {1}", eTarget, DateTime.Now.ToString("yyyy-MM-dd [ HH:mm:ss ]"));
        {
            SHUtils.CreateDirectory(SHPath.GetPathToExportAssetBundle());
            BuildPipeline.BuildAssetBundles(SHPath.GetPathToExportAssetBundle(), BuildAssetBundleOptions.None, eTarget);
        }
        Debug.LogFormat("** AssetBundles Packing End({0}) -> {1}", eTarget, DateTime.Now.ToString("yyyy-MM-dd [ HH:mm:ss ]"));
    }

    static void UpdateClientConfig(string strServerConfigPath, eServiceMode eMode)
    {
        var pConfigFile = Single.Table.GetTable<JsonClientConfig>();
        
        pConfigFile.SetServiceMode(eMode.ToString());
        pConfigFile.SetServerConfigPath(strServerConfigPath);
        pConfigFile.SaveJsonFile();
    }

    static string GetBuildName(BuildTarget eTarget, string strAppName, string strVersion)
    {
        if (BuildTarget.Android == eTarget)
            return string.Format("{0}.apk", strAppName);
        else
            return "xcode";
    }

    static string GetServerConfigPath(eNationType eNation)
    {
        return string.Empty;
        //return string.Format("{0}/{1}/{2}/ServerConfig.json", 
        //    "http://blueasa.synology.me/home/shmhlove", SHHard.GetNationByEnum(eNation), Application.productName);
    }

    static string[] FindEnabledEditorScenes()
    {
        var pScenes   = new List<string>();
        var iMaxCount = EditorBuildSettings.scenes.Length;
        for (int iLoop = 0; iLoop < iMaxCount; ++iLoop)
        {
            var pScene = EditorBuildSettings.scenes[iLoop];
            if (false == pScene.enabled)
                continue;

            pScenes.Add(pScene.path);
        }
        
        return pScenes.ToArray();
    }

    static void PostProcessor(BuildTarget eTarget)
    {
        SHGameObject.DestoryObject(GameObject.Find("SHSingletons(Destroy)"));
        SHGameObject.DestoryObject(GameObject.Find("SHSingletons(DontDestroy)"));
    }
    #endregion
}