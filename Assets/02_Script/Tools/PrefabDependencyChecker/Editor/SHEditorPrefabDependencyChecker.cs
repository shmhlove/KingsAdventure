#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SHEditorPrefabDependencyChecker : Editor 
{
    // 인터페이스 : 선택한 파일 혹은 폴더에서 Prefab 종속 검사
    [MenuItem("Assets/SHTools/Check Prefab Dependency", false, 1)]
    static void SelectFiles()
    {
        // 시작팝업
        if (false == ShowDialog("[SHTools] Prefab Dependency Check",
                                "리소스 폴더 내에 있는 원본 리소스들이\n선택한 프리팹에 종속되어 있는지 체크합니다.",
                                "확인", "취소"))
            return;

        // 선택 오브젝트 체크
        var pObjects = new List<UnityEngine.Object>(Selection.objects);
        if (0 == pObjects.Count)
        {
            ShowDialog("[SHTools] Resources Listing", "선택된 프리팹이 없습니다.", "확인");
            return;
        }

        // 알리아싱
        var pChecker = new SHPrefabDependencyChecker();

        // 절대경로처리
        var strAbsolutePath = SHPath.GetPathToAssets();
        strAbsolutePath = strAbsolutePath.Substring(0, (strAbsolutePath.IndexOf("Assets") - 1)).Replace("\\", "/");

        // 종속체크
        string strBuff = string.Empty;
        SHUtils.ForToList(pObjects, (pObject) =>
        {
            string strSearchPath = string.Format("{0}/{1}", strAbsolutePath, AssetDatabase.GetAssetPath(pObject));
            SHUtils.Search(strSearchPath, (pFileInfo) =>
            {
                var pDependencys = pChecker.GetDependency(pFileInfo.FullName);
                if (null == pDependencys)
                    return;

                strBuff += string.Format("< Prefab : {0} >\n", Path.GetFileNameWithoutExtension(pFileInfo.FullName));
                SHUtils.ForToList(pDependencys, (pDependency) =>
                {
                    strBuff += string.Format("    Dependency : {0}\n", pDependency);
                });
            });
        });

        string strSavePath = string.Format("{0}/{1}", strAbsolutePath, "DependencyList.txt");
        SHUtils.SaveFile(strBuff, strSavePath);
        System.Diagnostics.Process.Start(strSavePath);
    }

    // 유틸 : 팝업
    static bool ShowDialog(string strTitle, string strMessage, string strOkBtn, string strCancleBtn = "")
    {
        if (false == string.IsNullOrEmpty(strCancleBtn))
            return EditorUtility.DisplayDialog(strTitle, strMessage, strOkBtn, strCancleBtn);
        else
            return EditorUtility.DisplayDialog(strTitle, strMessage, strOkBtn);
    }
}
#endif