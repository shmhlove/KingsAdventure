/// <summary>
/// 
/// - 개요
///     범용화된 이벤트를 쉽게 사용할 수 있도록 합니다.
/// 
/// - 사용법
///     class Sender
///     {
///          public SHEvent pEvent = new SHEvent();
///          
///          void ABCD(...)
///          {
///              pEvent.Callback<Vector3>(this, new Vector3());
///          }
///          void AddEvent(EventHandler pHandler)
///          {
///             pEvent.Add(pHandler);
///          }
///          void DelEvent(EventHandler pHandler)
///          {
///             pEvent.Del(pHandler);
///          }
///     }
///     
///     class Observer
///     {
///          Sender pSender = new Sender();
///          Observer()
///          {
///              pSender.AddEvent(OnEventCallback);
///          }
///          void OnEventCallback(object pSender, EventArgs vArgs)
///          {   
///              Debug.Log( "Param : " + Single.Event.GetArgs<Vector3>(vArgs) );
///          }
///     }
/// 
/// </summary>

using UnityEngine;

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class SHEventUtil : SHSingleton<SHEventUtil>
{
    #region Members
    private SHEvent pEvent = new SHEvent();
    #endregion


    #region Virtual Functions
    public override void OnInitialize() { }
    public override void OnFinalize() { }
    #endregion


    #region Interface Functions
    public SHEventParam<T> SetArgs<T>(T pArgs)
    {
        return new SHEventParam<T>(pArgs);
    }

    public T GetArgs<T>(EventArgs pArgs)
    {
        if (null == pArgs)
            return default(T);

        return ((SHEventParam<T>)pArgs).GetData;
    }

    public void SendEvent<T>(EventHandler pObserver)
    {
        if (null == pObserver)
            return;

        pEvent.Clear();
        pEvent.Add(pObserver);
        pEvent.Callback(this);
    }

    public void SendEvent<T>(EventHandler pObserver, T pArgs)
    {
        if (null == pObserver)
            return;

        pEvent.Clear();
        pEvent.Add(pObserver);
        pEvent.Callback<T>(this, pArgs);
    }
    #endregion
}

public class SHEventParam<T> : EventArgs
{
    private T epData;
    public T GetData { get { return epData; } }
    public SHEventParam(T data) { epData = data; }
}

public sealed class SHEvent
{
    #region Members
    private event EventHandler m_pHandler = null;
    #endregion


    #region System Functions
    public SHEvent() { }
    public SHEvent(EventHandler pObserver)
    {
        Add(pObserver);
    }
    #endregion


    #region Interface Functions
    public void Add(EventHandler pObserver)
    {
        m_pHandler -= pObserver;
        m_pHandler += pObserver;
    }

    public void Remove(EventHandler pObserver)
    {
        m_pHandler -= pObserver;
    }

    public void Clear()
    {
        m_pHandler = null;
    }

    public void Callback(object pSender)
    {
        if (null == m_pHandler)
            return;

        m_pHandler(pSender, null);
    }

    public void Callback<T>(object pSender, T pArgs)
    {
        if (null == m_pHandler)
            return;

        m_pHandler(pSender, new SHEventParam<T>(pArgs));
    }

    public void Callback(object pSender, EventArgs pArgs)
    {
        if (null == m_pHandler)
            return;

        m_pHandler(pSender, pArgs);
    }
    #endregion
}