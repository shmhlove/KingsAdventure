using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Firebase.Storage;

namespace Firebase.Storage
{
    public class SHReplyUpload : SHReply
    {
        public StorageMetadata m_pMeta;

        public SHReplyUpload(StorageMetadata pMeta)
        {
            m_pMeta = pMeta;
        }
    }
}