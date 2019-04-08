local UIBase = class("UIBase")
function UIBase:ctor( ... )
    self.mAbPath = "" --ab资源路径
    self.mLayer = 1 --界面层级
    self.mUIName = "" --界面名字
    self.IsFullScreen = false --是否是全屏界面
    self.IsStack = false --是否进栈界面
    self.IsMainUI = false --是否是主界面
    self.IsDontDestroy = false --关闭界面是否要销毁掉还是隐藏而已
    self.PanelState = 0 --0未打开 1加载ab中 2打开完成 3隐藏
    self.PanelData = nil --界面缓存外部传入的数据
    self.IsRestore = false --战斗返回需要恢复的界面
    self.UseSelfSorting = false --是否使用自带层级
    self.useLayer = 1 --界面使用了多少层级，默认为1
end

function UIBase:OpenPanel(data)
    self.PanelData = data
    if self.PanelState == 0 then
        self.PanelState = 1
		HPrefabPacker.PackPrefab(self.mAbPath ,
		function(obj)
			if self.PanelState == 0 then
                self:RealClosePanel()
                return
            end

            if self.PanelState == 3 then
                self:HidePanel()
                return
            end

            --处理栈逻辑
            if self.IsStack then
                local view = UIManager.GetInstance().mViewStack:Peek()
                if view ~= nil then
                    if(self.IsFullScreen)then
                        view:HidePanel()
                    end
                end
                UIManager.GetInstance().mViewStack:Push(self)
                if UIManager.GetInstance():IsStackHaveFullScreen() then
                    if UIManager.GetInstance().mMainUI ~= nil then
                        UIManager.GetInstance().mMainUI:HidePanel()
                    end
                end
            end

            --缓存对象
            self.PanelObj = obj
            --绑定ui的控件
            UIComponentBind.BindToLua(obj, self)
            --把界面加载指定层级
            UIManager.GetInstance():AddLayer(self)
            --绑定
            self:OnBind()
            --显示界面
            self:ShowPanel(data)
        end)
    elseif self.PanelState == 3 then
        self:ShowPanel(data)
    end
end

--显示界面
function UIBase:ShowPanel(data)
    if self.PanelObj ~= nil then
        self.PanelObj:SetActive(true)
    end
    self.PanelState = 2
    --self:AnimationIn()
    self:SetCanvas()
    self:OnShow(data)
end

--隐藏界面
function UIBase:HidePanel()
    self.PanelState = 3
    if nil ~= self.PanelObj then
        self.PanelObj:SetActive(false)
    end
    self:OnHide()
end

--真正关闭界面
function UIBase:RealClosePanel()
    if self.PanelObj ~= nil then
        Object.Destroy(self.PanelObj)
    end

    UIUnionManager.GetInstance():RemoveMsgWithTarget(self)
    self:OnClose()
    self.PanelObj = nil
    self.PanelState = 0
end

--关闭页面
function UIBase:ClosePanel()
    local isDestroy = true
    if self.PanelObj ~= nil then
        if self.IsDontDestroy then
            self:HidePanel()
            isDestroy = false
        else
            self:AnimationOut()
        end
    else
        --界面prefab还没加载完成,但是关闭了，所以要重置他的状态
        if self.IsDontDestroy then
            self.PanelState = 3
        else
            self.PanelState = 0
        end
        isDestroy = false
    end
    return isDestroy
end

--设置canvas层级
function UIBase:SetCanvas()
    if(self.UseSelfSorting)then
        return
    end
    local canvas = GetComponent(self.PanelObj, "Canvas")
    if(canvas == nil)then
        return
    end
    canvas.overrideSorting = true
    if(self.IsMainUI)then
        UIManager.GetInstance().mPanelSortingOrder = 0
        canvas.sortingOrder = 0
        UIManager.GetInstance().mPanelSortingOrder = UIManager.GetInstance().mPanelSortingOrder + self.useLayer
        return
    end

    canvas.sortingOrder = UIManager.GetInstance().mPanelSortingOrder + 1
    UIManager.GetInstance().mPanelSortingOrder = UIManager.GetInstance().mPanelSortingOrder + self.useLayer
    --canvas.sortingOrder = UIManager.GetInstance().mPanelSortingOrder
end

--界面显示调用(为了让子类重写)
function UIBase:OnShow(data)

end

--界面隐藏调用(为了让子类重写)
function UIBase:OnHide()

end

--界面关闭调用(为了让子类重写)
function UIBase:OnClose()

end

--绑定各种事情
function UIBase:OnBind()

end

--断线的时候调用(为了让子类重写)
function UIBase:OnDisconect()

end

--重连成功调用(为了让子类重写)
function UIBase:OnReconect()

end

function UIBase:AnimationIn()
    local t_animation = GetComponent(self.PanelObj, "Animation")
    if(t_animation == nil)then
        return
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
    Log.info("#################### t_aniamtion_state.name="..t_aniamtion_state.name)
    t_aniamtion_state.speed = 1
    t_animation:Play(animation_name)
end

function UIBase:AnimationOut()
    local t_animation = GetComponent(self.PanelObj, "Animation")
    if(t_animation == nil)then
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

    Log.info("#################################################wait panel animation")
    self.m_timer = Timer.New(function (self)
        Log.info("#################################################real close")
        self:RealClosePanel()
        self.m_timer:Stop()
        self.m_timer = nil
    end, t_aniamtion_state.length, 1, true, self)
    self.m_timer:Start()
end