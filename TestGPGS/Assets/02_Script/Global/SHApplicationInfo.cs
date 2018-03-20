using UnityEngine;
using UnityEngine.EventSystems;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

using LitJson;

using DicRealLoadInfo = System.Collections.Generic.Dictionary<eSceneType, System.Collections.Generic.List<string>>;

public partial class SHApplicationInfo : SHSingleton<SHApplicationInfo>
{
    #region Members
    [Header("Release")]
    // 배포제한 시간정보
    [SerializeField] private SHReleaseTimer     m_pReleaseTime      = new SHReleaseTimer();

    [Header("Debug")]
    // 컴포넌트(디버그) : 디버그용 정보출력 
    [SerializeField] private GUIText            m_pDebugText        = null;

    // 기타(디버그) : FPS 출력용 델타타임
    [ReadOnlyField]
    [SerializeField] private float              m_fDeltaTime        = 0.0f;

    // 기타(디버그) : 로드 시도된 리소스 리스트
    [HideInInspector] private DicRealLoadInfo   m_dicRealLoadInfo   = new DicRealLoadInfo();

    // 기타 : 앱 종료 여부
    [HideInInspector] public bool               m_bIsAppQuit        = false;
    #endregion


    #region System Functions
    // 시스템 : App Quit
    void OnApplicationQuit()
    {
        m_bIsAppQuit = true;
    }

    // 시스템 : App Pause
    void OnApplicationPause(bool bIsPause)
    {
    }

    // 시스템 : App Focus
    eBOOL m_eIsFocus = eBOOL.None;
    void OnApplicationFocus(bool bIsFocus)
    {
        // 초기 실행으로 인해 Focus가 true일때는 체크 무시
        if (m_eIsFocus == eBOOL.None)
        {
            m_eIsFocus = bIsFocus ? eBOOL.True : eBOOL.False;
            return;
        }

        // Focus가 true일때 아래 기능 동작할 수 있도록
        if (eBOOL.True != (m_eIsFocus = bIsFocus ? eBOOL.True : eBOOL.False))
            return;

        // 서비스 상태 체크 후 Run이 아니면 Intro로 보낸다.
        CheckServiceState((eResult) =>
        {
            if (eServiceState.Run != eResult)
                Single.Scene.Addtive(eSceneType.Intro, true);
        });
    }
    
    // 시스템 : 업데이트
    public override void Update()
    {
        m_fDeltaTime += (Time.deltaTime - m_fDeltaTime) * 0.1f;

        if (true == Input.GetKeyDown(KeyCode.Escape))
        {
            SHUtils.GameQuit();
        }
    }

    // 시스템 : GUI 업데이트
    void OnGUI()
    {
        DrawAppInformation();
        ControlRenderFrame();
    }
    #endregion


    #region Virtual Functions
    // 다양화 : 생성시
    public override void OnInitialize()
    {
        SetDontDestroy();

        // 어플리케이션 정보설정
        SetApplicationInfo();

        // 스크린 로그 초기화
        ScreenLog(string.Empty);

        // 디바이스 정보 로그
        PrintDeviceInfo();

        // 디버그 기능
        StartCoroutine(CheckReleaseTime());
    }

    // 다양화 : 제거시
    public override void OnFinalize() { }
    #endregion


    #region Interface Functions
    // 인터페이스 : 어플리케이션 정보설정
    public void SetApplicationInfo()
    {
        var pClientInfo = Single.Table.GetTable<JsonClientConfig>();
        SetVSync(pClientInfo.GetVSyncCount());
        SetFrameRate(pClientInfo.GetFrameRate());
        SetCacheInfo(pClientInfo.GetCacheSize(), 30);
        SetSleepMode();
        SetOrientation();
        SetCrittercism();

        UnityEngine.Debug.LogFormat("ProcessID : {0}", GetProcessID());
        UnityEngine.Debug.LogFormat("DebugPort : {0}", GetDebugPort());
    }
    
