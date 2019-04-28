local ui_loading = BaseClass("ui_loading", UIBase)

--@start 导出的组件列表,使用self.xxx来访问
--local b_content Text
--local b_Slider Slider
--local b_progress Text
--@end

--构造函数
function ui_loading:__init(...)
	self.mAbPath = 'ui_loading'
end

--绑定事件(一次)
function ui_loading:OnBind()
    Messenger:GetInstance():Register(MsgEnum.ui_loading_refresh, self.OnRefresh, self)
end

--第一次打开界面调用(一次)
function ui_loading:OnInit()

end

--显示时调用(可多次)
function ui_loading:OnShow(...)
    Logger.Log(Logger.Module.UI, "ui_loading OnShow")
    self.b_content.text = ""
    self.b_progress.text = ""
    self.b_Slider.value = 0.0
end

--隐藏时调用(可多次)
function ui_loading:OnHide()

end

--关闭时调用(一次)
function ui_loading:OnClose()

end

function ui_loading:OnRefresh(...)
    Logger.Log(Logger.Module.UI, "ui_loading OnRefresh")
    local args = SafePack(...)
    self.b_content.text = tostring(args[1])
    self.b_progress.text = tostring(args[2])
end

local function Register()
    UIManager: GetInstance():RegisterCreateFunc('ui_loading', ui_loading.New)
end
Register()

return ui_loading