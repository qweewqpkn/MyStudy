@echo off
::jenkins的内容
:://D:\QP_Android\client\Project\Assets\Editor\BuildPackage.bat "C:\Program Files\Unity5.6.4p3\Editor\Unity.exe" "D:\QP_Android\client\Project" "D:\QP_Android\client\Project\Assets" 
:://"D:\QP_Android\client\ClientRes\Lua" #apk-"C:\Program Files (x86)\Jenkins\workspace\棋牌_Android" #log-%OpenLog% #log_file-%LogToFile%  #network-%Internet%
::%1代码unity.exe的路径
::%2工程路径
::%3svn更新c#路径
::%4svn更新lua路径

echo "build apk start"
::svn revert -R %3
::svn update %3
::svn revert -R %4
::svn update %4
%1 -quit -batchmode -logFile build.log -projectPath %2 -executeMethod BuildPackage.Build %*
echo "build apk over"
pause   