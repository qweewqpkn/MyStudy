local TestScene = BaseClass("TestScene")

function TestScene:Test1()
    SceneManager:GetInstance():SwitchScene(SceneConfig.LoginScene)
end

return TestScene