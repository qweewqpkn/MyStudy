@echo off
::UNITY程序的路径
set UNITY_PATH="C:\Program Files\Unity2017.2.0f3\Editor\Unity.exe"

::游戏程序路径
set PROJECT_PATH="E:\code\Unity\MyStudy"

::svn更新c#目录
set CSHARP_PATH="D:\test\client\Project\Assets"
 
::svn更新lua目录
set LUA_PATH="D:\test\client\ClientRes\Lua" 

echo "更新资源"
svn revert -R %CSHARP_PATH%
svn update %CSHARP_PATH%
svn revert -R %LUA_PATH%
svn update %LUA_PATH%

echo "开始打包apk"
%UNITY_PATH% -quit -batchmode -logFile build.log -projectPath %PROJECT_PATH% -executeMethod AutoBuildAndroid.BuildAndroid %*
echo "打包完成"

pause   