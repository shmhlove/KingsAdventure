using UnityEngine;
using Object = UnityEngine.Object;

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public partial class SHResourceData : SHBaseData
{
    #region Members
    // 리소스 리스트
    private Dictionary<string, Object> m_dicResources = new Dictionary<string, Object>();
    #endregion


    #region Virtual Functions
    // 다양화 : 초기화
    public override void OnInitialize()
    {
        m_dicResources.Clear();
    }

    // 다양화 : 마무리
    public override void OnFinalize()
    {
        m_dicResources.Clear();
    }
    
    // 다양화 : 로드할 데이터 리스트 알려주기
    public override Dictionary<string, SHLoadData> GetLoadList(eSceneType eType)
    {
        var dicLoadList  = new Dictionary<string, SHLoadData>();
        SHUtils.ForToList(Single.Table.GetPreLoadResourcesList(eType), (pValue) =>
        {
            if (true == IsLoadResource(pValue))
                return;

            dicLoadList.Add(pValue, CreateLoadInfo(pValue));
        });

        return dicLoadList;
    }
    
    // 다양화 : 로더로 부터 호출될 로드함수
    public override IEnumerator Load(SHLoadData pInfo, Action<string, SHLoadStartInfo> pStart, 
                                                       Action<string, SHLoadEndInfo> pDone)
    {
        pStart(pInfo.m_strName, new SHLoadStartInfo());

        if (true == IsLoadResource(pInfo.m_strName.ToLower()))
        {
            pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Succeed));
            yield break;
        }

        SHResourcesInfo pResourceInfo = Single.Table.GetResourcesInfo(pInfo.m_strName);
        if (null == pResourceInfo)
        {
            Debug.LogFormat("리소스 테이블에 {0}가 없습니다.(파일이 없거나 리소스 리스팅이 안되었음)", pInfo.m_strName);
            pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Resource_Not_ExsitTable));
            yield break;
        }

        Single.Coroutine.Routine(() => 
        {
            var pResource = GetResources(pInfo.m_strName);
            if (null == pResource)
                pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Resource_Load_Fail));
            else
                pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Succeed));

        }, LoadAsync<Object>(pResourceInfo));
        
        var pObject = LoadSync<Object>(pResourceInfo);
        if (null == pObject)
            pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Resource_Load_Fail));
        else
            pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Succeed));

        yield return null;
    }
    #endregion


    #region Interface Functions
    // 인터페이스 : 리소스 로드 확인
    public bool IsLoadResource(string strName)
    {
        return m_dicResources.ContainsKey(strName.ToLower());
    }
    // 인터페이스 : 리소스 얻기
    public Object GetResources(string strFileName)
    {
        return GetResources<Object>(strFileName);
    }
    public T GetResources<T>(string strFileName) where T : Object
    {
        if (true == string.IsNullOrEmpty(strFileName))
            return null;

        strFileName = Path.GetFileNameWithoutExtension(strFileName);
        if (false == IsLoadResource(strFileName.ToLower()))
        {
            SHResourcesInfo pInfo = Single.Table.GetResourcesInfo(strFileName);
            if (null == pInfo)
            {
                Debug.Log(string.Format("리소스 테이블에 {0}가 없습니다.(파일이 없거나 리소스 리스팅이 안되었음)", strFileName));
                return null;
            }

            return LoadSync<T>(pInfo);
        }

        return m_dicResources[strFileName.ToLower()] as T;
    }
    // 인터페이스 : 프리팹 원본 얻기
    public GameObject GetPrefab(string strName)
    {
        return GetResources<GameObject>(strName);
    }
    // 인터페이스 : 프리팹에서 게임오브젝트로 복사해서 얻기
    public GameObject GetGameObject(string strName)
    {
        return Instantiate<GameObject>(GetPrefab(strName));
    }
    // 인터페이스 : 텍스쳐 얻기
    public Texture GetTexture(string strName)
    {
        return GetResources<Texture>(strName);
    }
    // 인터페이스 : 텍스쳐2D 얻기
    public Texture2D GetTexture2D(string strName)
    {
        return GetResources<Texture2D>(strName);
    }
    // 인터페이스 : Sprite 데이터 얻기
    public Sprite GetSprite(string strName)
    {
        Texture2D pTexture = GetTexture2D(strName);
        if (null == pTexture)
            return null;

        return Sprite.Create(pTexture, new Rect(0.0f, 0.0f, pTexture.width, pTexture.height), new Vector2(0.5f, 0.5f));
    }
    // 인터페이스 : Texture 다운로드
    public Texture2D GetDownloadTexture(string strURL)
    {
        WWW pWWW = Single.Coroutine.WWWOfSync(new WWW(strURL));
        if (false == string.IsNullOrEmpty(pWWW.error))
            return null;

        return pWWW.texture;
    }
    public void GetDownloadTexture(string strURL, Action<Texture2D> pCallback)
    {
        if (null == pCallback)
            return;

        Single.Coroutine.WWW((pWWW) =>
        {
            if (false == string.IsNullOrEmpty(pWWW.error))
                pCallback(null);
            else
                pCallback(pWWW.texture);
        }, new WWW(strURL));
    }
    // 인터페이스 : 애니메이션 클립얻기
    public AnimationClip GetAniamiton(string strName)
    {
        return GetResources<AnimationClip>(strName);
    }
    // 인터페이스 : 메테리얼 얻기
    public Material GetMaterial(string strName)
    {
        return GetResources<Material>(strName);
    }
    // 인터페이스 : 사운드 클립 얻기
    public AudioClip GetSound(string strName)
    {
        return GetResources<AudioClip>(strName);
    }
    // 인터페이스 : Text 데이터 얻기
    public TextAsset GetTextAsset(string strName)
    {
        return GetResources<TextAsset>(strName);
    }
    // 인터페이스 : 게임 오브젝트를 생성해서 컴포넌트로 데이터 얻기
    public T GetObjectComponent<T>(string strName)
    {
        GameObject pObject = GetGameObject(strName);
        if (null == pObject)
            return default(T);

        return pObject.GetComponent<T>();
    }
    // 인터페이스 : 빈 게임오브젝트를 생성하고 컴퍼넌트를 추가한뒤 얻어낸다.
    public T GetCreateComponent<T>(string strName) where T : Component
    {
        return SHGameObject.GetComponent<T>(SHGameObject.CreateEmptyObject(strName));
    }
    // 인터페이스 : 오브젝트 복사
    public T Instantiate<T>(T pObject) where T : Object
    {
        if (true == Single.AppInfo.m_bIsAppQuit)
            return null;

        if (null == pObject)
        {
            Debug.LogErrorFormat("오브젝트 복사중 Null 프리팹이 전달되었습니다!!(Type : {0})", typeof(T));
            return default(T);
        }

#if UNITY_EDITOR
        DateTime pStartTime = DateTime.Now;
#endif

        T pGameObject    = Object.Instantiate<T>(pObject);
        var strName      = pGameObject.name;
        pGameObject.name = strName.Substring(0, strName.IndexOf("(Clone)"));
        
#if UNITY_EDITOR
        Single.AppInfo.SetLoadResource(string.Format("Instantiate : {0}({1}sec)", pObject.name, ((DateTime.Now - pStartTime).TotalMilliseconds / 1000.0f)));
#endif

        return pGameObject;
    }
    #endregion


    #region Utility Functions
    // 유틸 : 로드정보 만들기
    SHLoadData CreateLoadInfo(string strName)
    {
        return new SHLoadData()
        {
            m_eDataType = eDataType.Resources,
            m_strName = strName,
            m_pLoadFunc = Load,
            m_pLoadOkayTrigger = () =>
            {
                // 테이블 데이터를 먼저 로드하고 리소스 로드할 수 있도록 트리거 설정
                return Single.Data.IsLoadDone(eDataType.LocalTable);
            },
        };
    }
    
    // 유틸 : 어싱크로 리소스 로드하기
    IEnumerator LoadAsync<T>(SHResourcesInfo pTable) where T : UnityEngine.Object
    {
        if (null == pTable)
            yield break;

        if (true == IsLoadResource(pTable.m_strName.ToLower()))
            yield break;
        
#if UNITY_EDITOR
        DateTime pStartTime = DateTime.Now;
#endif

        Object pObject = null;
        //var pBundleData = Single.AssetBundle.GetBundleData(Single.Table.GetBundleInfoToResourceName(pTable.m_strName));
        //if (null != pBundleData)
        //{
        //    var pRequest = pBundleData.m_pBundle.LoadAssetAsync<T>(pTable.m_strName);
        //    yield return pRequest;

        //    pObject = pRequest.asset;
        //}
        //else
        {
            var pRequest = Resources.LoadAsync<T>(pTable.m_strPath);
            yield return pRequest;

            pObject = pRequest.asset;
        }
        
        if (null == pObject)
        {
            Debug.LogError(string.Format("{0}을 로드하지 못했습니다!!\n리소스 테이블에는 목록이 있으나 실제 파일은 없을 수도 있습니다.", pTable.m_strPath));
            yield break;
        }
        
#if UNITY_EDITOR
        Single.AppInfo.SetLoadResource(string.Format("Load : {0}({1}sec)", pTable.m_strName, ((DateTime.Now - pStartTime).TotalMilliseconds / 1000.0f)));
#endif

        m_dicResources.Add(pTable.m_strName.ToLower(), pObject);
    }

    // 유틸 : 싱크로 리소스 로드하기
    T LoadSync<T>(SHResourcesInfo pTable) where T : Object
    {
        if (null == pTable)
            return null;

        if (true == IsLoadResource(pTable.m_strName.ToLower()))
            return m_dicResources[pTable.m_strName.ToLower()] as T;

#if UNITY_EDITOR
        DateTime pStartTime = DateTime.Now;
#endif

        T pObject       = null;
        //var pBundleData = Single.AssetBundle.GetBundleData(Single.Table.GetBundleInfoToResourceName(pTable.m_strName));
        //if (null != pBundleData)
        //    pObject = pBundleData.m_pBundle.LoadAsset<T>(pTable.m_strName);
        //else
            pObject = Resources.Load<T>(pTable.m_strPath);

        if (null == pObject)
        {
            Debug.LogError(string.Format("{0}을 로드하지 못했습니다!!\n리소스 테이블에는 목록이 있으나 실제 파일은 없을 수 있습니다.", pTable.m_strPath));
            return null;
        }

#if UNITY_EDITOR
        Single.AppInfo.SetLoadResource(string.Format("Load : {0}({1}sec)", pTable.m_strName, ((DateTime.Now - pStartTime).TotalMilliseconds / 1000.0f)));
#endif

        m_dicResources.Add(pTable.m_strName.ToLower(), pObject);

        return pObject;
    }
    #endregion
}