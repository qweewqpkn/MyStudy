--主入口函数。从这里开始lua逻辑
require "Framework.Define.Global"
--local NetMessageMgr = require('NetProtocal.NetMessageMgr');
--local GameGlobalData = require('GameGlobalData');

Main = {}

local function Start()
	Logger.Log(Logger.Module.COMMON, "logic start")
	UpdateManager:GetInstance():Startup()
	TimerManager:GetInstance():Startup()
	UIManager:GetInstance():OpenPanel(UIConfig.ui_main.name)
end

Main.Start = Start
