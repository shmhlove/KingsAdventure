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
        // 2차 : 없으면 StreamingAssets에서 로드하도록 한다.

        if (null != (SetJsonData(LoadToPersistent(strFileName))))
            return;

        SetJsonData(LoadToStreamingForWWW(strFileName));
    }

    ~SHJson()
    {
        SetJsonData(null);
    }

    // 인터페이스 : JsonData설정
    public JsonData SetJsonData(JsonData pNode)
    {
        return (m_pJsonData = pNode);
    }

    // 인터페이스 : PersistentData 에서 로드
    public JsonData LoadToPersistent(string strFileName)
    {
        string strSavePath = string.Format("{0}/{1}.json", SHPath.GetPersistentDataJson(), Path.GetFileNameWithoutExtension(strFileName));
        if (false == File.Exists(strSavePath))
            return null;

        return SetJsonData(LoadLocal(strSavePath));
    }

    // 인터페이스 : Streaming에서 LoaclLoad로 로드
    public JsonData LoadToStreamingForLocal(string strFileName)
    {
        string strSavePath = string.Format("{0}/{1}.json", SHPath.GetStreamingAssetsJsonTable(), Path.GetFileNameWithoutExtension(strFileName));
        if (false == File.Exists(strSavePath))
            return null;

        return SetJsonData(LoadLocal(strSavePath));
    }

    // 인터페이스 : Streaming에서 WWW로 로드
    public JsonData LoadToStreamingForWWW(string strFileName)
    {
        return SetJsonData(LoadWWW(GetStreamingPath(strFileName)));
    }

    // 인터페이스 : Json파일 로드
    public JsonData LoadWWW(string strFilePath)
    {
        WWW pWWW = Single.Coroutine.WWWOfSync(new WWW(strFilePath));
        if (true != string.IsNullOrEmpty(pWWW.error))
        {
            Debug.LogErrorFormat("[LSH] Json(*.json)파일을 읽는 중 오류발생!!(Path:{0}, Error:{1})", strFilePath, pWWW.error);
            return null;
        }

        return GetJsonParseToString(pWWW.text);
    }
    
    // 인터페이스 : Byte로 Json파싱
    public JsonData GetJsonParseToByte(byte[] pByte)
    {
        return JsonMapper.ToObject((new System.Text.UTF8Encoding()).GetString(pByte));
    }

    // 인터페이스 : string으로 Json파싱
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

    // 인터페이스 : Json파일 로드 체크
    public bool CheckJson()
    {
        return (null != m_pJsonData);
    }
    
    // 유틸 : Json파일 로드
    public JsonData LoadLocal(string strFilePath)
    {
        if (false == File.Exists(strFilePath))
            return null;

        string strBuff = File.ReadAllText(strFilePath);
        if (true == string.IsNullOrEmpty(strBuff))
        {
            Debug.LogErrorFormat("[LSH] Json(*.json)파일을 읽는 중 오류발생!!(Path:{0})", strFilePath);
            return null;
        }

        return GetJsonParseToString(strBuff);
    }

    // 유틸 : StreamingPath경로 만들기
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