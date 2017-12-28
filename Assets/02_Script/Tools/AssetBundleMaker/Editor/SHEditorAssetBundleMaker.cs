#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using System;
using System.Collections;

public class SHEditorAssetBundleMaker : EditorWindow
{
    #region Members
    public enum PlatformType { ALL, AOS, IOS, PC };

    private static EditorWindow m_pEditorWindow;
    private static string       m_strOutputPath;
    private static int          m_iSelPlatformType;
    private static bool         m_bIsDeleteOriginal;
    #endregion


    #region System Functions
    // 시스템 : GUI 업데이트
    void OnGUI()
    {
        GUILayout.Label("Asset Bundle Maker", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal("Box");
        {
            m_iSelPlatformType  = GetPlatformType();
            m_strOutputPath     = GetOutputPath();
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        {
            m_bIsDeleteOriginal = IsDeleteOriginalResource();
        }
        EditorGUILayout.Space();

        StartPacking(m_iSelPlatformType, GetPackingType(), m_bIsDeleteOriginal, m_strOutputPath);
    }
    #endregion


    #region Interface Functions
    // 시스템 : 메뉴 선택
    [MenuItem("SHTools/AssetBundleMaker", false, 200)]
    [MenuItem("Assets/AssetBundleMaker", false, 200)]
    static void SelectMenu()
    {
        m_pEditorWindow = EditorWindow.GetWindow(typeof(SHEditorAssetBundleMaker));
        m_pEditorWindow.autoRepaintOnSceneChange = true;
        m_pEditorWindow.ShowUtility();

        m_strOutputPath     = SHPath.GetPathToExportAssetBundle();
        m_iSelPlatformType  = 0;
        m_bIsDeleteOriginal = false;
    }
    #endregion


    #region Utility Functions
    // 유틸 : 플랫폼 타입 얻기
    int GetPlatformType()
    {
        GUILayout.BeginVertical("Box");
        GUILayout.Label("Select Platform", EditorStyles.boldLabel);
        m_iSelPlatformType = GUILayout.SelectionGrid(m_iSelPlatformType, 
            new string[] {  PlatformType.ALL.ToString(),
                            PlatformType.AOS.ToString(),
                            PlatformType.IOS.ToString(),
                            PlatformType.PC.ToString() }, 1);
        GUILayout.EndVertical();
    
        return m_iSelPlatformType;
    }

    // 유틸 : 저장경로 얻기
    string GetOutputPath()
    {
        GUILayout.BeginVertical("Box");
        GUILayout.Label("Select Output Path", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(m_strOutputPath, MessageType.None);

        if (true == GUILayout.Button("Defualt Output Path"))
            m_strOutputPath = SHPath.GetPathToExportAssetBundle();

        if (true == GUILayout.Button("Change Output Path"))
        {
            string strChangePath = EditorUtility.OpenFolderPanel("Select Bundle Save Folder", "", "");
            if (false == string.IsNullOrEmpty(strChangePath))
                m_strOutputPath = strChangePath;
        }

        if (true == GUILayout.Button("Open Output Path"))
            SHUtils.OpenInFileBrowser(m_strOutputPath);

        GUILayout.EndVertical();
        return m_strOutputPath;
    }

    // 유틸 : 패킹 타입 얻기
    eBundlePackType GetPackingType()
    {
        if (true == GUILayout.Button("Make AssetBundles( All Packing )"))
            return eBundlePackType.All;

        if (true == GUILayout.Button("Make AssetBundles( Update Packing )"))
            return eBundlePackType.Update;

        return eBundlePackType.None;
    }

    // 유틸 : 원본 리소스 제거 유무
    bool IsDeleteOriginalResource()
    {
        return (m_bIsDeleteOriginal = 
                GUILayout.Toggle(m_bIsDeleteOriginal, "Delete Original Resource"));
    }

    // 유틸 : 번들패킹 명령
    void StartPacking(int iPlatformType, eBundlePackType ePackType, bool bIsDeleteOriginal, string strOutputPath)
    {
        if (eBundlePackType.None == ePackType)
            return;

        switch ((PlatformType)iPlatformType)
        {
            case PlatformType.AOS:  SHAssetBundleMaker.PackingAssetBundle(BuildTarget.Android,            ePackType, bIsDeleteOriginal, strOutputPath); break;
            case PlatformType.IOS:  SHAssetBundleMaker.PackingAssetBundle(BuildTarget.iOS,                ePackType, bIsDeleteOriginal, strOutputPath); break;
            case PlatformType.PC:   SHAssetBundleMaker.PackingAssetBundle(BuildTarget.StandaloneWindows,  ePackType, bIsDeleteOriginal, strOutputPath); break;
            case PlatformType.ALL:
                                    SHAssetBundleMaker.PackingAssetBundle(BuildTarget.StandaloneWindows,  ePackType, bIsDeleteOriginal, strOutputPath);
                                    SHAssetBundleMaker.PackingAssetBundle(BuildTarget.iOS,                ePackType, bIsDeleteOriginal, strOutputPath);
                                    SHAssetBundleMaker.PackingAssetBundle(BuildTarget.Android,            ePackType, bIsDeleteOriginal, strOutputPath);
                                    break;
        }
    }
    #endregion
}
#endif