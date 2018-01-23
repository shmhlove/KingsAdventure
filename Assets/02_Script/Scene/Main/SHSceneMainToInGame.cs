using UnityEngine;
using System;
using System.Collections;

public class SHSceneMainToInGame : MonoBehaviour 
{
    #region System Functions
    void Start()
    {
        Single.AppInfo.CreateSingleton();
        Single.Engine.StartEngine();
    }
    void FixedUpdate()
    {
        Single.Engine.FrameMove();
    }
    #endregion
}
