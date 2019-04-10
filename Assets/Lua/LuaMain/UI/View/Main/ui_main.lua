local ui_main = BaseClass("ui_main", UIBase)

--@start 导出的组件列表,使用self.xxx来访问
--local b_image Image
--local b_text Text
--local b_set Button
--local b_t_items Image

--b_t_items的导出的子元素
--b_text Text
--@end

function ui_main:OnSet(btn, ...)
    print(...)
    UIManager:GetInstance():ClosePanel("ui_main")
end

--构造函数
function ui_main:__init(...)
	self.mAbPath = 'ui_main'
end

--绑定事件(一次)
function ui_main: OnBind()
    print("ui_main OnBind")
    UIUtil.AddButtonEvent(self, self.b_set, self.OnSet)
end

--第一次打开界面调用(一次)
function ui_main: OnInit()
    print("ui_main OnInit")
end

--显示时调用(可多次)
function ui_main: OnShow(...)
    self.b_text.text = "成功啦我们"
    print("ui_main OnShow")
end

--隐藏时调用(可多次)
function ui_main: OnHide()
    print("ui_main OnHide")
end

--关闭时调用(一次)
function ui_main: OnClose()
    print("ui_main OnClose")
end

local function Register()
    UIManager: GetInstance():RegisterCreateFunc('ui_main', ui_main.New)
end
Register()

return ui_main