using UnityEngine;
using UnityEngine.SocialPlatforms;

using System.Collections;
using System.Collections.Generic;

namespace Google.Auth
{
    public class SHReplyLogin : SHReply
    {
        public string m_strUserID;
        public string m_strUserName;
        public UserState m_eUserState;
        public string m_strUserEmail;
        public string m_strIdToken;

        public SHReplyLogin(
            string strUserID, 
            string strUserName,
            UserState eUserState,
            string strUserEmail,
            string strIdToken)
        {
            m_strUserID    = strUserID;
            m_strUserName  = strUserName;
            m_eUserState   = eUserState;
            m_strUserEmail = strUserEmail;
            m_strIdToken   = strIdToken;
        }

        public override string ToString()
        {
            return string.Format("userID: {0}\nuserName: {1}\nuserState: {2}\nuserEmail: {3}\nidToken: {4}",
                m_strUserID, m_strUserName, m_eUserState, m_strUserEmail, m_strIdToken);
        }
    }
}