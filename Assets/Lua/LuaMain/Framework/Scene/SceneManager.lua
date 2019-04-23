local SceneManager = BaseClass("SceneManager", Singleton)

function SceneManager:__init()
	self.mCurSceneName = nil
	self.mCurScene = nil --当前场景
	self.mSwitching = false --是否在切换中
end

function SceneManager:SwitchScene(sceneConfig)
	if(sceneConfig == nil) then
		Logger.LogError(Logger.Module.SCENE, "sceneConfig is null!")
		return
	end

	if(self.mSwitching) then
		Logger.Log(Logger.Module.SCENE, "scene is switching, can`t switch scene : " .. sceneConfig.name)
		return
	end

	if(self.mCurSceneName == sceneConfig.name) then
		Logger.Log(Logger.Module.SCENE, "switch scene is same as cur scene " .. sceneConfig.name)
		return
	end

	coroutine.start(CoSwitchScene(sceneConfig))
end

function SceneManager:CoSwitchScene(sceneConfig)
	--关闭所有界面
	UIManager:GetInstance():CloseAllPanel()
	--打开loading界面
	UIManager:GetInstance():OpenPanel(Consts.UINAME.ui_loding)
	if(self.mCurScene ~= nil) then
		self.mCurScene:UnLoad()
		self.mCurScene:OnExit()
		self.mCurScene = nil
	end
	--GC
	collectgarbage("collect")
	CS.System.GC.Collect()
	collectgarbage("collect")
	CS.System.GC.Collect()
	--创建新的场景
	self.mCurScene = sceneConfig.type.New()
	--进入新的场景
	if(self.mCurScene ~= nil) then
		self.mCurSceneName = sceneConfig.name
		--预加载新场景的资源
		coroutine.yieldstart(self.mCurScene.PreLoad, function()

		end)
		--进入新的场景
		self.mCurScene:OnEnter()
		--等待新的场景完成
		coroutine.waituntil(function()
			return self.mCurScene:IsComplete()
		end )
		--关闭loading界面
		UIManager:GetInstance():ClosePanel(Consts.UINAME.ui_loding)
	end
end

return SceneManager
