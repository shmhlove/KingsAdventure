#!/bin/bash

## 스크립트 실행경로 알아내기
ScriptPath=$(dirname $0)

## CopyLibrary를 Library로 복원처리
if [ -d $ScriptPath/CopyLibrary ]
then
    chmod 777 $ScriptPath/Library;
    rm -r $ScriptPath/Library;
    mv $ScriptPath/CopyLibrary $ScriptPath/Library;
    chmod 777 $ScriptPath/Library;
else
    echo “CopyLibrary 디렉토리가 없습니다.”;
fi