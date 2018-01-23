using UnityEngine;
using System.Collections;

public class SHSceneMainToLoading : MonoBehaviour
{
    #region System Functions
    void Start()
    {
        Single.AppInfo.CreateSingleton();
    }
    #endregion
}
