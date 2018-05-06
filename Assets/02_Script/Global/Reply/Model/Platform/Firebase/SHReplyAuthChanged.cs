using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Firebase.Auth;

namespace Firebase.Auth
{
    public class SHReplyAuthChanged : SHReply
    {
        public FirebaseUser m_pUser;

        public SHReplyAuthChanged(FirebaseUser pUser)
        {
            m_pUser = pUser;
        }
    }
}