using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

// class : 번들 매이커
public static class SHAssetBundleMaker
{
    #region Interface Functions
    // 인터페이스 : 번들패킹
    public static bool PackingAssetBundle(BuildTarget eTarget, eBundlePackType ePackType)
    {
        return PackingAssetBundle(eTarget, ePackType, true, SHPath.GetPathToExportAssetBundle());
    }
    public static bool PackingAssetBundle(BuildTarget eTarget, eBundlePackType ePackType, bool bIsDelOriginal)
    {
        return PackingAssetBundle(eTarget, ePackType, bIsDelOriginal, SHPath.GetPathToExportAssetBundle());
    }
    public static bool PackingAssetBundle(BuildTarget eTarget, eBundlePackType ePackType, bool bIsDelOriginal, string strOutputPath)
    {
        strOutputPath = string.Format("{0}/{1}", strOutputPath, SHHard.GetPlatformStringByEnum(eTarget));

        // 정보 테이블 준비
        var pTableData = CreateTableData(eTarget);

        // 패킹할 번들정보 얻기
        var dicBundles = GetPackingBundleList(pTableData, ePackType);

        // 번들 패킹 시작 및 아웃풋
        if (false == MakeAssetBundle(eTarget, strOutputPath, dicBundles))
            return false;

        // 원본리소스 모두 제거
        if (true == bIsDelOriginal)
            DeleteOriginalResource(pTableData);

        // 번들정보파일 업데이트 및 아웃풋
        UpdateBundleInfoTable(eTarget, pTableData, dicBundles, strOutputPath);

        // 메시지 출력
        Debug.LogFormat("Success Make AssetBundles!!(Count : {0})", dicBundles.Count);

        return true;
    }
    #endregion


    #region Utility Functions
    // 유틸 : 패킹할 번들 리스트 만들기
    static Dictionary<string, AssetBundleInfo> GetPackingBundleList(SHTableData pTableData, eBundlePackType ePackingType)
    {
        var pBundleInfo = GetBundleTable(pTableData);
        
        switch(ePackingType)
        {
            case eBundlePackType.All:     return pBundleInfo.GetContainer();
            case eBundlePackType.Update:  return pBundleInfo.GetBundleListToCompare(GetResourceTable(pTableData));
        }

        return new Dictionary<string, AssetBundleInfo>();
    }

    // 유틸 : 번들 패킹 시작 및 아웃풋
    static bool MakeAssetBundle(BuildTarget eTarget, string strOutputPath, Dictionary<string, AssetBundleInfo> dicBundles)
    {
        // 디렉토리 정리
        SHUtils.DeleteDirectory(strOutputPath);
        SHUtils.CreateDirectory(strOutputPath);

        // 번들 빌드 정보 만들기
        List<AssetBundleBuild> pBuildList = new List<AssetBundleBuild>();
        SHUtils.ForToDic(dicBundles, (pKey, pValue) =>
        {
            List<string> pAssets = new List<string>();
            SHUtils.ForToDic(pValue.m_dicResources, (pResKey, pResValue) =>
            {
                pAssets.Add(string.Format("{0}/{1}{2}", "Assets/Resources", pResValue.m_strPath, pResValue.m_strExtension));
            });

            AssetBundleBuild pAssetInfo = new AssetBundleBuild();
            pAssetInfo.assetBundleName  = string.Format("{0}.unity3d", pValue.m_strBundleName);
            pAssetInfo.assetNames       = pAssets.ToArray();
            pBuildList.Add(pAssetInfo);
        });
        
        // 빌드할 번들이 없으면 종료
        if (0 == pBuildList.Count)
            return true;
        
        // 번들 빌드하기
        AssetBundleManifest pManifest = BuildPipeline.BuildAssetBundles(strOutputPath, pBuildList.ToArray(), BuildAssetBundleOptions.DeterministicAssetBundle, eTarget);
        if (null == pManifest)
        {
            Debug.LogErrorFormat("Error!!! Make Assets Bundle : {0}", strOutputPath);
            return false;
        }

        // 후 처리
        SHUtils.ForToList(pBuildList, (pBundle) =>
        {
            // 번들 크기와 해시코드 기록
            string strKey = pBundle.assetBundleName.Substring(0, pBundle.assetBundleName.Length - ".unity3d".Length).ToLower();
            if (true == dicBundles.ContainsKey(strKey))
            {
                dicBundles[strKey].m_pHash128    = pManifest.GetAssetBundleHash(pBundle.assetBundleName);
                dicBundles[strKey].m_lBundleSize = (new FileInfo(string.Format("{0}/{1}", strOutputPath, pBundle.assetBundleName))).Length;
            }

            // Manifest제거 ( 사용하지 않는 불필요한 파일이라 그냥 제거시킴 )
            SHUtils.DeleteFile(string.Format("{0}/{1}.manifest", strOutputPath, pBundle.assetBundleName));
        });
        SHUtils.DeleteFile(string.Format("{0}/{1}", strOutputPath, SHHard.GetPlatformStringByEnum(eTarget)));
        SHUtils.DeleteFile(string.Format("{0}/{1}.manifest", strOutputPath, SHHard.GetPlatformStringByEnum(eTarget)));
        
        return true;
    }

