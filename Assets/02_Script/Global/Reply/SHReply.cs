using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHReply
{
    public SHError m_pError;

    public SHReply()
    {
    }

    public SHReply(string strJson)
    {
    }

    public SHReply(SHError pError)
    {
        m_pError = pError;
    }

    public T GetAs<T>() where T : SHReply
    {
        return this as T;
    }
}
