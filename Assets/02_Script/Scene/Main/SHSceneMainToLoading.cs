using UnityEngine;
using System.Collections;

public class SHSceneMainToLoading : MonoBehaviour
{
    void Start()
    {
        Single.AppInfo.CreateSingleton();
    }
}
