--主入口函数。从这里开始lua逻辑
require "Framework.Define.Global"
require "UI.View.Main.ui_main"
require "UI.View.Loading.ui_loading"
Main = {}

local function Start()
	Logger.Init()
	Logger.Log(Logger.Module.COMMON, "logic start")
	UpdateManager:GetInstance():Startup()
	TimerManager:GetInstance():Startup()

	UIManager:GetInstance():OpenPanel(Consts.UINAME.ui_main)
end

Main.Start = Start
