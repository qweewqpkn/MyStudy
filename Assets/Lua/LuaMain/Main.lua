--主入口函数。从这里开始lua逻辑
require "Framework.Define.Global"
require "UI.View.Main.ui_main"
Main = {}

local function Start()					
	print("logic start")
	UpdateManager:GetInstance():Startup()
	TimerManager:GetInstance():Startup()

	UIManager:GetInstance():OpenPanel("ui_main")

	local info = debug.getinfo(2, "S")
	Logger.Log("info is : " .. info.what)

end

Main.Start = Start
