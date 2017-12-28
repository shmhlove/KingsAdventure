using UnityEngine;
using System;
using System.Collections;

public class SHGameEngine : SHSingleton<SHGameEngine>
{
    #region Members
    private SHGameStep   m_pGameStep   = new SHGameStep();
    #endregion


    #region Virtual Functions
    public override void OnInitialize() { }
    public override void OnFinalize()
    {
        if (null != m_pGameStep)
            m_pGameStep.OnFinalize();
    }
    #endregion


    #region System Functions
    #endregion


    #region Interface : System
    public void StartEngine()
    {
        if (null != m_pGameStep)
            m_pGameStep.OnInitialize();
    }
    public void FrameMove()
    {
        if (null != m_pGameStep)
            m_pGameStep.OnFrameMove();
    }
    #endregion


    #region Interface : Helpper
    public SHGameStep GetGameStep()
    {
        return m_pGameStep;
    }
    #endregion


    #region Event Handler
    #endregion
}
