using UnityEngine;
using System;
using System.Collections;

public class SHMonoWrapper : MonoBehaviour
{
    #region Members : Physics
    [HideInInspector] public Vector3     m_vSpeed            = Vector3.zero;
    [HideInInspector] public Vector3     m_vDirection        = Vector3.zero;
    #endregion


    #region Members : Animation
    [HideInInspector] public Animation   m_pAnim             = null;
    [HideInInspector] public bool        m_bIsAnimPlaying    = false;
    #endregion


    #region System Functions
    public virtual void Awake()
    {
    }
    public virtual void Start()
    {
    }
    public virtual void OnEnable()
    {
    }
    public virtual void OnDisable()
    {
    }
    public virtual void OnDestroy()
    {
    }
    public virtual void Update()
    {
    }
    public virtual void FixedUpdate()
    {
    }
    public virtual void LateUpdate()
    {
    }
    #endregion


    #region Interface : Init Physics
    public void InitPhysics()
    {
        m_vSpeed     = Vector3.zero;
        m_vDirection = Vector3.zero;
    }
    #endregion


    #region Interface : Active
    public void SetActive(bool bIsActive)
    {
        if (IsActive() == bIsActive)
            return;
        
        gameObject.SetActive(bIsActive);
    }
    public bool IsActive()
    {
        return gameObject.activeInHierarchy;
    }
    #endregion


    #region Interface : Position
    public void SetPosition(Vector3 vPos)
    {
        gameObject.transform.position = vPos;
    }
    public void SetPositionX(float fX)
    {
        Vector3 vPos = GetPosition();
        vPos.x = fX;
        SetPosition(vPos);
    }
    public void SetPositionY(float fY)
    {
        Vector3 vPos = GetPosition();
        vPos.y = fY;
        SetPosition(vPos);
    }
    public void SetLocalPosition(Vector3 vPos)
    {
        gameObject.transform.localPosition = vPos;
    }
    public void SetLocalPositionX(float fX)
    {
        Vector3 vPos = GetLocalPosition();
        vPos.x = fX;
        SetLocalPosition(vPos);
    }
    public void SetLocalPositionY(float fY)
    {
        Vector3 vPos = GetLocalPosition();
        vPos.y = fY;
        SetLocalPosition(vPos);
    }
    public void AddLocalPosition(Vector3 vPos)
    {
        gameObject.transform.localPosition = (GetLocalPosition() + vPos);
    }
    public void AddLocalPositionX(float fX)
    {
        Vector3 vPos = GetLocalPosition();
        vPos.x += fX;
        SetLocalPosition(vPos);
    }
    public void AddLocalPositionY(float fY)
    {
        Vector3 vPos = GetLocalPosition();
        vPos.y += fY;
        SetLocalPosition(vPos);
    }
    public Vector3 GetPosition()
    {
        return gameObject.transform.position;
    }
    public Vector3 GetLocalPosition()
    {
        return gameObject.transform.localPosition;
    }
    #endregion


    #region Interface : Scale
    public void SetLocalScale(Vector3 vScale)
    {
        gameObject.transform.localScale = vScale;
    }
    public void SetLocalScaleZ(float fScale)
    {
        Vector3 vScale = GetLocalScale();
        vScale.z = fScale;
        SetLocalScale(vScale);
    }
    public Vector3 GetLocalScale()
    {
        return gameObject.transform.localScale;
    }
    public bool IsZero2Scale()
    {
        Vector3 vScale = GetLocalScale();
        return ((0.0f == vScale.x) && (0.0f == vScale.y));
    }
    #endregion


    #region Interface : Rotate
    public void SetRotate(Vector3 vRotate)
    {
        var pRotation = GetRotate();
        pRotation.eulerAngles = vRotate;
        SetRotate(pRotation);
    }
    public void SetRotate(Quaternion qRotate)
    {
        gameObject.transform.rotation = qRotate;
    }
    public void SetLocalRotate(Vector3 vRotate)
    {
        var pRotation = GetLocalRotate();
        pRotation.eulerAngles = vRotate;
        SetLocalRotate(pRotation);
    }
    public void SetLocalRotate(Quaternion qRotate)
    {
        gameObject.transform.localRotation = qRotate;
    }
    public void SetRotateZ(float fValue)
    {
        Quaternion  qRot = GetLocalRotate();
        Vector3     vRet = qRot.eulerAngles;
        vRet.z = fValue;
        SetLocalRotate(vRet);
    }
    public void AddRotateZ(float fValue)
    {
        SetRotateZ(GetRotateZ() + fValue);
    }
    public float GetRotateZ()
    {
        return GetLocalRotate().eulerAngles.z;
    }
    public void SetRotateX(float fValue)
    {
        Quaternion  qRot = GetLocalRotate();
        Vector3     vRet = qRot.eulerAngles;
        vRet.x = fValue;
        SetLocalRotate(vRet);
    }
    public void AddRotateX(float fValue)
    {
        SetRotateX(GetRotateX() + fValue);
    }
    public float GetRotateX()
    {
        return GetLocalRotate().eulerAngles.x;
    }
    public void SetRotateY(float fValue)
    {
        Quaternion  qRot = GetLocalRotate();
        Vector3     vRet = qRot.eulerAngles;
        vRet.y = fValue;
        SetLocalRotate(vRet);
    }
    public void AddRotateY(float fValue)
    {
        SetRotateY(GetRotateY() + fValue);
    }
    public float GetRotateY()
    {
        return GetLocalRotate().eulerAngles.y;
    }
    public Quaternion GetRotate()
    {
        return gameObject.transform.rotation;
    }
    public Quaternion GetLocalRotate()
    {
        return gameObject.transform.localRotation;
    }
    #endregion


