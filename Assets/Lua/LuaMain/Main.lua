--主入口函数。从这里开始lua逻辑
require "Framework.Define.Global"
require "UI.View.Main.ui_main"
Main = {}

local function Start()
	Logger.Log(Logger.Module.COMMON, "logic start")
	UpdateManager:GetInstance():Startup()
	TimerManager:GetInstance():Startup()

	UIManager:GetInstance():OpenPanel(Consts.UINAME.ui_main)

	local co = coroutine.create(function()
		Logger.Log(Logger.Module.COMMON, "协程启动")
		local co1 = coroutine.create(function()
			Logger.Log(Logger.Module.COMMON, "子协程启动")
			for i = 1, 5 do
				Logger.Log(Logger.Module.COMMON, "子协程数据 : " .. i)
			end
			coroutine.yield("子协程返回")
			Logger.Log(Logger.Module.COMMON, "子协程结束")
		end)
		print(coroutine.resume(co1))
		Logger.Log(Logger.Module.COMMON, "协程结束")
		return "协程结束返回数据了哦", 100
	end)
	print(coroutine.resume(co))
	Logger.Log(Logger.Module.COMMON, "主线程继续执行了哦")
end

Main.Start = Start
