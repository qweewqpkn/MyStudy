--主入口函数。从这里开始lua逻辑
require "Framework.Define.Global"
require "UI.View.Main.ui_main"

--local NetMessageMgr = require('NetProtocal.NetMessageMgr');
--local GameGlobalData = require('GameGlobalData');

Main = {}

local function Start()
	Logger.Log(Logger.Module.COMMON, "logic start")
	UpdateManager:GetInstance():Startup()
	TimerManager:GetInstance():Startup()
	--UIManager:GetInstance():OpenPanel("ui_main")
	--NetMessageMgr:GetInstance():InitNet();
	local Test = require "Test.Test"
	Test:Start()
end

Main.Start = Start
