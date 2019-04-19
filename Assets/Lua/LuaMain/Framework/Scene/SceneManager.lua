local SceneManager = BaseClass("SceneManager", Singleton)

function SceneManager:SwitchScene(...)
	local args1 = {...}
	Logger.Log(Logger.Module.SCENE, "交换场景成功" .. args1[1] .. args1[2])
end

return SceneManager