    // 유틸 : 원본 리소스 모두 제거
    static void DeleteOriginalResource(SHTableData pTableData)
    {
        var pBundleInfo = GetBundleTable(pTableData);
        SHUtils.ForToDic(pBundleInfo.GetContainer(), (pKey, pValue) =>
        {
            SHUtils.ForToDic(pValue.m_dicResources, (pResKey, pResValue) =>
            {
                SHUtils.DeleteFile(string.Format("{0}/{1}{2}", SHPath.GetPathToResources(), pResValue.m_strPath, pResValue.m_strExtension));
                SHUtils.DeleteFile(string.Format("{0}/{1}{2}", SHPath.GetPathToResources(), pResValue.m_strPath, ".meta"));
            });
        });
    }

    // 유틸 : 번들정보 스크립트 아웃풋
    static void UpdateBundleInfoTable(BuildTarget eTarget, SHTableData pTableData, Dictionary<string, AssetBundleInfo> dicMakeBundles, string strOutputPath)
    {
        var pBundleTable   = GetBundleTable(pTableData);
        var pResourceTable = GetResourceTable(pTableData);
        SHUtils.ForToDic(dicMakeBundles, (pKey, pValue) =>
        {
            AssetBundleInfo pData = pBundleTable.GetBundleInfo(pValue.m_strBundleName);
            pData.m_lBundleSize   = pValue.m_lBundleSize;
            pData.m_pHash128      = pValue.m_pHash128;
            pData.CopyResourceInfo(pValue.m_dicResources);

            SHUtils.ForToDic(pData.m_dicResources, (pResKey, pResValue) =>
            {
                pResValue.CopyTo(pResourceTable.GetResouceInfo(pResKey));
            });
        });

        pBundleTable.SaveJsonFileByDic(
            string.Format("{0}/{1}.json", strOutputPath, pBundleTable.m_strFileName));
    }

    // 유틸 : 테이블 데이터 생성 ( 번들정보와 리소스정보 준비시키기 )
    static SHTableData CreateTableData(BuildTarget eTarget)
    {
        // 테이블 데이터 생성
        SHTableData pTableData = new SHTableData();
        pTableData.OnInitialize();
        {
            // 클라이언트 정보 테이블 준비
            var pClientInfo = GetClientConfigTable(pTableData);
            pClientInfo.LoadJsonToLocal(pClientInfo.m_strFileName);

            // 서버 정보 테이블 준비
            var pServerInfo = GetServerConfigTable(pTableData);
            if (false == string.IsNullOrEmpty(pClientInfo.GetConfigurationCDN()))
                pServerInfo.DownloadByCDNToSync(pClientInfo.GetConfigurationCDN());
            else
                pServerInfo.LoadJsonToLocal(pServerInfo.m_strFileName);
            
            // 번들정보 테이블 준비
            var pBundleInfo = GetBundleTable(pTableData);
            pBundleInfo.SetData(
                pBundleInfo.UpdateAssetBundlesMakeInfoByStreamingPath(pServerInfo.GetBundleCDN(), eTarget));

            // 리소스 리스트 테이블 준비
            var pResourceInfo = GetResourceTable(pTableData);
            pResourceInfo.LoadJsonToLocal(pResourceInfo.m_strFileName);
        }
        return pTableData;
    }

    // 유틸 : 클라이언트 정보 테이블 얻기
    static JsonClientConfig GetClientConfigTable(SHTableData pTableData)
    {
        return pTableData.GetTable<JsonClientConfig>();
    }

    // 유틸 : 서버 정보 테이블 얻기
    static JsonServerConfig GetServerConfigTable(SHTableData pTableData)
    {
        return pTableData.GetTable<JsonServerConfig>();
    }

    // 유틸 : 번들 테이블 얻기
    static JsonAssetBundleInfo GetBundleTable(SHTableData pTableData)
    {
        return pTableData.GetTable<JsonAssetBundleInfo>();
    }

    // 유틸 : 리소스 테이블 얻기
    static JsonResources GetResourceTable(SHTableData pTableData)
    {
        return pTableData.GetTable<JsonResources>();
    }
    #endregion
}