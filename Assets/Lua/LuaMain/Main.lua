--主入口函数。从这里开始lua逻辑
require "Framework.Define.Global"
require "UI.View.Main.ui_main"
Main = {}

local function Start()
	Logger.Log(Logger.Module.COMMON, "logic start")
	UpdateManager:GetInstance():Startup()
	TimerManager:GetInstance():Startup()

	UIManager:GetInstance():OpenPanel("ui_main")

	Logger.Log(Logger.Module.COMMON, "Log")
	Logger.LogWarning(Logger.Module.COMMON, "LogWarning")
	Logger.LogError(Logger.Module.COMMON, "LogError")
end

Main.Start = Start
