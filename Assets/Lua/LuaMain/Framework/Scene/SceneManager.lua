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

	self.mSwitching = true
	coroutine.start(self.CoSwitchScene, self, sceneConfig)
end

function SceneManager:CoSwitchScene(sceneConfig)
	local model = 0
	local newScene = sceneConfig.type.New()

	--关闭所有界面
	UIManager:GetInstance():CloseAllPanel()
	--是否显示loading界面
	if(newScene.mIsShowLoadingUI) then
		UIManager:GetInstance():OpenPanel(UIConfig.ui_loading.name)
	end
	--退出当前界面
	if(self.mCurScene ~= nil) then
		self.mCurScene:UnLoad()
		self.mCurScene:Exit()
		self.mCurScene:Delete()
		self.mCurScene = nil
	end

	model = model + 0.1
	Messenger:GetInstance():Broadcast(MsgEnum.ui_loading_refresh, "清理资源中", model)
	--GC
	collectgarbage("collect")
	CS.System.GC.Collect()
	collectgarbage("collect")
	CS.System.GC.Collect()

	--创建新的场景
	self.mCurScene = newScene
	--进入新的场景
	if(self.mCurScene ~= nil) then
		self.mCurSceneName = sceneConfig.name
		--预加载新场景的资源
		local curProgress = model
		coroutine.yieldstart(self.mCurScene.PreLoad, function(co, progress)
			Logger.Log(Logger.Module.SCENE, "self.mCurScene.PreLoad call back " .. progress)
			model = curProgress + 0.8 * progress
			Messenger:GetInstance():Broadcast(MsgEnum.ui_loading_refresh, "加载资源中", model)
		end, self.mCurScene)
		model = curProgress + 0.8
		Messenger:GetInstance():Broadcast(MsgEnum.ui_loading_refresh, "准备场景中", model)
		--进入新的场景
		self.mCurScene:Enter()
		--等待新的场景完成
		coroutine.waituntil(function()
			return self.mCurScene:IsComplete()
		end )

		model = model + 0.1
		Messenger:GetInstance():Broadcast(MsgEnum.ui_loading_refresh, "完成", model)
		coroutine.waitforseconds(0.5)

		if(newScene.mIsShowLoadingUI) then
			UIManager:GetInstance():ClosePanel(UIConfig.ui_loading.name)
		end

		self.mSwitching = false
	end
end

return SceneManager
