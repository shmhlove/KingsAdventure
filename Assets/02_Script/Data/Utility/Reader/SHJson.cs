using UnityEngine;

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using LitJson;

public class SHJson
{
    private JsonData m_pJsonData = null;
    public JsonData Node { get { return m_pJsonData; } }

    public SHJson() { }
    public SHJson(string strFileName)
    {
        if (true == string.IsNullOrEmpty(strFileName))
            return;

        strFileName = Path.GetFileNameWithoutExtension(strFileName);

        // 1차 : PersistentDataPath에 Json데이터가 있으면 그걸 로드하도록 한다.
        // 2차 : 없으면 패키지에서 로드하도록 한다.

        string strSavePath = string.Format("{0}/{1}.json", SHPath.GetPersistentDataJson(), strFileName);
        if (true == File.Exists(strSavePath))
            SetJsonData(LoadByPersistent(strSavePath));
        //else
        //    SetJsonData(LoadByStreamingAssets(strFileName));
        else
            SetJsonData(LoadByPackage(strFileName));
    }

    ~SHJson()
    {
        SetJsonData(null);
    }
    
    public JsonData SetJsonData(JsonData pNode)
    {
        return (m_pJsonData = pNode);
    }
    
    public JsonData LoadByPersistent(string strSavePath)
    {
        if (false == File.Exists(strSavePath))
            return null;
        
        string strBuff = File.ReadAllText(strSavePath);
        if (true == string.IsNullOrEmpty(strBuff))
        {
            Debug.LogErrorFormat("[LSH] Json(*.json)파일을 읽는 중 오류발생!!(Path:{0})", strSavePath);
            return null;
        }

        return GetJsonParseToString(strBuff);
    }
    
    public JsonData LoadByStreamingAssets(string strFileName)
    {
        WWW pWWW = Single.Coroutine.WWWOfSync(new WWW(GetStreamingPath(strFileName)));
        if (true != string.IsNullOrEmpty(pWWW.error))
        {
            Debug.LogErrorFormat("[LSH] Json(*.json)파일을 읽는 중 오류발생!!(File:{0}, Error:{1})", strFileName, pWWW.error);
            return null;
        }

        return GetJsonParseToString(pWWW.text);
    }
    
    public JsonData LoadByPackage(string strFileName)
    {
        var pTextAsset = Single.Resource.GetTextAsset(strFileName);
        if (null == pTextAsset)
            return null;

        return GetJsonParseToString(pTextAsset.text);
    }
    
    public JsonData GetJsonParseToByte(byte[] pByte)
    {
        return JsonMapper.ToObject((new System.Text.UTF8Encoding()).GetString(pByte));
    }
    
    public JsonData GetJsonParseToString(string strBuff)
    {
        if (true == string.IsNullOrEmpty(strBuff))
            return null;

        strBuff = Regex.Replace(strBuff, "(?<!\\r)\\n", "");

        MemoryStream pStream = new MemoryStream(Encoding.UTF8.GetBytes(strBuff));
        StreamReader pReader = new StreamReader(pStream, true);
        string strEncodingBuff = pReader.ReadToEnd().Trim();
        pReader.Close();
        pStream.Close();

        if (true == string.IsNullOrEmpty(strEncodingBuff))
            return null;
        
        return JsonMapper.ToObject(strEncodingBuff.TrimEnd());
    }
    
    public bool CheckJson()
    {
        return (null != m_pJsonData);
    }
    
    string GetStreamingPath(string strFileName)
    {
        string strPath = string.Empty;

#if UNITY_EDITOR || UNITY_STANDALONE
        strPath = string.Format("{0}{1}", "file://", SHPath.GetStreamingAssets());
#elif UNITY_ANDROID
        strPath = string.Format("{0}{1}{2}", "jar:file://", SHPath.GetAssets(), "!/assets");
#elif UNITY_IOS
        strPath = string.Format("{0}{1}{2}", "file://", SHPath.GetAssets(), "/Raw");
#endif

        return string.Format("{0}/JSons/{1}.json", strPath, Path.GetFileNameWithoutExtension(strFileName));
    }
}