using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Firebase.Auth;

namespace Firebase.Auth
{
    public class SHReplyCreateAccount : SHReply
    {
        public FirebaseUser m_pUser;

        public SHReplyCreateAccount(FirebaseUser pUser)
        {
            m_pUser = pUser;
        }
    }
}