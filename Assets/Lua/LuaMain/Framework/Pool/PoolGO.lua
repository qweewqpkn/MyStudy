local PoolGO = BaseClass("PoolGO")

function PoolGO:__init(go, parent, size, ctor, dtor)
    self.mUseList = {}
    self.mNotUseList = {}
    self.mGoTemplate = go
    self.mParentObj = parent
    self.mCtor = ctor
    self.mDtor = dtor
    size = size or 2
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
        Logger.LogError("PoolGo templte is nil")
        return
    end

    local newObj = nil
    if(#self.mNotUseList > 0) then
        newObj = table.remove(self.mNotUseList)
    else
        newObj = CS.UnityEngine.GameObject.Instantiate(self.mGoTemplate)
    end

    if(self.mCtor ~= nil) then
        self.mCtor(newObj)
    end

    table.insert(self.mUseList, newObj)
    GoUtil.SetActive(newObj, true)
    GoUtil.SetParent(newObj, self.mParentObj)
    return newObj
end

--回收一个对象
function PoolGO:DeSpawn(go)
    if(not IsNull(go)) then
        for i = 1, #self.mUseList do
            if(self.mUseList[i] ==  go) then
                table.remove(self.mUseList, i)
                break
            end
        end

        if(self.mDtor ~= nil) then
            self.mDtor(go)
        end

        GoUtil.SetActive(go, false)
        GoUtil.SetParent(go, self.mParentObj)
        table.insert(self.mNotUseList, go)
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