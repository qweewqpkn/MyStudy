--主入口函数。从这里开始lua逻辑
require "Framework.Define.Global"
Main = {}

local ui_main = require "UI.View.ui_main"

local function Start()					
	print("logic start")	
	UIManager:GetInstance():OpenPanel()
	SceneManager:GetInstance():SwitchScene("123", "ggggg", "xxxx123")
	UIManager:GetInstance():OpenPanel()
	SceneManager:GetInstance():SwitchScene("xxx", "xxxx123gg")

	local ui_main_new = ui_main.new()
	ui_main_new:OnShow()
	ui_main_new:OnClose()
	ui_main_new:OnHide()
end

Main.Start = Start
