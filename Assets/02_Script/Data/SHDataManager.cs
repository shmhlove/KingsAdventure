using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SHDataManager : SHSingleton<SHDataManager>
{
    // 테이블 데이터 : Excel, SQLite, Json, Byte, XML
    private SHTableData     m_pTable = new SHTableData();
    public SHTableData      Table { get { return m_pTable; } }

    // 리소스 데이터 : Prefab, Animation, Texture, Sound, Material
    private SHResourceData  m_pResources = new SHResourceData();
    public SHResourceData   Resources { get { return m_pResources; } }

    // 서버 데이터 : 로딩타임에 Req하려는 서버 데이터
    private SHServerData    m_pServer = new SHServerData();
    public SHServerData     Server { get { return m_pServer; } }
    
    // 로더
    private SHLoader        m_pLoader = new SHLoader();
    
    public override void OnInitialize()
    {
        Table.OnInitialize();
        Resources.OnInitialize();
        Server.OnInitialize();

        SetDontDestroy();
    }
    
    public override void OnFinalize() 
    {
        Table.OnFinalize();
        Resources.OnFinalize();
    }
    
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Table.FrameMove();
        Resources.FrameMove();
        Server.FrameMove();
    }
    
    public void Load(eSceneType eType, Action<SHLoadingInfo> pDone, Action<SHLoadingInfo> pProgress)
    {
        OnEventToLoadStart();

        EventHandler pDoneEventHandler = (sender, e) =>
        {
            if (null != pDone)
            {
                pDone(Single.Event.GetArgs<SHLoadingInfo>(e));
            }
        };

        EventHandler pProgressEventHandler = (sender, e) =>
        {
            if (null != pProgress)
            {
                pProgress(Single.Event.GetArgs<SHLoadingInfo>(e));
            }
        };

        m_pLoader.Process(GetLoadList(eType), (pDoneEventHandler + OnEventToLoadDone), pProgressEventHandler);
    }
    
    public void Patch(Action<SHLoadingInfo> pDone, Action<SHLoadingInfo> pProgress)
    {
        EventHandler pDoneEventHandler = (sender, e) =>
        {
            if (null != pDone)
            {
                pDone(Single.Event.GetArgs<SHLoadingInfo>(e));
            }
        };

        EventHandler pProgressEventHandler = (sender, e) =>
        {
            if (null != pProgress)
            {
                pProgress(Single.Event.GetArgs<SHLoadingInfo>(e));
            }
        };

        m_pLoader.Process(GetPatchList(), pDoneEventHandler, pProgressEventHandler);
    }
    
    public bool IsLoadDone()
    {
        return m_pLoader.IsLoadDone();
    }
    
    public bool IsLoadDone(string strFileName)
    {
        return m_pLoader.IsLoadDone(strFileName);
    }
    
    public bool IsLoadDone(eDataType eType)
    {
        return m_pLoader.IsLoadDone(eType);
    }
    
    List<Dictionary<string, SHLoadData>> GetLoadList(eSceneType eType)
    {
        return new List<Dictionary<string, SHLoadData>>()
        {
            Server.GetLoadList(eType),
            Table.GetLoadList(eType),
            Resources.GetLoadList(eType)
        };
    }
    
    List<Dictionary<string, SHLoadData>> GetPatchList()
    {
        return new List<Dictionary<string, SHLoadData>>()
        {
            Server.GetPatchList(),
            Table.GetPatchList(),
            Resources.GetPatchList()
        };
    }
    
    public void OnEventToLoadStart()
    {
        UnityEngine.Resources.UnloadUnusedAssets();
        
        for (int iLoop = 0; iLoop < System.GC.MaxGeneration; ++iLoop)
        {
            System.GC.Collect(iLoop, GCCollectionMode.Forced);
        }
    }
    
    public void OnEventToLoadDone(object pSender, EventArgs vArgs)
    {
        UnityEngine.Resources.UnloadUnusedAssets();
        
        for (int iLoop = 0; iLoop < System.GC.MaxGeneration; ++iLoop)
        {
            System.GC.Collect(iLoop, GCCollectionMode.Forced);
        }
    }
}