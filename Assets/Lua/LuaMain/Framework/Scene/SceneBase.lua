local SceneBase = BaseClass("SceneBase", Updatable)

function SceneBase:__init()
    self.mPreLoadList = {}					--子类填充该预加载表
    self.mIsShowLoadingUI = true			--是否显示loading界面
	self.mIsShowEntryAnimation = false		--是否显示场景进入动画(一旦场景切换完成就开始播放动画) by lpf - 20191015
	self.mIsShowExitAnimation = false		--是否显示场景退出动画(退出动画完成才开始真正的场景切换工作) by lpf - 20191015
end

function SceneBase:__delete()

end

--预加载资源
function SceneBase:PreLoad(preLoadList)
    --Logger.Log(Logger.Module.SCENE, "SceneBase:PreLoad start")
    if(preLoadList ~= nil) then
        table.insertto(self.mPreLoadList, preLoadList, 0)
    end
    local totalCount = table.count(self.mPreLoadList)
    local loadCount = 0
    if(totalCount <= 0) then
        return coroutine.yieldbreak()
    end

    for _,v in ipairs(self.mPreLoadList) do
        --Logger.Log(Logger.Module.SCENE, "self.mPreLoadList start" .. v.abName)
        local request = nil
        if(v.type == nil) then
            request = ResourceManager.instance:PreLoadPrefabCoRequest(v.abName, v.assetName)
        elseif(v.type == Consts.ResType.atlas) then
            request = ResourceManager.instance:LoadSpriteAtlasCoRequest(v.abName)
        elseif(v.type == Consts.ResType.texture) then
            request = ResourceManager.instance:LoadTextureCoRequest(v.abName, v.assetName)
        elseif(v.type == Consts.ResType.atlas_parse) then
            local armyResCfgData = ArmyResCfgMgr.Get(v.abName)
            if(armyResCfgData ~= nil) then
                request = CS.ArmSpriteAnimation.ParseCoRequest(v.abName, armyResCfgData.action_info)
            else
                request = {isDone = true, progress = 1}
            end
        elseif(v.type == Consts.ResType.material) then
            request = ResourceManager.instance:LoadMaterialCoRequest(v.abName, v.assetName)
        end
        coroutine.waitforasyncop(request, function () end)
        loadCount = loadCount + 1
        coroutine.yieldreturn(loadCount * 1.0 / totalCount)
            --Logger.Log(Logger.Module.SCENE, "self.mPreLoadList over" .. v.abName)
    end

    return coroutine.yieldbreak()
end

--卸载预加载的资源
function SceneBase:UnLoad()
    for _,v in ipairs(self.mPreLoadList) do
        ResourceManager.instance:Release(v.abName, v.assetName)
    end
end

--子类重载
function SceneBase:Enter()

end

--子类重载
function SceneBase:Exit()

end

--子类重载
function SceneBase:Update()

end

--子类重载
function SceneBase:LateUpdate()

end

--子类重载
function SceneBase:FixedUpdate()

end

--子类重载(用于表示场景是否处理完成,可以关闭loading界面)
function SceneBase:IsComplete()
    return true
end

function SceneBase:OnStart(dt)
    
end

function SceneBase:Reset()
    
end

function SceneBase:OnClosePanel()
    UIManager:GetInstance():DisposeAllPanel()
end

return SceneBase