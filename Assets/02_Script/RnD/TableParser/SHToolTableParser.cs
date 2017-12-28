#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class SHToolTableParser : MonoBehaviour
{
    #region Members
    public  string m_strFileName    = "파일명을 기입하면 그 파일만 파싱합니다.";
    private string m_strDescription = "파일명을 기입하면 그 파일만 파싱합니다.";

    private SHLoader    m_pLoader    = new SHLoader();
    private SHTableData m_pTableData = new SHTableData();
    #endregion


    #region System Functions
    void Awake()
    {
        m_pTableData.OnInitialize();
        m_pLoader.Initialize();
    }
    #endregion


    #region Interface Functions
    [FuncButton]
    void StartParse()
    {
        if (true == m_pLoader.IsReMainLoadFiles())
        {
            EditorUtility.DisplayDialog("SHToolTableParser", "파싱 중 입니다. 완료 후 시작해주세요.", "확인");
            return;
        }

        if ((true == string.IsNullOrEmpty(m_strFileName)) ||
            (true == m_strFileName.Equals(m_strDescription)))
        {
            m_pLoader.LoadStart(m_pTableData.GetLoadList(eSceneType.None));
        }
        else
        {
            SHLoadData pLoadInfo = m_pTableData.CreateLoadInfo(m_strFileName);
            if (null == pLoadInfo)
            {
                EditorUtility.DisplayDialog(
                    m_strFileName, string.Format("{0} 파일을 로드할 파싱클래스를 찾지 못했습니다.!!", m_strFileName), "확인");
                return;
            }

            m_pLoader.LoadStart(pLoadInfo);
        }
    }
    #endregion
}
#endif