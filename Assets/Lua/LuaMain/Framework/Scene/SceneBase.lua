local SceneBase = BaseClass("SceneBase")

function SceneBase:__init()
    self.mPreLoadList = {} --子类填充该预加载表
end

function SceneBase:__delete()

end

--预加载资源
function SceneBase:PreLoad()
    local totalCount = table.count(self.mPreLoadList)
    local loadCount = 0

    for _,v in ipairs(self.mPreLoadList) do
        local request = ResourceManager.Instance:PreLoadPrefabAsync(v.abName, v.assestName)
        coroutine.waitforasyncop(request, function () end)
        loadCount = loadCount + 1
    end

    return coroutine.yieldbreak()
end

--卸载预加载的资源
function SceneBase:UnLoad()

end

--子类重载
function SceneBase:OnEnter()

end

--子类重载
function SceneBase:OnExit()

end

--子类重载(用于表示场景是否处理完成,可以关闭loading界面)
function SceneBase:IsComplete()
    return true
end