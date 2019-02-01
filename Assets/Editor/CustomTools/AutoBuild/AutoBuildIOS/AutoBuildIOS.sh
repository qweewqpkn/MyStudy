#!/bin/sh

#UNITY程序的路径
UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity

#游戏程序路径
PROJECT_PATH=/Users/juncao/Desktop/qipai/Project

#svn更新c#目录
CSHARP_PATH=/Users/juncao/Desktop/qipai/Project/Assets
 
#svn更新lua目录
LUA_PATH=/Users/juncao/Desktop/qipai/ClientRes/Lua

#XCode工程目录
XCODE_PATH=/Users/juncao/Desktop/qipai/XCode

#ipa输出目录
IPA_PATH=/Users/juncao/.jenkins/workspace/IOS

#export IPA配置目录
EXPORT_IPA_PATH=$1

#配置环境，Release或者Debug
CONFIGURATION="Release"

echo $*

echo "更新代码资源"
#这里的用户名替换成你自己的
svn revert -R $CSHARP_PATH --username xxx --password xxx
svn update $CSHARP_PATH --username xxx --password xxx
svn revert -R $LUA_PATH --username xxx --password xxx
svn update $LUA_PATH --username xxx --password xxx

echo "开始打包xcode工程"
$UNITY_PATH -quit -batchmode -logFile build.log -projectPath $PROJECT_PATH -executeMethod AutoBuild.BuildIOS $*

echo "开始导出ipa"
Cur_Time=$(date "+%Y-%m-%d:%H:%M:%S")
sudo chmod -R 777 $XCODE_PATH
sudo chmod -R 777 $IPA_PATH
xcodebuild -project $XCODE_PATH/Unity-iPhone.xcodeproj -scheme Unity-iPhone -configuration $CONFIGURATION -archivePath $XCODE_PATH/Unity-iPhone-development.xcarchive clean archive build 
xcodebuild -exportArchive -archivePath $XCODE_PATH/Unity-iPhone-development.xcarchive  -exportOptionsPlist $EXPORT_IPA_PATH -exportPath "${IPA_PATH}/${Cur_Time}_doudizhu.ipa"  

echo "ipa生成完毕"