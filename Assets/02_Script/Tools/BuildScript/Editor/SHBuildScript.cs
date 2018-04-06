using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

class SHBuildScript
{
    static string[] SCENES    = FindEnabledEditorScenes();
    
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
    
    static void AppBuild(eNationType eNation, BuildTarget eTarget, eServiceMode eMode, BuildOptions eOption)
    {
        PreProcessor(eTarget);
		BuildApplication(SCENES, eTarget, eOption);
        PostProcessor(eTarget);
    }
    
    static void AssetBundlesPacking(BuildTarget eTarget, eBundlePackType ePackType)
    {
        PackingAssetBundles(eTarget, ePackType);
        PostProcessor(eTarget);
    }
    
    static void PreProcessor(BuildTarget eTarget)
    {
        var pConfigFile = Single.Table.GetTable<JsonClientConfig>();
            
        switch (eTarget)
        {
            case BuildTarget.Android:
                PlayerSettings.Android.keystoreName = string.Format("{0}/{1}", SHPath.GetRoot(), pConfigFile.AOS_KeyStoreName);
                PlayerSettings.Android.keystorePass = pConfigFile.AOS_KeyStorePass;
                PlayerSettings.Android.keyaliasName = pConfigFile.AOS_KeyAliasName;
                PlayerSettings.Android.keyaliasPass = pConfigFile.AOS_KeyAliasPass;
                EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ETC;
                break;
            case BuildTarget.iOS:
                EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.PVRTC;
                break;
        }

        PlayerSettings.bundleVersion = pConfigFile.Version;
    }

    static void PostProcessor(BuildTarget eTarget)
    {
        SHGameObject.DestoryObject(GameObject.Find("SHSingletons(Destroy)"));
        SHGameObject.DestoryObject(GameObject.Find("SHSingletons(DontDestroy)"));
    }

    static void BuildApplication(string[] strScenes, BuildTarget eTarget, BuildOptions eOptions)
    {
        string strBuildName = GetBuildName(eTarget, Single.AppInfo.GetProductName());
        Debug.LogFormat("** [SHBuilder] Build Start({0}) -> {1}", strBuildName, DateTime.Now.ToString("yyyy-MM-dd [ HH:mm:ss ]"));
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(eTarget);

            string strExportPath = string.Format("{0}/{1}", SHPath.GetBuild(), strBuildName);
            SHUtils.CreateDirectory(strExportPath);

			string strResult = BuildPipeline.BuildPlayer(strScenes, strExportPath, eTarget, eOptions);
            if (0 < strResult.Length)
            {
                throw new Exception("[SHBuilder] BuildPlayer failure: " + strResult);
            }
        }
        Debug.LogFormat("** [SHBuilder] Build End({0}) -> {1}", strBuildName, DateTime.Now.ToString("yyyy-MM-dd [ HH:mm:ss ]"));
    }
    
    static void PackingAssetBundles(BuildTarget eTarget, eBundlePackType eType)
    {
        Debug.LogFormat("** [SHBuilder] AssetBundles Packing Start({0}) -> {1}", eTarget, DateTime.Now.ToString("yyyy-MM-dd [ HH:mm:ss ]"));
        {
            string strExportPath = string.Format("{0}/{1}", SHPath.GetExportAssetBundle(), SHHard.GetPlatformStringByEnum(eTarget));
            SHUtils.CreateDirectory(strExportPath);

            BuildPipeline.BuildAssetBundles(strExportPath, BuildAssetBundleOptions.None, eTarget);
        }
        Debug.LogFormat("** [SHBuilder] AssetBundles Packing End({0}) -> {1}", eTarget, DateTime.Now.ToString("yyyy-MM-dd [ HH:mm:ss ]"));
    }
    
    static string GetBuildName(BuildTarget eTarget, string strAppName)
    {
        if (BuildTarget.Android == eTarget)
            return string.Format("{0}.apk", strAppName);
        else
            return "xcode";
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
}