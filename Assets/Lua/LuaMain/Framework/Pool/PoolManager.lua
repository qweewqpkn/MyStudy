local PoolManager = BaseClass("PoolManager", Singleton)
local PoolGO = require "Framework.Pool.PoolGO"

function PoolManager:__init()
    self.mPoolGoList = {}
    self.mPoolList = {}
    self.mObj = CS.UnityEngine.GameObject()
    self.mObj.name = "PoolManager"
end

function PoolManager:__delete()
    for k,v in pairs(self.mPoolGoList) do
        self:ReleasePoolGO(k)
    end
    self.mPoolGoList = nil

    if(self.mObj ~= nil) then
        GoUtil.Destroy(self.mObj)
        self.mObj = nil
    end
end

--获取一个缓存池
function PoolManager:GetPoolGO(go, size, ctor, dtor)
    if(IsNull(go)) then
        Logger.LogError("PoolManager GetPoolGo arg go is nil")
        return
    end

    if self.mPoolGoList[go] == nil then
        local obj = CS.UnityEngine.GameObject()
        obj.name = go.name .. "(PoolGo)"
        GoUtil.SetParent(obj, self.mObj)
        self.mPoolGoList[go] = PoolGO.New(go, obj, size, ctor, dtor)
    end

    return self.mPoolGoList[go]
end

--释放缓存池
function PoolManager:ReleasePoolGO(go)
    if(self.mPoolGoList[go] ~= nil) then
        self.mPoolGoList[go]:Release()
        self.mPoolGoList[go] = nil
    end
end

return PoolManager