using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class JsonPreloadResources : SHBaseTable
{
    #region Members
    Dictionary<eSceneType, List<string>> m_pData = new Dictionary<eSceneType, List<string>>();
    #endregion


    #region System Functions
    public JsonPreloadResources()
    {
        m_strFileName = "PreloadResources";
    }
    #endregion


    #region Virtual Functions
    public override void Initialize()
    {
        m_pData.Clear();
    }
    public override bool IsLoadTable()
    {
        return (0 != m_pData.Count);
    }
    public override eErrorCode LoadJsonTable(JsonData pJson, string strFileName)
    {
        if (null == pJson)
            return eErrorCode.Table_Load_Fail;
        
        SHUtils.For(0, pJson["PreLoadResourcesList"].Count, (iLoop) => 
        {
            var pDataNode = pJson["PreLoadResourcesList"][iLoop];
            SHUtils.ForToEnum<eSceneType>((eType) => 
            {
                SHUtils.For(0, pDataNode[eType.ToString()].Count, (iDataIndex) => 
                {
                    AddData(eType, (string)pDataNode[eType.ToString()][iDataIndex]);
                });
            });
        });

        return eErrorCode.Succeed;
    }
    #endregion


    #region Interface Functions
    public override ICollection GetData()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        return m_pData;
    }
    public List<string> GetData(eSceneType eType)
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        if (false == m_pData.ContainsKey(eType))
            return new List<string>();

        return m_pData[eType];
    }
    #endregion


    #region Utility Functions
    void AddData(eSceneType eType, string strData)
    {
        if (false == m_pData.ContainsKey(eType))
            m_pData.Add(eType, new List<string>());

        strData = strData.ToLower();
        m_pData[eType].Add(strData);
    }
    #endregion
}