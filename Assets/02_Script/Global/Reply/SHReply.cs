using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHReply
{
    public SHError Error;
    public bool IsSucceed
    {
        get { return (null == Error); }
    }
    
    public SHReply()
    {
    }

    public SHReply(string strJson)
    {
    }

    public SHReply(SHError pError)
    {
        Error = pError;
    }

    public T GetAs<T>() where T : SHReply
    {
        return this as T;
    }
}
