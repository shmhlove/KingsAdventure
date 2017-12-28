#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SHEditorResourcesLister : Editor
{
    #region Members
    public static string m_strMsg_1 = "리소스 폴더를 뒤질껍니다!!\n\n완료되면 완료 메시지 출력됩니다.\n\n조금만 기다려 주세요!! 싫으면 취소...ㅠ";
    public static string m_strMsg_2 = "선택한 리소스를 뒤질껍니다!!\n\n완료되면 완료 메시지 출력됩니다.\n\n조금만 기다려 주세요!! 싫으면 취소...ㅠ";
    public static string m_strMsg_3 = "{0}개의 리소스 파일이 리스팅 되었습니다.!!\n\n저장경로 : {1}\n\n리스팅 시간 : {2:F2}sec";
    #endregion


    #region Interface Functions
    // 인터페이스 : 리소스 폴더 전체파일 리스팅 : ResourcesTable.json, AssetBundleInfo.json, DuplicationResourcesList.txt
    [MenuItem("SHTools/Resources Listing/All Files In Resources Folder", false, 0)]
    [MenuItem("Assets/SHTools/Resources Listing/All Files In ResourcesFolder", false, 0)]
    static void AllFilsInResourcesFolderWithAssetBundleInfo()
    {
        // 시작팝업
        if (false == ShowDialog("[SHTools] Resources Listing",
                                SHEditorResourcesLister.m_strMsg_1, 
                                "확인", "취소"))
            return;

        // 알리아싱
        var pStartTime          = DateTime.Now;
        var strSaveResourcePath = string.Format("{0}/{1}", SHPath.GetPathToJson(), "ResourcesTable.json");
        var strSaveBundlePath   = string.Format("{0}/{1}", SHPath.GetPathToJson(), "AssetBundleInfo.json");
        var pLister             = new SHResourcesLister();

        // 리스팅
        int iFileCount = pLister.SetListing(SHPath.GetPathToResources());
        SHResourcesLister.SaveToResources(pLister.m_dicResources, strSaveResourcePath);
        SHResourcesLister.SaveToAssetBundleInfo(pLister.m_dicAssetBundles, strSaveBundlePath);
        SHResourcesLister.SaveToDuplicationList(pLister.m_dicDuplications, string.Format("{0}/{1}", SHPath.GetPathToJson(), "DuplicationResourcesList.txt"));

        // 종료팝업
        if (true == ShowDialog("[SHTools] Resources Listing",
                    string.Format(SHEditorResourcesLister.m_strMsg_3,
                    iFileCount, strSaveResourcePath, ((DateTime.Now - pStartTime).TotalMilliseconds / 1000.0)), 
                    "파일확인", "닫기"))
            System.Diagnostics.Process.Start(strSaveResourcePath);
    }

    // 인터페이스 : 선택한 파일 리스팅 : SelectFiles.txt
    [MenuItem("Assets/SHTools/Resources Listing/Select Files", false, 0)]
    static void SelectFiles()
    {
        // 시작팝업
        if (false == ShowDialog("[SHTools] Resources Listing",
                                SHEditorResourcesLister.m_strMsg_2,
                                "확인", "취소"))
            return;

        // 선택 오브젝트 체크
        var pObjects = Selection.objects;
        if ((null == pObjects) || (0 == pObjects.Length))
        {
            ShowDialog("[SHTools] Resources Listing", "선택된 오브젝트가 없습니다.", "확인");
            return;
        }

        // 알리아싱
        int iFileCount  = 0;
        var pStartTime  = DateTime.Now;
        var pLister     = new SHResourcesLister();

        // 절대경로처리
        var strAbsolutePath = SHPath.GetPathToAssets();
        strAbsolutePath = strAbsolutePath.Substring(0, (strAbsolutePath.IndexOf("Assets") - 1)).Replace("\\", "/");

        // 리스팅
        for (int iLoop = 0; iLoop < pObjects.Length; ++iLoop)
        {
            iFileCount += pLister.SetListing(
                string.Format("{0}/{1}", strAbsolutePath, AssetDatabase.GetAssetPath(pObjects[iLoop])));
        }
        var strSavePath = string.Format("{0}/{1}", strAbsolutePath, "SelectFiles.txt");
        SHResourcesLister.SaveToResources(pLister.m_dicResources, strSavePath);

        // 종료팝업
        ShowDialog("[SHTools] Resources Listing",
            string.Format(SHEditorResourcesLister.m_strMsg_3,
            iFileCount, strSavePath, ((DateTime.Now - pStartTime).TotalMilliseconds / 1000.0)),
            "확인");

        System.Diagnostics.Process.Start(strSavePath);
    }

    // 인터페이스 : 선택한 파일 번들포맷으로 리스팅 : SelectFilesBySeparateBundleFormat.txt
    [MenuItem("Assets/SHTools/Resources Listing/Select Files By Separate BundleFormat", false, 0)]
    static void SelectFilesBySeparateBundleFormat()
    {
        // 시작팝업
        if (false == ShowDialog("[SHTools] Resources Listing",
                                SHEditorResourcesLister.m_strMsg_2,
                                "확인", "취소"))
            return;

        // 선택 오브젝트 체크
        var pObjects = Selection.objects;
        if ((null == pObjects) || (0 == pObjects.Length))
        {
            ShowDialog("[SHTools] Resources Listing", "선택된 오브젝트가 없습니다.", "확인");
            return;
        }

        // 알리아싱
        int iFileCount = 0;
        var pStartTime = DateTime.Now;
        var pLister     = new SHResourcesLister();

        // 절대경로처리
        var strAbsolutePath = SHPath.GetPathToAssets();
        strAbsolutePath = strAbsolutePath.Substring(0, (strAbsolutePath.IndexOf("Assets") - 1)).Replace("\\", "/");

        // 리스팅
        for (int iLoop = 0; iLoop < pObjects.Length; ++iLoop)
        {
            iFileCount += pLister.SetListing(
                string.Format("{0}/{1}", strAbsolutePath, AssetDatabase.GetAssetPath(pObjects[iLoop])));
        }
        var strSavePath = string.Format("{0}/{1}", strAbsolutePath, "SelectFilesBySeparateBundleFormat.txt");
        SHResourcesLister.SaveToResourcesOfBundleFormat(pLister.m_dicResources, strSavePath);

        // 종료팝업
        ShowDialog("[SHTools] Resources Listing",
            string.Format(SHEditorResourcesLister.m_strMsg_3,
            iFileCount, strSavePath, ((DateTime.Now - pStartTime).TotalMilliseconds / 1000.0)),
            "확인");

        System.Diagnostics.Process.Start(strSavePath);
    }
    #endregion


    #region Utility Functions
    // 유틸 : 팝업
    static bool ShowDialog(string strTitle, string strMessage, string strOkBtn, string strCancleBtn = "")
    {
        if (false == string.IsNullOrEmpty(strCancleBtn))
            return EditorUtility.DisplayDialog(strTitle, strMessage, strOkBtn, strCancleBtn);
        else
            return EditorUtility.DisplayDialog(strTitle, strMessage, strOkBtn);
    }
    #endregion
}
#endif