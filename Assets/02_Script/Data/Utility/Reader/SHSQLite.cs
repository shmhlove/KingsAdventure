using UnityEngine;

using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using Community.CsharpSqlite;

public class SHSQLite
{
    #region Members
    private SQLiteDB m_pSQLiteDB = null;
    #endregion


    #region System Functions
    public SHSQLite() { }
    public SHSQLite(string strFileName)
    {
        if (true == string.IsNullOrEmpty(strFileName))
            return;

        // StreamingAssets에서 .db를 byte[]형태로 읽어서 
        // PersistentDataPath에 저장하고, 그걸 로드하도록 한다.

        SaveDBToBytes(GetPersistentPath(strFileName), LoadWWW(GetStreamingPath(strFileName)));

        try
        {
            m_pSQLiteDB = new SQLiteDB();
            m_pSQLiteDB.Open(GetPersistentPath(strFileName));
        }
        catch (System.Exception e)
        {
            Debug.LogError(string.Format("SQLite Read Fail : {0}", e.ToString()));
        }
    }

    ~SHSQLite()
    {
        Clear();
    }
    #endregion


    #region Interface Functions
    public void Clear()
    {
        if (null == m_pSQLiteDB)
            return;

        m_pSQLiteDB.Close();
        m_pSQLiteDB = null;
    }

    public bool CheckDBFile()
    {
        return (null != m_pSQLiteDB);
    }

#if UNITY_EDITOR
    public void Write(string strFileName, Dictionary<string, List<SHTableDataSet>> dicData)
    {
        if (null == dicData)
        {
            Debug.LogError(string.Format("SQLite로 저장할 데이터가 없습니다!!"));
            return;
        }   

        string strSavePath  = string.Format("{0}/{1}.db", SHPath.GetPathToSQLite(), Path.GetFileNameWithoutExtension(strFileName));

        File.Delete(strSavePath);

        try
        {
            m_pSQLiteDB = new SQLiteDB();
            m_pSQLiteDB.Open(strSavePath);
            SHUtils.ForToDic(dicData, (pKey, pValue) =>
            {
                // 테이블 생성
                if (false == CreateTable(pKey, pValue[0]))
                    return;

                // 생성한 테이블에 데이터 인설트
                if (false == InsertData(pKey, pValue))
                    return;
            });
        }
        catch (System.Exception e)
        {
            Debug.LogError(string.Format("SQLite Read Fail : {0}", e.ToString()));
        }
    }
#endif
    #endregion


    #region Query Functions
    // 쿼리 : 테이블 생성("CREATE TABLE `TableName` (`FieldName1` INTEGER, `FieldName2` INTEGER, `FieldName3` INTEGER);")
    public bool CreateTable(string strTableName, SHTableDataSet pRowDataSet)
    {
        if (null == pRowDataSet)
            return false;

        string strFields = string.Empty;
        for (int iCol = 0; iCol < pRowDataSet.m_iMaxCol; ++iCol)
        {
            string strColName = pRowDataSet.m_ColumnNames[iCol];
            string strColType = pRowDataSet.m_ColumnTypes[iCol];
            strFields += string.Format("\"{0}\" {1}", strColName, SHTableDataUtil.GetTypeToDB(strColType));

            if (iCol + 1 < pRowDataSet.m_iMaxCol)
                strFields += ", ";
        }

        string strQuery = string.Empty;
        if (string.Empty == strFields)
            strQuery = string.Format("CREATE TABLE \"{0}\";", strTableName);
        else
            strQuery = string.Format("CREATE TABLE \"{0}\" ({1});", strTableName, strFields);

        return WriteQuery(strQuery);
    }

    // 쿼리 : 데이터 추가("INSERT INTO 'TableName' VALUES(strValue1, 1);")
    public bool InsertData(string strTableName, List<SHTableDataSet> pRowDataSets)
    {
        if (null == pRowDataSets)
            return false;

        int iMaxRow = pRowDataSets.Count;
        for (int iRow = 0; iRow < iMaxRow; ++iRow)
        {
            string strValues = string.Empty;
            for (int iCol = 0; iCol < pRowDataSets[iRow].m_iMaxCol; ++iCol)
            {
                strValues += pRowDataSets[iRow].m_pDatas[iCol];

                if (iCol + 1 < pRowDataSets[iRow].m_iMaxCol)
                    strValues += ", ";
            }

            string strQuery = string.Format("INSERT INTO \"{0}\" VALUES({1});", strTableName, strValues);
            if (false == WriteQuery(strQuery))
                return false;
        }

        return true;
    }

    // 쿼리 : 테이블 얻기("SELECT * FROM PC1;";)
    public SQLiteQuery GetTable(string strTableName)
    {
        return ReadQuery(string.Format("SELECT * FROM {0};", strTableName));
    }