    // 인터페이스 : 에디터 모드 체크
    public bool IsEditorMode()
    {
        return ((Application.platform == RuntimePlatform.WindowsEditor) ||
                (Application.platform == RuntimePlatform.OSXEditor));
    }

    // 인터페이스 : 실행 플랫폼
    public RuntimePlatform GetRuntimePlatform()
    {
        return Application.platform;
    }
    
    // 인터페이스 : 현재 서비스 상태 체크
    public void CheckServiceState(Action<eServiceState> pCallback)
    {
        Single.Table.DownloadServerConfig(() =>
        {
            if (null == pCallback)
                return;

            pCallback(GetServiceState());
        });
    }

    // 인터페이스 : 서비스 상태
    public eServiceState GetServiceState()
    {
        var eState = Single.Table.GetServiceState();
        if (eServiceState.None == eState)
            return eServiceState.ConnectMarket;
        else
            return eState;
    }

    // 인터페이스 : 앱 이름
    public string GetAppName()
    {
        return Application.bundleIdentifier.Split('.')[2];
    }

    // 인터페이스 : 시스템 언어
    public SystemLanguage GetSystemLanguage()
    {
        return Application.systemLanguage;
    }

    // 인터페이스 : 프로세스 ID
    public int GetProcessID()
    {
        var pProcess = System.Diagnostics.Process.GetCurrentProcess();
        return pProcess.Id;
    }

    // 인터페이스 : 유니티 프로세스 포트번호 ( 디버깅용 )
    public int GetDebugPort()
    {
        return 56000 + (GetProcessID() % 1000);
    }

    // 인터페이스 : Screen Log
    public void ScreenLog(string strLog)
    {
        if (null == m_pDebugText)
            return;

        m_pDebugText.text = strLog;
    }
    #endregion


    #region Utility Functions
    // 유틸 : VSync 설정
    void SetVSync(int iCount)
    {
        QualitySettings.vSyncCount = iCount;
    }

    // 유틸 : 프레임 레이트 설정
    void SetFrameRate(int iFrame)
    {
        Application.targetFrameRate = iFrame;
    }

