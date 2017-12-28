#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SHPrefabDependencyChecker
{
    #region Members
    Dictionary<string, string> m_dicGUID = new Dictionary<string, string>();
    #endregion


    #region Interface Functions
    // 인터페이스 : 원본 리소스들의 GUID를 기록한다.
    public void ReadyGUID()
    {
        m_dicGUID.Clear();
        SHUtils.Search(SHPath.GetPathToResources(), (pFileInfo) =>
        {
            string strExtension = Path.GetExtension(pFileInfo.FullName);
            if (".meta" == strExtension.ToLower())
                return;

            if (true == m_dicGUID.ContainsKey(pFileInfo.FullName))
                return;

            string strRoot = "Assets";
            string strFullName = pFileInfo.FullName.Substring(pFileInfo.FullName.IndexOf(strRoot)).Replace("\\", "/");

            string strGUID = AssetDatabase.AssetPathToGUID(strFullName);
            m_dicGUID.Add(strFullName, strGUID);
        });
    }

    // 인터페이스 : 프리팹에 종속이 걸린 원본리소스 리스트 얻기
    public List<string> GetDependency(string strPrefabPath)
    {
        string strExtension = Path.GetExtension(strPrefabPath);
        if (".prefab" != strExtension.ToLower())
            return null;

        if (0 == m_dicGUID.Count)
            ReadyGUID();

        var pResult     = new List<string>();
        var strPrefab   = SHUtils.ReadFile(strPrefabPath);
        SHUtils.ForToDic(m_dicGUID, (pKey, pValue) =>
        {
            int iIndex = strPrefab.IndexOf(pValue);
            if (-1 == iIndex)
                return;

            pResult.Add(pKey);
        });

        return (0 == pResult.Count) ? null : pResult;
    }
    #endregion
}
#endif