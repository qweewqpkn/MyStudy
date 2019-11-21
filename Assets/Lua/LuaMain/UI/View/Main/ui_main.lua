local ui_main = BaseClass("ui_main", UIBase)

--@start 导出的组件列表,使用self.xxx来访问
--local b_image Image
--local b_text TextMeshProUGUI
--local b_set Button
--local b_t_items Image
--local b_release Button
--local b_loop_rect LoopVerticalScrollRect

--b_t_items的导出的子元素
--b_text TextMeshProUGUI
--@end

function ui_main:OnSet()

end

function ui_main:OnD1(a, b)

end

function ui_main:OnRelease()
    Logger.LogError(Logger.Module.UI, self.OnD1)
    CS.TestLuaDelegate.TestD1(10, self.OnD1)
end

--构造函数
function ui_main:__init(...)
    local testPoolType = require "Framework.Test.TestPoolManager"
    self.testPoolObj = testPoolType.New()
	self.mAbPath = 'ui_main'
    self.mIsDontDestroy = true
    self.mIsMainUI = true
end

--绑定事件(一次)
function ui_main: OnBind()
    Logger.Log(Logger.Module.UI, "ui_main OnBind")
    UIUtil.AddButtonEvent(self.b_set, self.OnSet, self)
    UIUtil.AddButtonEvent(self.b_release, self.OnRelease, self)
    Messenger:GetInstance():Register(MsgEnum.MSG_MAIN_REFRESH, self.OnRefresh, self, "第一个参数哦")
    Messenger:GetInstance():Register(MsgEnum.MSG_MAIN_REFRESH, self.OnRefresh1, self, "第xxx个参数哦")
end

--第一次打开界面调用(一次)
function ui_main: OnInit()
end

--显示时调用(可多次)
function ui_main: OnShow(...)
    self.b_text.text = "成功啦我们"
    self.b_loop_rect:Init(20, BindCallback(self, self.OnNotifyElement))
end

function ui_main:OnNotifyElement(t, index)
    t.b_text.text = index
    Logger.Log(Logger.Module.COMMON, "gege" .. index)
end

--隐藏时调用(可多次)
function ui_main: OnHide()
end

--关闭时调用(一次)
function ui_main: OnClose()
end

function ui_main:OnRefresh(...)
    for k,v in pairs({...}) do
        Logger.Log(Logger.Module.UI, v)
    end
end

function ui_main:OnRefresh1(...)
    for k,v in pairs({...}) do
        Logger.Log(Logger.Module.UI, v)
    end
end

return ui_main