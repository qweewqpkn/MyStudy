local UIBase = BaseClass("UIBase")

function UIBase:__init(...)
    self.mAbPath = "" --ab资源路径
    self.mUIName = "" --界面名字
    self.mIsFullScreen = true --是否是全屏界面,全屏界面会隐藏之前的界面
    self.mIsStack = false --是否进栈界面,为了支持返回时能返回之前的界面
    self.mIsMainUI = false --是否是主界面
    self.mIsDontDestroy = false --关闭界面是否要销毁掉还是隐藏
    self.mPanelState = 0 --0未打开 1加载ab中 2打开完成 3隐藏 4等待销毁(由于界面未加载完成，但是却销毁了该界面) 5等待隐藏(由于界面未加载完成，但是却隐藏了该界面)
    self.mPanelData = nil --界面缓存外部传入的数据
    --self.mIsRestore = false --战斗返回需要恢复的界面
    self.mUseSelfSorting = false --是否使用自带层级
    self.mUseLayer = 1 --界面使用了多少层级，默认为1
    self.mSortingOrder = 0 --界面使用的层级
end

function UIBase:OpenPanel(...)
    self.mPanelData = SafePack(...)

    if self.mPanelState == 0 then
        self.mPanelState = 1
        Logger.Log(Logger.Module.UI, "UIManager OpenPanel : " .. self.mAbPath)
        --加载界面资源
        ResourceManager.Instance:LoadPrefabAsync(self.mAbPath, self.mAbPath, function(obj)
            if(IsNull(obj)) then
                Logger.LogError(Logger.Module.UI, string.format("加载界面失败, 界面名字: %s", self.mAbPath))
                return
            end

            --加载完成后，界面被标记为销毁
            if self.mPanelState == 4 then
                Logger.Log(Logger.Module.UI, string.format("加载界面完成后, 立刻就被销毁了, 界面名字: %s", self.mAbPath))
                self:ClosePanel()
                return
            end

            --把界面加载指定层级
            obj.transform:SetParent(UIManager:GetInstance().mUIRoot, false)
            --绑定ui的控件
            CS.UIComponentBind.BindToLua(obj, self)
            --绑定
            self:OnBind()
            --初始化
            self:OnInit(SafeUnpack(self.mPanelData))
            --加载完成后，界面被标记为隐藏
            if self.mPanelState == 5 then
                Logger.Log(Logger.Module.UI, string.format("加载界面完成后, 立刻就被隐藏了, 界面名字: %s", self.mAbPath))
                self:ClosePanel()
            else
                self:ShowPanel(SafeUnpack(self.mPanelData))
                self:EnterStack()
            end
        end)
    elseif self.mPanelState == 2 then
        Logger.LogError(Logger.Module.UI, "界面已经打开了, 请勿重复打开, 检查逻辑!")
    elseif self.mPanelState == 3 then
        self:ShowPanel(...)
        self:EnterStack()
    elseif self.mPanelState == 4 then
        self.mPanelState = 1
    elseif self.mPanelState == 5 then
        self.mPanelState = 1
    end
end

--进栈　
function UIBase:EnterStack()
    if self.mIsStack then
        if(self.mIsFullScreen)then
            local view = UIManager:GetInstance().mViewStack:Peek()
            if view ~= nil then
                view:HidePanel()
            end
        end
        UIManager:GetInstance().mViewStack:Push(self)

        --如果有
        if UIManager:GetInstance():IsStackHaveFullScreen() then
            if UIManager:GetInstance().mMainUI ~= nil and UIManager:GetInstance().mMainUI.mPanelState == 2 then
                UIManager:GetInstance().mMainUI:HidePanel()
            end
        end
    end
end

--离栈
function UIBase:ExitStack()
    Logger.Log(Logger.Module.UI, "ExitStack call : " .. self.mUIName)
    if self.mIsStack and self.mPanelState == 2 then
        local viewStack = UIManager:GetInstance().mViewStack
        local view = viewStack:Peek()
        if view ~= nil then
            if view.mUIName == self.mUIName then
                viewStack:Pop()
                if viewStack:Count() > 0 then
                    view = viewStack:Peek()
                    view:ShowPanel()
                end
            else
                while true do
                    view = viewStack:Pop()
                    if view == nil or view.mUIName == self.mUIName then
                        break
                    end
                end

                view = viewStack:Peek()
                if view ~= nil then
                    view:ShowPanel()
                end
            end
        end
    end

    if not UIManager:GetInstance():IsStackHaveFullScreen() then
        if UIManager:GetInstance().mMainUI ~= nil and UIManager:GetInstance().mMainUI.mPanelState == 3 then
            UIManager:GetInstance().mMainUI:ShowPanel(UIManager:GetInstance().mMainUI.mPanelData)
        end
    end
end

--显示界面
function UIBase:ShowPanel(...)
    if not IsNull(self.gameObject) then
        self.mPanelState = 2
        self.gameObject:SetActive(true)
        self:AnimationIn()
        self:SetCanvas()
        self:OnShow(...)
    end
end

