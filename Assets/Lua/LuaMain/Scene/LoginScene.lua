local LoginScene = BaseClass("LoginScene", SceneBase)

function LoginScene:__init()
    self.mPreLoadList = {
        {abName = "", assetName = ""},
        {abName = "", assetName = ""},
    }
end

function LoginScene:__delete()

end

function LoginScene:OnEnter()

end

function LoginScene:OnExit()

end

return LoginScene