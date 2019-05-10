local SmartGOManager = BaseClass("SmartGOManager", Singleton)

function SmartGOManager:__init()
    self.mPoolList = {}
end

function SmartGOManager:__delete()
    self.mPoolList = nil
end

--获取一个指定资源名的实例
function SmartGOManager:Spawn(abName, assetName, isPool)
    if(abName == nil or abName == "") then
        return nil
    end

    if(assetName == nil or assetName == "") then
        return nil
    end

    if(isPool) then
        local isPool, prefab = self:IsPool(abName, assetName)
        if(not isPool) then
            local name = string.format("%s/%s", abName, assetName)
            prefab = ResourceManager.Instance:PreLoadPrefab(abName, assetName)
            self.mPoolList[name] = prefab
        end
        local newGO = PoolManager:GetInstance():Spawn(prefab)
        return newGO
    else
        local newGO = ResourceManager.Instance:LoadPrefab(abName, assetName)
        return newGO
    end
end

--回收一个实例
function SmartGOManager:DeSpawn(go)
    if(PoolManager:GetInstance():IsPoolInst(go)) then
        PoolManager:GetInstance():DeSpawn(go)
    else
        CS.UnityEngine.GameObject.Destroy(go)
    end
end

function SmartGOManager:IsPool(abName, assetName)
    local name = string.format("%s/%s", abName, assetName)
    if(IsNull(self.mPoolList[name])) then
        return false, nil
    else
        return true, self.mPoolList[name]
    end
end

--释放指定名字对应的对象池
function SmartGOManager:Release(abName, assetName)
    local name = string.format("%s/%s", abName, assetName)
    local isPool, prefab = self:IsPool(abName, assetName)
    if(isPool) then
        self.mPoolList[name] = nil
        ResourceManager.Instance:Release(abName, assetName)
        PoolManager:GetInstance():Release(prefab)
    end
end

return SmartGOManager