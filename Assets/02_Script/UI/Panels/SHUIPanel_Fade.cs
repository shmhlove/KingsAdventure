using UnityEngine;
using System;
using System.Collections;

public class SHUIPanel_Fade : SHUIBasePanel
{
    public override void OnAfterShow(params object[] pArgs)
    {
        if ((null == pArgs) || (1 > pArgs.Length))
            return;

        var pCallback = ((Action)pArgs[0]);
        if (null == pCallback)
            return;

        pCallback();
    }
}
