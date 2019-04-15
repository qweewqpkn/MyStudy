local PoolGO = BaseClass("PoolGO")

function PoolGO:__init(go, parent, size)
    self.mGoList = {}
    self.mGoTemplate = go
    self.mParentObj = parent
    size = size or 2
    for i = 1, size do
        local newObj = CS.UnityEngine.GameObject.Instantiate(self.mGoTemplate)
        GoUtil.SetActive(newObj, false)
        GoUtil.SetParent(newObj, self.mParentObj)
        table.insert(self.mGoList, newObj)
    end
end

--获得一个对象
function PoolGO:Spawn()
    if(IsNull(self.mGoTemplate)) then
        Logger.LogError("PoolGo templte is nil")
        return
    end

    local newObj = nil
    if(#self.mGoList > 0) then
        newObj = table.remove(self.mGoList)
    else
        newObj = CS.UnityEngine.GameObject.Instantiate(self.mGoTemplate)
    end

    GoUtil.SetActive(newObj, true)
    GoUtil.SetParent(newObj, self.mParentObj)
    return newObj
end

--回收一个对象
function PoolGO:DeSpawn(go)
    if(not IsNull(go)) then
        local bFind = false
        for _,v in ipairs(self.mGoList) do
            if(v == go) then
                bFind = true
                break
            end
        end

        if(not bFind) then
            GoUtil.SetActive(go, false)
            GoUtil.SetParent(go, self.mParentObj)
            table.insert(self.mGoList, go)
        end
    end
end

--释放对象池
function PoolGO:Release()
    local count = #self.mGoList
    for i = count, 1, -1 do
        local go = table.remove(self.mGoList, i)
        CS.UnityEngine.GameObject.Destroy(go)
    end

    GoUtil.Destroy(self.mParentObj)
    self.mGoList = nil
    self.mGoTemplate = nil
end

return PoolGO