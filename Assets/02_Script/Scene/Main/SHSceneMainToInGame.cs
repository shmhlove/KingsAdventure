using UnityEngine;
using System;
using System.Collections;

public class SHSceneMainToInGame : MonoBehaviour 
{
    #region Members
    #endregion


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


    #region Virtual Functions
    #endregion


    #region Interface Functions
    #endregion


    #region Utility Functions
    #endregion


    #region Event Handler
    #endregion
}