	public SQLiteQuery Query(string table, string whereState)
	{
        string query = string.Format("SELECT * FROM {0} WHERE {1};", table, whereState);
		return ReadQuery(query);
	}

    // 읽는 쿼리
    public SQLiteQuery ReadQuery(string strQuery)
    {
        if (null == m_pSQLiteDB)
            return null;

        if (string.Empty == strQuery)
            return null;

        return new SQLiteQuery(m_pSQLiteDB, strQuery);
    }

    // 쓰는 쿼리
    public bool WriteQuery(string strQuery)
    {
        if (null == m_pSQLiteDB)
            return false;

        if (string.Empty == strQuery)
            return false;

        string strErr = string.Empty;
        Sqlite3.exec(m_pSQLiteDB.Connection(), strQuery, null, null, ref strErr);

        if (string.Empty != strErr)
        {
            Debug.LogError(string.Format("QueryError : {0}\n{1}", strQuery, strErr));
            return false;
        }

        return true;
    }
    #endregion


    #region Utility Functions
    byte[] LoadWWW(string strFilePath)
    {
        WWW pWWW = Single.Coroutine.WWWOfSync(new WWW(strFilePath));
        if (true != string.IsNullOrEmpty(pWWW.error))
        {
            Debug.LogError(string.Format("SQLite(*.db)파일을 읽는 중 오류발생!!(Path:{0}, Error:{1})", strFilePath, pWWW.error));
            return null;
        }

        return pWWW.bytes;
    }

    string GetPersistentPath(string strFileName)
    {
        return string.Format("{0}/{1}.db", SHPath.GetPathToPersistentSQLite(), Path.GetFileNameWithoutExtension(strFileName));
    }

    string GetStreamingPath(string strFileName)
    {
        string strPath = string.Empty;

#if UNITY_EDITOR || UNITY_STANDALONE
        strPath = string.Format("{0}{1}",       "file://", SHPath.GetPathToStreamingAssets());
#elif UNITY_ANDROID
        strPath = string.Format("{0}{1}{2}",    "jar:file://", SHPath.GetPathToAssets(), "!/assets");
#elif UNITY_IOS
        strPath = string.Format("{0}{1}{2}",    "file://", SHPath.GetPathToAssets(), "/Raw");
#endif

        return string.Format("{0}/SQLite/{1}.db", strPath, Path.GetFileNameWithoutExtension(strFileName));
    }
    void SaveDBToBytes(string strFilePath, byte[] pBytes)
    {
        if (null == pBytes)
            return;

        try
        {
            SHUtils.SaveByte(pBytes, strFilePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError(string.Format("SQLite Write Fail : {0}", e.ToString()));
        }
    }

    // 데이터 셋 생성
    // DB에 어떤 시트 어떤 데이터가 있는지 모르는 상태에서 모든 데이터를 string형태로 뽑아 버리는 기능
    public Dictionary<string, List<SHTableDataSet>> GetDataSet()
    {
        var dicData = new Dictionary<string, List<SHTableDataSet>>();

        // 테이블 리스트 생성
        var strTableList        = "TableList";
        var pQuery              = GetTable(strTableList);
        var pTableList          = GetTableDataSet(pQuery, strTableList);
        dicData[strTableList]   = pTableList;
        pQuery.Release();

        // 테이블별 데이터 생성
        SHUtils.ForToList(pTableList, (pTable) =>
        {
            string strTableName     = pTable.m_pDatas[0];
            pQuery                  = GetTable(strTableName);
            strTableName            = strTableName.Trim('"');
            dicData[strTableName]   = GetTableDataSet(pQuery, strTableName);;
            pQuery.Release();
        });
        return dicData;
    }

    List<SHTableDataSet> GetTableDataSet(SQLiteQuery pQuery, string strTableName)
    {
        if (null == pQuery)
            return null;

        var pDataList = new List<SHTableDataSet>();
        while (true == pQuery.Step())
        {
            SHTableDataSet pData = new SHTableDataSet();
            int iMaxColumn  = pQuery.Names.Length;
            for (int iCol = 0; iCol < iMaxColumn; ++iCol)
            {
                string strColName = pQuery.Names[iCol];
                pData.AddData(strTableName,
                    strColName, SHTableDataUtil.GetTypeToName(strColName),
                    pQuery.GetString(strColName));
            }
            pDataList.Add(pData);
        }
        return pDataList;
    }
    #endregion
}

// 자주 사용하는 Query
// "DROP TABLE IF EXISTS PC1;";
// "CREATE TABLE IF NOT EXISTS PC1 (id INTEGER PRIMARY KEY, str_field TEXT, blob_field BLOB);";
// "INSERT INTO PC1 (str_field,blob_field) VALUES(?,?);";
// "SELECT * FROM PC1;";