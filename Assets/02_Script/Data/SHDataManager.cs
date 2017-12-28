using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SHDataManager : SHSingleton<SHDataManager>
{
    #region Members
    // 테이블 데이터 : Excel, SQLite, Json, Byte, XML
    private SHTableData     m_pTableData = new SHTableData();
    public SHTableData      TableData { get { return m_pTableData; } }

    // 리소스 데이터 : Prefab, Animation, Texture, Sound, Material
    private SHResourceData  m_pResourcesData = new SHResourceData();
    public SHResourceData   ResourcesData { get { return m_pResourcesData; } }

    // 애셋번들 데이터
    private SHAssetBundleData  m_pAssetBundleData = new SHAssetBundleData();
    public SHAssetBundleData   AssetBundleData { get { return m_pAssetBundleData; } }

    // 서버 데이터 : 로딩타임에 Req하려는 서버 데이터
    private SHServerData    m_pServerData = new SHServerData();
    public SHServerData     ServerData { get { return m_pServerData; } }

    // 유저 데이터
    private SHUserData      m_pUserData = new SHUserData();
    public SHUserData       UserData { get { return m_pUserData; } }
    
    // 로더
    private SHLoader        m_pLoader = new SHLoader();
    public SHLoader         Loader { get { return m_pLoader; } }
    #endregion


    #region Virtual Functions
    // 다양화 : 싱글턴 생성될때
    public override void OnInitialize()
    {
        TableData.OnInitialize();
        ResourcesData.OnInitialize();
        AssetBundleData.OnInitialize();
        ServerData.OnInitialize();
        UserData.OnInitialize();

        SetDontDestroy();
    }

    // 다양화 : 싱글턴 종료될때
    public override void OnFinalize() 
    {
        TableData.OnFinalize();
        ResourcesData.OnFinalize();
        AssetBundleData.OnFinalize();
        UserData.OnFinalize();
    }
    #endregion


    #region System Functions
    // 시스템 : 업데이트
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        TableData.FrameMove();
        ResourcesData.FrameMove();
        AssetBundleData.FrameMove();
        ServerData.FrameMove();
        UserData.FrameMove();
    }
    #endregion


    #region Interface Functions
    // 인터페이스 : 로드명령
    public void Load(eSceneType eType, EventHandler pDone, EventHandler pProgress, EventHandler pError)
    {
        OnEventToLoadStart();
        Loader.LoadStart(GetLoadList(eType), pDone + OnEventToLoadDone, pProgress, pError);
    }

    // 인터페이스 : 패치명령
    public void Patch(EventHandler pDone, EventHandler pProgress, EventHandler pError)
    {
        Loader.LoadStart(GetPatchList(), pDone, pProgress, pError);
    }
    
    // 인터페이스 : 로드가 완료되었는가?(성공/실패유무가 아님)
    public bool IsLoadDone()
    {
        return Loader.IsLoadDone();
    }

    // 인터페이스 : 특정 파일이 로드완료되었는가?(성공/실패유무가 아님)
    public bool IsLoadDone(string strFileName)
    {
        return Loader.IsLoadDone(strFileName);
    }

    // 인터페이스 : 특정 타입이 로드완료되었는가?(성공/실패유무가 아님)
    public bool IsLoadDone(eDataType eType)
    {
        return Loader.IsLoadDone(eType);
    }

    // 인터페이스 : 로드중인지 체크(로드 중이거나 할 파일이 있는지  체크)
    public bool IsReMainLoadFiles()
    {
        return Loader.IsReMainLoadFiles();
    }

    #endregion


    #region Utility Functions
    // 유틸 : 로드 리스트
    List<Dictionary<string, SHLoadData>> GetLoadList(eSceneType eType)
    {
        var pLoadList = new List<Dictionary<string, SHLoadData>>();

        pLoadList.Add(ServerData.GetLoadList(eType));
        pLoadList.Add(TableData.GetLoadList(eType));
        pLoadList.Add(ResourcesData.GetLoadList(eType));
        pLoadList.Add(AssetBundleData.GetLoadList(eType));
        
        return pLoadList;
    }

    // 유틸 : 패치 리스트
    List<Dictionary<string, SHLoadData>> GetPatchList()
    {
        var pPatchList = new List<Dictionary<string, SHLoadData>>();

        pPatchList.Add(ServerData.GetPatchList());
        pPatchList.Add(TableData.GetPatchList());
        pPatchList.Add(ResourcesData.GetPatchList());
        pPatchList.Add(AssetBundleData.GetPatchList());
        
        return pPatchList;
    }
    #endregion


    #region Event Handler
    // 이벤트 : 로드가 시작될때
    void OnEventToLoadStart()
    {
        // ResourcesData.OnFinalize();
        Resources.UnloadUnusedAssets();

        // 가비지를 동작시킨다.
        for (int iLoop = 0; iLoop < System.GC.MaxGeneration; ++iLoop)
        {
            System.GC.Collect(iLoop, GCCollectionMode.Forced);
        }
    }

    // 이벤트 : 로드가 완료되었을때
    public void OnEventToLoadDone(object pSender, EventArgs vArgs)
    {
        Resources.UnloadUnusedAssets();

        // 가비지를 동작시킨다.
        for (int iLoop = 0; iLoop < System.GC.MaxGeneration; ++iLoop)
        {
            System.GC.Collect(iLoop, GCCollectionMode.Forced);
        }
    }
    #endregion
}