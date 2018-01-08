using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public abstract class SHBaseData
{
    public abstract void OnInitialize();
    public abstract void OnFinalize();
    public abstract void FrameMove();

    public abstract Dictionary<string, SHLoadData> GetLoadList(eSceneType eType);
    public abstract Dictionary<string, SHLoadData> GetPatchList();

    // 로드가 성공하든 실패하든 pDone를 반드시 호출해줘야 한다~!!
    // pDone이 호출되지 않으면 로더가 무한히 기다린다. 
    // 데이터별 로드시간이 천차만별이기에 타임아웃을 넣을 수도 없다.
    // 어떻게 반드시 호출하게 강제 할수가 없나??...
    public abstract IEnumerator Load
    (
        SHLoadData pInfo,                           // 로드 할 데이터 정보
        Action<string, SHLoadStartInfo> pStart,     // 로드 시작시 호출해야 할 콜백
        Action<string, SHLoadEndInfo>   pDone       // 로드 완료시 호출해야 할 콜백
    );

    // 로드가 성공하든 실패하든 pDone를 반드시 호출해줘야 한다~!!
    // pDone이 호출되지 않으면 로더가 무한히 기다린다. 
    // 데이터별 로드시간이 천차만별이기에 타임아웃을 넣을 수도 없다.
    // 어떻게 반드시 호출하게 강제 할수가 없나??...
    public abstract void Patch
    (
        SHLoadData pInfo,                           // 패치 할 데이터 정보
        Action<string, SHLoadStartInfo> pStart,     // 패치 시작시 호출해야 할 콜백
        Action<string, SHLoadEndInfo>   pDone       // 패치 완료시 호출해야 할 콜백
    );
}