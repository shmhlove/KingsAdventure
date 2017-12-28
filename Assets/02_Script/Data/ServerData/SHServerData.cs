using UnityEngine;

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public partial class SHServerData : SHBaseData
{
    #region Virtual Functions
    public override void OnInitialize() { }

    public override void OnFinalize() { }

    public override void FrameMove() { }

    public override Dictionary<string, SHLoadData> GetLoadList(eSceneType eType) 
    { 
        return new Dictionary<string, SHLoadData>(); 
    }

    public override Dictionary<string, SHLoadData> GetPatchList() 
    { 
        return new Dictionary<string, SHLoadData>(); 
    }

    public override void Load(SHLoadData pInfo, Action<string, SHLoadStartInfo> pStart, 
                                                Action<string, SHLoadEndInfo> pDone) { }

    public override void Patch(SHLoadData pInfo, Action<string, SHLoadStartInfo> pStart, 
                                                 Action<string, SHLoadEndInfo> pDone) { }
    #endregion
}