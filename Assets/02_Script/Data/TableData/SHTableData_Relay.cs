using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public partial class SHTableData : SHBaseData
{
    #region PreloadResources
    public List<string> GetPreLoadResourcesList(eSceneType eType)
    {
        JsonPreloadResources pTable = GetTable<JsonPreloadResources>();
        if (null == pTable)
            return new List<string>();

        return pTable.GetData(eType);
    }
    #endregion

    #region ResourcesInfo
    public SHResourcesInfo GetResourcesInfo(string strFileName)
    {
        JsonResources pTable = GetTable<JsonResources>();
        if (null == pTable)
            return null;

        return pTable.GetResouceInfo(strFileName);
    }
    #endregion

    #region JsonClientConfig
    public string GetServerConfigCDN()
    {
        var pTable = GetTable<JsonClientConfig>();
        if (null == pTable)
            return string.Empty;

        return pTable.GetConfigurationCDN();
    }
    public string GetClientVersion()
    {
        var pTable = GetTable<JsonClientConfig>();
        if (null == pTable)
            return "0";

        return pTable.GetVersion();
    }
    public eServiceMode GetClientServiceMode()
    {
        var pTable = GetTable<JsonClientConfig>();
        if (null == pTable)
            return eServiceMode.None;

        return SHHard.GetEnumToServiceMode(pTable.GetServiceMode());
    }
    public int GetClientVersionToOrder(eOrderNum eOrder)
    {
        var pTable = GetTable<JsonClientConfig>();
        if (null == pTable)
            return 0;

        return pTable.GetVersionToOrder(eOrder);
    }
    #endregion

    #region JsonServerConfig
    public string GetServerURL()
    {
        var pTable = GetTable<JsonServerConfig>();
        if (null == pTable)
            return string.Empty;

        return pTable.GetGameServerURL();
    }
    public string GetBundleCDN()
    {
        var pTable = GetTable<JsonServerConfig>();
        if (null == pTable)
            return string.Empty;

        return pTable.GetBundleCDN();
    }
    public string GetMarketURL()
    {
        var pTable = GetTable<JsonServerConfig>();
        if (null == pTable)
            return string.Empty;

        return pTable.GetMarketURL();
    }
    public eServiceMode GetServiceMode()
    {
        var pTable = GetTable<JsonServerConfig>();
        if (null == pTable)
            return eServiceMode.None;

        return pTable.GetServiceMode();
    }
    public eServiceState GetServiceState()
    {
        var pTable = GetTable<JsonServerConfig>();
        if (null == pTable)
            return eServiceState.None;

        return pTable.GetServiceState();
    }
    public string GetServiceCheckMessage()
    {
        var pTable = GetTable<JsonServerConfig>();
        if (null == pTable)
            return string.Empty;

        return pTable.GetCheckMessage();
    }
    public void DownloadServerConfig(Action pComplate)
    {
        var pTable = GetTable<JsonServerConfig>();
        if (null == pTable)
        {
            pComplate();
            return;
        }

        pTable.DownloadByCDN(pComplate, SHPath.GetURLToServerConfigCDN());
    }
    public bool IsLoadServerConfig()
    {
        var pTable = GetTable<JsonServerConfig>();
        if (null == pTable)
            return false;

        return pTable.IsLoadTable();
    }
    #endregion

    #region AssetBundleInfo
    public Dictionary<string, AssetBundleInfo> GetAssetBundleInfo()
    {
        var pTable = GetTable<JsonAssetBundleInfo>();
        if (null == pTable)
            return new Dictionary<string, AssetBundleInfo>();

        return pTable.GetContainer();
    }
    public AssetBundleInfo GetAssetBundleInfo(string strBundleName)
    {
        var pTable = GetTable<JsonAssetBundleInfo>();
        if (null == pTable)
            return new AssetBundleInfo();

        return pTable.GetBundleInfo(strBundleName);
    }
    public AssetBundleInfo GetBundleInfoToResourceName(string strResourceName)
    {
        var pTable = GetTable<JsonAssetBundleInfo>();
        if (null == pTable)
            return new AssetBundleInfo();

        return pTable.GetBundleInfoToResourceName(strResourceName);
    }
    public void DownloadBundleInfo(Action pComplate)
    {
        var pTable = GetTable<JsonAssetBundleInfo>();
        if (null == pTable)
        {
            pComplate();
            return;
        }

        pTable.DownloadByCDN(pComplate);
    }
    #endregion
}