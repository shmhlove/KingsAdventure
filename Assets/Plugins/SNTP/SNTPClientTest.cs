using UnityEngine;
using System.Collections;

using System;
using InternetTime;

public class SNTPClientTest : MonoBehaviour
{
    private SNTPClient m_cSNTPClient = null;

	void Start()
	{
        try
        {
            m_cSNTPClient = new SNTPClient("time.nuri.net");   // 참조할 NTP 서버 주소.
            m_cSNTPClient.Connect(false);
            Debug.Log(m_cSNTPClient.ToString());
        }
        catch (Exception e)
        {
            Debug.LogError("ERROR: " + e.Message);
        }
	}
}
