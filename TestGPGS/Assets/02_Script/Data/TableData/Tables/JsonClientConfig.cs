using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class JsonClientConfig : SHBaseTable
{
    #region Members
    public string        m_strServerConfigURL = string.Empty;
    public string        m_strServiceMode     = string.Empty;
    public string        m_strVersion         = string.Empty;
    public bool          m_bVSyncCount        = false;
    public int           m_iFrameRate         = 60;
    public int           m_iCacheSize         = 200;
    #endregion


    #region System Functions
    public JsonClientConfig()
    {
        m_strFileName = "ClientConfig";
    }
    #endregion


    #region Virtual Functions
    public override void Initialize()
    {
        m_strServerConfigURL = string.Empty;
        m_strServiceMode     = string.Empty;
        m_strVersion         = string.Empty;
        m_bVSyncCount        = false;
        m_iFrameRate         = 60;
        m_iCacheSize         = 200;
    }
    public override bool IsLoadTable()
    {
        return (false == string.IsNullOrEmpty(m_strVersion));
    }
    public override eErrorCode LoadJsonTable(JsonData pJson, string strFileName)
    {
        if (null == pJson)
            return eErrorCode.Table_Load_Fail;

        JsonData pDataNode = pJson["ClientConfig"];

        m_strServerConfigURL = GetStrToJson(pDataNode, "ServerConfigURL");
        m_strServiceMode     = GetStrToJson(pDataNode, "ServiceMode");
        m_strVersion         = GetStrToJson(pDataNode, "Version");
        m_bVSyncCount        = GetBoolToJson(pDataNode, "VSyncCount");
        m_iFrameRate         = GetIntToJson(pDataNode, "FrameRate");
        m_iCacheSize         = GetIntToJson(pDataNode, "CacheSize(MB)");
        
        return eErrorCode.Succeed;
    }
    #endregion


    #region Interface Functions
    public void SaveJsonFile()
    {
        SaveJsonFile(SHPath.GetPathToJson());
    }
    public void SaveJsonFile(string strSavePath)
    {
        var pClientConfigJsonData = new JsonData();
        pClientConfigJsonData["ServerConfigURL"] = m_strServerConfigURL;
        pClientConfigJsonData["ServiceMode"]     = m_strServiceMode;
        pClientConfigJsonData["Version"]         = m_strVersion;
        pClientConfigJsonData["VSyncCount"]      = m_bVSyncCount;
        pClientConfigJsonData["FrameRate"]       = m_iFrameRate;
        pClientConfigJsonData["CacheSize(MB)"]   = m_iCacheSize;

        var pJsonData = new JsonData();
        pJsonData["ClientConfig"] = pClientConfigJsonData;

        var pJsonWriter = new JsonWriter();
        pJsonWriter.PrettyPrint = true;
        JsonMapper.ToJson(pJsonData, pJsonWriter);

        SHUtils.SaveFile(pJsonWriter.ToString(), string.Format("{0}/{1}.json", strSavePath, m_strFileName));
    }
    public void SetServerConfigPath(string strPath)
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        m_strServerConfigURL = strPath;
    }
    public void SetServiceMode(string strServiceMode)
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        m_strServiceMode = strServiceMode;
    }
    public string GetServerConfigPath()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        return m_strServerConfigURL;
    }
    public string GetServiceMode()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        return m_strServiceMode;
    }
    public string GetVersion()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);
        
        return m_strVersion;
    }
    public int GetVersionToOrder(eOrderNum eOrder)
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        string [] strSplit = m_strFileName.Split(new char[]{'.'});
        if ((int)eOrder > strSplit.Length)
            return 0;

        return int.Parse(strSplit[((int)eOrder) - 1]);
    }
    public int GetVSyncCount()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        return (true == m_bVSyncCount) ? 1 : 0;
    }
    public int GetFrameRate()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        return m_iFrameRate;
    }
    public int GetCacheSize()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        return m_iCacheSize;
    }
    #endregion
}