using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SHUIRoot_Global : MonoBehaviour
{
    #region Members : Singleton
    private static Transform    m_pRoot     = null;
    private static UICamera     m_pCamera   = null;
    #endregion
    

    #region System Functions
    void Awake()
    {
        var pPanels = gameObject.GetComponentsInChildren<SHUIBasePanel>();
        SHUtils.ForToArray(pPanels, (pPanel) =>
        {
            Single.UI.AddPanel(pPanel, false);
        });

        m_pRoot   = transform;
        m_pCamera = m_pRoot.GetComponentInChildren<UICamera>();
    }
    void OnDestroy()
    {
        if (m_pRoot != transform)
            return;

        m_pRoot   = null;
        m_pCamera = null;
    }
    #endregion


    #region Interface Functions
    public static Transform GetRoot()
    {
        return m_pRoot;
    }

    public static UICamera GetCamera()
    {
        return m_pCamera;
    }
    #endregion
}