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
    --self.mPoolGo = PoolManager:GetInstance():GetPoolGO(self.b_t_items, 10)
--
    --self.newObj1 = self.mPoolGo:Spawn()
    --self.newObj2 = self.mPoolGo:Spawn()
    --self.newObj3 = self.mPoolGo:Spawn()

    for i = 1, 100000 do
        --if(i == 10000001) then
        --    print("输出了")
        --end
        --Debuger.Log("other", "输出了")
        end
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
    Logger.Log(Logger.Module.UI, "ui_main OnBind")
    UIUtil.AddButtonEvent(self, self.b_set, self.OnSet)
    UIUtil.AddButtonEvent(self, self.b_release, self.OnRelease)
    Messenger:GetInstance():Register(MsgEnum.MSG_MAIN_REFRESH, self.OnRefresh, self, "第一个参数哦")
    Messenger:GetInstance():Register(MsgEnum.MSG_MAIN_REFRESH, self.OnRefresh1, self, "第xxx个参数哦")
end

--第一次打开界面调用(一次)
function ui_main: OnInit()
    Logger.Log(Logger.Module.UI, "ui_main OnInit")
end

--显示时调用(可多次)
function ui_main: OnShow(...)
    self.b_text.text = "成功啦我们"
    Logger.Log(Logger.Module.UI, "ui_main OnShow")
end

--隐藏时调用(可多次)
function ui_main: OnHide()
    Logger.Log(Logger.Module.UI, "ui_main OnHide")
end

--关闭时调用(一次)
function ui_main: OnClose()
    Logger.Log(Logger.Module.UI, "ui_main OnClose")
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

local function Register()
    UIManager: GetInstance():RegisterCreateFunc('ui_main', ui_main.New)
end
Register()

return ui_main