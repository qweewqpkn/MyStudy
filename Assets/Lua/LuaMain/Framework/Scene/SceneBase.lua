local SceneBase = BaseClass("SceneBase", Updatable)

function SceneBase:__init()
    self.mPreLoadList = {} --子类填充该预加载表
    self.mIsShowLoadingUI = true --是否显示loading界面
end

function SceneBase:__delete()

end

--预加载资源
function SceneBase:PreLoad()
    Logger.Log(Logger.Module.SCENE, "SceneBase:PreLoad start")
    local totalCount = table.count(self.mPreLoadList)
    local loadCount = 0
    if(totalCount <= 0) then
        return coroutine.yieldbreak()
    end

    for _,v in ipairs(self.mPreLoadList) do
        Logger.Log(Logger.Module.SCENE, "self.mPreLoadList start" .. v.abName)
        local request = ResourceManager.Instance:PreLoadPrefabRequest(v.abName, v.assetName)
        coroutine.waitforasyncop(request, function () end)
        loadCount = loadCount + 1
        coroutine.yieldreturn(loadCount * 1.0 / totalCount)
        Logger.Log(Logger.Module.SCENE, "self.mPreLoadList over" .. v.abName)
    end

    return coroutine.yieldbreak()
end

--卸载预加载的资源
function SceneBase:UnLoad()
    for _,v in ipairs(self.mPreLoadList) do
        ResourceManager.Instance:Release(v.abName, v.assetName)
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

return SceneBase