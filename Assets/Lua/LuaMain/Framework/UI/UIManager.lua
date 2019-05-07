--[[
    ---------------------------页面管理器-----------------------------------------
]]
local UIManager = BaseClass("UIManager", Singleton)
local Stack = require("Framework.Common.Stack")

function UIManager:__init(...)
    --获取默认的根节点
    self.mUIRoot = CS.UnityEngine.GameObject.Find("UIRoot").transform
    self.mLayerBG = self.mUIRoot:Find("LayerBG") --背景层
    self.mLayerWindow = self.mUIRoot:Find("LayerWindow") --窗口层
    self.mLayerGuide = self.mUIRoot:Find("LayerGuide") --引导层
    self.mLayerAlert = self.mUIRoot:Find("LayerAlert") --警告层
    self.mViewList = {} --存放所有打开的界面(不论隐藏与否)
    self.mViewStack = Stack.New() --存放进栈的界面
    self.mMainUI = nil --主界面
    self.mMainUIName = nil --主界面名字
    self.mInShowViewList = {} --显示的UI界面
    self.mSortingOrder = 0--界面排序，打开界面后+1
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
    local ret = self:GetPanel(uiName)
    ret:OpenPanel(...)
    return ret
end

function UIManager:ClosePanel(uiName)
    for k,v in ipairs(self.mViewList) do
        if v.mUIName == uiName then
            self:RemoveInShowViewList(uiName)
            v:ExitStack()
            v:ClosePanel()
            break
        end
    end
end

function UIManager:SwitchPanel(uiName, data)
    if #self.mInShowViewList > 0 then
        for i = #self.mInShowViewList,1,-1 do
            self:ClosePanel(self.mInShowViewList[i].mUIName)
        end
    end

    return self:OpenPanel(uiName, data)
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
        self.mViewList[i]:ClosePanel()
    end
    self.mViewStack:Clear()
    self.mInShowViewList = {}
end

function UIManager:DisposeAllPanel()
    for i = #self.mViewList,1,-1 do
        self.mViewList[i]:RealClosePanel()
    end
    self.mViewList = {}
    self.mViewStack:Clear()
    self.mInShowViewList = {}
    self.mMainUI = nil
    self.mMainUIName = nil
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
            break
        end
    end
end

function UIManager:RemoveViewList(uiName)
    for k,v in ipairs(self.mViewList) do
        if v.mUIName == uiName then
            table.remove(self.mViewList, k)
            break
        end
    end
end

function UIManager:AddLayer(layer, trans)
    if layer== 1 then
        trans:SetParent(self.mLayerBG, false)
    elseif layer ==2 then
        trans:SetParent(self.mLayerWindow, false)
    elseif layer==3 then
        trans:SetParent(self.mLayerAlert, false)
    elseif layer == 4 then
        trans:SetParent(self.mLayerGuide, false)
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

return UIManager