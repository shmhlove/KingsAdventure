using UnityEngine;

using System.Collections;
using System.Collections.Generic;


public enum eErrorCode
{
    // 공통
    Succeed,
    Failed,

    // 씬 관련
    Scene_Load_Fail,

    // 리소스 관련
    Resource_Load_Fail,
    Resource_Not_ExsitTable,

    // 테이블 관련
    Table_Load_Fail,
    Table_Not_AddClass,
    Table_Not_Override,
    Table_Not_ExsitFile,
    Table_Error_Grammar,

    // 업데이트 관련
    Patch_Table,
    Patch_Bundle_Download_Fail,
    Patch_Bundle_Upload_Fail,

    // Firebase
    FB_CreateAccount_Fail,
    FB_Login_Fail,
    FB_Guest_Login_Fail,
    FB_Google_Login_Fail,
    FB_Logout_Fail,

    // Google
    Google_Login_Fail,
    Google_Logout_Fail,

    // Apple
    Apple_Login_Fail,
    Apple_Logout_Fail,
}

public class SHError
{
    public eErrorCode   m_eCode;
    public string       m_strMessage;

    public SHError(eErrorCode eCode, string strMessage)
    {
        m_eCode      = eCode;
        m_strMessage = strMessage;
    }

    public override string ToString()
    {
        return string.Format("ErrorCode: {0}, Message: {1}", m_eCode, m_strMessage);
    }
}
