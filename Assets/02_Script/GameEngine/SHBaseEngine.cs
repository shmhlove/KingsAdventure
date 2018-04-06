using UnityEngine;
using System.Collections;

public class SHBaseEngine
{
    public bool m_bIsPause = false;

    public virtual void OnInitialize() { }
    public virtual void OnFinalize() { }
    public virtual void OnFrameMove() { }
    public virtual void SetPause(bool bIsPause)
    {
        m_bIsPause = bIsPause;
    }
}
