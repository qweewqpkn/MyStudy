local UIUtil = BaseClass("UIUtil")

UIUtil.ButtonEventType = {
    PointerClick = "PointerClick",
    PointerDown = "PointerDown",
    PointerUp = "PointerUp",
    PointerEnter = "PointerEnter",
    PointerExit = "PointerExit",
}

function UIUtil.CommonClick(obj)
    UIUtil.AutoUserTrack(obj)
    AudioMgr:GetInstance():Play2D("BtnClick")
    GuideMgr:GetInstance():ClickCommon()
    Messenger:GetInstance():Broadcast(MsgEnum.GUIDE_CLICK_BUTTON, obj.gameObject.name)
end

--是否点击有效
UIUtil.mLastClickTime = 0
UIUtil.mClickGapTime = 0.25
UIUtil.mCacheType = nil
function UIUtil.IsClickValid(type)
    if(UIUtil.mCacheType == type) then
        if(Time.realtimeSinceStartup > UIUtil.mLastClickTime) then
            UIUtil.mLastClickTime = Time.realtimeSinceStartup + UIUtil.mClickGapTime
            return true
        else
            return false
        end
    else
        UIUtil.mCacheType = type
        return true
    end
end

--引导中，只能点击引导指定的按钮
function UIUtil.IsGuideLock(name)
    if(GuideMgr:GetInstance().net_failed)then
        return false
    end
    if(GuideMgr:GetInstance().guide_only_button == nil)then
        return false--引导没有指定按钮
    end
    CHUtil.Log("######guide_only_button="..GuideMgr:GetInstance().guide_only_button.."__name="..name)
    --指定了按钮
    return GuideMgr:GetInstance().guide_only_button ~= name
end

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
            if(UIUtil.IsGuideLock(obj.gameObject.name))then
                CHUtil.Log("###########Click Event Ignore For Guide.")
                return
            end
            if(UIUtil.IsClickValid()) then
                UIUtil.CommonClick(obj)
                callback(self, obj.gameObject, SafeUnpack(params))
            end
        end)
    else
        local pointer = obj.gameObject:GetComponent("MouseEventPointer")
        if(IsNull(pointer)) then
            pointer = obj.gameObject:AddComponent(typeof(CS.MouseEventPointer))
        end

        local action = function (go, eventData)
            if(UIUtil.IsGuideLock(obj.gameObject.name))then
                CHUtil.Log("###########Click Event Ignore For Guide.")
                return
            end
            if(UIUtil.IsClickValid(type)) then
                UIUtil.CommonClick(obj)
                callback(self, go, eventData, SafeUnpack(params))
            end
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

function UIUtil.AutoUserTrack(obj)
    local userTrack = obj.gameObject:GetComponent("UserTrack")
    if(IsNull(userTrack)) then
        return
    end
    if(userTrack.m_track_id == "")then
        return
    end
    UserTrackMgr:GetInstance():UserActionTrack(userTrack.m_track_id)
end


