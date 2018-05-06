using UnityEngine;
using UnityEngine.SocialPlatforms;

using System.Collections;
using System.Collections.Generic;

namespace Apple.Auth
{
    public class SHReplyLogin : SHReply
    {
        public string m_strUserID;
        public string m_strUserName;
        public UserState m_eUserState;

        public SHReplyLogin(
            string strUserID, 
            string strUserName,
            UserState eUserState)
        {
            m_strUserID    = strUserID;
            m_strUserName  = strUserName;
            m_eUserState   = eUserState;
        }

        public override string ToString()
        {
            return string.Format("userID: {0}\nuserName: {1}\nuserState: {2}",
                m_strUserID, m_strUserName, m_eUserState);
        }
    }
}