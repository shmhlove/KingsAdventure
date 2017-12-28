< 프로파일링 디바이스 연결 >

1. PC => 안드로이드 SDK 설치

2. PC => 빌드 옵션에 DevelopmentBuild, Autoconnect Profiler체크 후 빌드

3. 디바이스 => 빌드된 apk 설치 후 PC에 USB 연결

4. PC => ADBSetup.bat 실행 (ADB 서비스 재시작 후 포워드 명령)
         **파일을 메모장으로 열어서 adbpath와 bundlename 각자 컴퓨터에 맞게 수정필요
         
5. 디바이스 => 어플실행

6. 유니티 프로파일링 윈도우에 Active Profiler를 AndroidPlayer(ADB@127.0.0.1:54999) 선택