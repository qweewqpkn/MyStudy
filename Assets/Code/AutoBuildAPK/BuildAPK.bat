@echo off
echo "build apk start"
set UNITY_PATH="C:\Program Files\Unity2017.2.0f3\Editor\Unity.exe"
set PROJECT_PATH="E:\code\Unity\MyStudy"
%UNITY_PATH% -quit -batchmode -logFile build.log -projectPath %PROJECT_PATH% -executeMethod BuildAPK.Build %*
echo "build apk over"
pause   