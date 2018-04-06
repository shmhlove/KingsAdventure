using UnityEngine;
using System;
using System.Collections;

public class SHGameEngine : SHSingleton<SHGameEngine>
{
    private SHGameStep m_pGameStep = new SHGameStep();
    
    public override void OnInitialize()
    {
        m_pGameStep.OnInitialize();
    }
    public override void OnFinalize()
    {
        m_pGameStep.OnFinalize();
    }
    
    public void StartEngine()
    {
        m_pGameStep.OnInitialize();
    }

    public void FrameMove()
    {
        m_pGameStep.OnFrameMove();
    }
    
    public SHGameStep GetGameStep()
    {
        return m_pGameStep;
    }
}
