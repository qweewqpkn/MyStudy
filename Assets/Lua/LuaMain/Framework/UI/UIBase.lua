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

    self.mHideWildMap = false
end

function UIBase:OpenPanel(...)
    self.mPanelData = SafePack(...)

    if self.mPanelState == 0 then
        self.mPanelState = 1
        Logger.Log(Logger.Module.UI, "UIManager OpenPanel : " .. self.mAbPath)
        self:OnPreInit()
        --加载界面资源
        UIManager:GetInstance():ShowMask(true)
        ResourceManager.instance:LoadPrefabAsync(self.mAbPath, self.mAbPath, function(obj)
            UIManager:GetInstance():ShowMask(false)
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
            --初始化
            self:OnInit(SafeUnpack(self.mPanelData))
            --加载完成后，界面被标记为隐藏
            if self.mPanelState == 5 then
                Logger.Log(Logger.Module.UI, string.format("加载界面完成后, 立刻就被隐藏了, 界面名字: %s", self.mAbPath))
                self:ClosePanel()
            else
                self:EnterStack()
                self:ShowPanel(SafeUnpack(self.mPanelData))
            end

            self:GuideOnOpen()
        end)
    elseif self.mPanelState == 2 then
        --关闭界面 播放动画的过程中，又来了打开消息
        if(self.m_animation_out_timer ~= nil)then
            self.m_animation_out_timer:Stop()
            self.m_animation_out_timer = nil
        end
        Messenger:GetInstance():RemoveByTarget(self)
        self:EnterStack()
        self:ShowPanel(...)
    elseif self.mPanelState == 3 then
        self:EnterStack()
        self:ShowPanel(...)
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
                Logger.Log(Logger.Module.UI, "111 ExitStack call : " .. self.mUIName)
                view:ClosePanel()
                viewStack:Pop()
                if viewStack:Count() > 0 then
                    view = viewStack:Peek()
                    view:ShowPanel(SafeUnpack(view.mPanelData))
                end
            else
                while true do
                    view = viewStack:Pop()
                    if view == nil or view.mUIName == self.mUIName then
                        break
                    else
                        Logger.Log(Logger.Module.UI, "222 ExitStack call : " .. self.mUIName)
                        view:ClosePanel()
                    end
                end

                view = viewStack:Peek()
                if view ~= nil then
                    view:ShowPanel(SafeUnpack(view.mPanelData))
                end
            end
        end
    else
        Logger.Log(Logger.Module.UI, "333 ExitStack call : " .. self.mUIName)
        self:ClosePanel()
    end

    UIManager:GetInstance():ShowMainUI()
end

--显示界面
function UIBase:ShowPanel(...)
    if not IsNull(self.gameObject) then
        self.mPanelState = 2
        self:OnBind()
        self.gameObject.transform.anchoredPosition = Vector2(0, 0)
        GoUtil.SetActive(self.gameObject, true)
        self:AnimationIn()
        self:SetCanvas()
        self:OnShow(...)
    end
end

--真正隐藏界面
function UIBase:RealHidePanel()
    self.gameObject.transform.anchoredPosition = Vector2(5000, 0)
    UIManager:GetInstance():EnableWildMapCam(self, true)
end

--隐藏界面
function UIBase:HidePanel()
    if not IsNull(self.gameObject) then
        self.mPanelState = 3
        Messenger:GetInstance():RemoveByTarget(self)
        self:AnimationOut("hide")
        self:OnHide()
    else
        self.mPanelState = 5
    end
end

--真正关闭界面
function UIBase:RealClosePanel()
    if not IsNull(self.gameObject) then
        UIManager:GetInstance():EnableWildMapCam(self, true)

        self.mPanelState = 0
        CS.UnityEngine.Object.Destroy(self.gameObject)
        self.gameObject = nil
        Messenger:GetInstance():RemoveByTarget(self)
        UIManager:GetInstance():RemoveViewList(self.mUIName)
        --引导需要检测UI被关闭
        Messenger:GetInstance():Broadcast(MsgEnum.GUIDE_CLOSE_UI_FINISH, self.mUIName)
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
    elseif(self.mPanelState == 2) then
        --界面加载完成了
        if self.mIsDontDestroy then
            self:HidePanel()
        else
            Messenger:GetInstance():RemoveByTarget(self)
            self:OnClose()
            self:AnimationOut("close")
        end
    elseif(self.mPanelState == 3) then
        if self.mIsDontDestroy then

        else
            self:OnClose()
            self:RealClosePanel()
        end
    elseif(self.mPanelState == 4) then
        --界面销毁中
        self:OnClose()
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
        --打开主界面，那么重置界面order
        UIManager:GetInstance().mSortingOrder = 0
    end
    self.mSortingOrder = UIManager:GetInstance().mSortingOrder
    UIManager:GetInstance().mSortingOrder = UIManager:GetInstance():GetMaxSortingOrder() + self.mUseLayer
    canvas.overrideSorting = true
    canvas.sortingOrder = self.mSortingOrder
end

--绑定各种事情(为了让子类重写)
function UIBase:OnBind()

end

--界面预初始(调用一次,在加载界面prefab之前)
function UIBase:OnPreInit()

end

--界面初始化调用(为了让子类重写)
function UIBase:OnInit()

end

--界面显示调用(为了让子类重写)
function UIBase:OnShow(...)

