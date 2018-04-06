using UnityEngine;
using System;
using System.Collections;

public class SHSceneMainToIntro : SHMonoWrapper
{
    public override void Start() 
    {
        base.Start();
        Single.AppInfo.CreateSingleton();
	}
}
