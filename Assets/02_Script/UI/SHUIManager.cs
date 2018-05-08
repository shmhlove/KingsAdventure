﻿using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using DicPanels = System.Collections.Generic.Dictionary<string, SHUIBasePanel>;

public partial class SHUIManager : SHSingleton<SHUIManager>
{
    #region Members
    [ReadOnlyField][SerializeField] private DicPanels m_dicPanels = new DicPanels();
    #endregion


    #region Virtual Functions
    public override void OnInitialize()
    {
        SetDontDestroy();
        Single.Scene.AddEventOfAddtiveScene(OnEventOfAddtiveScene);
        SHGameObject.SetParent(Single.Resource.GetGameObject("UIRoot_Global"), gameObject);
    }
    #endregion
    

    #region Interface Functions
    public void AddPanel(SHUIBasePanel pPanel, bool bIsActive)
    {
        if (null == pPanel)
        {
            Debug.LogError("[LSH] Error!!! Can not add panel because panel Is null");
            return;
        }

        if (true == m_dicPanels.ContainsKey(pPanel.name))
        {
            m_dicPanels[pPanel.name] = pPanel;
            Debug.LogWarningFormat("[LSH] Warning!!! Detection same UI Panel(Name : {0})", pPanel.name);
        }
        else
        {
            m_dicPanels.Add(pPanel.name, pPanel);
        }

        pPanel.Initialize(bIsActive);
    }
    public SHUIBasePanel Show(string strName, params object[] pArgs)
    {
        var pPanel = GetPanel(strName);
        if (null == pPanel)
        {
            Debug.LogErrorFormat("[LSH] No Exist Panel(Name : {0})", strName);
            return null;
        }
        
        pPanel.Show(pArgs);
        return pPanel;
    }
    public SHUIBasePanel Close(string strName)
    {
        var pPanel = GetPanel(strName);
        if (null == pPanel)
        {
            Debug.LogErrorFormat("[LSH] Close() - No Exist Panel(Name : {0})", strName);
            return null;
        }

        pPanel.Close();
        return pPanel;
    }
    public bool IsExistPanel(string strName)
    {
        return (null != GetPanel(strName));
    }
    public Transform GetRootToGlobal()
    {
        return SHUIRoot_Global.GetRoot();
    }
    public Transform GetRootToScene()
    {
        return SHUIRoot_Scene.GetRoot();
    }
    #endregion


    #region Utility Functions
    SHUIBasePanel GetPanel(string strName)
    {
        if (false == m_dicPanels.ContainsKey(strName))
        {
            AddPanel(Single.ObjectPool.Get<SHUIBasePanel>(
                strName, ePoolReturnType.None, ePoolDestroyType.None), false);
        }

        if (false == m_dicPanels.ContainsKey(strName))
        {
            return null;
        }
        
        return m_dicPanels[strName];
    }
    void DestoryPanel(Dictionary<string, SHUIBasePanel> dicPanels)
    {
        if (null == dicPanels)
            return;

        SHUtils.ForToDic(new DicPanels(dicPanels), (pKey, pValue) =>
        {
            DestroyPanel(pValue);
            m_dicPanels.Remove(pKey);
        });
    }
    void DestroyPanel(SHUIBasePanel pPanel)
    {
        if (null == pPanel)
            return;

        SHGameObject.DestoryObject(pPanel);
    }
    #endregion


    #region Event Handler
    public void OnEventOfAddtiveScene(object pSender, EventArgs vArgs)
    {
        var pDestroyPanels = new DicPanels();
        SHUtils.ForToDic(m_dicPanels, (pKey, pValue) =>
        {
            if (eObjectDestoryType.ChangeScene != pValue.m_eDestroyType)
                return;

            pDestroyPanels.Add(pKey, pValue);
        });
        
        DestoryPanel(pDestroyPanels);
    }
    #endregion
}