    #region Interface : Animation
    public void PlayAnim(eDirection ePlayDir, GameObject pObject, AnimationClip pClip, Action pEndCallback)
    {
        if (null == pObject)
            pObject = gameObject;

        if (null == pClip)
        {
            if (null != pEndCallback)
                pEndCallback();
            return;
        }

        if (false == pObject.activeInHierarchy)
        {
            if (null != pEndCallback)
                pEndCallback();
            return;
        }

        var pAnim = GetAnimation(pObject);
        if (null == pAnim.GetClip(pClip.name))
            pAnim.AddClip(pClip, pClip.name);

        if (1.0f == Time.timeScale)
        {
            var pState = pAnim[pClip.name];
            pState.normalizedTime = (eDirection.Front == ePlayDir) ? 0.0f :  1.0f;
            pState.speed          = (eDirection.Front == ePlayDir) ? 1.0f : -1.0f;

            pAnim.Stop();
            pAnim.Play(pClip.name);

            if (WrapMode.Loop != pState.wrapMode)
                StartCoroutine(CoroutinePlayAnim_WaitTime(pState.length, pEndCallback));
        }
        else
        {
            switch (ePlayDir)
            {
                case eDirection.Front:
                    StartCoroutine(CoroutinePlayAnim_UnScaledForward(pObject, pAnim[pClip.name], pEndCallback));
                    break;
                case eDirection.Back:
                    StartCoroutine(CoroutinePlayAnim_UnScaledBackward(pObject, pAnim[pClip.name], pEndCallback));
                    break;
            }
        }
    }
    private IEnumerator CoroutinePlayAnim_WaitTime(float fSec, Action pCallback)
    {
        yield return new WaitForSeconds(fSec);

        if (null != pCallback)
            pCallback();
    }
    private IEnumerator CoroutinePlayAnim_UnScaledForward(GameObject pObject, AnimationState pState, Action pEndCallback)
    {
        m_bIsAnimPlaying = true;

        float fStart     = Time.unscaledTime;
        float fElapsed   = 0.0f;
        
        while (true)
        {
            if ((null == pObject) || (null == pState))
                break;
            
            if (false == pObject.activeInHierarchy)
                break;

            fElapsed = Time.unscaledTime - fStart;
            pState.clip.SampleAnimation(pObject, fElapsed);

            if (pState.length <= fElapsed)
            {
                fStart = Time.unscaledTime;
                if (WrapMode.Loop != pState.wrapMode)
                    break;
            }

            yield return null;
        }

        if ((null != pObject) || (null != pState))
            pState.clip.SampleAnimation(pObject, pState.length);

        if (null != pEndCallback)
            pEndCallback();

        m_bIsAnimPlaying = false;
    }
    private IEnumerator CoroutinePlayAnim_UnScaledBackward(GameObject pObject, AnimationState pState, Action pEndCallback)
    {
        m_bIsAnimPlaying = true;

        float fStart    = Time.unscaledTime;
        float fElapsed  = 0.0f;

        while (true)
        {
            if ((null == pObject) || (null == pState))
                break;

            if (false == pObject.activeInHierarchy)
                break;

            fElapsed = pState.length - (Time.unscaledTime - fStart);
            pState.clip.SampleAnimation(pObject, fElapsed);

            if (0.0f >= fElapsed)
            {
                fStart = Time.unscaledTime;
                if (WrapMode.Loop != pState.wrapMode)
                    break;
            }

            yield return null;
        }

        if ((null != pObject) || (null != pState))
            pState.clip.SampleAnimation(pObject, 0.0f);

        if (null != pEndCallback)
            pEndCallback();

        m_bIsAnimPlaying = false;
    }
    #endregion


    #region Utility Functions
    Animation GetAnimation(GameObject pObject = null)
    {
        if (null != m_pAnim)
            return m_pAnim;

        if (null == pObject)
            pObject = gameObject;
        
        return (m_pAnim = SHGameObject.GetComponent<Animation>(pObject));
    }
    #endregion
}