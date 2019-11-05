local GoUtil = BaseClass("GoUtil")

function GoUtil.DestroyAllChild(trans)
    if(trans == nil) then
        return
    end

    trans = trans.transform
    local num = trans.childCount
    for i = num-1,0,-1 do
        local child = trans:GetChild(i)
        GoUtil.SetParent(child, nil)--添加原因：GameObject.Destroy不是立即删除，这里先释放和父节点的关联
        GoUtil.Destroy(child.gameObject)
    end
end

function GoUtil.SetActive(obj, state)
    if(obj ~= nil) then
        if(obj.gameObject.activeSelf ~= state) then
            obj.gameObject:SetActive(state)
        end
    end
end

function GoUtil.SetParent(obj, parent, stayWorldPos)
    if(obj ~= nil) then
        if(parent ~= nil) then
            if(stayWorldPos == nil) then
                stayWorldPos = true
            end
            obj.transform:SetParent(parent.transform, stayWorldPos)
        else
            obj.transform:SetParent(nil)
        end
    end
end

function GoUtil.Destroy(obj)
    if(not IsNull(obj)) then
        SmartGOManager:GetInstance():DeSpawn(obj)
    end
end

function GoUtil.ResetTransform(obj)
    if(not IsNull(obj)) then
        obj.transform.localPosition = Vector3.zero
        obj.transform.localScale = Vector3.one
        obj.transform.localEulerAngles = Vector3.zero
    end
end

return GoUtil