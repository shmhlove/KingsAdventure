using UnityEngine;
using UnityEngine.SocialPlatforms;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Firebase;
using Firebase.Auth;

public class SHFirebaseAuth
{
    private FirebaseAuth m_pAuth;
    private FirebaseUser m_pUser;
    
    public void OnInitialize()
    {
        Debug.LogErrorFormat("[SHFirebaseAuth] Call is OnInitialize");

        if (null != m_pAuth)
            return;

        m_pAuth = FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance);
        m_pAuth.StateChanged += OnEventByAuthStateChanged;

    }

    public void OnFinalize()
    {
        Debug.LogErrorFormat("[SHFirebaseAuth] Call is OnFinalize");

        if (null == m_pAuth)
            return;

        m_pAuth.StateChanged -= OnEventByAuthStateChanged;
    }

    public void CreateAccount(string strUserEmail, string strUserPassword)
    {
        Debug.LogErrorFormat("[SHFirebaseAuth] Call is CreateAccount (Email : {0}, Password : {1})", strUserEmail, strUserPassword);

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

            m_pUser = pTask.Result;

            Debug.LogErrorFormat("[SHFirebaseAuth] Firebase user created successfully: {0} ({1})",
                m_pUser.DisplayName, m_pUser.UserId);
        });
    }

    public void Login(string strUserEmail, string strUserPassword)
    {
        Debug.LogErrorFormat("[SHFirebaseAuth] Call is Login (Email : {0}, Password : {1})", strUserEmail, strUserPassword);

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

            Debug.LogErrorFormat("[SHFirebaseAuth] User signed in successfully: {0} ({1})",
                m_pUser.DisplayName, m_pUser.UserId);
        });
    }

    public void GuestLogin()
    {
        Debug.LogErrorFormat("[SHFirebaseAuth] Call is GuestLogin");

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

    public void GoogleSignIn(string strGoogleIdToken, Action<bool> pCallback)
    {
        Debug.LogErrorFormat("[SHFirebaseAuth] Call is GoogleLogin");

        if (string.IsNullOrEmpty(strGoogleIdToken))
        {
            Debug.LogErrorFormat("[SHFirebaseAuth] Google IdToken is Empty!!");
            pCallback(false);
            return;
        }

#if UNITY_ANDROID
        m_pAuth.SignInWithCredentialAsync(
                GoogleAuthProvider.GetCredential(strGoogleIdToken, null)).ContinueWith(pTask =>
            {
                if (pTask.IsCanceled)
                {
                    Debug.LogError("[SHFirebaseAuth] SignInWithCredentialAsync was canceled.");
                    pCallback(false);
                    return;
                }
                if (pTask.IsFaulted)
                {
                    Debug.LogError("[SHFirebaseAuth] SignInWithCredentialAsync encountered an error: " + pTask.Exception);
                    pCallback(false);
                    return;
                }

                Debug.LogErrorFormat("[SHFirebaseAuth] User signed in successfully: {0} ({1})", m_pUser.DisplayName, m_pUser.UserId);
                
                m_pUser = pTask.Result;
                pCallback(false);
            });
        });
#else
        Debug.LogErrorFormat("[SHFirebaseAuth] Not Supported this platform");
        pCallback(false);
#endif
    }

    public void Signout()
    {
        Debug.LogWarningFormat("[SHFirebaseAuth] Call is Logout");
        
        m_pAuth.SignOut();
    }
    
    void OnEventByAuthStateChanged(object sender, EventArgs eventArgs)
    {
        Debug.LogErrorFormat("[SHFirebaseAuth] Call is OnEventByAuthStateChanged");

        if (m_pAuth.CurrentUser != m_pUser)
        {
            bool bIsSignedIn = (m_pUser != m_pAuth.CurrentUser) && (null != m_pAuth.CurrentUser);
            if ((false == bIsSignedIn) && (null != m_pUser))
            {
                Debug.LogErrorFormat("[SHFirebaseAuth] Signed out " + m_pUser.UserId);
            }

            m_pUser = m_pAuth.CurrentUser;
            if (true == bIsSignedIn)
            {
                Debug.LogErrorFormat("[SHFirebaseAuth] Signed in {0}, DisplayName({1}), Email({2}), PhotoURL({3})",
                    m_pUser.UserId, m_pUser.DisplayName, m_pUser.Email, m_pUser.PhotoUrl);
            }
        }
    }
}