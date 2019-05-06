--主入口函数。从这里开始lua逻辑
require "Framework.Define.Global"
--local NetMessageMgr = require('NetProtocal.NetMessageMgr');
--local GameGlobalData = require('GameGlobalData');

Main = {}

local function Start()
	Logger.Log(Logger.Module.COMMON, "logic start")
	UpdateManager:GetInstance():Startup()
	TimerManager:GetInstance():Startup()
	--UIManager:GetInstance():OpenPanel("ui_mail")
	--NetMessageMgr:GetInstance():InitNet();
	UIManager:GetInstance():OpenPanel("ui_main")
end

Main.Start = Start
