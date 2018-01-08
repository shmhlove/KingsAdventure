using UnityEngine;

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class SHAssetBundle
{
    public string      m_pBundleName = string.Empty;
    public AssetBundle m_pBundle     = null;
}

public partial class SHAssetBundleData : SHBaseData
{
    #region Members
    public Dictionary<string, SHAssetBundle> m_dicBundles = new Dictionary<string, SHAssetBundle>();
    #endregion


    #region System Functions
    #endregion


    #region Virtual Functions
    public override void OnInitialize() { }
    public override void OnFinalize()
    {
        SHUtils.ForToDic(m_dicBundles, (pKey, pValue) =>
        {
            pValue.m_pBundle.Unload(false);
        });
        
        m_dicBundles.Clear();
    }
    public override void FrameMove() { }
    public override Dictionary<string, SHLoadData> GetLoadList(eSceneType eType)
    {
        return new Dictionary<string, SHLoadData>();;
    }
    public override Dictionary<string, SHLoadData> GetPatchList()
    {
        var dicLoadList = new Dictionary<string, SHLoadData>();
        
        // 서버정보파일(ServerConfig.json)에 URL이 없으면 패치하지 않는다.
        if (true == string.IsNullOrEmpty(SHPath.GetURLToBundleCDN()))
            return dicLoadList;

        SHUtils.ForToDic(Single.Table.GetAssetBundleInfo(), (pKey, pValue) =>
        {
            if (true == IsExist(pKey))
                return;

            dicLoadList.Add(pKey, CreatePatchInfo(pValue));
        });

        return dicLoadList;
    }
    public override void Load(SHLoadData pInfo, Action<string, SHLoadStartInfo> pStart, 
                                                Action<string, SHLoadEndInfo> pDone)
    {
    }
    public override void Patch(SHLoadData pInfo, Action<string, SHLoadStartInfo> pStart,
                                                 Action<string, SHLoadEndInfo> pDone)
    {
        if (true == IsExist(pInfo.m_strName))
        {
            pStart(pInfo.m_strName, new SHLoadStartInfo());
            pDone(pInfo.m_strName, new SHLoadEndInfo(true, eErrorCode.None));
            return;
        }

        WWW pAsync = Single.Coroutine.WWW((pWWW) =>
        {
            bool bIsSuccess = string.IsNullOrEmpty(pWWW.error);
            if (true == bIsSuccess)
            {
                AddBundleData(pInfo.m_strName, pWWW.assetBundle);
                pDone(pInfo.m_strName, new SHLoadEndInfo(true, eErrorCode.None));
            }
            else
            {
                pDone(pInfo.m_strName, new SHLoadEndInfo(false, eErrorCode.Patch_Bundle));
            }
            
        }, WWW.LoadFromCacheOrDownload(string.Format("{0}/{1}.unity3d", SHPath.GetURLToBundleCDNWithPlatform(), pInfo.m_strName.ToLower()),
                                       Single.Table.GetAssetBundleInfo(pInfo.m_strName).m_pHash128));

        pStart(pInfo.m_strName, new SHLoadStartInfo(pAsync));
    }
    #endregion


    #region Interface Functions
    public SHLoadData CreatePatchInfo(AssetBundleInfo pInfo)
    {
        return new SHLoadData()
        {
            m_eDataType = eDataType.BundleData,
            m_strName = pInfo.m_strBundleName,
            m_pLoadFunc = Patch,
            m_pTriggerLoadCall = () =>
            {
                return Caching.ready;
            },
        };
    }
    public SHAssetBundle GetBundleData(AssetBundleInfo pInfo)
    {
        if (null == pInfo)
            return null;

        if (false == IsExist(pInfo.m_strBundleName))
            return null;

        return m_dicBundles[pInfo.m_strBundleName];
    }
    public SHAssetBundle GetBundleData(string strBundleName)
    {
        if (false == IsExist(strBundleName))
            return null;

        return m_dicBundles[strBundleName];
    }
    public bool IsExist(string strBundleName)
    {
        return m_dicBundles.ContainsKey(strBundleName);
    }
    #endregion


    #region Utility Functions
    void AddBundleData(string strBundleName, AssetBundle pBundle)
    {
        SHAssetBundle pBundleData = new SHAssetBundle();
        pBundleData.m_pBundleName = strBundleName;
        pBundleData.m_pBundle = pBundle;
        m_dicBundles[strBundleName] = pBundleData;
    }
    #endregion
}