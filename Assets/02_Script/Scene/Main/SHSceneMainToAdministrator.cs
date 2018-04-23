using UnityEngine;
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
        Single.Scene.Addtive(eSceneType.Intro, false, (strError) =>
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
        Single.Scene.Addtive(eSceneType.Patch, false, (strError) =>
        {
            Debug.LogErrorFormat("undle Scene Load Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("SceneLoadTime"));
        });
    }

    public void OnClickOfRemovePatch()
    {
        Single.Scene.Remove(eSceneType.Patch);
    }

    public void OnClickOfAddtiveLogin()
    {
        Single.Timer.StartDeltaTime("SceneLoadTime");
        Single.Scene.Addtive(eSceneType.Login, false, (strError) => 
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
        Single.Scene.Addtive(eSceneType.Loading, false, (strError) =>
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
        Single.Scene.Addtive(eSceneType.InGame, false, (strError) =>
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
        if (string.IsNullOrEmpty(m_strUserEmail) || string.IsNullOrEmpty(m_strUserPassword))
        {
            Debug.LogError("Need E-mail and Password!!!");
            return;
        }

        Single.Firebase.Auth.CreateAccount(m_strUserEmail, m_strUserPassword);
    }

    public void OnClickOfFBAuth_Login()
    {
        if (string.IsNullOrEmpty(m_strUserEmail) || string.IsNullOrEmpty(m_strUserPassword))
        {
            Debug.LogError("Need E-mail and Password!!!");
            return;
        }

        Single.Firebase.Auth.Login(m_strUserEmail, m_strUserPassword);
    }

    public void OnClickOfFBAuth_GuestLogin()
    {
        Single.Firebase.Auth.GuestLogin();
    }

    public void OnClickOfFBAuth_GoogleSignIn()
    {
        Single.Firebase.Auth.GoogleSignIn(Single.Google.Auth.GetIdToken(), (isSucceed) =>
        {
        
        });
    }

    public void OnClickOfGoogle_Login()
    {
        Single.Google.Auth.Login((isSucceed) =>
        {
            
        });
    }

    public void OnclickOfApple_Login()
    {
        Single.Apple.Auth.Login((isSucceed) =>
        {

        });
    }

    public void OnClickOfProvider_LogoutAll()
    {
        Single.Firebase.Auth.Signout();
        Single.Google.Auth.Logout();
        Single.Apple.Auth.Logout();
    }
}