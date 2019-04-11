local UIUtil = BaseClass("UIUtil")

--给按钮添加点击事件
--obj 添加事件的对象
--callback 回调函数
--data 自定义数据
function UIUtil.AddButtonEvent(self, obj, callback, ...)
    local btn = obj.gameObject:GetComponent("Button")
    if(btn == nil) then
        btn = obj.gameObject:AddComponent(typeof(Button))
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
function UIUtil.AddValueChangedEvent(component, callback, data)
    component.onValueChanged:RemoveAllListeners()
    component.onValueChanged:AddListener(function (t)
        --ComponentClick(component, data)
        callback(component, t, data)
    end)
end

--template 模版对象
--parent 父对象
--t table表其中包含模版对象的导出的控件元素
function UIUtil.SpawnGridItem(template, parent, t)
    local newItem = CS.UnityEngine.GameObject.Instantiate(template)
    newItem.gameObject:SetActive(true)
    newItem.transform:SetParent(parent.transform, false)
    CS.UIComponentBind.BindToLua(newItem.gameObject, t)
    newItem.transform.rotation = Quaternion.identity
    newItem.transform.localScale = Vector3.one
end

function UIUtil.ChangeImage(image, abName, spriteName, isGray)
    isGray = isGray or false
    local imageExt = image.gameObject:GetComponent(typeof(ImageExt))
    imageExt:SetSprite(abName, spriteName)
    imageExt:SetGray(isGray)
end

function UIUtil.ChangeUrlImage(image, url, isGray)
    isGray = isGray or false
    local imageExt = image.gameObject:GetComponent(typeof(ImageExt))
    imageExt:SetUrlSprite(url)
    imageExt:SetGray(isGray)
end

--通过texture名字加载本地纹理
function UIUtil.ChangeRawImage(rawImage, textureName, isGray, callBack)
    local rawImageExt = rawImage.gameObject:GetComponent(typeof(RawImageExt))
    rawImageExt:SetTexture(textureName, callBack)
    rawImageExt:SetGray(isGray)
end

--通过texture字节填充一张纹理
function UIUtil.ChangeRawImage(rawImage, textureBytes, isGray)
    local rawImageExt = rawImage.gameObject:GetComponent(typeof(RawImageExt))
    rawImageExt:SetTextureBytes(textureBytes)
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