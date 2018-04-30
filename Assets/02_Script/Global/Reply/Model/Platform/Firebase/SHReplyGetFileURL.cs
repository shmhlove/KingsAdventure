using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace SH.Platform.Firebase
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
