using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHLoadPrograss
{
    #region Members
    // 로드 카운트 : <Total, Current>
    private SHPair<int, int> m_pLoadCount               = new SHPair<int, int>(0, 0);

    // 남은 데이터 정보
    private Queue<SHLoadData> m_qLoadQueue              = new Queue<SHLoadData>();

    // 로드 중인 데이터 정보
    private Dictionary<string, SHLoadStartInfo> m_dicLoadingFiles = new Dictionary<string, SHLoadStartInfo>();
    public Dictionary<string, SHLoadStartInfo> LoadingFiles { get { return m_dicLoadingFiles; } }

    // 전체 데이터 정보 : <데이터타입, <파일명, 파일정보>>
    private Dictionary<eDataType, Dictionary<string, SHLoadData>> m_dicTotalLoadData = new Dictionary<eDataType, Dictionary<string, SHLoadData>>();

    // 실패한 파일이 하나라도 있는가?
    public bool m_bIsFail = false;

    // 로드를 완료했는가?
    public bool m_bIsDone = false;
    #endregion


    #region Virtual Functions
    public void Initialize()
    {
        m_pLoadCount.Initialize();
        m_qLoadQueue.Clear();
        m_dicTotalLoadData.Clear();
        m_dicLoadingFiles.Clear();
        m_bIsFail = false;
        m_bIsDone = false;
    }
    #endregion


    #region Interface Functions
    public void AddLoadInfo(Dictionary<string, SHLoadData> dicLoadList)
    {
        SHUtils.ForToDic(dicLoadList, (pKey, pValue) => 
        {
            // 무결성체크
            if (null == pValue)
                return;

            // 중복파일체크
            SHLoadData pLoadData = GetLoadDataInfo(pValue.m_strName);
            if (null != pLoadData)
            {
                Debug.LogError(string.Format("중복파일 발견!!!(FileName : {0})", pValue.m_strName));
                return;
            }

            // 초기화 및 등록
            SetLoadData(pValue);
            m_pLoadCount.Value1++;
        });
    }

    public SHLoadData GetLoadDataInfo(string strName)
    {
        strName = strName.ToLower();
        foreach(var kvp in m_dicTotalLoadData)
        {
            if (true == kvp.Value.ContainsKey(strName))
                return kvp.Value[strName];
        }
        return null;
    }

    public void SetLoadData(SHLoadData pData)
    {
        if (null == pData)
            return;

        pData.m_bIsDone = false;
        m_qLoadQueue.Enqueue(pData);

        if (false == m_dicTotalLoadData.ContainsKey(pData.m_eDataType))
            m_dicTotalLoadData.Add(pData.m_eDataType, new Dictionary<string, SHLoadData>());

        m_dicTotalLoadData[pData.m_eDataType][pData.m_strName.ToLower()] = pData;
    }

    public SHLoadData GetLoadData()
    {
        if (0 == m_qLoadQueue.Count)
            return null;

        var pData = m_qLoadQueue.Dequeue();
        if (null == pData)
            return null;

        Single.Timer.StartDeltaTime(pData.m_strName);
        return pData;
    }

    public SHLoadData SetLoadFinish(string strFileName, bool bIsSuccess)
    {
        var pData = GetLoadDataInfo(strFileName);
        if (null == pData)
        {
            Debug.LogError(string.Format("추가되지 않은 파일이 로드됫다고 합니다~~({0})", strFileName));
            return null;
        }

        pData.m_bIsSuccess  = bIsSuccess;
        pData.m_bIsDone     = true;

        if (true == m_dicLoadingFiles.ContainsKey(strFileName))
            m_dicLoadingFiles.Remove(strFileName);

        if (false == m_bIsFail)
            m_bIsFail = (false == bIsSuccess);

        if (false == m_bIsDone)
            m_bIsDone = (m_pLoadCount.Value1 <= ++m_pLoadCount.Value2);

        return pData;
    }

    public void SetLoadStart(string strFileName, SHLoadStartInfo pInfo)
    {
        if (null == pInfo)
            return;

        var pData = GetLoadDataInfo(strFileName);
        if (null == pData)
            return;

        if (true == pData.m_bIsDone)
            return;

        m_dicLoadingFiles[strFileName] = pInfo;
    }

    public SHPair<int, int> GetCountInfo()
    {
        return m_pLoadCount;
    }

    public void StartLoadTime()
    {
        Single.Timer.StartDeltaTime("LoadingTime");
    }

    public float GetLoadTime()
    {
        return Single.Timer.GetDeltaTimeToSecond("LoadingTime");
    }

    public SHPair<float, float> GetLoadTime(string strFileName)
    {
        return new SHPair<float, float>(GetLoadTime(),
             Single.Timer.GetDeltaTimeToSecond(strFileName));
    }

    public bool IsDone(string strFileName)
    {
        var pData = GetLoadDataInfo(strFileName);
        if (null == pData)
            return true;

        return pData.m_bIsDone;
    }

    public bool IsDone(eDataType eType)
    {
        if (false == m_dicTotalLoadData.ContainsKey(eType))
            return true;

        var dicDataInfo = m_dicTotalLoadData[eType];
        foreach (var kvp in dicDataInfo)
        {
            if (null == kvp.Value)
                continue;

            if (false == kvp.Value.m_bIsDone)
                return false;
        }

        return true;
    }
    #endregion
}
