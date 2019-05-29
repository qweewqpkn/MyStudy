local UIUtil = BaseClass("UIUtil")

UIUtil.ButtonEventType = {
    PointerClick = "PointerClick",
    PointerDown = "PointerDown",
    PointerUp = "PointerUp",
    PointerEnter = "PointerEnter",
    PointerExit = "PointerExit",
}
--给按钮添加点击事件
--obj 添加事件的对象
--callback 回调函数
--self self对象
--type 按钮事件类型
function UIUtil.AddButtonEvent(obj, callback, self, type, ...)
    if(IsNull(obj)) then
        Logger.LogError(Logger.Module.COMMON, "UIUtil.AddButtonEvent obj is null")
        return
    end

    local params = SafePack(...)
    if(type == nil) then
        local btn = obj.gameObject:GetComponent("Button")
        if(IsNull(btn)) then
            btn = obj.gameObject:AddComponent(typeof(CS.UnityEngine.UI.Button))
        end

        btn.onClick:RemoveAllListeners()
        btn.onClick:AddListener(function ()
            --ComponentClick(btn, data)
            callback(self, obj.gameObject, SafeUnpack(params))
        end)
    else
        local pointer = obj.gameObject:GetComponent("MouseEventPointer")
        if(IsNull(pointer)) then
            pointer = obj.gameObject:AddComponent(typeof(CS.MouseEventPointer))
        end

        local action = function (go, eventData)
             --ComponentClick(btn, data)
            callback(self, go, eventData, SafeUnpack(params))
        end

        if(type == UIUtil.ButtonEventType.PointerClick) then
            pointer.mClickAction = action
        elseif(type == UIUtil.ButtonEventType.PointerDown) then
            pointer.mDownAction = action
        elseif(type == UIUtil.ButtonEventType.PointerUp) then
            pointer.mUpAction = action
        elseif(type == UIUtil.ButtonEventType.PointerEnter) then
            pointer.mExitAction = action
        elseif(type == UIUtil.ButtonEventType.PointerExit) then
            pointer.mEnterAction = action
        end
    end
end


UIUtil.DragEventType = {
    Drag = "Drag",
    BeginDrag = "BeginDrag",
    EndDrag = "EndDrag",
}
--为对象添加拖动事件处理
function UIUtil.AddDragEvent(obj, callback, self, type, ...)
    if(IsNull(obj)) then
        Logger.LogError(Logger.Module.COMMON, "UIUtil.AddDragEvent obj is null")
        return
    end

    local drag = obj.gameObject:GetComponent("MouseEventDrag")
    if(drag == nil) then
        drag = obj.gameObject:AddComponent(typeof(CS.MouseEventDrag))
    end

    local params = SafePack(...)
    local action = function(go, eventData)
        callback(self, go, eventData ,SafeUnpack(params))
    end
    if(type == UIUtil.DragEventType.Drag or type == nil) then
        drag.mMouseDrag = action
    elseif(type == UIUtil.DragEventType.BeginDrag) then
        drag.mMouseBeginDrag = action
    elseif(type == UIUtil.DragEventType.EndDrag) then
        drag.mMouseEngDrag = action
    end
end

--在scrollrect中的子对象添加拖动事件(这时scrollrect 和 scrollrect的子对象都会监听drag)
function UIUtil.AddScrollRectDragEvent(obj, callback, self, type, ...)
    if(IsNull(obj)) then
        Logger.LogError(Logger.Module.COMMON, "UIUtil.AddScrollRectDragEvent obj is null")
        return
    end

    local drag = obj.gameObject:GetComponent("MouseEventScrollRectDrag")
    if(drag == nil) then
        drag = obj.gameObject:AddComponent(typeof(CS.MouseEventScrollRectDrag))
    end

    local params = SafePack(...)
    local action = function(go, eventData)
        callback(self, go, eventData ,SafeUnpack(params))
    end
    if(type == UIUtil.DragEventType.Drag) then
        drag.mMouseDrag = action
    elseif(type == UIUtil.DragEventType.BeginDrag) then
        drag.mMouseBeginDrag = action
    elseif(type == UIUtil.DragEventType.EndDrag) then
        drag.mMouseEngDrag = action
    end
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
    local imageExt = image.gameObject:GetComponent(typeof(CS.ImageExt))
    imageExt:SetSprite(abName, spriteName)
    imageExt:SetGray(isGray)
end

--通过texture名字加载本地纹理
function UIUtil.SetRawImage(rawImage, textureName, isGray, callBack)
    local rawImageExt = rawImage.gameObject:GetComponent(typeof(CS.RawImageExt))
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

--灰显
function UIUtil.SetGray(image, isGray)
    --isGray = isGray or false
    --local imageExt = image.gameObject:GetComponent(typeof(CS.ImageExt))
    --imageExt:SetGray(isGray)
end

function UIUtil.IsTouchedUI()
    local touchedUI = false
    if CS.UnityEngine.Application.isMobilePlatform then
        if CS.UnityEngine.Input.touchCount > 0 and CS.UnityEngine.EventSystems.EventSystem.current:IsPointerOverGameObject(CS.UnityEngine.Input.GetTouch(0).fingerId) then
            touchedUI = true
        end
    elseif CS.UnityEngine.EventSystems.EventSystem.current:IsPointerOverGameObject() then
        touchedUI = true
    end

    return touchedUI
end

--星星
function UIUtil.SetStar(obj, star)
    if(star == 0)then
        return
    end
    local childCount = obj.transform.childCount
    for i = 1, #childCount do
        obj.transform:GetChild(i - 1).gameObject:SetActive(star == i)
    end
end

--品质
function UIUtil.SetQuality(icon, quality, isGray)
    local qualityName = "ui_common_bg_hero_bg_" ..tostring(quality)
    if(quality == 1) then
        UIUtil.SetSprite(icon, "ui_common_atlas", qualityName, isGray)
    elseif(quality == 2) then
        UIUtil.SetSprite(icon, "ui_common_atlas", qualityName, isGray)
    elseif(quality == 3) then
        UIUtil.SetSprite(icon, "ui_common_atlas", qualityName, isGray)
    elseif(quality == 4) then
        UIUtil.SetSprite(icon, "ui_common_atlas", qualityName, isGray)
    elseif(quality == 5) then
        UIUtil.SetSprite(icon, "ui_common_atlas", qualityName, isGray)
    else
        Logger.E("UIUtil:SetQuality quality value is error, quality : " .. quality)
    end
end

return UIUtil