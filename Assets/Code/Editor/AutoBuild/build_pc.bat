set UnityPath="C:\Program Files\Unity5.6.4p3\Editor\Unity.exe"
set TrankPath=D:\QP_PC\Project
set LuaPath=D:\QP_PC\ClientRes
set OutPath="C:\Program Files (x86)\Jenkins\workspace\棋牌_PC"
set UNITY_LOGFILE="C:\Program Files (x86)\Jenkins\workspace\棋牌_PC\BuildPC.log"

echo 删除美术资源
rd /s/q %TrankPath%\Assets\Art
rd /s/q %TrankPath%\Assets\Atlas

cd %TrankPath%\Assets
svn cleanup
svn revert -R .
svn revert --depth=infinity .
svn update --accept theirs-full
svn resolve --accept theirs-full -R .

cd %TrankPath%\ProjectSettings
svn update

cd %LuaPath%\Lua
svn cleanup
svn revert -R .
svn revert --depth=infinity .
svn update --accept theirs-full
svn resolve --accept theirs-full -R .

cd %LuaPath%\config
svn cleanup
svn revert -R .
svn revert --depth=infinity .
svn update --accept theirs-full
svn resolve --accept theirs-full -R .

echo build package
%UnityPath% -batchmode -quit -nographics -projectPath %TrankPath% -logFile %UNITY_LOGFILE% -executeMethod AutoBuild.BuildPC *output_path-%OutPath% *network-false *log-true *log_file-true

echo finish

::svn add %OutPath%\*
::svn commit %OutPath% -m "提交pc"

::echo "commit pc over"

pause