--隐藏界面
function UIBase:HidePanel()
    if not IsNull(self.gameObject) then
        self.mPanelState = 3
        self.gameObject:SetActive(false)
        self:OnHide()
    else
        self.mPanelState = 5
    end
end

--真正关闭界面
function UIBase:RealClosePanel()
    if not IsNull(self.gameObject) then
        self.mPanelState = 0
        CS.UnityEngine.Object.Destroy(self.gameObject)
        self.gameObject = nil
        self:OnClose()
        Messenger:GetInstance():RemoveByTarget(self)
        UIManager:GetInstance():RemoveViewList(self.mUIName)
    else
        self.mPanelState = 4
    end
end

--关闭页面
function UIBase:ClosePanel()
    if(self.mPanelState == 1) then
        --界面加载中
        if self.mIsDontDestroy then
            self.mPanelState = 5
        else
            self.mPanelState = 4
        end
    elseif(self.mPanelState == 2 or self.mPanelState == 3) then
        --界面加载完成了
        if self.mIsDontDestroy then
            self:HidePanel()
        else
            self:AnimationOut()
        end
    elseif(self.mPanelState == 4) then
        --界面销毁中
        self:RealClosePanel()
    elseif(self.mPanelState == 5) then
        --界面标记为隐藏
        self:HidePanel()
    end
end

--是否界面被销毁
function UIBase:IsDestroy()
    if(self == nil) then
        return true
    else
        if(self.mPanelState == 0) then
            return true
        end

        return false
    end
end

--设置canvas层级
function UIBase:SetCanvas()
    if(self.mUseSelfSorting)then
        return
    end
    local canvas = self.gameObject:GetComponent( "Canvas")
    if(canvas == nil)then
        return
    end
    if self.mIsMainUI then
        UIManager:GetInstance().mSortingOrder = 0 --打开主界面，那么重置界面order
    end
    self.mSortingOrder =  UIManager:GetInstance().mSortingOrder
    UIManager:GetInstance().mSortingOrder =  UIManager:GetInstance().mSortingOrder + self.mUseLayer
    canvas.overrideSorting = true
    canvas.sortingOrder = self.mSortingOrder
end

--绑定各种事情(为了让子类重写)
function UIBase:OnBind()

end

--界面初始化调用(为了让子类重写)
function UIBase:OnInit()

end

--界面显示调用(为了让子类重写)
function UIBase:OnShow(...)

end

--界面隐藏调用(为了让子类重写)
function UIBase:OnHide()

end

--界面关闭调用(为了让子类重写)
function UIBase:OnClose()

end

--断线的时候调用(为了让子类重写)
function UIBase:OnDisconnect()

end

--重连成功调用(为了让子类重写)
function UIBase:OnReconnect()

end

function UIBase:AnimationIn()
    local t_animation = self.gameObject:GetComponent("Animation")
    if(IsNull(t_animation))then
        return
    else
        Logger.Log(Logger.Module.UI, t_animation)
    end
    local animation_name = "ui_anim_"..string.lower(self.mUIName)
    local t_animation_clip = t_animation:GetClip(animation_name)

    if(t_animation_clip == nil)then
        animation_name = "ui_anim_shop"
        t_animation_clip = t_animation:GetClip(animation_name)--默认是这个
    end

    if(t_animation_clip == nil)then
        return
    end
    local t_aniamtion_state = t_animation:get_Item(animation_name)
    Logger.Log(Logger.Module.UI, "#################### t_aniamtion_state.name="..t_aniamtion_state.name)
    t_aniamtion_state.speed = 1
    t_animation:Play(animation_name)
end

function UIBase:AnimationOut()
    local t_animation = self.gameObject:GetComponent("Animation")
    if(IsNull(t_animation))then
        self:RealClosePanel()
        return
    end

    local animation_name = "ui_anim_"..string.lower(self.mUIName)
    local animation_name2 = animation_name.."2"

    local t_aniamtion_state = nil
    --找第二个动画
    local t_animation_clip2 = t_animation:GetClip(animation_name2)
    if(t_animation_clip2 == nil)then
        --没有。找第一个动画
        local t_animation_clip1 = t_animation:GetClip(animation_name)

        if(t_animation_clip1 == nil)then
            animation_name = "ui_anim_shop"
            t_animation_clip1 = t_animation:GetClip(animation_name)--默认是这个
        end

        if(t_animation_clip1 == nil)then
            --还是没有。退出
            self:RealClosePanel()
            return
        else
            --倒播第一个动画
            t_aniamtion_state = t_animation:get_Item(animation_name)
            t_aniamtion_state.speed = -1
            t_aniamtion_state.time = t_aniamtion_state.length
            t_animation:Play(animation_name)

        end
    else
        --直接播第二个动画
        t_aniamtion_state.speed = 1
        t_animation:Play(animation_name2)
    end

    Logger.Log(Logger.Module.UI, "#################################################wait panel animation")
    self.m_timer = Timer.New(function (self)
        Logger.Log(Logger.Module.UI, "#################################################real close")
        self:RealClosePanel()
        self.m_timer:Stop()
        self.m_timer = nil
    end, t_aniamtion_state.length, 1, true, self)
    self.m_timer:Start()
end

return UIBase