#!/bin/bash

## 스크립트 실행경로 알아내기
ScriptPath=$(dirname $0);

## CopyLibrary 디렉토리 제거
if [ -d $ScriptPath/CopyLibrary ]
then
    chmod 777 $ScriptPath/CopyLibrary;
    rm -r $ScriptPath/CopyLibrary;
fi

## Library 디렉토리를 CopyLibrary 디렉토리로 복사
echo “Copy Library Directory”;
cp -r $ScriptPath/Library $ScriptPath/CopyLibrary;
chmod 777 $ScriptPath/CopyLibrary;