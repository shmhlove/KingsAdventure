using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

using DicData = System.Collections.Generic.Dictionary<eServiceMode, JsonServerConfigData>;

public class JsonServerConfigData
{
    public eServiceMode m_eServiceMode        = eServiceMode.Dev;
    public string       m_strClientVersion    = string.Empty;
    public string       m_strGameServerURL    = string.Empty;
    public string       m_strBundleCDN        = string.Empty;
    public string       m_strCheckMessage     = string.Empty;
    public string       m_strServiceState     = string.Empty;
}

public class JsonServerConfig : SHBaseTable
{
    #region Members
    private string  m_strAOSMarketURL = string.Empty;
    private string  m_strIOSMarketURL = string.Empty;
    private DicData m_dicServerInfo   = new DicData();
    #endregion


    #region System Functions
    public JsonServerConfig()
    {
        m_strFileName = "ServerConfig";
    }
    #endregion


    #region Virtual Functions
    public override void Initialize()
    {
        m_strAOSMarketURL = string.Empty;
        m_strIOSMarketURL = string.Empty;
        m_dicServerInfo.Clear();
    }

    public override bool IsLoadTable()
    {
        return (0 != m_dicServerInfo.Count);
    }

    public override eErrorCode LoadJsonTable(JsonData pJson, string strFileName)
    {
        if (null == pJson)
            return eErrorCode.Table_Load_Fail;

        JsonData pDataNode = pJson["ServerConfig"];

        // 마켓 URL
        m_strAOSMarketURL = GetStrToJson(pDataNode, "AOS_MarketURL");
        m_strIOSMarketURL = GetStrToJson(pDataNode, "IOS_MarketURL");

        // 모드별 정보
        SHUtils.ForToEnum<eServiceMode>((eMode) => 
        {
            if (eServiceMode.None == eMode)
                return;

            var pData                = new JsonServerConfigData();
            pData.m_strClientVersion = GetStrToJson(pDataNode[eMode.ToString()], "ClientVersion");
            pData.m_strGameServerURL = GetStrToJson(pDataNode[eMode.ToString()], "GameServerURL");
            pData.m_strBundleCDN     = GetStrToJson(pDataNode[eMode.ToString()], "BundleCDN");
            pData.m_strCheckMessage  = GetStrToJson(pDataNode[eMode.ToString()], "CheckMessage");
            pData.m_strServiceState  = GetStrToJson(pDataNode[eMode.ToString()], "ServiceState");
            pData.m_eServiceMode     = eMode;
            m_dicServerInfo[eMode]   = pData;
        });
        
        return eErrorCode.Succeed;
    }
    #endregion


    #region Interface Functions
    // 인터페이스 : CDN에서 정보파일 다운로드
    public void DownloadByCDN(Action pComplete, string strURL)
    {
        // URL이 없으면 다운받지 않는다.
        if (true == string.IsNullOrEmpty(strURL))
        {
            pComplete();
            return;
        }

        Single.Coroutine.WWW((pWWW) =>
        {
            if (true == string.IsNullOrEmpty(pWWW.error))
            {
                SHJson pJson = new SHJson();
                pJson.SetJsonData(pJson.GetJsonParseToString(pWWW.text));
                LoadJsonTable(pJson.Node, m_strFileName);
                pComplete();
            }
            else
            {
                Debug.LogErrorFormat("Error!!! Download ServerConfig.json : (Error : {0}, URL : {1}", pWWW.error, pWWW.url);
            }

        }, new WWW(SHPath.GetURLToServerConfig()));
    }

    // 인터페이스 : CDN에서 정보파일 다운로드
    public bool DownloadByCDNToSync(string strURL)
    {
        // URL이 없으면 다운받지 않는다.
        if (true == string.IsNullOrEmpty(strURL))
            return false;

        WWW pWWW = Single.Coroutine.WWWOfSync(new WWW(strURL));
        if (false == string.IsNullOrEmpty(pWWW.error))
            return false;

        SHJson pJson = new SHJson();
        pJson.SetJsonData(pJson.GetJsonParseToString(pWWW.text));
        LoadJsonTable(pJson.Node, m_strFileName);

        return true;
    }

