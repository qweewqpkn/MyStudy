local UIBase = BaseClass("UIBase")

function UIBase:__init(...)
    self.mAbPath = "" --ab资源路径
    self.mLayer = 1 --界面层级
    self.mUIName = "" --界面名字
    self.mIsFullScreen = false --是否是全屏界面,全屏界面会隐藏之前的界面
    self.mIsStack = false --是否进栈界面,为了支持返回时能返回之前的界面
    self.mIsMainUI = false --是否是主界面
    self.mIsDontDestroy = false --关闭界面是否要销毁掉还是隐藏
    self.mPanelState = 0 --0未打开 1加载ab中 2打开完成 3隐藏
    self.mPanelData = nil --界面缓存外部传入的数据
    self.mIsRestore = false --战斗返回需要恢复的界面
    self.mUseSelfSorting = false --是否使用自带层级
    self.mUseLayer = 1 --界面使用了多少层级，默认为1
    self.mSortingOrder = 0 --界面使用的层级
end

function UIBase:OpenPanel(...)
    self.mPanelData = SafePack(...)
    if self.mPanelState == 0 then
        self.mPanelState = 1

        --加载界面资源
        ResourceManager.Instance:LoadPrefabAsync(self.mAbPath, self.mAbPath, function(obj)
            --加载完成后，界面被标记为关闭
			if self.mPanelState == 0 then
                self:RealClosePanel()
                return
            end

            --加载完成后，界面被标记为隐藏
            if self.mPanelState == 3 then
                self:HidePanel()
                return
            end

            --绑定ui的控件
            CS.UIComponentBind.BindToLua(obj, self)
            --把界面加载指定层级
            UIManager:GetInstance():AddLayer(self)
            --处理栈逻辑
            self:StackProcess()
            --绑定
            self:OnBind()
            --初始化
            self:OnInit()
            --显示界面
            self:ShowPanel(SafeUnpack(self.mPanelData))
        end)
    elseif self.mPanelState == 3 then
        self:ShowPanel(...)
        self:StackProcess()
    end
end

--栈处理　
function UIBase:StackProcess()
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
            if UIManager:GetInstance().mMainUI ~= nil then
                UIManager:GetInstance().mMainUI:HidePanel()
            end
        end
    end
end

--显示界面
function UIBase:ShowPanel(...)
    self.mPanelState = 2
    if self.gameObject ~= nil then
        self.gameObject:SetActive(true)
    end
    self:AnimationIn()
    self:SetCanvas()
    self:OnShow(...)
end

--隐藏界面
function UIBase:HidePanel()
    self.mPanelState = 3
    if nil ~= self.gameObject then
        self.gameObject:SetActive(false)
    end
    self:OnHide()
end

--真正关闭界面
function UIBase:RealClosePanel()
    if self.gameObject ~= nil then
        CS.UnityEngine.Object.Destroy(self.gameObject)
    end

    Messenger:GetInstance():RemoveByTarget(self)
    self:OnClose()
    self.gameObject = nil
    self.mPanelState = 0
end

--关闭页面
function UIBase:ClosePanel()
    local isDestroy = true
    if self.gameObject ~= nil then
        if self.mIsDontDestroy then
            self:HidePanel()
            isDestroy = false
        else
            self:AnimationOut()
        end
    else
        --界面prefab还没加载完成,但是关闭了，所以要重置他的状态
        if self.mIsDontDestroy then
            self.mPanelState = 3
        else
            self.mPanelState = 0
        end
        isDestroy = false
    end
    return isDestroy
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
function UIBase:OnDisconect()

end

--重连成功调用(为了让子类重写)
function UIBase:OnReconect()

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