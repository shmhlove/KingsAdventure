using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace Firebase.Storage
{
    public class SHReplyDownloadForBundle : SHReply
    {
        public WWW m_pWWW;

        public SHReplyDownloadForBundle(WWW pWWW)
        {
            m_pWWW = pWWW;
        }
    }
}