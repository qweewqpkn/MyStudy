local ui_test = BaseClass("ui_test", UIBase)

--@start 导出的组件列表,使用self.xxx来访问
--local b_back Button
--local b_content Text
--@end

--构造函数
function ui_test:__init(...)
	self.mAbPath = 'ui_test'
    self.mIsStack = true
    self.mUseLayer = 5
    self.mIsDontDestroy = true
end

function ui_test:OnBack()
    UIManager:GetInstance():ClosePanel(UIConfig.ui_test.name)
end

--绑定事件(一次)
function ui_test:OnBind()
    UIUtil.AddButtonEvent(self, self.b_back, self.OnBack)
end

--第一次打开界面调用(一次)
function ui_test:OnInit()

end

--显示时调用(可多次)
function ui_test:OnShow(...)
    self.b_content.text = "sdafa"
    
end

--隐藏时调用(可多次)
function ui_test:OnHide()

end

--关闭时调用(一次)
function ui_test:OnClose()

end

return ui_test