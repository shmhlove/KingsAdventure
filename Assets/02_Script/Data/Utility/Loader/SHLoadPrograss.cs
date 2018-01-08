using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

// public class : 로드 프로그래스
/* Summary
 * --------------------------------------------------------------------------------------
 * 로딩 플로우의 세부기능을 담고 있으며 Loader를 통해 제어됩니다.
 * --------------------------------------------------------------------------------------
 */
public class SHLoadPrograss
{
    #region Members
    // 로드 대기 중인 데이터 정보
    private Queue<SHLoadDataStateInfo> m_qLoadDatumWaitQueue = new Queue<SHLoadDataStateInfo>();

    // 로드 진행 중인 데이터 정보<파일명, 파일정보>
    private Dictionary<string, SHLoadDataStateInfo> m_dicLoadingDatum = new Dictionary<string, SHLoadDataStateInfo>();

    // 전체 데이터 정보 : <데이터 타입, <파일명, 파일정보>>
    private Dictionary<eDataType, Dictionary<string, SHLoadDataStateInfo>> m_dicAllLoadDatum = new Dictionary<eDataType, Dictionary<string, SHLoadDataStateInfo>>();
    
    public bool m_bIsLoadFail     = false; // 파일 하나라도 실패한 적이 있는가?
    public bool m_bIsDone         = false; // 모든 데이터 로드를 완료했는가?
    #endregion


    #region Virtual Functions
    public void Initialize()
    {
        m_qLoadDatumWaitQueue.Clear();
        m_dicAllLoadDatum.Clear();
        m_dicLoadingDatum.Clear();
    
        m_iLoadDoneCount  = 0;
        m_iTotalDataCount = 0;
        m_bIsLoadFail     = false;
        m_bIsDone         = false;
    }
    #endregion


    #region Interface Functions
    public void AddLoadDatum(Dictionary<string, SHLoadData> dicLoadDatum)
    {
        SHUtils.ForToDic(dicLoadDatum, (strName, pData) => 
        {
            // 무결성체크
            if (null == pData)
                return;

            // 중복파일체크
            var pLoadData = GetLoadDataInfo(strName);
            if (null != pLoadData)
            {
                Debug.LogErrorFormat("데이터 로드 중 중복파일 발견!!!(FileName : {0})", strName);
                return;
            }

            // 등록
            AddLoadData(pData);
        });
    }
    
    private void AddLoadData(SHLoadData pData)
    {
        if (null == pData)
            return;

        if (false == m_dicAllLoadDatum.ContainsKey(pData.m_eDataType))
            m_dicAllLoadDatum.Add(pData.m_eDataType, new Dictionary<string, SHLoadDataStateInfo>());

        var pDataStateInfo = new SHLoadDataStateInfo(pData);
        m_qLoadDatumWaitQueue.Enqueue(pDataStateInfo);
        m_dicAllLoadDatum[pData.m_eDataType][pData.m_strName.ToLower()] = pDataStateInfo;
    }

    public SHLoadDataStateInfo GetLoadDataInfo(string strName)
    {
        foreach (var kvp in m_dicAllLoadDatum)
        {
            if (true == kvp.Value.ContainsKey(strName.ToLower()))
                return kvp.Value[strName.ToLower()];
        }
        return null;
    }

    public void EnqueueWaitingDataInfo(SHLoadDataStateInfo pLoadDataInfo)
    {
        if (true == m_qLoadDatumWaitQueue.Contains(pLoadDataInfo))
            return;

        m_qLoadDatumWaitQueue.Enqueue(pLoadDataInfo);
    }

    public SHLoadDataStateInfo DequeueWaitingDataInfo()
    {
        if (0 == m_qLoadDatumWaitQueue.Count)
            return null;

        var pDataInfo = m_qLoadDatumWaitQueue.Dequeue();
        if (null == pDataInfo)
            return null;

        return pDataInfo;
    }

    public int GetLoadDoneCount()
    {
        int iLoadDoneCount = 0;
        foreach (var kvp1 in m_dicAllLoadDatum)
        {
            foreach(var kvp2 in kvp1.Value)
            {
                if (true == kvp2.Value.IsDone())
                    ++iLoadDoneCount;
            }
        }
        return iLoadDoneCount;
    }

    public int GetTotalCount()
    {
        int iTotalCount = 0;
        foreach(var kvp in m_dicAllLoadDatum)
        {
            iTotalCount += kvp.Value.Count;
        }
        return iTotalCount;
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

        if (true == m_dicLoadingDatum.ContainsKey(strFileName))
            m_dicLoadingDatum.Remove(strFileName);

        if (false == m_bIsFail)
            m_bIsFail = (false == bIsSuccess);

        if (false == m_bIsDone)
            m_bIsDone = (m_pLoadCount.Value1 <= ++m_pLoadCount.Value2);

        return pData;
    }

    public void AddLoadStartInfo(string strFileName, SHLoadStartInfo pInfo)
    {
        if (null == pInfo)
            return;

        var pData = GetLoadDataInfo(strFileName);
        if (null == pData)
            return;

        if (true == pData.m_bIsDone)
            return;

        m_dicLoadingDatum[strFileName] = pInfo;
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

        return pData.IsDone();
    }

    public bool IsDone(eDataType eType)
    {
        if (false == m_dicAllLoadDatum.ContainsKey(eType))
            return true;
        
        foreach (var kvp in m_dicAllLoadDatum[eType])
        {
            if (false == kvp.Value.IsDone())
                return false;
        }

        return true;
    }
    #endregion
}
