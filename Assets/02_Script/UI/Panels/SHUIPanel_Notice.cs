using UnityEngine;
using System;
using System.Collections;

public enum eNoticeButton
{
    None,
    One,
    Two,
    Three,
}

public enum eNoticeIcon
{
    None,
    Error,
    Warning,
    Information,
}

public class NoticeUI_Param
{
    public eNoticeButton m_eButtonType    = eNoticeButton.None;
    public eNoticeIcon   m_eIconType      = eNoticeIcon.None;
    public string        m_strTitle       = string.Empty;
    public string        m_strMessage     = string.Empty;

    public Action        m_pEventToOK     = null;
    public Action        m_pEventToCancel = null;
    public Action        m_pEventToRetry  = null;
    public Action        m_pEventToClose  = null;
}

public class SHUIPanel_Notice : SHUIBasePanel
{
    #region Members : Inspector
    [Header("Notice Info")]
    [SerializeField] private UILabel       m_pTitle     = null;
    [SerializeField] private UILabel       m_pMessage   = null;
    [Header("Button Info")]
    [SerializeField] private GameObject    m_pOneButton = null;
    [SerializeField] private GameObject    m_pTwoButton = null;
    [SerializeField] private GameObject    m_pThrButton = null;
    [Header("Icon Info")]
    [SerializeField] private GameObject    m_pErrorIcon = null;
    [SerializeField] private GameObject    m_pWarningIcon = null;
    [SerializeField] private GameObject    m_pInfoIcon  = null;
    #endregion


    #region Members : Info
    private NoticeUI_Param m_pInfo = null;
    #endregion


    #region System Functions
    #endregion


    #region Virtual Functions
    public override void OnBeforeShow(params object[] pArgs)
    {
        if ((null == pArgs) || (1 > pArgs.Length))
            return;

        if (false == (pArgs[0] is NoticeUI_Param))
            return;

        var pParam = (NoticeUI_Param)pArgs[0];
        if (null == pParam)
            return;

        m_pInfo = pParam;

        SetButton(pParam.m_eButtonType);
        SetIcon(pParam.m_eIconType);
        SetTitle(pParam.m_strTitle);
        SetMessage(pParam.m_strMessage);
    }
    #endregion


    #region Interface Functions
    #endregion


    #region Utility Functions
    void SetButton(eNoticeButton eType)
    {
        m_pOneButton.SetActive(eNoticeButton.One   == eType);
        m_pTwoButton.SetActive(eNoticeButton.Two   == eType);
        m_pThrButton.SetActive(eNoticeButton.Three == eType);
    }
    void SetIcon(eNoticeIcon eType)
    {
        m_pErrorIcon.SetActive(eNoticeIcon.Error        == eType);
        m_pWarningIcon.SetActive(eNoticeIcon.Warning    == eType);
        m_pInfoIcon.SetActive(eNoticeIcon.Information   == eType);
    }
    void SetTitle(string strTitle)
    {
        m_pTitle.text = strTitle;
    }
    void SetMessage(string strMessage)
    {
        m_pMessage.text = strMessage;
    }
    #endregion


    #region Event Handler
    public override void OnClickToClose()
    {
        if (null != m_pInfo.m_pEventToClose)
            m_pInfo.m_pEventToClose();

        base.OnClickToClose();
    }
    public void OnClickToOK()
    {
        if (null != m_pInfo.m_pEventToOK)
            m_pInfo.m_pEventToOK();

        Close();
    }
    public void OnClickToCancel()
    {
        if (null != m_pInfo.m_pEventToCancel)
            m_pInfo.m_pEventToCancel();

        Close();
    }
    public void OnClickToRetry()
    {
        if (null != m_pInfo.m_pEventToRetry)
            m_pInfo.m_pEventToRetry();

        Close();
    }
    #endregion
}
