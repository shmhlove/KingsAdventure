using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using DicStep = System.Collections.Generic.Dictionary<eGameStep, SHStepBase>;

public enum eGameStep
{
    None,
}

public class SHStepBase
{
    public eGameStep m_eStep = eGameStep.None;

    public virtual void OnAwake() { }                  // MoveTo가 호출되는 순간 호출
    public virtual void OnInitialize() { }             // ChangeStep가 호출되고 난 다음 프레임에 호출
    public virtual void OnFrameMove(int iCallCnt) { }  // 매프레임 호출
    public virtual void OnFinalize() { }               // ChangeStep가 호출되고 난 다음 프레임에 호출
    public virtual void OnPause() { }                  // Step이 Pause될때
    public virtual void OnResume() { }                 // Step이 Resume될때

    public void MoveTo(eGameStep eMoveStep)
    {
        Single.GameStep.MoveTo(eMoveStep);
    }
}

public class SHGameStep : SHBaseEngine
{
    private DicStep         m_dicSteps      = new DicStep();

    public int              m_iCallCnt      = 0;
    public eGameStep        m_eBeforeStep   = eGameStep.None;
    public eGameStep        m_eCurrentStep  = eGameStep.None;
    public eGameStep        m_eMoveTo       = eGameStep.None;

    public override void OnInitialize()
    {
        m_dicSteps.Clear();

        m_iCallCnt     = 0;
        m_eBeforeStep  = eGameStep.None;
        m_eCurrentStep = eGameStep.None;
        m_eMoveTo      = eGameStep.None;
    }
    
    public override void OnFrameMove()
    {
        if (true == m_bIsPause)
            return;

        if (0 == m_dicSteps.Count)
            return;

        ChangeStep();

        if (false == IsExistStep(m_eCurrentStep))
            return;

        m_dicSteps[m_eCurrentStep].OnFrameMove(++m_iCallCnt);
    }

    public override void SetPause(bool bIsPause)
    {
        m_bIsPause = bIsPause;

        if (false == IsExistStep(m_eCurrentStep))
            return;

        if (true == m_bIsPause)
            m_dicSteps[m_eCurrentStep].OnPause();
        else
            m_dicSteps[m_eCurrentStep].OnResume();
    }

    public void MoveTo(eGameStep eStep)
    {
        if (false == IsExistStep(eStep))
        {
            Debug.LogWarningFormat("SHGameStep:MoveStep() - Not Register Step : {0}", eStep);
            return;
        }

        m_eMoveTo = eStep;
        m_dicSteps[m_eMoveTo].m_eStep = m_eMoveTo;
        m_dicSteps[m_eMoveTo].OnAwake();
    }
    
    private void ChangeStep()
    {
        if (eGameStep.None == m_eMoveTo)
            return;

        if (false == IsExistStep(m_eMoveTo))
            return;

        if (true == IsExistStep(m_eCurrentStep))
            m_dicSteps[m_eCurrentStep].OnFinalize();

        m_iCallCnt      = 0;
        m_eBeforeStep   = m_eCurrentStep;
        m_eCurrentStep  = m_eMoveTo;
        m_eMoveTo       = eGameStep.None;

        m_dicSteps[m_eCurrentStep].OnInitialize();
    }

    private bool IsExistStep(eGameStep eStep)
    {
        return m_dicSteps.ContainsKey(eStep);
    }
}