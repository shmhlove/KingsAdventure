using UnityEngine;
using System;
using System.Collections;

public class SHUISpriteAnimation : MonoBehaviour
{
    #region Members
    public enum eWrapMode
    {
        Once,
        ClampToLast,
        ClampToStart,
        Loop,
    };
    public eWrapMode    m_eWrapMode     = eWrapMode.ClampToStart;

    public float        m_fTimeGap      = 1.0f;
    public int          m_iTileCntX     = 1;
    public int          m_iTileCntY     = 1;
    public UITexture    m_pUITexture    = null;

    private bool        m_bIsStop;
    private float       m_fAccTime;
    private int         m_iMaxIndex;
    private int         m_iCurIndex;
    private Vector2     m_vTileSize;
    #endregion


    #region System Functions
    void Start()
    {
        if (null == m_pUITexture)
        {
            Debug.LogError("UISpriteAnimation은 UITexture 컴포넌트로 동작합니다. UITexture를 컴포넌트로 추가하세요!!");
            return;
        }

        Initialize();
    }

    void Update()
    {
        if (true == CheckStop())
            return;

        UpdateToAnimation();
        UpdateToTime();
    }
    #endregion


    #region Interface Functions
    public void Play()
    {
        Initialize();
    }

    public void Pause()
    {
        m_bIsStop = true;
    }

    public void Resum()
    {
        m_bIsStop = false;
    }

    public bool IsPlay()
    {
        return (false == m_bIsStop);
    }

    public void SetTexture(Texture pTexture, int iTileX, int iTileY)
    {
        m_iTileCntX              = iTileX;
        m_iTileCntY              = iTileY;
        Initialize();

        m_pUITexture.mainTexture = pTexture;
    }

    public void SetSpeed(float fSpeed)
    {
        m_fTimeGap = fSpeed;
    }

    [FuncButton]
    void Reset()
    {
        Initialize();
    }
    #endregion


    #region Utility Functions
    void Initialize()
    {
        m_iMaxIndex = m_iTileCntX * m_iTileCntY;

        m_bIsStop   = false;
        m_fAccTime  = 0.0f;
        m_iCurIndex = 0;

        m_vTileSize = new Vector2(SHMath.Divide(1.0f, (float)m_iTileCntX),
                                  SHMath.Divide(1.0f, (float)m_iTileCntY));
    }

    void UpdateToAnimation()
    {
        if (true == CheckStop())
            return;

        Rect pRect      = m_pUITexture.uvRect;
        pRect.x         = (m_iCurIndex % m_iTileCntX) * m_vTileSize.x;
        pRect.y         = ((m_iTileCntY - 1) - (m_iCurIndex / m_iTileCntX)) * m_vTileSize.y;
        pRect.width     = m_vTileSize.x;
        pRect.height    = m_vTileSize.y;
        m_pUITexture.uvRect = pRect;
    }

    void UpdateToTime()
    {
        m_fAccTime += Time.deltaTime;
        if (m_fTimeGap > m_fAccTime)
            return;

        m_fAccTime = 0.0f;
        ++m_iCurIndex;

        switch (m_eWrapMode)
        {
            case eWrapMode.Once:
                Single.Coroutine.NextUpdate(() => Destroy(gameObject));
                break;
            case eWrapMode.ClampToLast:
                m_bIsStop = (m_iMaxIndex <= m_iCurIndex);
                break;
            case eWrapMode.ClampToStart:
                m_bIsStop = (m_iMaxIndex < m_iCurIndex);
                break;
            case eWrapMode.Loop:
                m_iCurIndex = SHMath.LoopingNumber(m_iCurIndex, 0, m_iMaxIndex);
                break;
        }

        m_iCurIndex = Mathf.Clamp(m_iCurIndex, 0, m_iMaxIndex);
    }

    bool CheckStop()
    {
        return (true == m_bIsStop) || (null == m_pUITexture);
    }
    #endregion
}