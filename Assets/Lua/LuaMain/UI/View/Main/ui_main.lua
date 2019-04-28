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
    ResourceManager.Instance:PreLoadPrefabAsync("ui_loading", "ui_loading", function()
        Logger.Log(Logger.Module.COMMON, "预加载完成")
        SceneManager:GetInstance():SwitchScene(SceneConfig.LoginScene)
    end)
end

function ui_main:OnRelease()

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
    coroutine.start(function ()
        Logger.Log(Logger.Module.UI, "协程开始")
        coroutine.waitforseconds(1)
        Logger.Log(Logger.Module.UI, "协程结束")

        Logger.Log(Logger.Module.UI, "当前" .. CS.UnityEngine.Time.frameCount)
        coroutine.waitforframes(5)
        Logger.Log(Logger.Module.UI, "然后" .. CS.UnityEngine.Time.frameCount)
        coroutine.waitforendofframe()
        Logger.Log(Logger.Module.UI, "然后" .. CS.UnityEngine.Time.frameCount)

        local i = 0
        coroutine.waituntil(function()
            Logger.Log(Logger.Module.UI, i)
            i = i + 1
            if(i == 1) then
                return true
            end

            return false
        end)

        local request = ResourceManager.Instance:LoadPrefabAsync("cube", "cube")
        coroutine.waitforasyncop(request, function()
            --Logger.Log(Logger.Module.UI, "异步加载中.....")
        end )
        local obj = request.Asset
        obj.name = "测试啦"
        Logger.Log(Logger.Module.UI, "结果来了" .. CS.UnityEngine.Time.frameCount)
        end)


    self.b_text.text = "成功啦我们"
    Logger.Log(Logger.Module.UI, "ui_main OnShow")
    self.timer = TimerManager:GetInstance():GetTimer(1, function()
       Logger.Log(Logger.Module.UI, "Timer is cal")
    end, nil, 1)
    self.timer:Start()
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