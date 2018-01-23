﻿using UnityEngine;
using System;
using System.Collections;

public class SHSceneMainToAdministrator : SHMonoWrapper
{
    #region System Functions
    public override void Start() 
    {
        base.Start();
        Single.AppInfo.CreateSingleton();
	}
    #endregion
    

    public void OnClick()
    {
        Single.Scene.Addtive(eSceneType.InGame);
    }
}
