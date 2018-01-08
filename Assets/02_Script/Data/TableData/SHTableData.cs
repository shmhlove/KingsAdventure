using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public partial class SHTableData : SHBaseData
{
    #region Members
    private Dictionary<Type, SHBaseTable> m_dicTables = new Dictionary<Type, SHBaseTable>();
    public Dictionary<Type, SHBaseTable> Tables { get { return m_dicTables; } }
    #endregion


    #region Virtual Functions
    public override void OnInitialize()
    {
        m_dicTables.Clear();

        m_dicTables.Add(typeof(JsonClientConfig),          new JsonClientConfig());
        m_dicTables.Add(typeof(JsonServerConfig),          new JsonServerConfig());
        m_dicTables.Add(typeof(JsonPreloadResources),        new JsonPreloadResources());
        m_dicTables.Add(typeof(JsonResources),               new JsonResources());
        m_dicTables.Add(typeof(JsonAssetBundleInfo),              new JsonAssetBundleInfo());
    }
    public override void OnFinalize()
    {
        m_dicTables.Clear();
    }
    public override void FrameMove()
    {
    }
    public override Dictionary<string, SHLoadData> GetLoadList(eSceneType eType)
    {
        var dicLoadList = new Dictionary<string, SHLoadData>();

        // 로컬 테이블 데이터
        SHUtils.ForToDic<Type, SHBaseTable>(m_dicTables,
        (pKey, pValue) =>
        {
            // 이미 로드된 데이터인지 체크
            if (true == pValue.IsLoadTable())
                return;

            dicLoadList.Add(pValue.m_strFileName, CreateLoadInfo(pValue.m_strFileName));
        });

        return dicLoadList; 
    }
    public override Dictionary<string, SHLoadData> GetPatchList()
    {
        return new Dictionary<string, SHLoadData>();
    }
    public override IEnumerator Load(SHLoadData pInfo, 
                                     Action<string, SHLoadStartInfo> pStart,
                                     Action<string, SHLoadEndInfo> pDone)
    {
        pStart(pInfo.m_strName, new SHLoadStartInfo());

        SHBaseTable pTable = GetTable(pInfo.m_strName);
        if (null == pTable)
        {
            Debug.LogError(string.Format("[TableData] 등록된 테이블이 아닙니다.!!({0})", pInfo.m_strName));
            pDone(pInfo.m_strName, new SHLoadEndInfo(false, eErrorCode.Load_Table));
            return;
        }

        SHUtils.ForToList(GetLoadOrder(pTable), (pLambda) =>
        {
            pLambda((eResult) =>
            {
                switch(eResult)
                {
                    case eErrorCode.Table_Not_ExsitFile:
                    case eErrorCode.Table_Error_Grammar:
                    case eErrorCode.Table_Load_Fail:
                }
                if (null != bIsSuccess)
                {
                    if (true == bIsSuccess.Value)
                        pDone(pInfo.m_strName, new SHLoadEndInfo(true, eErrorCode.None));
                    else
                        pDone(pInfo.m_strName, new SHLoadEndInfo(true, eErrorCode.Load_Table));
                    return;
                }
            });
        });

        pDone(pInfo.m_strName, new SHLoadEndInfo(false, eErrorCode.Load_Table));
    }
    public override void Patch(SHLoadData pInfo, Action<string, SHLoadStartInfo> pStart,
                                                 Action<string, SHLoadEndInfo> pDone)
    {
    }
    #endregion


    #region Interface Functions
    public SHLoadData CreateLoadInfo(string strName)
    {
        return new SHLoadData()
        {
            m_eDataType = eDataType.LocalTable,
            m_strName   = strName,
            m_pLoadFunc = Load
        };
    }
    public T GetTable<T>() where T : SHBaseTable
    {
        return GetTable(typeof(T)) as T;
    }
    public SHBaseTable GetTable(Type pType)
    {
        if (0 == m_dicTables.Count)
            OnInitialize();

        if (false == m_dicTables.ContainsKey(pType))
            return null;

        return m_dicTables[pType];
    }
    public SHBaseTable GetTable(string strFileName)
    {
        if (true == string.IsNullOrEmpty(strFileName))
            return null;

        return GetTable(GetTypeToFileName(strFileName));
    }
    public ICollection GetData<T>()
    {
        return GetData(typeof(T));
    }
    public ICollection GetData(string strClassType)
    {
        return GetData(Type.GetType(strClassType));
    }
    public ICollection GetData(Type pType)
    {
        SHBaseTable pTable = GetTable(pType);
        if (null == pTable)
            return null;

        return pTable.GetData();
    }
    public Type GetTypeToFileName(string strFileName)
    {
        strFileName = Path.GetFileNameWithoutExtension(strFileName);
        foreach (var kvp in m_dicTables)
        {
            if (true == kvp.Value.m_strFileName.Equals(strFileName))
                return kvp.Key;
        }

        return null;
    }
    #endregion


    #region Utility Functions
    // 유틸 : 테이블 타입별 로드 순서 ( 앞선 타입의 로드에 성공하면 뒤 타입들은 로드명령 하지 않는다 )
    List<Func<bool?>> GetLoadOrder(SHBaseTable pTable)
    {
        var pLoadOrder = new List<Func<bool?>>();
        //if (true == Single.AppInfo.IsEditorMode())
        //{
        //    pLoadOrder.Add(() => { return pTable.LoadStatic();                        });
        //    pLoadOrder.Add(() => { return pTable.LoadXML(pTable.m_strFileName);       });
        //    pLoadOrder.Add(() => { return pTable.LoadBytes(pTable.m_strByteFileName); });
        //    pLoadOrder.Add(() => { return pTable.LoadJson(pTable.m_strFileName);      });
        //    pLoadOrder.Add(() => { return pTable.LoadDB(pTable.m_strFileName);        });
        //}
        //else
        {
            pLoadOrder.Add(() => { return pTable.LoadStatic();                        });
            pLoadOrder.Add(() => { return pTable.LoadBytes(pTable.m_strByteFileName); });
            pLoadOrder.Add(() => { return pTable.LoadXML(pTable.m_strFileName);       });
            pLoadOrder.Add(() => { return pTable.LoadJson(pTable.m_strFileName);      });
            pLoadOrder.Add(() => { return pTable.LoadDB(pTable.m_strFileName);        });
        }

        return pLoadOrder;
    }
    #endregion
}