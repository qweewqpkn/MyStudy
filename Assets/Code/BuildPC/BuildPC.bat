@echo off
::%1代码unity.exe的路径
::%2工程路径
::%3svn更新c#路径
::%4svn更新lua路径
echo "build pc start"
::svn revert -R %3
::svn update %3
::svn revert -R %4
::svn update %4
%1 -quit -batchmode -logFile build.log -projectPath %2 -executeMethod BuildPC.Build %*
echo "build pc over"
pause   