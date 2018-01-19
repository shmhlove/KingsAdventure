using UnityEngine;
using System;
using System.Collections;

public class SHSceneMainToEntrance : SHMonoWrapper
{
    #region System Functions
    public override void Start() 
    {
        base.Start();
        Single.AppInfo.CreateSingleton();
	}
    #endregion
    

    #region Event Handler
    void OnEventToNextScene()
    {
        Single.Scene.GoTo(eSceneType.InGame);
    }
    #endregion
}
