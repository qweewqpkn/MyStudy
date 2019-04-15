local ui_main = BaseClass("ui_main", UIBase)

--@start 导出的组件列表,使用self.xxx来访问
--local b_image Image
--local b_text Text
--local b_set Button
--local b_t_items Image
--local b_content GridLayoutGroup
--local b_release Button

--b_t_items的导出的子元素
--b_text Text
--@end

function ui_main:OnSet()
    self.mPoolGo = PoolManager:GetInstance():GetPoolGO(self.b_t_items, 10)

    self.newObj1 = self.mPoolGo:Spawn()
    self.newObj2 = self.mPoolGo:Spawn()
    self.newObj3 = self.mPoolGo:Spawn()
end

function ui_main:OnRelease()
    PoolManager:GetInstance():ReleasePoolGO(self.b_t_items)
    SingletonManager:GetInstance():Release()
end

--构造函数
function ui_main:__init(...)
	self.mAbPath = 'ui_main'
    self.mIsDontDestroy = true
end

--绑定事件(一次)
function ui_main: OnBind()
    print("ui_main OnBind")
    UIUtil.AddButtonEvent(self, self.b_set, self.OnSet)
    UIUtil.AddButtonEvent(self, self.b_release, self.OnRelease)
    Messenger:GetInstance():Register(MsgEnum.MSG_MAIN_REFRESH, self.OnRefresh, self, "第一个参数哦")
    Messenger:GetInstance():Register(MsgEnum.MSG_MAIN_REFRESH, self.OnRefresh1, self, "第xxx个参数哦")
end

--第一次打开界面调用(一次)
function ui_main: OnInit()
    print("ui_main OnInit")
end

--显示时调用(可多次)
function ui_main: OnShow(...)

    coroutine.start(function ()
        print("协程开始")
        coroutine.waitforseconds(10)
        print("协程结束")
    end)

    self.b_text.text = "成功啦我们"
    print("ui_main OnShow")
    self.timer = TimerManager:GetInstance():GetTimer(1, function()
        print("Timer is cal")
    end, nil, 1)
    self.timer:Start()

    local co = coroutine.create(function (...)
        print()
    end)
    print("i am here")
    coroutine.resume(co, 45, 4123, "123")
end

--隐藏时调用(可多次)
function ui_main: OnHide()
    print("ui_main OnHide")
end

--关闭时调用(一次)
function ui_main: OnClose()
    print("ui_main OnClose")
end

function ui_main:OnRefresh(...)
    for k,v in pairs({...}) do
        print(v)
    end
end

function ui_main:OnRefresh1(...)
    for k,v in pairs({...}) do
        print(v)
    end
end

local function Register()
    UIManager: GetInstance():RegisterCreateFunc('ui_main', ui_main.New)
end
Register()

return ui_main