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
    
    public void Login(Action<SHReply> pCallback)
    {
        Debug.LogErrorFormat("[SHAppleAuth] Call is Login");

        if (true == IsLogin())
        {
            pCallback(new SHReply(new SHError(eErrorCode.Apple_Login_Fail, "Already Logined")));
            return;
        }

#if UNITY_IOS
        Social.localUser.Authenticate((isSucceed, strErrorMessage) =>
        {
            if (isSucceed)
            {
                Debug.LogErrorFormat("[SHAppleAuth] Id {0}", GetUserID());
                Debug.LogErrorFormat("[SHAppleAuth] Name {0}", GetUserName());
                Debug.LogErrorFormat("[SHAppleAuth] State {0}", GetUserState());

                pCallback(new Apple.Auth.SHReplyLogin(
                    GetUserID(), GetUserName(), GetUserState()));
            }
            else
            {
                pCallback(new SHReply(new SHError(eErrorCode.Apple_Login_Fail, strErrorMessage)));
            }
        }); 
#else
        pCallback(new SHReply(new SHError(eErrorCode.Apple_Login_Fail, "Not Supported this platform")));
#endif
    }

    public void Logout(Action<SHReply> pCallback)
    {
        Debug.LogErrorFormat("[SHAppleAuth] Call is Logout");

#if UNITY_IOS
        
#endif
        pCallback(new Apple.Auth.SHReplyLogout());
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

    public UserState GetUserState()
    {
        return Social.localUser.state;
    }

    public Texture2D GetProfileImage()
    {
        return Social.localUser.image;
    }
}