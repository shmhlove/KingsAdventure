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
    #region Members
    public eGameStep m_eStep = eGameStep.None;
    #endregion


    #region Virtual Functions
    public virtual void Awake() { }                    // MoveTo가 호출되는 순간 호출
    public virtual void InitialStep() { }              // ChangeStep가 호출되고 난 다음 프레임에 호출
    public virtual void FrameMove(int iCallCnt) { }    // 매프레임 호출
    public virtual void FinalStep() { }                // ChangeStep가 호출되고 난 다음 프레임에 호출
    public virtual void Pause() { }                    // Step이 Pause될때
    public virtual void Resume() { }                   // Step이 Resume될때
    #endregion


    #region System Functions
    #endregion


    #region Interface Functions
    public void MoveTo(eGameStep eMoveStep)
    {
        Single.GameStep.MoveTo(eMoveStep);
    }
    public void DirectMoveTo(eGameStep eMoveStep)
    {
        Single.GameStep.DirectMoveTo(eMoveStep);
    }
    #endregion


    #region Utility Functions
    #endregion


    #region Event Handler
    #endregion
}

public class SHGameStep : SHBaseEngine
{
    #region Members : Step
    private DicStep         m_dicSteps      = new DicStep();
    #endregion


    #region Members : Step Info
    public int              m_iCallCnt      = 0;
    public eGameStep        m_eBeforeStep   = eGameStep.None;
    public eGameStep        m_eCurrentStep  = eGameStep.None;
    public eGameStep        m_eMoveTo       = eGameStep.None;
    #endregion
    

    #region Virtual Functions
    public override void OnInitialize()
    {
        m_dicSteps.Clear();

        m_iCallCnt     = 0;
        m_eBeforeStep  = eGameStep.None;
        m_eCurrentStep = eGameStep.None;
        m_eMoveTo      = eGameStep.None;
    }

    public override void OnFinalize()
    {
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

        m_dicSteps[m_eCurrentStep].FrameMove(++m_iCallCnt);
    }
    public override void SetPause(bool bIsPause)
    {
        m_bIsPause = bIsPause;

        if (false == IsExistStep(m_eCurrentStep))
            return;

        if (true == m_bIsPause)
            m_dicSteps[m_eCurrentStep].Pause();
        else
            m_dicSteps[m_eCurrentStep].Resume();
    }
    #endregion


    #region Interface : Control
    public void MoveTo(eGameStep eStep)
    {
        if (false == IsExistStep(eStep))
        {
            Debug.LogWarningFormat("SHGameStep:MoveStep() - Not Register Step : {0}", eStep);
            return;
        }

        m_eMoveTo = eStep;
        m_dicSteps[m_eMoveTo].m_eStep = m_eMoveTo;
        m_dicSteps[m_eMoveTo].Awake();
    }
    public void DirectMoveTo(eGameStep eStep)
    {
        MoveTo(eStep);
        ChangeStep();
    }
    #endregion


    #region Utility Functions
    private void ChangeStep()
    {
        if (eGameStep.None == m_eMoveTo)
            return;

        if (false == IsExistStep(m_eMoveTo))
            return;

        if (true == IsExistStep(m_eCurrentStep))
            m_dicSteps[m_eCurrentStep].FinalStep();

        m_iCallCnt      = 0;
        m_eBeforeStep   = m_eCurrentStep;
        m_eCurrentStep  = m_eMoveTo;
        m_eMoveTo       = eGameStep.None;

        m_dicSteps[m_eCurrentStep].InitialStep();
    }
    private bool IsExistStep(eGameStep eStep)
    {
        return m_dicSteps.ContainsKey(eStep);
    }
    #endregion
}