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
	--关闭所有界面
	UIManager:GetInstance():CloseAllPanel()
	--打开loading界面
	UIManager:GetInstance():OpenPanel(UIConfig.ui_loading.name)
	if(self.mCurScene ~= nil) then
		self.mCurScene:UnLoad()
		self.mCurScene:OnExit()
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
	self.mCurScene = sceneConfig.type.New()
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
		self.mCurScene:OnEnter()
		--等待新的场景完成
		coroutine.waituntil(function()
			return self.mCurScene:IsComplete()
		end )

		model = model + 0.1
		Messenger:GetInstance():Broadcast(MsgEnum.ui_loading_refresh, "完成", model)
		coroutine.waitforseconds(0.5)
		--关闭loading界面
		UIManager:GetInstance():ClosePanel(UIConfig.ui_loading.name)

		self.mSwitching = false
	end
end

return SceneManager
