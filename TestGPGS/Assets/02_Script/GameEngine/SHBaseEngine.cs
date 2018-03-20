using UnityEngine;
using System.Collections;

public class SHBaseEngine
{
    #region Members
    public bool m_bIsPause = false;
    #endregion


    #region Virtual Functions
    public virtual void OnInitialize() { }
    public virtual void OnFinalize() { }
    public virtual void OnFrameMove() { }
    public virtual void SetPause(bool bIsPause)
    {
        m_bIsPause = bIsPause;
    }
    #endregion
}
