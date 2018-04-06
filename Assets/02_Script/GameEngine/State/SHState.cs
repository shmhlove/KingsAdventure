using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using DicState      = System.Collections.Generic.Dictionary<int, SHStateInfo>;
using ListCallQueue = System.Collections.Generic.List<System.Func<int, eReturnCallQueueType>>;

public enum eReturnCallQueueType
{
    Keep,   // 아직 현재 함수가 종료되지 않았음
    Next,   // 현재 함수를 종료하고, 다음 함수로 넘김
}

public abstract class SHState : SHMonoWrapper
{
    public  int      m_iCurrentStateID  = -1;
    public  int      m_iBeforeStateID   = -1;
    public  int      m_iFixedTick       = -1;
    private DicState m_dicState         = new DicState();

    // CallQueue : 등록한 함수를 순차적으로 호출해주는 기능
    // AddAutoFlowState를 호출한 순서대로 함수를 호출해주며,
    // ReturnValue의 타입에 따라 다음으로 넘길지 말지를 결정.
    private ListCallQueue m_pCallQueue = new ListCallQueue();

    #region Virtual Functions
    public abstract void RegisterState();
    #endregion
    
    public void FrameMove()
    {
        if (false == IsActive())
            return;

        CallQueue();
        CallToFixedUpdate();
    }

    public SHStateInfo CreateState(int iStateID)
    {
        return new SHStateInfo()
        {
            m_iStateID = iStateID,
        };
    }

    public void AddState(int iStateID, SHStateInfo pInfo)
    {
        if (null == pInfo)
        {
            Debug.LogErrorFormat("SHState::AddState - StateInfo Is Null!! : {0}", iStateID);
            return;
        }

        if (true == m_dicState.ContainsKey(iStateID))
            m_dicState[iStateID] = pInfo;
        else
            m_dicState.Add(iStateID, pInfo);
    }

    public void ChangeState(int iChangeStateID)
    {
        var pChangeState = GetStateInfo(iChangeStateID);
        if (null == pChangeState)
            return;

        var pCurrentState = GetStateInfo(m_iCurrentStateID);
        if (null != pCurrentState)
            pCurrentState.OnExitState(iChangeStateID);
        
        m_iBeforeStateID = m_iCurrentStateID;
        m_iCurrentStateID = iChangeStateID;
        pChangeState.m_iFixedTick = (m_iFixedTick = -1);
        pChangeState.OnEnterState(m_iBeforeStateID);
    }

    public bool IsExistCallQueue()
    {
        return (0 != m_pCallQueue.Count);
    }

    public void AddCallQueue(Func<int, eReturnCallQueueType> pFunc)
    {
        m_pCallQueue.Add(pFunc);
    }

    void CallQueue()
    {
        if (false == IsExistCallQueue())
            return;

        var pFunc = m_pCallQueue[0];
        switch(pFunc(m_iCurrentStateID))
        {
            case eReturnCallQueueType.Next:
                m_pCallQueue.Remove(pFunc);
                break;
        }
    }

    void CallToFixedUpdate()
    {
        var pState = GetCurrentState();
        if (null == pState)
            return;

        pState.m_iFixedTick = ++m_iFixedTick;
        pState.OnFixedUpdate();
    }

    SHStateInfo GetCurrentState()
    {
        return GetStateInfo(m_iCurrentStateID);
    }

    SHStateInfo GetStateInfo(int iStateID)
    {
        if (false == m_dicState.ContainsKey(iStateID))
            return null;
    
        return m_dicState[iStateID];
    }
}