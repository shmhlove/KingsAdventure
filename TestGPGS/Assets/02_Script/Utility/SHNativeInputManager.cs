using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct TouchEventParam
{
    public int iTouchPingerID;
    public Vector2 vScreenTouchPos;
    public Vector2 vWorldTouchPos;
}

public class SHNativeInputManager : SHSingleton<SHNativeInputManager>
{
    private Dictionary<int, Vector2> m_dicTouchEnter = new Dictionary<int, Vector2>();
    public Dictionary<int, Vector2> TouchEnter
    { get { return m_dicTouchEnter; } }

    private Dictionary<int, Vector2> m_dicTouchEnd = new Dictionary<int, Vector2>();
    public Dictionary<int, Vector2> TouchEnd
    { get { return m_dicTouchEnd; } }

    private Dictionary<int, Vector2> m_dicTouchMove = new Dictionary<int, Vector2>();
    public Dictionary<int, Vector2> TouchMove
    { get { return m_dicTouchMove; } }

    private List<int> m_pTouchOrders = new List<int>();
    public List<int> TouchOrders
    { get { return m_pTouchOrders; } }

    public SHEvent EventToEnter = new SHEvent();
    public SHEvent EventToDrag  = new SHEvent();
    public SHEvent EventToEnd   = new SHEvent();

    public override void OnInitialize() { }
    public override void OnFinalize() { }

    public override void Update()
    {
        base.Update();

        // Mobile
        if (0 < Input.touchCount)
        {
            for (int iLoop = 0; iLoop < Input.touchCount; ++iLoop)
            {
                Touch pTouch = Input.GetTouch(iLoop);
                if (pTouch.phase.Equals(TouchPhase.Began))
                {
                    OnTouchEnter(pTouch.fingerId, pTouch.position);
                }
                else if (pTouch.phase.Equals(TouchPhase.Ended) ||
                        pTouch.phase.Equals(TouchPhase.Canceled))
                {
                    OnTouchEnd(pTouch.fingerId, pTouch.position);
                }
                else if (pTouch.phase.Equals(TouchPhase.Moved))
                {
                    OnTouchMove(pTouch.fingerId, pTouch.position);
                }
            }
        }
        // PC
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                OnTouchEnter(0, new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                OnTouchEnd(0, new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            }
            else if (Input.GetButton("Fire1"))
            {
                OnTouchMove(0, new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            }
        }
    }

    void OnTouchEnter(int iFingerID, Vector2 vTouchPos)
    {
        Vector3 vScreenPos = Vector3.zero;
        vScreenPos.x = (vTouchPos.x / Screen.width);
        vScreenPos.y = (vTouchPos.y / Screen.height);

        m_dicTouchEnter[iFingerID] = vScreenPos;
        m_dicTouchMove[iFingerID] = vScreenPos;
        m_dicTouchEnd.Remove(iFingerID);
        m_pTouchOrders.Add(iFingerID);

        TouchEventParam pEventParam;
        pEventParam.iTouchPingerID = iFingerID;
        pEventParam.vScreenTouchPos = vScreenPos;
        pEventParam.vWorldTouchPos = vTouchPos;
        EventToEnter.Callback<TouchEventParam>(this, pEventParam);
    }

    void OnTouchEnd(int iFingerID, Vector2 vTouchPos)
    {
        Vector3 vScreenPos = Vector3.zero;
        vScreenPos.x = (vTouchPos.x / Screen.width);
        vScreenPos.y = (vTouchPos.y / Screen.height);

        m_dicTouchEnd[iFingerID] = vScreenPos;
        m_dicTouchEnter.Remove(iFingerID);
        m_dicTouchMove.Remove(iFingerID);
        m_pTouchOrders.Remove(iFingerID);

        TouchEventParam pEventParam;
        pEventParam.iTouchPingerID = iFingerID;
        pEventParam.vScreenTouchPos = vScreenPos;
        pEventParam.vWorldTouchPos = vTouchPos;
        EventToEnd.Callback<TouchEventParam>(this, pEventParam);
    }

    void OnTouchMove(int iFingerID, Vector2 vTouchPos)
    {
        Vector3 vScreenPos = Vector3.zero;
        vScreenPos.x = (vTouchPos.x / Screen.width);
        vScreenPos.y = (vTouchPos.y / Screen.height);

        m_dicTouchMove[iFingerID] = vScreenPos;

        TouchEventParam pEventParam;
        pEventParam.iTouchPingerID = iFingerID;
        pEventParam.vScreenTouchPos = vScreenPos;
        pEventParam.vWorldTouchPos = vTouchPos;
        EventToDrag.Callback<TouchEventParam>(this, pEventParam);
    }

    public int GetFirstFingerID()
    {
        if (0 == m_pTouchOrders.Count)
            return -1;

        return m_pTouchOrders[0];
    }

    public int GetLastFingerID()
    {
        if (0 == m_pTouchOrders.Count)
            return -1;

        return m_pTouchOrders[(m_pTouchOrders.Count - 1)];
    }
}
