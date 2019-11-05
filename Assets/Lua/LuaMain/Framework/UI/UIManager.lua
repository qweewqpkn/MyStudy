--[[
    ---------------------------页面管理器-----------------------------------------
]]
local UIManager = BaseClass("UIManager", Singleton)
local Stack = require("Framework.Common.Stack")

function UIManager:__init(...)
    --获取默认的根节点
    self.mUIRoot = CS.UnityEngine.GameObject.Find("UIRoot").transform
    self.mMask = self.mUIRoot:Find("mask")
    self.mMaskCount = 0
    self.mViewList = {} --存放所有打开的界面(不论隐藏与否)
    self.mViewStack = Stack.New() --存放进栈的界面
    self.mMainUI = nil --主界面
    self.mMainUIName = nil --主界面名字
    self.mInShowViewList = {} --显示的UI界面
    self.mSortingOrder = 0--界面排序，打开界面后+1
    self.mNotCloseUI = {}--切景的时候不能关闭的界面
    self.mUICamera = nil
    self:SetCanvas()
end

function UIManager:SetCanvas()
    local canvas = self.mUIRoot.gameObject:GetComponent(typeof(CS.UnityEngine.Canvas))
    self.mUICamera = canvas.worldCamera
    local canvasScaler = self.mUIRoot.gameObject:GetComponent(typeof(CS.UnityEngine.UI.CanvasScaler))
    if(canvasScaler ~= nil)then
        local default_width = 1920
        local default_height = 1080
        local width = CS.UnityEngine.Screen.width
        local height = CS.UnityEngine.Screen.height
        if(width/height >= default_width/default_height)then
            canvasScaler.matchWidthOrHeight = 1
        else
            canvasScaler.matchWidthOrHeight = 0
        end
    end
end

function UIManager:GetPanel(uiName)
    local ret = nil

    for _,v in ipairs(self.mViewList) do
        if v.mUIName == uiName then
            ret = v
            break
        end
    end

    if nil == ret then
        if(UIConfig[uiName] ~= nil) then
            ret = UIConfig[uiName].type.New()
            ret.mUIName = uiName
            table.insert(self.mViewList, ret) --添加到界面管理器中
        end
    end

    if ret.mIsMainUI then
        self.mMainUI = ret
        self.mMainUIName = uiName
    end

    self:AddInShowViewList(ret)
    return ret
end

function UIManager:OpenPanel(uiName, ...)
    --如果在引导中，全屏界面存在的情况下又打开了一些小界面（处理划出界面的问题）
    if(HotUpdateMgr:GetInstance().complete)then--2019.10.17 这里要在热更之后执行
        if(GuideMgr:GetInstance():OpenPanelCheck(uiName))then
            return
        end
    end

    local ret = self:GetPanel(uiName)
    ret:OpenPanel(...)
    return ret
end

function UIManager:ClosePanel(uiName)
    for k,v in ipairs(self.mViewList) do
        if v.mUIName == uiName then
            --這裡進行檢測的原因是：由於現在的界面有動畫，會延時關閉，所以mViewList的值也會延時更新，所以連續關閉一個界面導致關閉了其他界面。
            --為啥不把mViewList的更新放到這裡呢？因為有些界面不是銷毀，而只是隱藏，所以行為不確定，不能再這裡就進行移除
            local isRemoveOk = self:RemoveInShowViewList(uiName)
            if(isRemoveOk) then
              --v:ClosePanel()
              v:ExitStack()
            end
            break
        end
    end
end

function UIManager:HideAllPanel()
    for i = #self.mViewList,1,-1 do
        self.mViewList[i]:HidePanel()
    end
    self.mViewStack:Clear()
    self.mInShowViewList = {}
end

function UIManager:CloseAllPanel()
    for i = #self.mViewList,1,-1 do
        local notClose = self:IsNotCloseUI(self.mViewList[i].mUIName)
        if(not notClose)then
            self.mViewList[i]:ClosePanel()
        end
    end
    self.mViewStack:Clear()
    self.mInShowViewList = {}
    self.mMainUI = nil
    self.mMainUIName = nil
end

function UIManager:DisposeAllPanel()
    for i = #self.mViewList,1,-1 do
        local notClose = self:IsNotCloseUI(self.mViewList[i].mUIName)
        if(not notClose)then
            self.mViewList[i]:OnClose()
            self.mViewList[i]:RealClosePanel()
        end
    end

    self.mViewStack:Clear()
    self.mInShowViewList = {}
    self.mMainUI = nil
    self.mMainUIName = nil
end

