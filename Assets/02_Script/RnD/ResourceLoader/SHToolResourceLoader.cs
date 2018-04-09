﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class SHToolResourceLoader : MonoBehaviour
{
    public  string m_strFileName    = "파일명을 기입하면 그 파일만 로드합니다.";
    private string m_strDescription = "파일명을 기입하면 그 파일만 로드합니다.";

    private SHLoader    m_pLoader    = new SHLoader();
    private SHTableData m_pTableData = new SHTableData();

    void Awake()
    {
        m_pLoader.Initialize();
        m_pTableData.OnInitialize();
    }

    [FuncButton]
    void StartLoadTableData()
    {
        if (true == m_pLoader.IsRemainLoadFiles())
        {
            EditorUtility.DisplayDialog("[SHToolResourceLoader]", "파싱 중 입니다. 완료 후 시작해주세요.", "확인");
            return;
        }

        if ((true == string.IsNullOrEmpty(m_strFileName)) ||
            (true == m_strFileName.Equals(m_strDescription)))
        {
            m_pLoader.Process(m_pTableData.GetLoadList(eSceneType.None));
        }
        else
        {
            m_pLoader.Process(m_pTableData.CreateLoadInfo(m_strFileName));
        }
    }
}
#endif