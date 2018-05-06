using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Firebase.Auth;

namespace Firebase.Auth
{
    public class SHReplyLogin : SHReply
    {
        public FirebaseUser m_pUser;

        public SHReplyLogin(FirebaseUser pUser)
        {
            m_pUser = pUser;
        }
    }
}