﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Text;
using System.Collections;

public class SHSceneMainToAdministrator : SHMonoWrapper
{
    public override void Start() 
    {
        base.Start();
        Single.AppInfo.CreateSingleton();
	}
    
    public void OnClickOfAddtiveIntro()
    {
        Single.Timer.StartDeltaTime("SceneLoadTime");
        Single.Scene.Addtive(eSceneType.Intro, false, (pReply) =>
        {
            Debug.LogErrorFormat("Bundle Scene Load Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("SceneLoadTime"));
        });
    }

    public void OnClickOfRemoveIntro()
    {
        Single.Scene.Remove(eSceneType.Intro);
    }

    public void OnClickOfAddtivePatch()
    {
        Single.Timer.StartDeltaTime("SceneLoadTime");
        Single.Scene.Addtive(eSceneType.Patch, false, (pReply) =>
        {
            Debug.LogErrorFormat("Bundle Scene Load Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("SceneLoadTime"));
        });
    }

    public void OnClickOfRemovePatch()
    {
        Single.Scene.Remove(eSceneType.Patch);
    }

    public void OnClickOfAddtiveLogin()
    {
        Single.Timer.StartDeltaTime("SceneLoadTime");
        Single.Scene.Addtive(eSceneType.Login, false, (pReply) => 
        {
            Debug.LogErrorFormat("Bundle Scene Load Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("SceneLoadTime"));
        });
    }

    public void OnClickOfRemoveLogin()
    {
        Single.Scene.Remove(eSceneType.Login);
    }

    public void OnClickOfAddtiveLoading()
    {
        Single.Timer.StartDeltaTime("SceneLoadTime");
        Single.Scene.Addtive(eSceneType.Loading, false, (pReply) =>
        {
            Debug.LogErrorFormat("Bundle Scene Load Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("SceneLoadTime"));
        });
    }

    public void OnClickOfRemoveLoading()
    {
        Single.Scene.Remove(eSceneType.Loading);
    }
    
    public void OnClickOfAddtiveInGame()
    {
        Single.Timer.StartDeltaTime("SceneLoadTime");
        Single.Scene.Addtive(eSceneType.InGame, false, (pReply) =>
        {
            Debug.LogErrorFormat("Bundle Scene Load Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("SceneLoadTime"));
        });
    }

    public void OnClickOfRemoveInGame()
    {
        Single.Scene.Remove(eSceneType.InGame);
    }
    
    string m_strUserEmail;
    public void OnSubmitOfFBAuth_Email(string strEmail)
    {
        m_strUserEmail = strEmail;
    }

    string m_strUserPassword;
    public void OnSubmitOfFBAuth_Password(string strPassword)
    {
        m_strUserPassword = strPassword;
    }

    public void OnClickOfFBAuth_CreateAccount()
    {
        Single.Firebase.Auth.CreateAccount(m_strUserEmail, m_strUserPassword, (pReply) => 
        {
            if (pReply.IsSucceed)
            {
                var pAsReply = pReply.GetAs<Firebase.Auth.SHReplyCreateAccount>();

                Debug.LogErrorFormat("Firebase user created successfully: {0} ({1})",
                    pAsReply.m_pUser.DisplayName, pAsReply.m_pUser.UserId);
            }
            else
            {
                Debug.LogErrorFormat("Firebase user created Failed: {0}",
                    pReply.Error.ToString());
            }
        });
    }

    public void OnClickOfFBAuth_Login()
    {
        Single.Firebase.Auth.Login(m_strUserEmail, m_strUserPassword, (pReply) =>
        {
            if (pReply.IsSucceed)
            {
                var pAsReply = pReply.GetAs<Firebase.Auth.SHReplyLogin>();

                Debug.LogErrorFormat("Firebase user login successfully: {0} ({1})",
                    pAsReply.m_pUser.DisplayName, pAsReply.m_pUser.UserId);
            }
            else
            {
                Debug.LogErrorFormat("Firebase user login Failed: {0}", pReply.Error.ToString());
            }
        });
    }

    public void OnClickOfFBAuth_GuestLogin()
    {
        Single.Firebase.Auth.GuestLogin((pReply) =>
        {
            if (pReply.IsSucceed)
            {
                var pAsReply = pReply.GetAs<Firebase.Auth.SHReplyGuestLogin>();

                Debug.LogErrorFormat("Firebase user guest login successfully: {0} ({1})",
                    pAsReply.m_pUser.DisplayName, pAsReply.m_pUser.UserId);
            }
            else
            {
                Debug.LogErrorFormat("Firebase user guest login Failed: {0}", pReply.Error.ToString());
            }
        });
    }

    public void OnClickOfFBAuth_GoogleSignIn()
    {
        Single.Firebase.Auth.GoogleSignIn(Single.Google.Auth.GetIdToken(), (pReply) =>
        {
            if (pReply.IsSucceed)
            {
                var pAsReply = pReply.GetAs<Firebase.Auth.SHReplyGoogleLogin>();

                Debug.LogErrorFormat("Firebase user google login successfully: {0} ({1})",
                    pAsReply.m_pUser.DisplayName, pAsReply.m_pUser.UserId);
            }
            else
            {
                Debug.LogErrorFormat("Firebase user guest login Failed: {0}", pReply.Error.ToString());
            }
        });
    }

    public void OnClickOfGoogle_Login()
    {
        Single.Google.Auth.Login((pReply) =>
        {
            
        });
    }

    public void OnclickOfApple_Login()
    {
        Single.Apple.Auth.Login((pReply) =>
        {

        });
    }

    public void OnClickOfProvider_LogoutAll()
    {
        Single.Firebase.Auth.Logout((pReply)=> { });
        Single.Google.Auth.Logout();
        Single.Apple.Auth.Logout();
    }
}