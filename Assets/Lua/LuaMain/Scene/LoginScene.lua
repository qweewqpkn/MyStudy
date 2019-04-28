local LoginScene = BaseClass("LoginScene", SceneBase)

function LoginScene:__init()
    self.mPreLoadList = {
        {abName = "chibang_emo", assetName = "chibang_emo"},
        {abName = "chibang_jichebao", assetName = "chibang_jichebao"},
        {abName = "chibang_jingling", assetName = "chibang_jingling"},
        {abName = "chibang_ribendao", assetName = "chibang_ribendao"},
        {abName = "chibang_tianshi", assetName = "chibang_tianshi"},
        {abName = "chibang_zhisan", assetName = "chibang_zhisan"},
        {abName = "female_body_01", assetName = "female_body_01"},
        {abName = "female_body_02", assetName = "female_body_02"},
        {abName = "female_card_01", assetName = "female_card_01"},
        {abName = "female_hair_01", assetName = "female_hair_01"},
        {abName = "female_hair_02", assetName = "female_hair_02"},
        {abName = "female_hair_03", assetName = "female_hair_03"},
        {abName = "female_head_01", assetName = "female_head_01"},
        {abName = "female_head_02", assetName = "female_head_02"},
        {abName = "female_head_03", assetName = "female_head_03"},
        {abName = "male_body_01", assetName = "male_body_01"},
        {abName = "male_body_02", assetName = "male_body_02"},
        {abName = "male_card_01", assetName = "male_card_01"},
        {abName = "male_hair_01", assetName = "male_hair_01"},
        {abName = "male_hair_02", assetName = "male_hair_02"},
        {abName = "male_hair_03", assetName = "male_hair_03"},
    }
end

function LoginScene:__delete()

end

function LoginScene:OnEnter()
    Logger.Log(Logger.Module.SCENE, "LoginScene Enter")
    ResourceManager.Instance:LoadPrefabAsync("female_hair_01", "female_hair_01", function(obj)
        if(not IsNull(obj)) then
            obj.name = "哈哈哈"
        end
    end)
end

function LoginScene:OnExit()
    Logger.Log(Logger.Module.SCENE, "LoginScene Exit")
end

function LoginScene:IsComplete()
    return true
end

return LoginScene