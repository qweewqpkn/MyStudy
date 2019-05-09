local PoolManager = BaseClass("PoolManager", Singleton)
local PoolGO = require "Framework.Pool.PoolGO"

function PoolManager:__init()
    self.mPoolGoList = {} --缓存池列表
    self.mInstMapPrefabList = {} --实例映射模板对象列表
    self.mRootObj = CS.UnityEngine.GameObject()
    self.mRootObj.name = "PoolManager"
end

function PoolManager:__delete()
    for k,v in pairs(self.mPoolGoList) do
        self:Release(v)
    end
    self.mPoolGoList = nil

    if(self.mRootObj ~= nil) then
        GoUtil.Destroy(self.mRootObj)
        self.mRootObj = nil
    end
end

--创建一个缓存池
function PoolManager:CreatePool(prefab, size)
    if( self.mPoolGoList[prefab] == nil) then
        self.mPoolGoList[prefab] = PoolGO.New(prefab, self.mRootObj, size)
        return self.mPoolGoList[prefab]
    else
        Logger.LogError(Logger.Module.COMMON, "PoolManager CreatePool is failed, have same prafab exist")
    end
end

--获取一个缓存池
function PoolManager:GetPool(prefab)
    if(IsNull(prefab)) then
        Logger.LogError(Logger.Module.COMMON, "PoolManager GetPoolGo arg go is nil")
        return
    end

    return self.mPoolGoList[prefab]
end


--根据prefab获取一个go
function PoolManager:Spawn(prefab)
    local pool = self:GetPool(prefab)
    if(pool == nil) then
        pool = self:CreatePool(prefab, 1)
    end
    local newGO = pool:Spawn()
    self.mInstMapPrefabList[newGO] = prefab
    return newGO
end

--回收一个实例go
function PoolManager:DeSpawn(go)
    local isPool, prefab = self:IsPoolInst(go)
    if(isPool) then
        local poolGO = self:GetPool(prefab)
        if(poolGO ~= nil) then
            poolGO:DeSpawn(go)
        end
    end
end

--是否有该name的缓存池
function PoolManager:IsPoolPrefab(prefab)
    if(IsNull(go)) then
        return false
    end

    if(self.mPoolGoList[prefab] ~= nil) then
        return true
    else
        return false
    end
end

--是否缓存了该实例
function PoolManager:IsPoolInst(go)
    if(IsNull(go)) then
        return false, nil
    end

    if(self.mInstMapPrefabList[go] == nil) then
        return false, nil
    else
        return true, self.mInstMapPrefabList[go]
    end
end

--释放缓存池
function PoolManager:Release(prefab)
    --释放对象池
    if(self.mPoolGoList[prefab] ~= nil) then
        self.mPoolGoList[prefab]:Release()
        self.mPoolGoList[prefab] = nil
    end

    --移除所有实例对应prefab的缓存
    for k,v in ipairs(self.mInstMapPrefabList) do
        if(v == prefab) then
            self.mInstMapPrefabList[k] = nil
        end
    end
end

return PoolManager