function UIManager:CloseAllStackPanel()
    while(true) do
        local view = self.mViewStack:Pop()
        if(view == nil) then
            break
        else
            view:ClosePanel()
            self:RemoveInShowViewList(view.mUIName)
        end
    end

    self.mViewStack:Clear()
    self:ShowMainUI()
end

function UIManager:ShowMainUI()
    if not self:IsStackHaveFullScreen() then
        if self.mMainUI ~= nil and self.mMainUI.mPanelState == 3 then
            self.mMainUI:ShowPanel(SafeUnpack(self.mMainUI.mPanelData))
        end
    end
end

function UIManager:AddInShowViewList(view)
    local ret = false
    for _,v in ipairs(self.mInShowViewList) do
        if v.mUIName == view.mUIName then
            ret = true
            break
        end
    end

    if not ret then
        table.insert(self.mInShowViewList, view)
    end
end

function UIManager:RemoveInShowViewList(uiName)
    for k,v in ipairs(self.mInShowViewList) do
        if v.mUIName == uiName then
            table.remove(self.mInShowViewList, k)
            return true
        end
    end

    return false
end

function UIManager:RemoveViewList(uiName)
    for k,v in ipairs(self.mViewList) do
        if v.mUIName == uiName then
            table.remove(self.mViewList, k)
            break
        end
    end
end

function UIManager:IsStackHaveFullScreen()
    for i,v in ipairs(self.mViewStack.list) do
        if v.mIsFullScreen then
            return true
        end
    end

    return false
end

--
function UIManager:AddNotCloseUI(ui_name)
    for _,v in ipairs(self.mInShowViewList) do
        if v.mUIName == ui_name then
            table.insert(self.mNotCloseUI, v)
            v:HidePanel()
        end
    end
end
function UIManager:RestoreNotCloseUI()
    for i = 1, #self.mNotCloseUI do
        table.insert(self.mInShowViewList, self.mNotCloseUI[i])
        self.mNotCloseUI[i]:ShowPanel()
        self.mNotCloseUI[i]:EnterStack()
    end
    self.mNotCloseUI = {}
end
function UIManager:IsNotCloseUI(ui_name)
    for i = 1, #self.mNotCloseUI do
        if(ui_name == self.mNotCloseUI[i].mUIName)then
            return true
        end
    end
    return false
end


function UIManager:OnDisconnect()
    for i, v in ipairs(self.mViewList) do
        v:OnDisconnect()
    end
end

function UIManager:OnReconnect()
    for i, v in ipairs(self.mViewList) do
        v:OnReconnect()
    end
end

function UIManager:ShowMask(isShow)
    if(isShow) then
        self.mMaskCount = self.mMaskCount + 1
    else
        self.mMaskCount = self.mMaskCount - 1
    end

    if(not IsNull(self.mMask))then
        if(self.mMaskCount > 0) then
            GoUtil.SetActive(self.mMask, true)
        else
            GoUtil.SetActive(self.mMask, false)
        end
    end
end

function UIManager:GetMaxSortingOrder()
    local max = 0
    for i = #self.mViewList,1,-1 do
        if(self.mViewList[i].mSortingOrder > max)then
            max = self.mViewList[i].mSortingOrder
        end
    end
    return max
end

function UIManager:IsOpenPanel(uiName)
    local ret = nil

    for _,v in ipairs(self.mViewList) do
        if v.mUIName == uiName then
            ret = v
            break
        end
    end

    if(ret ~= nil)then
        --如果处于打开动画
        --CHUtil.DumpTable("############ret.mPanelState="..tostring(ret.mPanelState))
        if(ret.mPanelState < 2)then
            return false
        end
        if(ret.m_guide_timer ~= nil)then
            --CHUtil.DumpTable("##########如果处于打开动画1")
            return false
        else
            --CHUtil.DumpTable("##########如果处于打开动画2")
        end
    end

    return ret ~= nil
end

function UIManager:GetOpenedPanel(uiName)
    local ret = nil
    for _,v in ipairs(self.mViewList) do
        if v.mUIName == uiName then
            ret = v
            break
        end
    end
    return ret
end

function UIManager:EnableWildMapCam(view, bEnable)
    if(view.mHideWildMap) then
        local isHaveHideWildMap = false
        for k,v in ipairs(self.mViewList) do
            if v.mHideWildMap and v ~= view then
                isHaveHideWildMap = true
                break
            end
        end

        --Logger.E("bEnable and isHaveHideWildMap" .. tostring(bEnable) .. "/" .. tostring(isHaveHideWildMap))
        local enableCam = bEnable and not isHaveHideWildMap
        local moveWildMap = not enableCam
        WildMapMgr:GetInstance():EnableWildMapCam(enableCam, moveWildMap)
    end
end

return UIManager