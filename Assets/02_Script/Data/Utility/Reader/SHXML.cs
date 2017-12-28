using UnityEngine;

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using System.Xml;

public class SHXML
{
    #region Members
    public string m_strXMLData = string.Empty;
    #endregion

    #region System Functions
    public SHXML() { }
    public SHXML(string strFileName)
    {
        if (true == string.IsNullOrEmpty(strFileName))
            return;

        strFileName = Path.GetFileNameWithoutExtension(strFileName);

        // 에디터상 실행이 안된 상태 ( Convert툴로 실행 )에서 호출이 되었을 경우
        // SHDataManager가 생성되면 하이어라키에 싱글턴 찌꺼기가 남기 때문에 다이렉트로 로드할 수 있도록 한다.
        TextAsset pTextAsset = null;
        if (false == SHDataManager.IsExists)
        {
            string strResources = "Resources";
            string strFilePath  = string.Format("{0}/{1}", SHPath.GetPathToXML(), strFileName);
            strFilePath         = strFilePath.Substring(strFilePath.IndexOf(strResources) + strResources.Length + 1).Replace('\\', '/');

            pTextAsset = Resources.Load<TextAsset>(strFilePath);
        }
        else
            pTextAsset = Single.Resource.GetTextAsset(strFileName);

        if (null != pTextAsset)
            SetXMLData(pTextAsset.text);
    }
    #endregion


    #region Interface Functions
    // 인터페이스 : XML데이터가 로드되었는지 체크
    public bool CheckXML()
    {
        return (false == string.IsNullOrEmpty(m_strXMLData));
    }

    // 인터페이스 : XML데이터 설정
    public void SetXMLData(string strBuff)
    {
        var pStream = new MemoryStream(Encoding.UTF8.GetBytes(strBuff));
        var pReader = new StreamReader(pStream, true);
        m_strXMLData = pReader.ReadToEnd();
        pReader.Close();
        pStream.Close();
    }

    // 인터페이스 : XML데이터 얻기
    public string GetXML()
    {
        return m_strXMLData;
    }

    // 인터페이스 : XML데이터를 파싱가능한 Node형태로 얻기
    public XmlDocument GetDocument()
    {
        var strData = GetXML();
        if (true == string.IsNullOrEmpty(strData))
            return null;

        var pDocument = new XmlDocument();
        pDocument.LoadXml(strData);
        return pDocument;
    }

    // 인터페이스 : XmlDocument에서 Node 얻기
    public XmlNode GetNodeToTag(XmlDocument pDoc, string strTag, int iItemIndex)
    {
        if (null == pDoc)
            pDoc = GetDocument();

        if (null == pDoc)
            return null;

        return pDoc.GetElementsByTagName(strTag).Item(iItemIndex);
    }

    // 인터페이스 : XmlNode에서 자식 Node 리스트 얻기
    public XmlNodeList GetNodeList(string strTagName)
    {
        return GetNodeList(GetNodeToTag(GetDocument(), strTagName, 0));
    }
    public XmlNodeList GetNodeList(XmlNode pNode)
    {
        if (null == pNode)
            return null;

        return pNode.ChildNodes;
    }
    #endregion
}