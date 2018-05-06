using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Firebase.Auth;

namespace Firebase.Auth
{
    public class SHReplyGuestLogin : SHReply
    {
        public FirebaseUser m_pUser;

        public SHReplyGuestLogin(FirebaseUser pUser)
        {
            m_pUser = pUser;
        }
    }
}