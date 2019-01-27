@echo off
::%1代码unity.exe的路径
::%2工程路径
::%3svn更新c#路径
::%4svn更新lua路径
::%6svn上传Apk路径
echo "build apk start"

echo 删除美术资源
set TrankPath=D:\QP_Android\client\Project
rd /s/q %TrankPath%\Assets\Art
rd /s/q %TrankPath%\Assets\Atlas

svn revert -R %3
svn update %3
svn revert -R %4
svn update %4

set config_path=D:\QP_Android\client\ClientRes\config
svn revert -R %config_path%
svn update %config_path%

%1 -quit -batchmode -logFile build.log -projectPath %2 -executeMethod AutoBuild.BuildAPK %*
echo "build apk over"
::if %6 == true (
::	svn add %5\*
::	svn commit %5 -m "apk"	
::)
::echo "commit apk over"
pause   