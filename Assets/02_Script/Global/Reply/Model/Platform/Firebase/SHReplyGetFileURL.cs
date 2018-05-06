using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace Firebase.Storage
{
    public class SHReplyGetFileURL : SHReply
    {
        public string m_strURL;

        public SHReplyGetFileURL(string strURL)
        {
            m_strURL = strURL;
        }
    }
}