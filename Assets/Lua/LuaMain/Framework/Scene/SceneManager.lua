local SceneManager = Class(Singleton)

function SceneManager:SwitchScene(...)
	local args1 = {...}
	print("交换场景成功" .. args1[1] .. args1[2])
end

return SceneManager