    // 유틸 : SleepMode 설정
    void SetSleepMode()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // 유틸 : 화면회전 방향 설정
    void SetOrientation()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToLandscapeLeft = true;
    }

    // 유틸 : 캐시크기 및 완료기간 설정
    void SetCacheInfo(long lSizeMB, int iExpirationMonth)
    {
        Caching.maximumAvailableDiskSpace   = lSizeMB * 1024 * 1024;
        Caching.expirationDelay             = 60 * 60 * 24 * iExpirationMonth;
    }

    // 유틸 : 크래시 래포트 설정
    void SetCrittercism()
    {
// #if UNITY_ANDROID
//         UnityEngine.Debug.LogFormat("Crittercism.DidCrashOnLastLoad = {0}", CrittercismAndroid.DidCrashOnLastLoad());
//         CrittercismAndroid.Init("20fb64bf760d44589b6aefeb6bcb220700555300");
//         CrittercismAndroid.SetLogUnhandledExceptionAsCrash(true);
// #elif UNITY_IPHONE || UNITY_IOS
//         UnityEngine.Debug.LogFormat("Crittercism.DidCrashOnLastLoad = {0}", CrittercismIOS.DidCrashOnLastLoad());
//         CrittercismIOS.Init("7d02af2372694b93b84d75a999dd7dd400555300");
//         CrittercismIOS.SetLogUnhandledExceptionAsCrash(true);
// #endif
    }
    
    // 유틸 : 해상도 비율값
    int GetRatioW(int iValue)
    {
        return (int)(iValue * (Screen.width / 1280.0f));
    }
    int GetRatioH(int iValue)
    {
        return (int)(iValue * (Screen.height / 720.0f));
    }
    #endregion


    #region 에디터 테스트
    // 디버그 : 실시간 로드 리소스 리스트 파일로 출력
    [FuncButton]
    public void SaveLoadResourceList()
    {
        var pJsonData = new JsonData();
        
        SHUtils.ForToDic(m_dicRealLoadInfo, (pKey, pValue) =>
        {
            SHUtils.ForToList(pValue, (pInfo) =>
            {
                pJsonData[string.Format("Scene : {0}", pKey)].Add(pInfo);
            });
        });
        
        var pJsonWriter = new JsonWriter();
        pJsonWriter.PrettyPrint = true;
        JsonMapper.ToJson(pJsonData, pJsonWriter);

        string strSavePath = string.Format("{0}/{1}.json", SHPath.GetPathToAssets(), "RealTimeLoadResource");
        SHUtils.SaveFile(pJsonWriter.ToString(), strSavePath);

        System.Diagnostics.Process.Start(strSavePath);
    }
    [FuncButton]
    public void ClearLoadResourceList()
    {
        m_dicRealLoadInfo.Clear();
    }
    #endregion


    #region 디버그 로그
    // 디버그 : 앱정보 출력
    void DrawAppInformation()
    {
        var pServerInfo = Single.Table.GetTable<JsonServerConfig>();
        if (false == pServerInfo.IsLoadTable())
            return;

        GUIStyle pStyle = new GUIStyle(GUI.skin.box);
        pStyle.fontSize = GetRatioW(20);

        GUI.Box(new Rect(0, (Screen.height - GetRatioH(30)), GetRatioW(350), GetRatioH(30)),
            string.Format("{0} - {1} : {2} Scene", Single.Table.GetServiceMode(), GetRuntimePlatform(), Single.Scene.GetActiveScene()), pStyle);

        GUI.Box(new Rect((Screen.width * 0.5f) - (GetRatioW(120) * 0.5f), (Screen.height - GetRatioH(30)), GetRatioW(120), GetRatioH(30)),
            string.Format("Ver {0}", Single.Table.GetClientVersion()), pStyle);
    }

    // 디버그 : 렌더 프레임 제어
    void ControlRenderFrame()
    {
        //if (true == GUI.Button(new Rect(GetRatioW(150), 0, GetRatioW(150), GetRatioH(50)), string.Format("Up RenderFrame : {0}", GetFrameRate())))
        //    SetFrameRate(GetFrameRate() + 1);
        //if (true == GUI.Button(new Rect(GetRatioW(150), GetRatioH(50), GetRatioW(150), GetRatioH(50)), string.Format("Down RenderFrame : {0}", GetFrameRate())))
        //    SetFrameRate(GetFrameRate() - 1);
    }

    // 디버그 : 배포제한시간 체크
    IEnumerator CheckReleaseTime()
    {
        if (true == IsEditorMode())
            yield break;

        yield return new WaitForSeconds(1.0f);

        if (true == Single.Timer.IsPastTimeToLocal(m_pReleaseTime))
            SHUtils.GameQuit();
        else
            StartCoroutine(CheckReleaseTime());
    }

    // 디버그 : 실시간 로드 리소스 리스트
    public void SetLoadResource(string strInfo)
    {
        if (false == m_dicRealLoadInfo.ContainsKey(Single.Scene.GetActiveScene()))
            m_dicRealLoadInfo.Add(Single.Scene.GetActiveScene(), new List<string>());

        //// 콜스택 남기기
        //strInfo += string.Format("\n< CallStack >\n{0}", SHUtils.GetCallStack());

        m_dicRealLoadInfo[Single.Scene.GetActiveScene()].Add(strInfo);
    }

    [FuncButton]
    public void PrintDeviceInfo()
    {
        string strBuff = "<color=yellow>==================Device Information==================</color>\n";
        {
            strBuff += string.Format("Screen.currentResolution.height : {0}\n", Screen.currentResolution.height);
            strBuff += string.Format("Screen.currentResolution.width : {0}\n", Screen.currentResolution.width);
            strBuff += string.Format("Screen.dpi : {0}\n", Screen.dpi);
            strBuff += string.Format("Screen.fullScreen : {0}\n", Screen.fullScreen);
            strBuff += string.Format("Screen.sleepTimeout : {0}\n", Screen.sleepTimeout);
            strBuff += string.Format("Screen.width : {0}\n", Screen.width);
            strBuff += string.Format("Screen.height : {0}\n", Screen.height);
            strBuff += string.Format("deviceModel : {0}\n", SystemInfo.deviceModel);
            strBuff += string.Format("deviceName : {0}\n", SystemInfo.deviceName);
            strBuff += string.Format("deviceType : {0}\n", SystemInfo.deviceType);
#if UNITY_EDITOR
            strBuff += string.Format("deviceUniqueIdentifier : {0}\n", SystemInfo.deviceUniqueIdentifier);
#endif
            strBuff += string.Format("graphicsDeviceID : {0}\n", SystemInfo.graphicsDeviceID);
            strBuff += string.Format("graphicsDeviceName : {0}\n", SystemInfo.graphicsDeviceName);
            strBuff += string.Format("graphicsDeviceType : {0}\n", SystemInfo.graphicsDeviceType);
            strBuff += string.Format("graphicsDeviceVendor : {0}\n", SystemInfo.graphicsDeviceVendor);
            strBuff += string.Format("graphicsDeviceVendorID : {0}\n", SystemInfo.graphicsDeviceVendorID);
            strBuff += string.Format("graphicsDeviceVersion : {0}\n", SystemInfo.graphicsDeviceVersion);
            strBuff += string.Format("graphicsMemorySize : {0}\n", SystemInfo.graphicsMemorySize);
            strBuff += string.Format("graphicsMultiThreaded : {0}\n", SystemInfo.graphicsMultiThreaded);
            strBuff += string.Format("graphicsShaderLevel : {0}\n", SystemInfo.graphicsShaderLevel);
            strBuff += string.Format("maxTextureSize : {0}\n", SystemInfo.maxTextureSize);
            strBuff += string.Format("npotSupport : {0}\n", SystemInfo.npotSupport);
            strBuff += string.Format("operatingSystem : {0}\n", SystemInfo.operatingSystem);
            strBuff += string.Format("processorCount : {0}\n", SystemInfo.processorCount);
            strBuff += string.Format("processorFrequency : {0}\n", SystemInfo.processorFrequency);
            strBuff += string.Format("processorType : {0}\n", SystemInfo.processorType);
            strBuff += string.Format("supportedRenderTargetCount : {0}\n", SystemInfo.supportedRenderTargetCount);
            strBuff += string.Format("supports3DTextures : {0}\n", SystemInfo.supports3DTextures);
            strBuff += string.Format("supportsAccelerometer : {0}\n", SystemInfo.supportsAccelerometer);
            strBuff += string.Format("supportsComputeShaders : {0}\n", SystemInfo.supportsComputeShaders);
            strBuff += string.Format("supportsGyroscope : {0}\n", SystemInfo.supportsGyroscope);
            strBuff += string.Format("supportsImageEffects : {0}\n", SystemInfo.supportsImageEffects);
            strBuff += string.Format("supportsInstancing : {0}\n", SystemInfo.supportsInstancing);
            strBuff += string.Format("supportsLocationService : {0}\n", SystemInfo.supportsLocationService);
            strBuff += string.Format("supportsRawShadowDepthSampling : {0}\n", SystemInfo.supportsRawShadowDepthSampling);
            strBuff += string.Format("supportsRenderToCubemap : {0}\n", SystemInfo.supportsRenderToCubemap);
            strBuff += string.Format("supportsShadows : {0}\n", SystemInfo.supportsShadows);
            strBuff += string.Format("supportsSparseTextures : {0}\n", SystemInfo.supportsSparseTextures);
            strBuff += string.Format("supportsVibration : {0}\n", SystemInfo.supportsVibration);
            strBuff += string.Format("systemMemorySize : {0}\n", SystemInfo.systemMemorySize);
        }

        UnityEngine.Debug.Log(strBuff);
    }
    #endregion
}
