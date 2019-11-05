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
function PoolManager:GetPoolOrCreate(template, size)
    template = template.gameObject
    if( self.mPoolGoList[template] == nil) then
        self.mPoolGoList[template] = PoolGO.New(template, self.mRootObj, size)
        return self.mPoolGoList[template]
    else
        return self.mPoolGoList[template]
    end
end

--获取一个缓存池
function PoolManager:GetPool(template)
    if(IsNull(template)) then
        Logger.LogError(Logger.Module.COMMON, "PoolManager GetPoolGo arg go is nil")
        return
    end

    template = template.gameObject
    return self.mPoolGoList[template]
end


--根据template获取一个实例go
--size 指定pool的大小，只有第一次创建这个template的缓存池时候有效
function PoolManager:Spawn(template, size)
    if(IsNull(template)) then
        Logger.E("PoolManager:Spawn template is nil")
        return
    end

    template = template.gameObject
    size = size or 1
    local pool = self:GetPoolOrCreate(template, size)
    local newGO = pool:Spawn()
    self.mInstMapPrefabList[newGO] = template
    return newGO
end

--回收一个实例go
function PoolManager:DeSpawn(inst)
    if(IsNull(inst)) then
        Logger.E("PoolManager:DeSpawn inst is nil")
        return
    end

    inst = inst.gameObject
    local isPool, template = self:IsPoolInst(inst)
    if(isPool) then
        local poolGO = self:GetPool(template)
        if(poolGO ~= nil) then
            poolGO:DeSpawn(inst)
        end
    end
end

--回收一个模板的所有实例对象
function PoolManager:DeSpawnTemplate(template)
    if(IsNull(template)) then
        Logger.E("PoolManager:DeSpawnTemplate template is nil")
        return
    end

    template = template.gameObject
    for k,v in pairs(self.mInstMapPrefabList) do
        if(v == template) then
            self:DeSpawn(k)
        end
    end
end

--是否有该prefab的缓存池
function PoolManager:IsPoolTemplate(template)
    if(IsNull(template)) then
        return false
    end

    template = template.gameObject
    if(self.mPoolGoList[template] ~= nil) then
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

    go = go.gameObject
    if(IsNull(self.mInstMapPrefabList[go])) then
        return false, nil
    else
        return true, self.mInstMapPrefabList[go]
    end
end

--释放缓存池
function PoolManager:Release(template)
    if(IsNull(template)) then
        Logger.E("PoolManager:Release template is nil")
        return
    end

    template = template.gameObject
    --释放对象池
    if(self.mPoolGoList[template] ~= nil) then
        self.mPoolGoList[template]:Release()
        self.mPoolGoList[template] = nil
    end

    --移除所有实例对应prefab的缓存
    for k,v in pairs(self.mInstMapPrefabList) do
        if(v == template) then
            self.mInstMapPrefabList[k] = nil
        end
    end
end

return PoolManager