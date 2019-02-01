set UnityPath="C:\Program Files\Unity2017.2.0f3\Editor\Unity.exe"
set ProjectPath=E:\code\Unity\MyStudy

echo "开始打包PC"
%UnityPath% -batchmode -quit -projectPath %ProjectPath% -logFile build.log -executeMethod AutoBuildPC.BuildPC %*
echo "打包完成"