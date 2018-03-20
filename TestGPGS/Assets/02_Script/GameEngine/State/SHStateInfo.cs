using UnityEngine;
using System;
using System.Collections;

public class SHStateInfo
{
    #region Members
    public int              m_iStateID      = -1;
    public int              m_iFixedTick    = -1;

    public Action<int, int> m_OnEnterState  = null; // <iBeforeState,  iCurrentState>
    public Action<int, int> m_OnExitState   = null; // <iBeforeState,  iCurrentState>
    public Action<int, int> m_OnFixedUpdate = null; // <iCurrentState, iFixedTick>
    #endregion


    #region Interface Functions
    public void OnEnterState(int iBeforeState)
    {
        if (null == m_OnEnterState)
            return;

        m_OnEnterState(iBeforeState, m_iStateID);
    }

    public void OnExitState(int iAfterState)
    {
        if (null == m_OnExitState)
            return;

        m_OnExitState(m_iStateID, iAfterState);
    }
    
    public void OnFixedUpdate()
    {
        if (null == m_OnFixedUpdate)
            return;

        m_OnFixedUpdate(m_iStateID, m_iFixedTick);
    }
    #endregion
}