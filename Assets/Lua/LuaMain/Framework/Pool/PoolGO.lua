local PoolGO = BaseClass("PoolGO")

function PoolGO:__init(go, root, size)
    size = size or 1
    self.mUseList = {}
    self.mNotUseList = {}
    self.mGoTemplate = go
    self.mParentObj = CS.UnityEngine.GameObject()
    self.mParentObj.name = go.name .. "(PoolGo)"
    GoUtil.SetParent(self.mParentObj, root)

    for i = 1, size do
        local newObj = CS.UnityEngine.GameObject.Instantiate(self.mGoTemplate)
        GoUtil.SetActive(newObj, false)
        GoUtil.SetParent(newObj, self.mParentObj)
        table.insert(self.mNotUseList, newObj)
    end
end

--获得一个对象
function PoolGO:Spawn()
    if(IsNull(self.mGoTemplate)) then
        Logger.LogError(Logger.Module.COMMON, "PoolGo templte is nil")
        return
    end

    local newObj = nil
    if(#self.mNotUseList > 0) then
        newObj = table.remove(self.mNotUseList)
    else
        newObj = CS.UnityEngine.GameObject.Instantiate(self.mGoTemplate)
    end

    table.insert(self.mUseList, newObj)
    GoUtil.SetActive(newObj, true)
    GoUtil.SetParent(newObj, self.mParentObj)
    return newObj
end

--回收一个对象
function PoolGO:DeSpawn(go)
    if(not IsNull(go)) then
        local bFind = false
        for i = 1, #self.mUseList do
            if(self.mUseList[i] ==  go) then
                bFind = true
                table.remove(self.mUseList, i)
                break
            end
        end

        if(bFind) then
            GoUtil.SetActive(go, false)
            GoUtil.SetParent(go, self.mParentObj)
            table.insert(self.mNotUseList, go)
        end
    end
end

--释放对象池
function PoolGO:Release()
    local OnRelease = function (list)
        if(list == nil) then
            return
        end

        local count = #list
        for i = count, 1, -1 do
            local go = table.remove(list, i)
            if(not IsNull(go)) then
                GoUtil.Destroy(go)
            end
        end
    end

    OnRelease(self.mNotUseList)
    OnRelease(self.mUseList)
    GoUtil.Destroy(self.mParentObj)
    self.mUseList = nil
    self.mNotUseList = nil
    self.mGoTemplate = nil
end

return PoolGO