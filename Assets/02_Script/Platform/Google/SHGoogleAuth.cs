using UnityEngine;
using UnityEngine.SocialPlatforms;

using System;
using System.Collections;
using System.Collections.Generic;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class SHGoogleAuth
{
    public void OnInitialize()
    {
        Debug.LogErrorFormat("[SHGoogleAuth] Call is OnInitialize");
        
#if UNITY_ANDROID
        var pGPGConfig = new PlayGamesClientConfiguration.Builder()
            .RequestIdToken()
            .RequestEmail()
            .EnableSavedGames()
            .Build();
        PlayGamesPlatform.InitializeInstance(pGPGConfig);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
#endif
    }

    public void OnFinalize()
    {
        Debug.LogErrorFormat("[SHGoogleAuth] Call is OnFinalize");
    }
    
    public void Login(Action<bool> pCallback)
    {
        Debug.LogErrorFormat("[SHGoogleAuth] Call is Login");

        if (true == IsLogin())
        {
            Debug.LogErrorFormat("[SHGoogleAuth] Already Login!!({0})", GetUserEmail());
            pCallback(true);
            return;
        }

#if UNITY_ANDROID
        PlayGamesPlatform.Instance.Authenticate((isSucceed) =>
        {
            Debug.LogErrorFormat("[SHGoogleAuth] GoogleLogin is {0}", isSucceed);
            Debug.LogErrorFormat("[SHGoogleAuth] Id {0}", GetUserID());
            Debug.LogErrorFormat("[SHGoogleAuth] Name {0}", GetUserName());
            Debug.LogErrorFormat("[SHGoogleAuth] State {0}", GetUserState());
            Debug.LogErrorFormat("[SHGoogleAuth] Email {0}", GetUserEmail());
            Debug.LogErrorFormat("[SHGoogleAuth] IdToken {0}", GetIdToken());
            
            pCallback(isSucceed);
        });
#else
        pCallback(false);
#endif
    }

    public void Logout()
    {
        Debug.LogErrorFormat("[SHGoogleAuth] Call is Logout");

#if UNITY_ANDROID
        ((GooglePlayGames.PlayGamesPlatform)Social.Active).SignOut();
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

    public string GetUserEmail()
    {
#if UNITY_ANDROID
        return PlayGamesPlatform.Instance.GetUserEmail();
#else
        return string.Empty;
#endif
    }

    public UserState GetUserState()
    {
        return Social.localUser.state;
    }

    public Texture2D GetProfileImage()
    {
        return Social.localUser.image;
    }

    public string GetIdToken()
    {
#if UNITY_ANDROID
        return PlayGamesPlatform.Instance.GetIdToken();
#else
        return string.Empty;
#endif
    }
}