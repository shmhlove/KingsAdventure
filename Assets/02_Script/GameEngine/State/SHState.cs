using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using DicState     = System.Collections.Generic.Dictionary<int, SHStateInfo>;
using ListAutoFlow = System.Collections.Generic.List<System.Func<int, eReturnAutoFlow>>;

public enum eReturnAutoFlow
{
    Keep,
    Next,
}

public abstract class SHState : SHMonoWrapper
{
    #region Members : Basic
    public  int      m_iCurrentStateID  = -1;
    public  int      m_iBeforeStateID   = -1;
    public  int      m_iFixedTick       = -1;
    private DicState m_dicState         = new DicState();
    #endregion


    // AutoFlow : 등록한 함수를 순차적으로 호출해주는 기능
    // AddAutoFlowState를 호출한 순서대로 함수를 호출해주며,
    // ReturnValue의 타입에 따라 다음으로 넘길지 말지를 결정.
    #region Members : AutoFlowState
    private ListAutoFlow m_pAutoFlowState = new ListAutoFlow();
    #endregion


    #region Virtual Functions
    public abstract void RegisterState();
    #endregion


    #region System Functions
    public void FrameMove()
    {
        if (false == IsActive())
            return;

        CallToAutoFlow();
        CallToFixedUpdate();
    }
    #endregion


    #region Interface Functions
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
        
        m_iBeforeStateID  = m_iCurrentStateID;
        m_iCurrentStateID = iChangeStateID;
        pChangeState.m_iFixedTick = (m_iFixedTick = -1);
        pChangeState.OnEnterState(m_iBeforeStateID);
    }
    public bool IsExistAutoFlowState()
    {
        return (0 != m_pAutoFlowState.Count);
    }
    public void AddAutoFlowState(Func<int, eReturnAutoFlow> pStateFunc)
    {
        m_pAutoFlowState.Add(pStateFunc);
    }
    #endregion


    #region Utility : Update Functions
    void CallToAutoFlow()
    {
        if (false == IsExistAutoFlowState())
            return;

        var pFunc = m_pAutoFlowState[0];
        switch(pFunc(m_iCurrentStateID))
        {
            case eReturnAutoFlow.Next:
                m_pAutoFlowState.Remove(pFunc);
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
    #endregion


    #region Utility : Helpper Functions
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
    #endregion
}