UIUtil.DragEventType = {
    Drag = 1,
    BeginDrag = 2,
    EndDrag = 3,
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

function UIUtil.AddSliderValueChanged(obj, callback, self, ...)
    if(IsNull(obj)) then
        Logger.LogError(Logger.Module.COMMON, "UIUtil.AddValueChangedEvent obj is null")
        return
    end

    local slider = obj.gameObject:GetComponent("Slider")
    if(IsNull(slider)) then
        Logger.LogError(Logger.Module.COMMON, "UIUtil.AddValueChangedEvent slider is null")
        return
    end

    local params = SafePack(...)
    slider.onValueChanged:RemoveAllListeners()
    slider.onValueChanged:AddListener(function (value)
        --ComponentClick(component, data)
        callback(self, slider, value, SafeUnpack(params))
    end)
end

--销毁这个模板的所有实例对象
function UIUtil.DestroyGridAllItem(template)
    if(IsNull(template)) then
        Logger.LogError(Logger.Module.UI, "DestroyGridAllItem template is nil")
        return
    end

    PoolManager:GetInstance():Release(template)
end

--template 模版对象
--parent 父对象
--这里内部使用了对象池，所以你得配合DestroyGridAllItem 和 DeSpawnGridAllItem使用
function UIUtil.SpawnGridItem(template, parent)
    if(IsNull(template)) then
        Logger.LogError(Logger.Module.UI, "UIUtil.SpawnGridItem template is nil")
        return
    end

    if(IsNull(parent)) then
        Logger.LogError(Logger.Module.UI, "UIUtil.SpawnGridItem  parent is nil")
        return
    end

    local t = {}
    local newItem = PoolManager:GetInstance():Spawn(template)
    newItem.gameObject:SetActive(true)
    newItem.transform:SetParent(parent.transform, false)
    CS.UIComponentBind.BindToLua(newItem.gameObject, t)
    newItem.transform.anchoredPosition = Vector2(0, 0)
    newItem.transform.rotation = Quaternion.identity
    newItem.transform.localScale = Vector3.one
    return t
end

function UIUtil.DeSpawnGridItem(inst)
    PoolManager:GetInstance():DeSpawn(inst)
end

--回收这个模板对象的所有实例
function UIUtil.DeSpawnGridAllItem(template)
    if(IsNull(template)) then
        Logger.LogError(Logger.Module.UI, "DeSpawnGridAllItem template is nil")
        return
    end

    PoolManager:GetInstance():DeSpawnTemplate(template)
end

--回收这个父grid对象下的所有子元素
function UIUtil.DeSpawnGridAllItemByParent(grid)
    if(IsNull(grid)) then
        Logger.LogError(Logger.Module.UI, "DeSpawnGridAllItemByParent grid is nil")
        return
    end

    local transform = grid.transform
    local count = transform.childCount
    local despawnList = {}
    if(count ~= 0) then
        for i = 0, count-1 do
            local childTrans = transform:GetChild(i)
            table.insert(despawnList, childTrans)
        end

        for k,v in ipairs(despawnList) do
            PoolManager:GetInstance():DeSpawn(v)
        end
    end
end

function UIUtil.SetSprite(image, abName, spriteName, isGray)
    isGray = isGray or false
    local imageExt = image.gameObject:GetComponent(typeof(CS.ImageExt))
    if(imageExt ~= nil) then
        imageExt:SetSprite(abName, spriteName)
        imageExt:SetGray(isGray)
    end
end

function UIUtil.SetSpriteByCfg(image, name, isGray)
    --if(name == nil) then
    --    return
    --end

    local atlasAndSprite = string.split(name, "|")
    UIUtil.SetSprite(image, atlasAndSprite[1], atlasAndSprite[2], isGray)
end

--通过texture名字加载本地纹理
function UIUtil.SetRawImage(rawImage, textureName, isGray, callBack)
    local rawImageExt = rawImage.gameObject:GetComponent(typeof(CS.RawImageExt))
    if(rawImageExt ~= nil) then
        rawImageExt:SetTexture(textureName, callBack)
        rawImageExt:SetGray(isGray)
    end
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
    isGray = isGray or false
    local imageExt = image.gameObject:GetComponent(typeof(CS.ImageExt))
    imageExt:SetGray(isGray)
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

--size 0 是小图标 1是大图标
function UIUtil.SetArmyType(icon, type, size)
    local armyCfgData = ArmyCfgMgr.Get(type)
    if(armyCfgData ~= nil) then
        if(icon~= nil) then
            local iconData = string.split(armyCfgData.icon, "|")
            if(size == 0) then
                UIUtil.SetSprite(icon, iconData[1], iconData[2])
            else
                UIUtil.SetSprite(icon, iconData[1], iconData[2] .. "_m")
            end
        end
    end

end

function UIUtil.SetStar(obj, star)
    local count = obj.transform.childCount
    for i = 1, count do
        local childObj = obj.transform:Find(string.format("star_%d", i))
        GoUtil.SetActive(childObj, true)
        local imageExt = childObj.gameObject:GetComponent(typeof(CS.ImageExt))
        if(i > star) then
            imageExt.color = Color(1, 1, 1, 0.3)
        else
            imageExt.color = Color(1, 1, 1, 1)
        end
    end
end

function UIUtil.SetStarNew(obj, star)
    local count = obj.transform.childCount
    for i = 1, count do
        local childObj = obj.transform:Find(string.format("star_%d", i))
        local rawImageExt = childObj.gameObject:GetComponent(typeof(CS.ImageExt))
        if(i <= star) then
            if(rawImageExt ~= nil)then
                rawImageExt:SetSprite("ui_hero_atlas_v2", "ui_hero_stone")
            else
                GoUtil.SetActive(childObj, true)
            end
        else
            if(rawImageExt ~= nil)then
                rawImageExt:SetSprite("ui_hero_atlas_v2", "ui_hero_stoneshadow")
            else
                GoUtil.SetActive(childObj, false)
            end
        end
    end
end

function UIUtil.SetStarGray(obj, isGray)
    local count = obj.transform.childCount
    for i = 1, count do
        local childObj = obj.transform:Find(string.format("star_%d", i))
        local rawImageExt = childObj.gameObject:GetComponent(typeof(CS.ImageExt))
        if(rawImageExt ~= nil)then
            rawImageExt:SetGray(isGray)
        end
    end
end

function UIUtil.SetItemQuality(icon, itemCfg)
    if(itemCfg ~= nil) then
        if(itemCfg.op_type == Consts.ItemType.Hero) then
            local heroCfgData = HeroCfgMgr.Get(itemCfg.op_val)
            if(heroCfgData ~= nil) then
                UIUtil.SetSprite(icon, "ui_hero_atlas_v2_5", "ui_hero_quality0" .. heroCfgData.quality)
            end
        elseif(itemCfg.op_type == Consts.ItemType.Rune) then
            local runeCfgData = SkillCfgMgr.GetRuneBase(itemCfg.op_val)
            local skillCfgData = SkillCfgMgr.Get(runeCfgData.skill_id)
            if(skillCfgData ~= nil) then
                UIUtil.SetSprite(icon, "ui_hero_atlas_v2_5", "ui_hero_quality0" .. skillCfgData.star)
            end
        end
    end
end

function UIUtil.SetHeroQuality(icon, quality, isBig, isGray)
    local qualityName = nil
    if(isBig) then
        qualityName = "ui_hero_card0" .. quality
    else
        qualityName = "ui_hero_quality0" .. quality
    end

    UIUtil.SetSprite(icon, "ui_hero_atlas_v2_5", qualityName, isGray)
end

--设置英雄品质图标
function UIUtil.SetAttriIcon(icon, attri)
    if(attri == Consts.StarAttrType.ad) then
        UIUtil.SetSprite(icon, Consts.Atlas.ui_hero_atlas_v2_5, "ui_hero_icon_atn")
    elseif(attri == Consts.StarAttrType.ap) then
        UIUtil.SetSprite(icon, Consts.Atlas.ui_hero_atlas_v2_5, "ui_hero_icon_int")
    elseif(attri == Consts.StarAttrType.addef) then
        UIUtil.SetSprite(icon, Consts.Atlas.ui_hero_atlas_v2_5, "ui_hero_icon_def")
    elseif(attri == Consts.StarAttrType.apdef) then
        UIUtil.SetSprite(icon, Consts.Atlas.ui_hero_atlas_v2_5, "ui_hero_icon_res")
    elseif(attri == Consts.StarAttrType.siege) then
        UIUtil.SetSprite(icon, Consts.Atlas.ui_hero_atlas_v2_5, "ui_hero_icon_aac")
    elseif(attri == Consts.StarAttrType.speed) then
        UIUtil.SetSprite(icon, Consts.Atlas.ui_hero_atlas_v2_5, "ui_hero_icon_spd")
    end
end

--RES_WOOD    = 1, -- 木头
--RES_STONE   = 2, -- 石头
--RES_IRON    = 3, -- 铁矿
--RES_FOOD    = 4, -- 粮食
--RES_MONEY   = 5, -- 铜钱
--RES_DECREE  = 6, -- 政令
--RES_FAME    = 7, -- 名望
--RES_DIAMOND = 8, -- 钻石
--设置资源icon(钻石，粮食，金币等)
--type 0是small 1是big
function UIUtil.SetResIcon(icon, resType, type)
    type = type or 1
    local iconName = ""
    if(resType == GLobalConst.RES_WOOD) then
        if(type == 0) then
            iconName = "ui_com_res_oil_s"
        else
            iconName = "ui_com_res_oil"
        end
    elseif(resType ==GLobalConst.RES_STONE) then
        if(type == 0) then
            iconName = "ui_com_res_water_s"
        else
            iconName = "ui_com_res_water"
        end
    elseif(resType == GLobalConst.RES_IRON) then
        if(type == 0) then
            iconName = "ui_com_res_ore_s"
        else
            iconName = "ui_com_res_ore"
        end
    elseif(resType == GLobalConst.RES_FOOD) then
        if(type == 0) then
            iconName = "ui_com_res_metal_s"
        else
            iconName = "ui_com_res_metal"
        end
    elseif(resType == GLobalConst.RES_MONEY) then
        if(type == 0) then
            iconName = "ui_com_res_money_s"
        else
            iconName = "ui_com_res_money"
        end
    elseif(resType == GLobalConst.RES_DECREE) then
        if(type == 0) then
            iconName = "ui_com_res_money_s" --暂时使用
        else
            iconName = "ui_com_res_money"  --暂时使用
        end
    elseif(resType == GLobalConst.RES_FAME) then
        if(type == 0) then
            iconName = "ui_com_res_money_s"  --暂时使用
        else
            iconName = "ui_com_res_money"  --暂时使用
        end
    elseif(resType == GLobalConst.RES_DIAMOND) then
        if(type == 0) then
            iconName = "ui_com_res_gem_s"
        else
            iconName = "ui_com_res_gem"
        end
    end

    UIUtil.SetSprite(icon, Consts.Atlas.ui_common_atlas_v2_5, iconName)
end

--转换资源类型为id
function UIUtil.ConvertResTypeToItemID(resType)
    local id = 0
    if(resType == GLobalConst.RES_WOOD) then
        id = 49
    elseif(resType ==GLobalConst.RES_STONE) then
        id = 50
    elseif(resType == GLobalConst.RES_IRON) then
        id = 51
    elseif(resType == GLobalConst.RES_FOOD) then
        id = 52
    elseif(resType == GLobalConst.RES_MONEY) then
        id = 53
    elseif(resType == GLobalConst.RES_DECREE) then
        id = 53  --暂时使用
    elseif(resType == GLobalConst.RES_FAME) then
        id = 53  --暂时使用
    elseif(resType == GLobalConst.RES_DIAMOND) then
        id = 54
    end

    return id
end

function UIUtil.SetItemIcon(icon, itemCfg, isBig)
    if(isBig == nil) then
        isBig = true
    end

    if(itemCfg ~= nil) then
        local iconName = ""
        if(isBig) then
            iconName = itemCfg.icon
        else
            iconName = itemCfg.small_icon
        end

        if(itemCfg.op_type == Consts.ItemType.Hero) then
            UIUtil.SetRawImage(icon, iconName)
        else
            if(iconName ~= nil) then
                local resultList = string.split(iconName, "|")
                local atlas = resultList[1]
                local name = resultList[2]
                UIUtil.SetSprite(icon, atlas, name)
            else
                UIUtil.SetSprite(icon, "", "")
            end
        end
    end
end

--根据品质设置名字
function UIUtil.SetNameByQuality(txt, name, quality)
    if(IsNull(txt)) then
        Logger.E("UIUtil.SetNameByQuality txt is nil")
        return
    end

    if(quality == Consts.Quality.white) then
        txt.text = string.format("<color=#e7deae>%s</color>", name)
    elseif(quality == Consts.Quality.green) then
        txt.text = string.format("<color=#0db841>%s</color>", name)
    elseif(quality == Consts.Quality.blue) then
        txt.text = string.format("<color=#00a0f3>%s</color>", name)
    elseif(quality == Consts.Quality.purple) then
        txt.text = string.format("<color=#ed06f0>%s</color>", name)
    elseif(quality == Consts.Quality.orange) then
        txt.text = string.format("<color=#ffa621>%s</color>", name)
    end
end

--设置武将头像边框（觉醒）
function UIUtil.SetHeroHeadSide(imageExt, isAwake)
    if(isAwake)then
        UIUtil.SetSprite(imageExt, Consts.BattleReport.hero_ab_name, "image_avatar_board02")
    else
        UIUtil.SetSprite(imageExt, Consts.BattleReport.hero_ab_name, "image_avatar_board01")
    end
end

--提示
function UIUtil.ShowTips(msg)
    UIManager:GetInstance():OpenPanel(UIConfig.ui_confirm.name, {title = UIUtil.GetGameText("lc_ui_battlereport_jumptitle"), desc = msg, buttons = {UIUtil.GetGameText("lc_ui_ui_common_confirm")}})
end

function UIUtil.ShowItemTips(id, eventData)
    if(eventData == nil) then
        Logger.E("UIUtil.ShowItemTips eventData is nil")
        return
    end

    local itemCfg = ItemCfgMgr.Get(id)
    if(itemCfg ~= nil) then
        local atlas = ""
        local icon = ""
        if(itemCfg.icon ~= nil) then
            local iconData = string.split(itemCfg.icon, "|")
            atlas = iconData[1]
            icon = iconData[2]
        end
        local tipInfo = {title = UIUtil.GetGameText(itemCfg.name),
                         des = UIUtil.GetGameText(itemCfg.des),
                         atlas = atlas, icon = icon, data = eventData}
        UIManager:GetInstance():OpenPanel(UIConfig.ui_tip.name, tipInfo)
    end
end

--飘框
function UIUtil.ShowFloatWindow(tips)
    FloatWindowMgr:GetInstance():Show(tips)
end

--功能开发中
function UIUtil.Develop()
    UIUtil.ShowTips(MultiLanguageMgr:GetInstance():GetGameText("lc_ui_coming_soon"))
end
--获取本地化文本
function UIUtil.GetGameText(key,...)
    local params = SafePack(...)
    return MultiLanguageMgr:GetInstance():GetGameText(key,SafeUnpack(params))
end

--显示获得资源的飘窗
function UIUtil.ShowResTips(resList)
    local count = 1
    local OnTips = function()
        ResourceManager.instance:LoadPrefabAsync("fx_ui_res_get_tips", "fx_ui_res_get_tips", function(go)
            local t = {}
            GoUtil.SetParent(go, UIManager:GetInstance().mUIRoot, false)
            go.transform.anchorMin = Vector2(0.5, 0.5)
            go.transform.anchorMax = Vector2(0.5, 0.5)
            go.transform.anchoredPosition = Vector2(0, 0)
            CS.UIComponentBind.BindToLua(go, t)
            local itemCfg = ItemCfgMgr.Get(resList[count].id)
            if(itemCfg ~= nil) then
                UIUtil.SetItemIcon(t.b_icon, itemCfg)
            end
            t.b_num.text = resList[count].num
            count = count + 1

            local timer = TimerManager:GetInstance():GetTimer(2, function()
                CS.UnityEngine.GameObject.Destroy(go)
            end, nil, 1, false)
            timer:Start()
        end)
    end

    OnTips()
    if(#resList > 1) then
        local timer = TimerManager:GetInstance():GetTimer(0.6, function()
            OnTips()
        end, nil, #resList - 1, false)
        timer:Start()
    end
end

--检测资源是否足够，并做提示
function UIUtil.CheckResNum(resType, resNeedNum, tips)
    local isEnough = true
    resNeedNum = Mathf.Floor(resNeedNum)
    if(resType == GLobalConst.RES_MONEY) then
        if(resNeedNum > UserResMgr:GetInstance().money) then
            if(tips ~= nil) then
                tips = string.format("需要%s金币", resNeedNum) .. tips
            else
                tips = string.format("需要%s金币", resNeedNum)
            end
            isEnough = false
        end
    elseif(resType == GLobalConst.RES_DIAMOND) then
        if(resNeedNum > UserResMgr:GetInstance().diamond) then
            if(tips ~= nil) then
                tips = string.format(UIUtil.GetGameText("lc_ui_event_buy_fail1"), resNeedNum) .. tips
            else
                tips = string.format(UIUtil.GetGameText("lc_ui_event_buy_fail1"), resNeedNum)
            end
            isEnough = false
        end
    end

    if(not isEnough and tips ~= nil) then
        UIUtil.ShowFloatWindow(tips)
        --UIUtil.ShowTips(tips)
    end

    return isEnough
end

function UIUtil.WorldPos2UIPos(sceneCamera, uiCamera, worldPos, uiParentTrans)
    local screenPos = sceneCamera:WorldToScreenPoint(worldPos)
    local isHit, localPos = CS.UnityEngine.RectTransformUtility.ScreenPointToLocalPointInRectangle(uiParentTrans, Vector2(screenPos.x, screenPos.y), uiCamera)
    return localPos
end

function UIUtil.TweenPosition(target,start_pos,end_pos,duration,call_back,self)
    target.transform.position = start_pos
    target.transform:DOMove(end_pos, duration):OnComplete(function()
        call_back(self)
    end)
end

return UIUtil