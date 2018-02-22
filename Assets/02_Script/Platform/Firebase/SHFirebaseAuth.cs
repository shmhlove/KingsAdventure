using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase.Auth;

public class SHFirebaseAuth
{
    public void CreateAccount(string strUserEmail, string strUserPassword)
    {
        Debug.LogErrorFormat("Email : {0}, Password : {1}", strUserEmail, strUserPassword);
        //FirebaseAuth pAuth = FirebaseAuth
        //auth.CreateUserWithEmailAndPasswordAsync(strUserEmail, strUserPassword).ContinueWith(task =>
        //{
        //    if (task.IsCanceled)
        //    {
        //        Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
        //        return;
        //    }
        //    if (task.IsFaulted)
        //    {
        //        Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
        //        return;
        //    }
        //
        //    // Firebase user has been created.
        //    Firebase.Auth.FirebaseUser newUser = task.Result;
        //    Debug.LogFormat("Firebase user created successfully: {0} ({1})",
        //        newUser.DisplayName, newUser.UserId);
        //});
    }
}
