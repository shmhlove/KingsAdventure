using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHTableDataSet
{
    public string           m_strTableName;
    public int              m_iMaxCol;
    public List<string>     m_ColumnNames = new List<string>();
    public List<string>     m_ColumnTypes = new List<string>();
    public List<string>     m_pDatas      = new List<string>();
    
    public void AddData(string strTableName, string strColName, string strColType, string strData)
    {
        m_strTableName = strTableName;

        m_ColumnNames.Add(strColName);
        m_ColumnTypes.Add(strColType);

        if ("text" == strColType)
            m_pDatas.Add(string.Format("\"{0}\"", strData));
        else
            m_pDatas.Add(strData);

        m_iMaxCol++;
    }
}