end

--在打开动画之后调用
function UIBase:OnShowAfterAnimation()
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

-----------------------------------------打开关闭动画-----------------------------------------
function UIBase:AnimationIn()
    --关闭中来了打开消息
    if(self.m_animation_out_timer ~= nil)then
        self.m_animation_out_timer:Stop()
        self.m_animation_out_timer = nil
    end

    --进入动画
    local t_animation = self.gameObject:GetComponent("Animation")
    if(IsNull(t_animation))then
        self:OnAnimationInEnd()
        return
    end
    local nameList = Utility.GetAnimations(t_animation)
    if(nameList.Count == 0)then
        self:OnAnimationInEnd()
        return
    end
    local animation_name = nil
    for i = 1, nameList.Count do
        if(string.contains(nameList[i - 1], "_appear", false))then
            animation_name = nameList[i - 1]
        end
    end
    if(animation_name == nil)then
        self:OnAnimationInEnd()
        return
    end
    local t_animation_clip = t_animation:GetClip(animation_name)
    if(IsNull(t_animation_clip))then
        self:OnAnimationInEnd()
        return
    end
    t_animation:Play(animation_name)

    --动画结束回调
    if(self.m_animation_in_timer == nil)then
        self.m_animation_in_timer = TimerManager:GetInstance():GetTimer(t_animation.clip.length,
                self.OnAnimationInEnd, self, 1, false, false)
        self.m_animation_in_timer:Start()
    else
        --不处理
    end
end

function UIBase:OnAnimationInEnd()
    if(self.m_animation_in_timer ~= nil)then
        self.m_animation_in_timer:Stop()
        self.m_animation_in_timer = nil
    end
    --引导需要检测UI被打开
    Messenger:GetInstance():Broadcast(MsgEnum.GUIDE_ANIMATION_IN_UI_FINISH, self.mUIName)
    --通知界面
    self:OnShowAfterAnimation()

    UIManager:GetInstance():EnableWildMapCam(self, false)
end

function UIBase:AnimationOut(state)
    UIManager:GetInstance():EnableWildMapCam(self, true)

    local t_animation = self.gameObject:GetComponent("Animation")
    if(IsNull(t_animation))then
        self:OnAnimationOutEnd(state)
        return
    end
    local nameList = Utility.GetAnimations(t_animation)
    if(nameList.Count == 0)then
        self:OnAnimationOutEnd(state)
        return
    end
    local animation_name = nil
    for i = 1, nameList.Count do
        if(string.contains(nameList[i - 1], "_disappear", false))then
            animation_name = nameList[i - 1]
        end
    end
    if(animation_name == nil)then
        self:OnAnimationOutEnd(state)
        return
    end

    local t_animation_clip = t_animation:GetClip(animation_name)
    if(IsNull(t_animation_clip))then
        self:OnAnimationOutEnd(state)
        return
    end

    if(self.m_animation_out_timer == nil)then
        t_animation:Play(animation_name)
        self.m_animation_out_timer = TimerManager:GetInstance():GetTimer(t_animation_clip.length,
                self.OnAnimationOutEnd, self, 1, false, false, state)
        self.m_animation_out_timer:Start()
    else
        --关闭的过程中又来了关闭消息，让之前的关闭动画播完即可，只是动画表现，和逻辑无关
    end
end

function UIBase:OnAnimationOutEnd(state)
    if(self.m_animation_out_timer ~= nil)then
        self.m_animation_out_timer:Stop()
        self.m_animation_out_timer = nil
    end

    if(state == "hide")then
        self:RealHidePanel()
    elseif(state == "close")then
        self:RealClosePanel()
    end
end

function UIBase:DoAnimation(animation_name)
    local t_animation = self.gameObject:GetComponent("Animation")
    if(IsNull(t_animation))then
        return
    end
    local t_animation_clip = t_animation:GetClip(animation_name)
    if(IsNull(t_animation_clip))then
        return
    end
    t_animation:Play(animation_name)
end
-----------------------------------------打开关闭动画-----------------------------------------




-----------------------------------------引导-----------------------------------------
function UIBase:GuideOnOpen()
    --CHUtil.DumpTable("###########GuideOnOpen 1 "..self.mUIName)
    local t_animation = self.gameObject:GetComponent("Animation")
    if(IsNull(t_animation))then
        self:OnGuideOpen()
        return
    end
    if(self.m_guide_timer ~= nil)then
        self.m_guide_timer:Stop()
        self.m_guide_timer = nil
    end
    self.m_guide_timer = TimerManager:GetInstance():GetTimer(t_animation.clip.length,
            self.OnGuideOpen, self, 1, false, false)
    self.m_guide_timer:Start()
end
function UIBase:OnGuideOpen()
    if(self.m_guide_timer ~= nil)then
        self.m_guide_timer:Stop()
        self.m_guide_timer = nil
    end
    --CHUtil.DumpTable("###########GuideOnOpen 2 "..self.mUIName)
    --延时2帧
    local timer = TimerManager:GetInstance():GetTimer(1, function()
        --引导需要检测UI被打开
        Messenger:GetInstance():Broadcast(MsgEnum.GUIDE_OPEN_UI_FINISH, self.mUIName)
    end, self, 2, true)
    timer:Start()
end
-----------------------------------------引导-----------------------------------------

return UIBase