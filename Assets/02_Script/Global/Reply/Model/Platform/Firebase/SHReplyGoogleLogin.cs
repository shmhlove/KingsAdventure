using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Firebase.Auth;

namespace Firebase.Auth
{
    public class SHReplyGoogleLogin : SHReply
    {
        public FirebaseUser m_pUser;

        public SHReplyGoogleLogin(FirebaseUser pUser)
        {
            m_pUser = pUser;
        }
    }
}