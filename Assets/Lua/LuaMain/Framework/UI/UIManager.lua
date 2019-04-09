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
    self.mCreateUIFuncList = {}
    self.mCurrentView = nil
    self.mViewStack = Stack.New() --存放进栈的界面
    self.mMainUI = nil --主界面
    self.mInShowViewList = {} --显示的UI界面
    self.mIsInMiniGame = false --是否在子游戏中
    self.mPanelSortingOrder = 0--界面排序，打开界面后+1
    self.mRestoreList = {} --战斗中返回，需要恢复的界面

    self.mCloseCallBack = nil --关闭页面的回调
    self.mCurBlackBg = nil --当前半透背景
end

function UIManager:RegisterCreateFunc(uiName,func)
    local tab = {name = uiName,func = func}
    self.mCreateUIFuncList[uiName] = tab
end

function UIManager:OpenPanel(uiName, data)
    local ret = self:GetPanel(uiName,false)
    if nil == ret then
        if self.mCreateUIFuncList[uiName] ~= nil then
            ret = self.mCreateUIFuncList[uiName].func()
            ret.mUIName = uiName
            table.insert(self.mViewList, ret) --添加到界面管理器中
        end
    end

    if nil ~= ret then
        if ret.IsMainUI then
            self.mMainUI = ret
        end


        self:SetCurrentView(ret)
        ret:OpenPanel(data)
    end
    return ret
end


function UIManager:SwitchPanel(uiName, data)
    if #self.mInShowViewList > 0 then
        for i = #self.mInShowViewList,1,-1 do
            self:ClosePanel(self.mInShowViewList[i].mUIName)
        end
        self.mCurrentView = nil
    end

    return self:OpenPanel(uiName, data)
end

function UIManager:RemoveInShowViewList(uiname)
    for k,v in ipairs(self.mInShowViewList) do
        if v.mUIName == uiname then
            table.remove(self.mInShowViewList, k)
            break
        end
    end
    
end

function UIManager:ClosePanel(uiName)
    local ret = nil
    for k,v in ipairs(self.mViewList) do
        if v.mUIName == uiName then
            self:RemoveInShowViewList(uiName)
            if v:ClosePanel() then 
                table.remove( self.mViewList,k)
            end
            if v == self.mCurrentView then
                self.mCurrentView = nil
            end
            ret = v
            break
        end
    end

    if nil ~= ret and ret.IsStack then
        local view = self.mViewStack:Peek()
        if view ~= nil then
            if view.mUIName == uiName then
                self.mViewStack:Pop()
                if self.mViewStack:Count() > 0 then
                    view = self.mViewStack:Peek()
                    --view:OpenPanel(view.PanelData)
                    view:ShowPanel()
                end
            else
                while true do
                    view = self.mViewStack:Pop()
                    if view == nil or view.mUIName == uiName then
                        break
                    end
                end

                view = self.mViewStack:Peek()
                if view ~= nil then
                    view:ShowPanel()
                end
            end
        end

        if(not(self.mIsInMiniGame))then
            if not self:IsStackHaveFullScreen() then
                if self.mMainUI ~= nil then
                    self.mMainUI:OpenPanel(self.mMainUI.PanelData)
                end
            end
        end
    end

    if self.mCloseCallBack ~= nil then
        self.mCloseCallBack(uiName)
    end
    return ret
end

function UIManager:SetCurrentView(view)
    self.mCurrentView = view

    local ret = false
    for _,v in ipairs(self.mInShowViewList) do
        if v.mUIName == view.mUIName then
            ret = true
            break
        end
    end
    if not ret then
        table.insert(self.mInShowViewList,view)
    end
end

function UIManager:GetCurrentViewName()
    if nil ~= self.mCurrentView then
        return self.mCurrentView.mUIName
    else
        return nil
    end
end

function UIManager:GetPanel(uiName,log)
    local ret = nil

    for _,v in ipairs(self.mViewList) do
        if v.mUIName == uiName then
            ret = v
            break
        end
    end
    if nil == ret and log then
        Log.warn("UIManager.GetPanel,panel is null,uiname=",uiName)
    end
    return ret
end

function UIManager:HideAllPanel()
    for _,v in pairs(self.mViewList) do
        if(v.IsRestore)then
            table.insert(self.mRestoreList, v)
        end
        v:HidePanel()
    end
    self.mViewStack:Clear()
    self.mInShowViewList = {}
    self.mCurrentView = nil
end

function UIManager:RestorePanel()
    for k, v in pairs(self.mRestoreList) do
        if(v.IsRestore)then
            self:OpenPanel(v.mUIName, v.PanelData)
            v:ShowPanel()
        end
    end
    self.mRestoreList = {}
end

function UIManager:CloseAllPanel()
    DebugLog.LogError(Logger.ModuleType.ui, "UIManager:CloseAllPanel()")
    for i = #self.mViewList,1,-1 do
        if self.mViewList[i]:ClosePanel() then
            table.remove( self.mViewList,i) --如果是销毁页面的，直接将对应的UI表也消除
        end
    end
    self.mViewStack:Clear()
    self.mInShowViewList = {}
end

function UIManager:DisposeAllPanel()
    DebugLog.LogError(Logger.ModuleType.ui, "UIManager:DisposeAllPanel()")
    for i = #self.mViewList,1,-1 do
        self.mViewList[i]:RealClosePanel()
    end
    self.mViewList = {}
    self.mViewStack:Clear()
    self.mMainUI = nil
    self.mInShowViewList = {}
end

function UIManager:AddLayer(ui)
    if ui.mLayer == 1 then
        ui.PanelObj.transform:SetParent(self.mLayerBG, false)
    elseif ui.mLayer ==2 then
        ui.PanelObj.transform:SetParent(self.mLayerWindow, false)
    elseif ui.mLayer ==3 then
        ui.PanelObj.transform:SetParent(self.mLayerAlert, false)
    elseif ui.mLayer == 4 then
        ui.PanelObj.transform:SetParent(self.mLayerGuide, false)
    end
end

function UIManager:IsStackHaveFullScreen()
    for i,v in ipairs(self.mViewStack.list) do
        if v.IsFullScreen then
            return true
        end
    end

    return false
end

function UIManager:OnDisconnect()
    for i, v in ipairs(self.mViewList) do
        v:OnDisconect()
    end
end

function UIManager:OnReconnect()
    for i, v in ipairs(self.mViewList) do
        v:OnReconect()
    end
end

return UIManager