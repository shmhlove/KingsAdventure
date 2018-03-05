using UnityEngine;
using UnityEngine.SocialPlatforms;

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Firebase;
using Firebase.Auth;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
#endif

/*
Google PlayGame Service OAuth Infomation

keytool -exportcert -list -v -alias kingsadventure -keystore /Users/HoYaMacBook/Documents/GitHub/KingsAdventure/GoogleKeyStore/user.keystore
MD5: B9:F4:48:12:3B:48:81:54:D0:2C:CD:0B:E2:09:D8:D6
SHA1: 34:10:51:81:3D:B6:8C:DC:12:9B:5B:93:04:A4:75:C3:6D:D8:2B:94
SHA256: F2:64:7B:78:1A:E4:AC:89:A3:7A:51:9D:7F:02:61:65:2F:0E:E7:E2:84:8E:48:B5:EC:98:E8:BD:D8:17:8D:20

iOS Google Key
590366760406-l923de3j119tjefi6214dkeoqlt7nl2f.apps.googleusercontent.com
*/

public class SHFirebaseAuth
{
    private FirebaseAuth m_pAuth;
    private FirebaseUser m_pUser;

#if UNITY_ANDROID
    private PlayGamesClientConfiguration m_pGPGConfig;
#endif

    public void OnInitialize()
    {
        Debug.LogWarningFormat("[SHFirebaseAuth] Call is OnInitialize");

        if (null != m_pAuth)
            return;

        m_pAuth = FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance);
        m_pAuth.StateChanged += OnEventByAuthStateChanged;

#if UNITY_ANDROID
        m_pGPGConfig = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(m_pGPGConfig);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
#endif
    }

    public void OnFinalize()
    {
        Debug.LogWarningFormat("[SHFirebaseAuth] Call is OnFinalize");

        if (null == m_pAuth)
            return;

        m_pAuth.StateChanged -= OnEventByAuthStateChanged;
    }
    
    public void CreateAccount(string strUserEmail, string strUserPassword)
    {
        Debug.LogWarningFormat("[SHFirebaseAuth] Call is CreateAccount (Email : {0}, Password : {1})", strUserEmail, strUserPassword);

        m_pAuth.CreateUserWithEmailAndPasswordAsync(strUserEmail, strUserPassword).ContinueWith((pTask) =>
        {
            if (pTask.IsCanceled)
            {
                Debug.LogError("[SHFirebaseAuth] CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (pTask.IsFaulted)
            {
                Debug.LogError("[SHFirebaseAuth] CreateUserWithEmailAndPasswordAsync encountered an error: " + pTask.Exception);
                return;
            }

            // Firebase user has been created.
            m_pUser = pTask.Result;

            Debug.LogWarningFormat("[SHFirebaseAuth] Firebase user created successfully: {0} ({1})",
                m_pUser.DisplayName, m_pUser.UserId);
        });
    }

    public void Login(string strUserEmail, string strUserPassword)
    {
        Debug.LogWarningFormat("[SHFirebaseAuth] Call is Login (Email : {0}, Password : {1})", strUserEmail, strUserPassword);
        
        m_pAuth.SignInWithEmailAndPasswordAsync(strUserEmail, strUserPassword).ContinueWith((pTask) =>
        {
            if (pTask.IsCanceled)
            {
                Debug.LogError("[SHFirebaseAuth] SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (pTask.IsFaulted)
            {
                Debug.LogError("[SHFirebaseAuth] SignInWithEmailAndPasswordAsync encountered an error: " + pTask.Exception);
                return;
            }

            m_pUser = pTask.Result;

            Debug.LogWarningFormat("[SHFirebaseAuth] User signed in successfully: {0} ({1})",
                m_pUser.DisplayName, m_pUser.UserId);
        });
    }

    public void GuestLogin()
    {
        Debug.LogWarningFormat("[SHFirebaseAuth] Call is GuestLogin");
        
        // 계전전환
        // 1. 사용자가 가입하면 해당 사용자가 선택한 인증 제공업체의 로그인 흐름을 진행하되 메소드 호출 전까지만 진행합니다. 
        //    예를 들어 사용자의 Google ID 토큰, Facebook 액세스 토큰 또는 이메일 주소와 비밀번호를 가져옵니다.
        // 2. 새로운 인증 제공업체의 을 가져옵니다.
        // 3. 개체를 로그인 사용자의 메소드에 전달합니다.
        // * 이 방법은 임의의 계정 2개를 연결할 때도 사용할 수 있습니다.

        m_pAuth.SignInAnonymouslyAsync().ContinueWith((pTask) =>
        {
            if (pTask.IsCanceled)
            {
                Debug.LogError("[SHFirebaseAuth] SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (pTask.IsFaulted)
            {
                Debug.LogError("[SHFirebaseAuth] SignInAnonymouslyAsync encountered an error: " + pTask.Exception);
                return;
            }

            m_pUser = pTask.Result;
            Debug.LogWarningFormat("[SHFirebaseAuth] User signed in successfully: {0} ({1})",
                m_pUser.DisplayName, m_pUser.UserId);
        });
    }

    public void GoogleLogin()
    {
        Debug.LogWarningFormat("[SHFirebaseAuth] Call is GoogleLogin");
        
        Social.localUser.Authenticate(isSucceed => 
        {
            Debug.LogWarningFormat("[SHFirebaseAuth] GoogleLogin is {0}", isSucceed);
            if (false == isSucceed)
                return;

            string strIdToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();

            Credential credential = GoogleAuthProvider.GetCredential(strIdToken, null);
            m_pAuth.SignInWithCredentialAsync(credential).ContinueWith(pTask => 
            {
                if (pTask.IsCanceled)
                {
                    Debug.LogError("[SHFirebaseAuth] SignInWithCredentialAsync was canceled.");
                    return;
                }
                if (pTask.IsFaulted)
                {
                    Debug.LogError("[SHFirebaseAuth] SignInWithCredentialAsync encountered an error: " + pTask.Exception);
                    return;
                }

                m_pUser = pTask.Result;
                Debug.LogWarningFormat("[SHFirebaseAuth] User signed in successfully: {0} ({1})",
                    m_pUser.DisplayName, m_pUser.UserId);
            });
        });
    }

    public void Logout()
    {
        Debug.LogWarningFormat("[SHFirebaseAuth] Call is Logout");

        m_pAuth.SignOut();
    }

    public bool IsLogin()
    {
        return Social.localUser.authenticated;
    }

    public string GetUserID()
    {
        if (false == IsLogin())
            return string.Empty;

        return Social.localUser.id;
    }

    public string GetUserName()
    {
        if (false == IsLogin())
            return string.Empty;

        return Social.localUser.userName;
    }

    public Texture2D GetProfileImage()
    {
        if (false == IsLogin())
            return null;

        return Social.localUser.image;
    }

    void OnEventByAuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Debug.LogWarningFormat("[SHFirebaseAuth] Call is OnEventByAuthStateChanged");

        if (m_pAuth.CurrentUser != m_pUser)
        {
            bool bIsSignedIn = (m_pUser != m_pAuth.CurrentUser) && (null != m_pAuth.CurrentUser);
            if ((false == bIsSignedIn) && (null != m_pUser))
            {
                Debug.LogWarningFormat("[SHFirebaseAuth] Signed out " + m_pUser.UserId);
            }

            m_pUser = m_pAuth.CurrentUser;
            if (true == bIsSignedIn)
            {
                Debug.LogWarningFormat("[SHFirebaseAuth] Signed in {0}, DisplayName({1}), Email({2}), PhotoURL({3})", 
                    m_pUser.UserId, m_pUser.DisplayName, m_pUser.Email, m_pUser.PhotoUrl);
            }
        }
    }
}