/// <summary>
///
/// - 개요
///     BBCode를 이용한 하이퍼링크 컴포넌트 입니다.
///     UILabel 게임 오브젝트에 해당 스크립트를 컴포넌트로 추가하면 
///     Text 터치시 설정된 URL로 연결되거나 이벤트를 콜 받을 수 있습니다.
///
/// - 사용법
///    1. Label의 Text에 BBCode 설정 : [url="xxx"]...[/url]
///    2. "..." Text 터치시 "xxx"로 URL 연결              : 인스팩터에서 m_bIsAutoOpenURL를 체크
///    3. "..." Text 터치시 "xxx"를 파라미터로 이벤트 받기 : 해당 스크립트의 AddEvent로 콜 받을 함수등록
///    
/// - 주의사항
///    1. URL연결시 URL이 올바른지 체크하지 않습니다.
///    2. 이벤트로 등록한 함수가 자동 해제되지 않습니다.
///    3. 등록한 함수의 부모 객체가 제거 되었을때 이벤트 해제를 하지 않으면 이벤트 발생시 크래시가 발생할 수 있습니다.
/// 
/// </summary>

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUILabelHyperLink : MonoBehaviour
{
    #region Members
    public bool         m_bIsAutoOpenURL    = true; // 터치 시 URL로 자동 연결합니다.
    public bool         m_bAddUnderLine     = true; // Link문자에 UnderLine을 추가합니다.

    private UILabel     m_pLabel            = null;
    private BoxCollider m_pCollider         = null;
    private Vector2     m_vSizeToCollider   = Vector3.zero;
    private List<Action<string>> m_pEvent   = new List<Action<string>>();
    #endregion


    #region System Functions
    public void Start()
    {
        CreateCollider();
        CreateLabel();
    }
    
    public void Update()
    {
        SetSizeToCollider();
        SetUnderLine();
    }
    #endregion


    #region Interface Functions
    public void AddEvent(Action<string> pEvent)
    {
        if (true == m_pEvent.Contains(pEvent))
            return;

        m_pEvent.Add(pEvent);
    }
    
    public void DelEvent(Action<string> pEvent)
    {
        if (true == m_pEvent.Contains(pEvent))
            return;

        m_pEvent.Remove(pEvent);
    }
    #endregion


    #region Event Handler
    void OnClick()
    {
        if (null == m_pLabel)
            return;

        var strURL = m_pLabel.GetUrlAtPosition(UICamera.lastHit.point);
        if (true == string.IsNullOrEmpty(strURL))
            return;

        if (true == m_bIsAutoOpenURL)
            Application.OpenURL(strURL);

        SHUtils.ForToList(m_pEvent, (pEvent) =>
        {
            pEvent(strURL);
        });
    }
    #endregion


    #region Utility Functions
    // 유틸 : 콜리더 생성
    void CreateCollider()
    {
        if (null != m_pCollider)
            return;
        
        m_pCollider = gameObject.GetComponent<BoxCollider>();
        if (null == m_pCollider)
        {
            m_pCollider = gameObject.AddComponent<BoxCollider>();
        }
    }

    // 유틸 : Label 생성
    void CreateLabel()
    {
        if (null != m_pLabel)
            return;

        m_pLabel = gameObject.GetComponent<UILabel>();
        if (null == m_pLabel)
        {
            m_pLabel = gameObject.AddComponent<UILabel>();
        }
    }

    // 유틸 : 콜리더 크기변경
    void SetSizeToCollider()
    {
        if (null == m_pLabel)
            return;

        if (null == m_pCollider)
            return;
        
        if (m_pLabel.printedSize == m_vSizeToCollider)
            return;

        m_vSizeToCollider   = m_pLabel.printedSize;
        m_pCollider.center  = new Vector3(m_vSizeToCollider.x * 0.5f, 0, 0);
        m_pCollider.size    = new Vector3(m_vSizeToCollider.x, m_vSizeToCollider.y, 0);
        m_pCollider.enabled = true;
    }

    // 유틸 : 언더라인 설정
    void SetUnderLine()
    {
        if (false == m_bAddUnderLine)
            return;

        if (null == m_pLabel)
            return;

        var strLabel = m_pLabel.text;
        if (true == string.IsNullOrEmpty(strLabel))
            return;

        if (-1 != strLabel.IndexOf("[u]"))
            return;

        var iLinkStart = strLabel.LastIndexOf("[url=");
        if (-1 == iLinkStart)
            return;

        // Link 시작부분에 [u] BBCode 추가
        var iLinkEnd = strLabel.IndexOf("]", iLinkStart);
        if (-1 == iLinkEnd)
            return;
        strLabel = strLabel.Insert(iLinkEnd + 1, "[u]");

        // Link 끝부분에 [/u] BBCode 추가
        var iURLEnd = strLabel.IndexOf("[/url]", iLinkEnd);
        if (-1 == iURLEnd)
            return;
        strLabel = strLabel.Insert(iURLEnd, "[/u]");

        m_pLabel.text = strLabel;
    }
    #endregion
}