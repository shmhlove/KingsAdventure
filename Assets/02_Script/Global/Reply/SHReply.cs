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

    public SHReply(SHReply pReply)
    {
        Error = pReply.Error;
    }

    public T GetAs<T>() where T : SHReply
    {
        if (this is T)
            return this as T;
        else
            return (new SHReply(this)) as T;
    }
}
