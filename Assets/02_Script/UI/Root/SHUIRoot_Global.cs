using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SHUIRoot_Global : MonoBehaviour
{
    private static Transform    m_pRoot     = null;
    private static UICamera     m_pCamera   = null;

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

    public static Transform GetRoot()
    {
        return m_pRoot;
    }

    public static UICamera GetCamera()
    {
        return m_pCamera;
    }
}