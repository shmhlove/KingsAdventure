using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public partial class SHTableData : SHBaseData
{
    #region PreLoadResourcesTable
    public List<string> GetPreLoadResourcesList(eSceneType eType)
    {
        JsonPreLoadResourcesTable pTable = GetTable<JsonPreLoadResourcesTable>();
        if (null == pTable)
            return new List<string>();

        return pTable.GetData(eType);
    }
    #endregion

    #region ResourcesTable
    public SHResourcesTableInfo GetResourcesInfo(string strFileName)
    {
        JsonResourcesTable pTable = GetTable<JsonResourcesTable>();
        if (null == pTable)
            return null;

        return pTable.GetResouceInfo(strFileName);
    }
    #endregion

    #region JsonClientConfiguration
    public string GetServerConfigurationCDN()
    {
        var pTable = GetTable<JsonClientConfiguration>();
        if (null == pTable)
            return string.Empty;

        return pTable.GetConfigurationCDN();
    }
    public string GetClientVersion()
    {
        var pTable = GetTable<JsonClientConfiguration>();
        if (null == pTable)
            return "0";

        return pTable.GetVersion();
    }
    public eServiceMode GetClientServiceMode()
    {
        var pTable = GetTable<JsonClientConfiguration>();
        if (null == pTable)
            return eServiceMode.None;

        return SHHard.GetEnumToServiceMode(pTable.GetServiceMode());
    }
    public int GetClientVersionToOrder(eOrderNum eOrder)
    {
        var pTable = GetTable<JsonClientConfiguration>();
        if (null == pTable)
            return 0;

        return pTable.GetVersionToOrder(eOrder);
    }
    #endregion

    #region JsonServerConfiguration
    public string GetServerURL()
    {
        var pTable = GetTable<JsonServerConfiguration>();
        if (null == pTable)
            return string.Empty;

        return pTable.GetGameServerURL();
    }
    public string GetBundleCDN()
    {
        var pTable = GetTable<JsonServerConfiguration>();
        if (null == pTable)
            return string.Empty;

        return pTable.GetBundleCDN();
    }
    public string GetMarketURL()
    {
        var pTable = GetTable<JsonServerConfiguration>();
        if (null == pTable)
            return string.Empty;

        return pTable.GetMarketURL();
    }
    public eServiceMode GetServiceMode()
    {
        var pTable = GetTable<JsonServerConfiguration>();
        if (null == pTable)
            return eServiceMode.None;

        return pTable.GetServiceMode();
    }
    public eServiceState GetServiceState()
    {
        var pTable = GetTable<JsonServerConfiguration>();
        if (null == pTable)
            return eServiceState.None;

        return pTable.GetServiceState();
    }
    public string GetServiceCheckMessage()
    {
        var pTable = GetTable<JsonServerConfiguration>();
        if (null == pTable)
            return string.Empty;

        return pTable.GetCheckMessage();
    }
    public void DownloadServerConfiguration(Action pComplate)
    {
        var pTable = GetTable<JsonServerConfiguration>();
        if (null == pTable)
        {
            pComplate();
            return;
        }

        pTable.DownloadByCDN(pComplate, SHPath.GetURLToServerConfigurationCDN());
    }
    public bool IsLoadServerConfiguration()
    {
        var pTable = GetTable<JsonServerConfiguration>();
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