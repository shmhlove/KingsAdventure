using UnityEngine;
using UnityEngine.SocialPlatforms;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHAppleAuth
{
    public void OnInitialize()
    {
        Debug.LogErrorFormat("[SHAppleAuth] Call is OnInitialize");
    }

    public void OnFinalize()
    {
        Debug.LogErrorFormat("[SHAppleAuth] Call is OnFinalize");
    }
    
    public void Login(Action<bool> pCallback)
    {
        Debug.LogErrorFormat("[SHAppleAuth] Call is Login");

        if (true == IsLogin())
        {
            Debug.LogErrorFormat("[SHAppleAuth] Already Login!!({0})", GetUserName());
            pCallback(true);
            return;
        }

#if UNITY_IOS
        Social.localUser.Authenticate((isSucceed) =>
        {
            Debug.LogErrorFormat("[SHAppleAuth] Try Login is {0}", isSucceed);
            Debug.LogErrorFormat("[SHAppleAuth] Social.localUser.id {0}", Social.localUser.id);
            Debug.LogErrorFormat("[SHAppleAuth] Social.localUser.isFriend {0}", Social.localUser.isFriend);
            Debug.LogErrorFormat("[SHAppleAuth] Social.localUser.state {0}", Social.localUser.state);
            Debug.LogErrorFormat("[SHAppleAuth] Social.localUser.userName {0}", Social.localUser.userName);

            pCallback(isSucceed);
        }); 
#else
        pCallback(false);
#endif
    }

    public void Logout()
    {
        Debug.LogErrorFormat("[SHAppleAuth] Call is Logout");
        
#if UNITY_IOS
        
#endif
    }

    public bool IsLogin()
    {
        return Social.localUser.authenticated;
    }

    public string GetUserID()
    {
        return Social.localUser.id;
    }

    public string GetUserName()
    {
        return Social.localUser.userName;
    }

    public Texture2D GetProfileImage()
    {
        return Social.localUser.image;
    }
}