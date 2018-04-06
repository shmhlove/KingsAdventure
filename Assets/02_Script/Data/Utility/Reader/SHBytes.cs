using UnityEngine;

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class SHBytes
{
    public byte[] m_pBytes = null;
    
    public SHBytes() { }
    public SHBytes(string strFileName)
    {
        if (true == string.IsNullOrEmpty(strFileName))
            return;

        strFileName = Path.GetFileNameWithoutExtension(strFileName);

        // 1차 : PersistentData에 Byte 데이터가 있으면 그걸 로드하도록 한다.
        // 2차 : 없으면 패키지에서 로드하도록 한다.

        string strSavePath = string.Format("{0}/{1}.bytes", SHPath.GetPersistentDataBytes(), strFileName);
        if (true == File.Exists(strSavePath))
            m_pBytes = LoadLocal(strSavePath);
        else
            m_pBytes = LoadPackage(strFileName);
    }
    
    public bool CheckBytes()
    {
        return (null != m_pBytes);
    }

    public byte[] GetBytes()
    {
        return m_pBytes;
    }
    
    byte[] LoadWWW(string strFilePath)
    {
        WWW pWWW = Single.Coroutine.WWWOfSync(new WWW(strFilePath));
        if (true != string.IsNullOrEmpty(pWWW.error))
        {
            Debug.LogError(string.Format("[SHBytes] Byte(*.bytes)파일을 읽는 중 오류발생!!(Path:{0}, Error:{1})", strFilePath, pWWW.error));
            return null;
        }
        
        return pWWW.bytes;
    }
    
    byte[] LoadLocal(string strFilePath)
    {
        var pBuff = File.ReadAllBytes(strFilePath);
        if (null == pBuff)
        {
            Debug.LogError(string.Format("[SHBytes] Byte(*.bytes)파일을 읽는 중 오류발생!!(Path:{0})", strFilePath));
            return null;
        }

        return pBuff;
    }

    byte[] LoadPackage(string strFileName)
    {
        var pTextAsset = Single.Resource.GetTextAsset(Path.GetFileNameWithoutExtension(strFileName));
        if (null == pTextAsset)
            return null;

        return pTextAsset.bytes;
    }
}