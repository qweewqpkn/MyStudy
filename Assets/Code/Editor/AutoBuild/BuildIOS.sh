#!/bin/sh

#UNITY程序的路径#
UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity

#游戏程序路径#
PROJECT_PATH=/Users/juncao/Desktop/qipai/Project

#svn更新c#目录#
CSHARP_PATH=/Users/juncao/Desktop/qipai/Project/Assets
 
#svn更新lua目录#
LUA_PATH=/Users/juncao/Desktop/qipai/ClientRes/Lua

echo "更新代码资源"

svn revert -R $CSHARP_PATH --username liheng --password liheng
svn update $CSHARP_PATH --username liheng --password liheng
svn revert -R $LUA_PATH --username liheng --password liheng
svn update $LUA_PATH --username liheng --password liheng

echo "开始打包xcode工程"
$UNITY_PATH -quit -batchmode -logFile build.log -projectPath $PROJECT_PATH -executeMethod AutoBuild.BuildIOS $*
echo "XCODE工程生成完毕"

echo "开始导出ipa"
Project_Name="/Users/juncao/Desktop/qipai/XCode"
#配置环境，Release或者Debug
Configuration="Debug"
Output_Path="/Users/juncao/.jenkins/workspace/IOS"
PList_Path="/Users/juncao/Desktop/qipai/QipaiCode/ExportOptions.plist"
Cur_Time=$(date "+%Y-%m-%d:%H:%M:%S")

sudo chmod -R 777 $Project_Name
sudo chmod -R 777 $Output_Path
#xcodebuild clean -xcodeproj ${Project_Name}/Unity-iPhone.xcodeproj -configuration $Configuration -alltargets

#appstore脚本
xcodebuild -project $Project_Name/Unity-iPhone.xcodeproj -scheme Unity-iPhone -configuration $Configuration -archivePath $Project_Name/Unity-iPhone-development.xcarchive clean archive build 
xcodebuild -exportArchive -archivePath $Project_Name/Unity-iPhone-development.xcarchive  -exportOptionsPlist $PList_Path -exportPath "${Output_Path}/${Cur_Time}_doudizhu.ipa"  

 
echo "ipa生成完毕"