    // 인터페이스 : 클라버전에 맞는 서버정보 얻기(Live부터 Dev까지 순차검색)
    public JsonServerConfigData GetServerInfo(string strClientVersion)
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        var pFindOrder = new List<eServiceMode>()
        {
            eServiceMode.Live,
            eServiceMode.Review,
            eServiceMode.QA,
            eServiceMode.Dev,
        };
        
        foreach (eServiceMode eMode in pFindOrder)
        {
            if (eServiceMode.None == eMode)
                continue;
            
            var pServerInfo = m_dicServerInfo[eMode];
            if (false == pServerInfo.m_strClientVersion.Equals(strClientVersion))
                continue;

            return pServerInfo;
        }

        return null;
    }

    // 인터페이스 : 클라버전에 맞는 서버정보 얻기
    public JsonServerConfigData GetServerInfo(eServiceMode eMode)
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        if (false == m_dicServerInfo.ContainsKey(eMode))
            return null;

        return m_dicServerInfo[eMode];
    }

    // 인터페이스 : 서버 URL
    public string GetGameServerURL()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        var pServerInfo = GetServerInfo(Single.Table.GetClientServiceMode());
        if (null == pServerInfo)
            pServerInfo = GetServerInfo(Single.Table.GetClientVersion());
        if (null == pServerInfo)
            return string.Empty;

        return pServerInfo.m_strGameServerURL;
    }

    // 인터페이스 : 번들 다운로드 CDN URL
    public string GetBundleCDN()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        var ClientConfig = Single.Table.GetTable<JsonClientConfig>();
        if (null == ClientConfig)
            return string.Empty;

        var ServiceMode   = ClientConfig.GetServiceMode();
        var ClientVersion = ClientConfig.GetVersion();

        var pServerInfo = GetServerInfo(ServiceMode);
        if (null == pServerInfo)
            return string.Empty;

        if (false == pServerInfo.m_strClientVersion.Equals(ClientVersion))
            return string.Empty;

        return pServerInfo.m_strBundleCDN;
    }

    // 인터페이스 : 옙 스토어 URL
    public string GetMarketURL()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        switch(Single.AppInfo.GetRuntimePlatform())
        {
            case RuntimePlatform.Android:        return m_strAOSMarketURL;
            case RuntimePlatform.IPhonePlayer:   return m_strIOSMarketURL;
        }

        return string.Empty;
    }

    // 인터페이스 : 서비스 모드얻기
    public eServiceMode GetServiceMode()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        var pServerInfo = GetServerInfo(Single.Table.GetClientServiceMode());
        if (null == pServerInfo)
            pServerInfo = GetServerInfo(Single.Table.GetClientVersion());
        if (null == pServerInfo)
            return eServiceMode.None;

        return pServerInfo.m_eServiceMode;
    }

    // 인터페이스 : 서비스 상태
    public eServiceState GetServiceState()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        var pServerInfo = GetServerInfo(Single.Table.GetClientServiceMode());
        if (null == pServerInfo)
            pServerInfo = GetServerInfo(Single.Table.GetClientVersion());
        if (null == pServerInfo)
            return eServiceState.None;

        return GetEnumToServiceState(pServerInfo.m_strServiceState);
    }

    // 인터페이스 : 점검 중 메시지 얻기
    public string GetCheckMessage()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        var pServerInfo = GetServerInfo(Single.Table.GetClientServiceMode());
        if (null == pServerInfo)
            pServerInfo = GetServerInfo(Single.Table.GetClientVersion());
        if (null == pServerInfo)
            return string.Empty;

        return pServerInfo.m_strCheckMessage;
    }
    #endregion


    #region Utility Functions
    // 유틸 : 서비스 상태 Enum 얻기
    eServiceState GetEnumToServiceState(string strState)
    {
        switch(strState.ToLower())
        {
            case "run":             return eServiceState.Run;
            case "check":           return eServiceState.Check;
            case "connectmarket":   return eServiceState.ConnectMarket;
            default:                return eServiceState.None;
        }
    }
    #endregion
}