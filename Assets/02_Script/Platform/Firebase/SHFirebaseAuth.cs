using UnityEngine;

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
        if (null != m_pAuth)
            return;

        m_pAuth = FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance);
        m_pAuth.StateChanged += OnEventByAuthStateChanged;
    }

    public void OnFinalize()
    {
        if (null == m_pAuth)
            return;

        m_pAuth.StateChanged -= OnEventByAuthStateChanged;
    }

    public void CreateAccount(string strUserEmail, string strUserPassword)
    {
        Debug.LogWarningFormat("CreateAccount!! (Email : {0}, Password : {1})", strUserEmail, strUserPassword);

        m_pAuth.CreateUserWithEmailAndPasswordAsync(strUserEmail, strUserPassword).ContinueWith((pTask) =>
        {
            if (pTask.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (pTask.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + pTask.Exception);
                return;
            }

            // Firebase user has been created.
            m_pUser = pTask.Result;

            Debug.LogWarningFormat("Firebase user created successfully: {0} ({1})",
                m_pUser.DisplayName, m_pUser.UserId);
        });
    }

    public void Login(string strUserEmail, string strUserPassword)
    {
        Debug.LogWarningFormat("Login!! (Email : {0}, Password : {1})", strUserEmail, strUserPassword);
        
        m_pAuth.SignInWithEmailAndPasswordAsync(strUserEmail, strUserPassword).ContinueWith((pTask) =>
        {
            if (pTask.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (pTask.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + pTask.Exception);
                return;
            }

            m_pUser = pTask.Result;

            Debug.LogWarningFormat("User signed in successfully: {0} ({1})",
                m_pUser.DisplayName, m_pUser.UserId);
        });
    }

    public void Logout()
    {
        Debug.LogWarningFormat("Logout!!");

        m_pAuth.SignOut();
    }

    void OnEventByAuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Debug.LogWarningFormat("OnEventByAuthStateChanged!!");

        if (m_pAuth.CurrentUser != m_pUser)
        {
            bool bIsSignedIn = (m_pUser != m_pAuth.CurrentUser) && (null != m_pAuth.CurrentUser);
            if ((false == bIsSignedIn) && (null != m_pUser))
            {
                Debug.LogWarningFormat("Signed out " + m_pUser.UserId);
            }

            m_pUser = m_pAuth.CurrentUser;
            if (true == bIsSignedIn)
            {
                Debug.LogWarningFormat("Signed in {0}, DisplayName({1}), Email({2}), PhotoURL({3})", 
                    m_pUser.UserId, m_pUser.DisplayName, m_pUser.Email, m_pUser.PhotoUrl);
            }
        }
    }
}