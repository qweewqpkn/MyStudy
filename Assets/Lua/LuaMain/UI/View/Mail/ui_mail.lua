local ui_mail = BaseClass("ui_mail", UIBase)

--@start 导出的组件列表,使用self.xxx来访问
--local b_back Button
--local b_content Text
--@end

--构造函数
function ui_mail:__init(...)
	self.mAbPath = 'ui_mail'
    self.mIsStack = true
end

function ui_mail:OnBack()
    UIManager:GetInstance():ClosePanel(UIConfig.ui_mail.name)
end

--绑定事件(一次)
function ui_mail:OnBind()
    UIUtil.AddButtonEvent(self, self.b_back, self.OnBack)
end

--第一次打开界面调用(一次)
function ui_mail:OnInit()

end

--显示时调用(可多次)
function ui_mail:OnShow(...)
    
end

--隐藏时调用(可多次)
function ui_mail:OnHide()

end

--关闭时调用(一次)
function ui_mail:OnClose()

end

return ui_mail