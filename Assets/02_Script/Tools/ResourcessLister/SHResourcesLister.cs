using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SHResourcesLister
{
    #region Members
    // 멤버 : 리스팅된 리소스 리스트
    public Dictionary<string, SHResourcesInfo>      m_dicResources      = new Dictionary<string, SHResourcesInfo>();

    // 멤버 : 리스팅된 번들 리스트
    public Dictionary<string, AssetBundleInfo>      m_dicAssetBundles   = new Dictionary<string, AssetBundleInfo>();

    // 멤버 : 중복파일 리스트
    public Dictionary<string, List<string>>         m_dicDuplications   = new Dictionary<string, List<string>>();
    #endregion


    #region Interface Functions
    // 인터페이스 : 초기화
    public void Initialize()
    {
        m_dicResources.Clear();
        m_dicAssetBundles.Clear();
        m_dicDuplications.Clear();
    }

    // 인터페이스 : Resources폴더 내에 있는 파일을 SHResourceTableData형식에 맞게 Json으로 리스팅
    public int SetListing(string strSearchPath)
    {
        SHUtils.Search(strSearchPath, 
        (pFileInfo) => 
        {
            // 파일 정보 생성
            SHResourcesInfo pInfo = MakeFileInfo(pFileInfo);
            if (null == pInfo)
                return;
            
            // 예외체크 : 파일명 중복
            string strDupPath = CheckToDuplication(m_dicResources, pInfo.m_strFileName);
            if (false == string.IsNullOrEmpty(strDupPath))
            {
                string strFirst  = string.Format("{0}", strDupPath);
                string strSecond = string.Format("{0}", pInfo.m_strPath);

#if UNITY_EDITOR
                EditorUtility.DisplayDialog("[SHTools] Resources Listing",
                    string.Format("중복 파일발견!! 파일명은 중복되면 안됩니다!!\r\n1번 파일을 리스팅 하겠습니다.\r\n1번 : {0}\r\n2번 {1}",
                    strFirst, strSecond), "확인");
#endif

                if (false == m_dicDuplications.ContainsKey(pInfo.m_strFileName))
                {
                    m_dicDuplications[pInfo.m_strFileName] = new List<string>();
                    m_dicDuplications[pInfo.m_strFileName].Add(strFirst);
                }

                m_dicDuplications[pInfo.m_strFileName].Add(strSecond);
                return;
            }

            AddResourceInfo(pInfo);
            AddAssetBundleInfo(pInfo);
        });

        return m_dicResources.Count;
    }

    // 인터페이스 : 리소스 리스트를 Json형태로 쓰기
    public static void SaveToResources(Dictionary<string, SHResourcesInfo> dicTable, string strSaveFilePath)
    {
        if (0 == dicTable.Count)
            return;
    
        string strNewLine = "\r\n";
        string strBuff    = "{" + strNewLine;
        
        // 테이블별 내용작성
        strBuff += string.Format("\t\"{0}\": [{1}", "ResourcesInfo", strNewLine);
        SHUtils.ForToDic(dicTable, (pKey, pValue) =>
        {
            strBuff += "\t\t{" + strNewLine;
            strBuff += SHResourcesLister.MakeSaveFormat(pValue, "\t\t");
            strBuff += "\t\t}," + strNewLine;
        });
        strBuff = string.Format("{0}{1}", strBuff.Substring(0, strBuff.Length - (strNewLine.Length + 1)), strNewLine);
        strBuff += string.Format("\t]{0}", strNewLine);
        strBuff += "}";

        // 저장
        SHUtils.SaveFile(strBuff, strSaveFilePath);
    }

    // 인터페이스 : 리소스 리스트를 Json형태로 번들정보파일포맷으로 쓰기
    public static void SaveToResourcesOfBundleFormat(Dictionary<string, SHResourcesInfo> dicTable, string strSaveFilePath)
    {
        if (0 == dicTable.Count)
            return;

        string strNewLine = "\r\n";
        string strBuff = "{" + strNewLine;

        // 테이블별 내용작성
        strBuff += string.Format("\t\"{0}\": [{1}", "ResourcesInfo", strNewLine);
        SHUtils.ForToDic(dicTable, (pKey, pValue) =>
        {
            strBuff += "\t\t{" + strNewLine;

            strBuff += string.Format("\t\t\t\"s_BundleName\": \"{0}\",{1}",
                pValue.m_strName,
                strNewLine);

            strBuff += string.Format("\t\t\t\"s_BundleSize\": \"{0}\",{1}",
                0,
                strNewLine);

            strBuff += string.Format("\t\t\t\"s_BundleHash\": \"{0}\",{1}",
                0,
                strNewLine);

            strBuff += string.Format("\t\t\t\"p_Resources\": {0}", strNewLine);
            strBuff += "\t\t\t[" + strNewLine;
            strBuff += "\t\t\t\t{" + strNewLine;
            strBuff += SHResourcesLister.MakeSaveFormat(pValue, "\t\t\t\t");
            strBuff += "\t\t\t\t}" + strNewLine;
            strBuff += "\t\t\t]" + strNewLine;
            strBuff += "\t\t}," + strNewLine;
        });
        strBuff = string.Format("{0}{1}", strBuff.Substring(0, strBuff.Length - (strNewLine.Length + 1)), strNewLine);
        strBuff += string.Format("\t]{0}", strNewLine);
        strBuff += "}";

        // 저장
        SHUtils.SaveFile(strBuff, strSaveFilePath);
    }

    // 인터페이스 : 번들 리스트를 Json형태로 번들정보파일포맷으로 쓰기
    public static void SaveToAssetBundleInfo(Dictionary<string, AssetBundleInfo> dicTable, string strSaveFilePath)
    {
        if (0 == dicTable.Count)
            return;

        string strNewLine = "\r\n";
        string strBuff = "{" + strNewLine;

        // 테이블별 내용작성
        strBuff += string.Format("\t\"{0}\": [{1}", "AssetBundleInfo", strNewLine);
        SHUtils.ForToDic(dicTable, (pKey, pValue) =>
        {
            strBuff += "\t\t{" + strNewLine;

            strBuff += string.Format("\t\t\t\"s_BundleName\": \"{0}\",{1}",
                pValue.m_strBundleName,
                strNewLine);

            strBuff += string.Format("\t\t\t\"s_BundleSize\": \"{0}\",{1}",
                pValue.m_lBundleSize,
                strNewLine);

            strBuff += string.Format("\t\t\t\"s_BundleHash\": \"{0}\",{1}",
                pValue.m_pHash128.ToString(),
                strNewLine);

            strBuff += string.Format("\t\t\t\"p_Resources\": {0}", strNewLine);
            strBuff += "\t\t\t[" + strNewLine;

            SHUtils.ForToDic(pValue.m_dicResources, (pResKey, pResValue) =>
            {
                strBuff += "\t\t\t\t{" + strNewLine;
                strBuff += MakeSaveFormat(pResValue, "\t\t\t\t");
                strBuff += "\t\t\t\t}," + strNewLine;
            });
            strBuff = string.Format("{0}{1}", strBuff.Substring(0, strBuff.Length - (strNewLine.Length + 1)), strNewLine);

            strBuff += "\t\t\t]" + strNewLine;
            strBuff += "\t\t}," + strNewLine;
        });
        strBuff = string.Format("{0}{1}", strBuff.Substring(0, strBuff.Length - (strNewLine.Length + 1)), strNewLine);
        strBuff += string.Format("\t]{0}", strNewLine);
        strBuff += "}";

        // 저장
        SHUtils.SaveFile(strBuff, strSaveFilePath);
    }

    // 인터페이스 : 중복파일 리스트 내보내기
    public static void SaveToDuplicationList(Dictionary<string, List<string>> dicDuplications, string strSaveFilePath)
    {
        if (0 == dicDuplications.Count)
            return;

        string strNewLine = "\r\n";
        string strBuff    = string.Empty;
        SHUtils.ForToDic(dicDuplications, (pKey, pValue) =>
        {
            strBuff += string.Format("FileName : {0}{1}", pKey, strNewLine);
            SHUtils.ForToList(pValue, (strPath) =>
            {
                strBuff += string.Format("\tPath : Resources/{0}{1}", strPath, strNewLine);
            });
            strBuff += string.Format("{0}", strNewLine);
        });

        SHUtils.SaveFile(strBuff, strSaveFilePath);
    }

    // 인터페이스 : 파일정보 Json 포맷으로 만들어주기
    public static string MakeSaveFormat(SHResourcesInfo pInfo, string strPreFix)
    {
        if (null == pInfo)
            return string.Empty;

        string strNewLine   = "\r\n";
        string strBuff      = string.Empty;

        strBuff += string.Format("{0}\t\"s_Name\": \"{1}\",{2}",
            strPreFix,
            pInfo.m_strName,
            strNewLine);

        strBuff += string.Format("{0}\t\"s_FileName\": \"{1}\",{2}",
            strPreFix,
            pInfo.m_strFileName,
            strNewLine);

        strBuff += string.Format("{0}\t\"s_Extension\": \"{1}\",{2}",
            strPreFix,
            pInfo.m_strExtension,
            strNewLine);

        strBuff += string.Format("{0}\t\"s_Size\": \"{1}\",{2}",
            strPreFix,
            pInfo.m_strSize,
            strNewLine);

        //strBuff += string.Format("{0}\t\"s_LastWriteTime\": \"{1}\",{2}",
        //    strPreFix,
        //    pInfo.m_strLastWriteTime,
        //    strNewLine);

        strBuff += string.Format("{0}\t\"s_Hash\": \"{1}\",{2}",
            strPreFix,
            pInfo.m_strHash,
            strNewLine);

        strBuff += string.Format("{0}\t\"s_Path\": \"{1}\"{2}",
            strPreFix,
            pInfo.m_strPath,
            strNewLine);

        return strBuff;
    }
    #endregion


    #region Utility Functions
    // 유틸 : 리소스 리스트 추가
    void AddResourceInfo(SHResourcesInfo pInfo)
    {
        if (null == pInfo)
            return;

        m_dicResources[pInfo.m_strFileName] = pInfo;
    }

    // 유틸 : 번들 리스트 추가
    // 1. 리소스의 최상위 폴더이름을 번들이름으로 하여 등록시킴.
    // 2. 프리팹을 제외한 모든 리소스를 번들 리스트로 등록시킴.
    void AddAssetBundleInfo(SHResourcesInfo pInfo)
    {
        if (null == pInfo)
            return;

        if (true == CheckFilteringToAssetBundleInfo(pInfo))
            return;

        // 번들이름 만들기
        string strBundleName    = "Root";
        string[] strSplitPath   = pInfo.m_strPath.Split(new char[] { '/' });
        if (1 < strSplitPath.Length)
            strBundleName = strSplitPath[0];

        // 번들정보 생성하기
        if (false == m_dicAssetBundles.ContainsKey(strBundleName))
        {
            AssetBundleInfo pBundleInfo = new AssetBundleInfo();
            pBundleInfo.m_strBundleName = strBundleName;
            m_dicAssetBundles.Add(strBundleName, pBundleInfo);
        }

        m_dicAssetBundles[strBundleName].AddResourceInfo(pInfo);
    }

    // 유틸 : 번들로 묶지 않을 파일에 대한 필터링
    bool CheckFilteringToAssetBundleInfo(SHResourcesInfo pInfo)
    {
        // 프리팹파일 필터링
        if (".prefab" == pInfo.m_strExtension)
            return true;

        // 테이블파일 필터링
        if (".bytes" == pInfo.m_strExtension)
            return true;
        
        return false;
    }

    // 유틸 : 파일로 부터 정보얻어서 테이블 데이터 객체만들기
    SHResourcesInfo MakeFileInfo(FileInfo pFile)
    {
        // 알리아싱
        string strRoot              = "Resources";
        string strFullName          = pFile.FullName.Substring(pFile.FullName.IndexOf(strRoot) + strRoot.Length + 1).Replace ("\\", "/");
        string strExtension         = Path.GetExtension(strFullName);
      
        // 예외처리 : 리스팅에서 제외할 파일
        if (true == CheckExceptionFile(pFile))
            return null;
        
        // 기록
        var pInfo                   = new SHResourcesInfo();
        pInfo.m_strName             = Path.GetFileNameWithoutExtension(strFullName);
        pInfo.m_strFileName         = Path.GetFileName(strFullName);
        pInfo.m_strExtension        = strExtension;
        pInfo.m_strSize             = pFile.Length.ToString();
        //pInfo.m_strLastWriteTime    = pFile.LastWriteTime.ToString("yyyy-MM-dd-HH:mm:ss.fff");
        pInfo.m_strHash             = SHHash.GetMD5ToFile(pFile.FullName);
        pInfo.m_strPath             = strFullName.Substring(0, strFullName.Length - strExtension.Length);;
        
        return pInfo;
    }
    
    // 유틸 : 예외파일 체크
    bool CheckExceptionFile(FileInfo pFile)
    {
        if (null == pFile)
            return true;

        switch(Path.GetExtension(pFile.FullName).ToLower())
        {
            case ".meta":       return true;
            case ".shader":     return true;
        }

        return false;
    }

    // 유틸 : 이름중복체크
    string CheckToDuplication(Dictionary<string, SHResourcesInfo> dicFiles, string strFileName)
    {
        foreach (var kvp in dicFiles)
        {
            if (kvp.Key == strFileName)
                return kvp.Value.m_strPath;
        }
        return null;
    }
    #endregion
}