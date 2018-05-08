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
    
    public void OnClickOfOpenConsole()
    {
        LunarConsolePlugin.LunarConsole.Show();
    }

    public void OnClickOfAddtiveIntro()
    {
        Single.UI.Show("Panel_Indicator");

        Single.Timer.StartDeltaTime("SceneLoadTime");
        Single.Scene.Addtive(eSceneType.Intro, false, (pReply) =>
        {
            Debug.LogErrorFormat("[LSH] Bundle Scene Load Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("SceneLoadTime"));
            Single.UI.Close("Panel_Indicator");
        });
    }

    public void OnClickOfRemoveIntro()
    {
        Single.Scene.Remove(eSceneType.Intro);
    }

    public void OnClickOfAddtivePatch()
    {
        Single.UI.Show("Panel_Indicator");

        Single.Timer.StartDeltaTime("SceneLoadTime");
        Single.Scene.Addtive(eSceneType.Patch, false, (pReply) =>
        {
            Debug.LogErrorFormat("[LSH] Bundle Scene Load Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("SceneLoadTime"));
            Single.UI.Close("Panel_Indicator");
        });
    }

    public void OnClickOfRemovePatch()
    {
        Single.Scene.Remove(eSceneType.Patch);
    }

    public void OnClickOfAddtiveLogin()
    {
        Single.UI.Show("Panel_Indicator");

        Single.Timer.StartDeltaTime("SceneLoadTime");
        Single.Scene.Addtive(eSceneType.Login, false, (pReply) => 
        {
            Debug.LogErrorFormat("[LSH] Bundle Scene Load Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("SceneLoadTime"));
            Single.UI.Close("Panel_Indicator");
        });
    }

    public void OnClickOfRemoveLogin()
    {
        Single.Scene.Remove(eSceneType.Login);
    }

    public void OnClickOfAddtiveLoading()
    {
        Single.UI.Show("Panel_Indicator");

        Single.Timer.StartDeltaTime("SceneLoadTime");
        Single.Scene.Addtive(eSceneType.Loading, false, (pReply) =>
        {
            Debug.LogErrorFormat("[LSH] Bundle Scene Load Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("SceneLoadTime"));
            Single.UI.Close("Panel_Indicator");
        });
    }

    public void OnClickOfRemoveLoading()
    {
        Single.Scene.Remove(eSceneType.Loading);
    }
    
    public void OnClickOfAddtiveInGame()
    {
        Single.UI.Show("Panel_Indicator");

        Single.Timer.StartDeltaTime("SceneLoadTime");
        Single.Scene.Addtive(eSceneType.InGame, false, (pReply) =>
        {
            Debug.LogErrorFormat("[LSH] Bundle Scene Load Time is {0}sec", Single.Timer.GetDeltaTimeToSecond("SceneLoadTime"));
            Single.UI.Close("Panel_Indicator");
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
        Single.UI.Show("Panel_Indicator");

        Single.Firebase.Auth.CreateAccount(m_strUserEmail, m_strUserPassword, (pReply) => 
        {
            if (pReply.IsSucceed)
            {
                var pAsReply = pReply.GetAs<Firebase.Auth.SHReplyCreateAccount>();

                Debug.LogErrorFormat("[LSH] Firebase user created successfully: {0} ({1})",
                    pAsReply.m_pUser.DisplayName, pAsReply.m_pUser.UserId);
            }
            else
            {
                Debug.LogErrorFormat("[LSH] Firebase user created Failed: {0}",
                    pReply.Error.ToString());
            }

            Single.UI.Close("Panel_Indicator");
        });
    }

    public void OnClickOfFBAuth_Login()
    {
        Single.UI.Show("Panel_Indicator");

        Single.Firebase.Auth.Login(m_strUserEmail, m_strUserPassword, (pReply) =>
        {
            if (pReply.IsSucceed)
            {
                var pAsReply = pReply.GetAs<Firebase.Auth.SHReplyLogin>();

                Debug.LogErrorFormat("[LSH] Firebase user login successfully: {0} ({1})",
                    pAsReply.m_pUser.DisplayName, pAsReply.m_pUser.UserId);
            }
            else
            {
                Debug.LogErrorFormat("[LSH] Firebase user login Failed: {0}", pReply.Error.ToString());
            }

            Single.UI.Close("Panel_Indicator");
        });
    }

    public void OnClickOfFBAuth_GuestLogin()
    {
        Single.UI.Show("Panel_Indicator");

        Single.Firebase.Auth.GuestLogin((pReply) =>
        {
            if (pReply.IsSucceed)
            {
                var pAsReply = pReply.GetAs<Firebase.Auth.SHReplyGuestLogin>();

                Debug.LogErrorFormat("[LSH] Firebase user guest login successfully: {0} ({1})",
                    pAsReply.m_pUser.DisplayName, pAsReply.m_pUser.UserId);
            }
            else
            {
                Debug.LogErrorFormat("[LSH] Firebase user guest login Failed: {0}", pReply.Error.ToString());
            }

            Single.UI.Close("Panel_Indicator");
        });
    }

    public void OnClickOfFBAuth_GoogleSignIn()
    {
        Single.UI.Show("Panel_Indicator");

        Single.Google.Auth.Login((pGoogleReply) =>
        {
            if (pGoogleReply.IsSucceed)
            {
                var pAsGoogleReply = pGoogleReply.GetAs<Google.Auth.SHReplyLogin>();

                Single.Firebase.Auth.GoogleSignIn(pAsGoogleReply.m_strIdToken, (pReply) =>
                {
                    if (pReply.IsSucceed)
                    {
                        var pAsReply = pReply.GetAs<Firebase.Auth.SHReplyGoogleLogin>();

                        Debug.LogErrorFormat("[LSH] Firebase user google login successfully: {0} ({1})",
                            pAsReply.m_pUser.DisplayName, pAsReply.m_pUser.UserId);
                    }
                    else
                    {
                        Debug.LogErrorFormat("[LSH] Firebase user guest login Failed: {0}", pReply.Error.ToString());
                    }

                    Single.UI.Close("Panel_Indicator");
                });
            }
            else
            {
                Single.UI.Close("Panel_Indicator");
            }
        });
    }

    public void OnClickOfGoogle_Login()
    {
        Single.UI.Show("Panel_Indicator");

        Single.Google.Auth.Login((pReply) =>
        {
            if (pReply.IsSucceed)
            {
                var pAsReply = pReply.GetAs<Google.Auth.SHReplyLogin>();

                Debug.LogErrorFormat("[LSH] Google login successfully: {0}",
                    pAsReply.ToString());
            }
            else
            {
                Debug.LogErrorFormat("[LSH] Google login Failed: {0}", pReply.Error.ToString());
            }

            Single.UI.Close("Panel_Indicator");
        });
    }

    public void OnclickOfApple_Login()
    {
        Single.UI.Show("Panel_Indicator");

        Single.Apple.Auth.Login((pReply) =>
        {
            if (pReply.IsSucceed)
            {
                var pAsReply = pReply.GetAs<Apple.Auth.SHReplyLogin>();

                Debug.LogErrorFormat("[LSH] Apple login successfully: {0}",
                    pAsReply.ToString());
            }
            else
            {
                Debug.LogErrorFormat("[LSH] Apple login Failed: {0}", pReply.Error.ToString());
            }

            Single.UI.Close("Panel_Indicator");
        });
    }

    public void OnClickOfProvider_LogoutAll()
    {
        Single.Firebase.Auth.Logout((pReply) => { });
        Single.Google.Auth.Logout((pReply) => { });
        Single.Apple.Auth.Logout((pReply) => { });
    }
}