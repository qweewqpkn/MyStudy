local UIUtil = BaseClass("UIUtil")

--给按钮添加点击事件
--obj 添加事件的对象
--callback 回调函数
--data 自定义数据
function UIUtil.AddButtonEvent(obj, callback, self, ...)
    if(IsNull(obj)) then
        Logger.LogError(Logger.Module.COMMON, "UIUtil.AddButtonEvent obj is null")
        return
    end

    local btn = obj.gameObject:GetComponent("Button")
    if(btn == nil) then
        btn = obj.gameObject:AddComponent(typeof(CS.UnityEngine.UI.Button))
    end
    local params = SafePack(...)
    btn.onClick:RemoveAllListeners()
    btn.onClick:AddListener(function ()
        --ComponentClick(btn, data)
        callback(self, btn, SafeUnpack(params))
    end)
end

--给toggle添加点击事件
--toggle 添加事件的对象
--callback 回调函数
--data 自定义数据
function UIUtil.AddValueChangedEvent(obj, callback, self, ...)
    if(IsNull(obj)) then
        Logger.LogError(Logger.Module.COMMON, "UIUtil.AddValueChangedEvent obj is null")
        return
    end

    local toggle = obj.gameObject:GetComponent("Toggle")
    if(IsNull(toggle)) then
        Logger.LogError(Logger.Module.COMMON, "UIUtil.AddValueChangedEvent toggle is null")
        return
    end

    local params = SafePack(...)
    toggle.onValueChanged:RemoveAllListeners()
    toggle.onValueChanged:AddListener(function (isToggle)
        --ComponentClick(component, data)
        callback(self, toggle, isToggle, SafeUnpack(params))
    end)
end

--template 模版对象
--parent 父对象
--这里内部使用了对象池，所以你得配合DestroyGridAllItem 和 DeSpawnGridAllItem使用
function UIUtil.SpawnGridItem(template, parent)
    if(IsNull(template) or IsNull(parent)) then
        Logger.LogError(Logger.Module.UI, "UIUtil.SpawnGridItem template or parent is nil")
        return
    end

    local t = {}
    local newItem = PoolManager:GetInstance():Spawn(template)
    newItem.gameObject:SetActive(true)
    newItem.transform:SetParent(parent.transform, false)
    CS.UIComponentBind.BindToLua(newItem.gameObject, t)
    newItem.transform.rotation = Quaternion.identity
    newItem.transform.localScale = Vector3.one
    return t
end

--销毁这个模板的所有实例对象
function UIUtil.DestroyGridAllItem(template)
    if(IsNull(template)) then
        Logger.LogError(Logger.Module.UI, "DestroyGridAllItem template is nil")
        return
    end

    PoolManager:GetInstance():Release(template)
end

--回收这个模板对象的所有实例
function UIUtil.DeSpawnGridAllItem(template)
    if(IsNull(template)) then
        Logger.LogError(Logger.Module.UI, "DeSpawnGridAllItem template is nil")
        return
    end

    PoolManager:GetInstance():DeSpawnTemplate(template)
end

function UIUtil.SetSprite(image, abName, spriteName, isGray)
    isGray = isGray or false
    local imageExt = image.gameObject:GetComponent(typeof(ImageExt))
    imageExt:SetSprite(abName, spriteName)
    imageExt:SetGray(isGray)
end

--通过texture名字加载本地纹理
function UIUtil.SetRawImage(rawImage, textureName, isGray, callBack)
    local rawImageExt = rawImage.gameObject:GetComponent(typeof(RawImageExt))
    rawImageExt:SetTexture(textureName, callBack)
    rawImageExt:SetGray(isGray)
end

--递归修改layer
function UIUtil.ChangeLayerRecursive(obj, name)
    if(obj == nil) then
        return
    end

    obj.gameObject.layer = LayerMask.NameToLayer(name)
    local trans = obj.transform
    local num = trans.childCount
    for i = 0, num-1 do
        local child = trans:GetChild(i)
        child.gameObject.layer = LayerMask.NameToLayer(name)
        ChangeLayerRecursive(child, name)
    end
end

return UIUtil