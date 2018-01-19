SET adbpath="C:\Program Files (x86)\Android\android-sdk\platform-tools"
SET bundlename=com.mangogames.kingsadventure

%adbpath%\adb.exe kill-server
%adbpath%\adb.exe start-server
%adbpath%\adb.exe forward tcp:54999 localabstract:Unity-%bundlename%

PAUSE