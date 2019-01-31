set UnityPath="C:\Program Files\Unity2017.2.0f3\Editor\Unity.exe"
set ProjectPath=E:\code\Unity\MyStudy

echo build package
%UnityPath% -batchmode -quit -projectPath %ProjectPath% -logFile build.log -executeMethod AutoBuildPC.BuildPC %*
echo finish