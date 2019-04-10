local GoUtil = BaseClass("GoUtil")

function GoUtil.DestroyAllChild(trans)
    if(trans == nil) then
        return
    end

    trans = trans.transform
    local num = trans.childCount
    for i = num-1,0,-1 do
        local child = trans:GetChild(i)
        child:SetParent(nil)--添加原因：GameObject.Destroy不是立即删除，这里先释放和父节点的关联
        GameObject.Destroy(child.gameObject)
    end
end

function GoUtil.SetActive(obj, state)
    if(obj ~= nil) then
        obj.gameObject:SetActive(state)
    end
end

return GoUtil