local SceneManager = BaseClass("SceneManager", Singleton)

function SceneManager:__init()
	self.mCurSceneName = nil
	self.mCurScene = nil			--当前场景
	self.mSwitching = false			--是否在切换中
	self.mSwitchAnimation = nil 	--场景切换动画 by lpf - 20191015
end

--sceneConfig 在SceneConfig中配置的数据
--preloadList 由外部传入的预加载资源列表
function SceneManager:SwitchScene(sceneConfig, preloadList)
	if(sceneConfig == nil) then
		Logger.LogError(Logger.Module.SCENE, "sceneConfig is null!")
		return false
	end

	if(self.mSwitching) then
		Logger.Log(Logger.Module.SCENE, "scene is switching, can`t switch scene : " .. sceneConfig.name)
		return false
	end

	if(self.mCurSceneName == sceneConfig.name) then
		Logger.Log(Logger.Module.SCENE, "switch scene is same as cur scene " .. sceneConfig.name)
		return false
	end

	self.mSwitching = true

	-- 播放退出动画 by lpf 20191015
	if self.mCurScene ~= nil and
		self.mCurScene.mIsShowExitAnimation then
		self:OnPlayExitAnimation()
		local timer = TimerManager:GetInstance():GetTimer(1.0,
			function()
				self.mSwitchAnimation.gameObject:SetActive(false)
                coroutine.start(self.CoSwitchScene, self, sceneConfig, preloadList)
				Messenger:GetInstance():Broadcast(MsgEnum.GUIDE_SCENE_EXIT_ANIMATION)
            end, nil, 1, false)
		timer:Start()
	else
		coroutine.start(self.CoSwitchScene, self, sceneConfig, preloadList)
	end
	return true
end

function SceneManager:CoSwitchScene(sceneConfig, preloadList)
	local model = 0
	local newScene = sceneConfig.type.New()

	--关闭和释放所有音乐
	AudioMgr:GetInstance():StopAndDisposeAll()

	--退出当前界面
	if(self.mCurScene ~= nil) then
		--关闭界面
		self.mCurScene:OnClosePanel()
		self.mCurScene:UnLoad()
		self.mCurScene:Exit()
		self.mCurScene:Delete()
		self.mCurScene = nil
	end

	--是否显示loading界面
	if(newScene.mIsShowLoadingUI) then
		UIManager:GetInstance():OpenPanel(UIConfig.ui_loading.name)
	end

	model = model + 0.1
	Messenger:GetInstance():Broadcast(MsgEnum.ui_loading_refresh, UIUtil.GetGameText("lc_common_loading_1"), model)--"清理资源中"
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
			--Logger.Log(Logger.Module.SCENE, "self.mCurScene.PreLoad call back " .. progress)
			model = curProgress + 0.8 * progress
			Messenger:GetInstance():Broadcast(MsgEnum.ui_loading_refresh, UIUtil.GetGameText("lc_common_loading_2"), model)--"加载资源中"
		end, self.mCurScene, preloadList)
		model = curProgress + 0.8
		Messenger:GetInstance():Broadcast(MsgEnum.ui_loading_refresh, UIUtil.GetGameText("lc_common_loading_3"), model)--"准备场景中"
		
		-- 播放进入动画 by lpf 20191015
		if newScene.mIsShowEntryAnimation then
			self:OnPlayEntryAnimation()
			local timer = TimerManager:GetInstance():GetTimer(1.0,
					function()
						Messenger:GetInstance():Broadcast(MsgEnum.GUIDE_SCENE_ENTRY_ANIMATION)
					end, nil, 1, false)
			timer:Start()
		end
		
		--进入新的场景
		self.mCurScene:Enter()
		--等待新的场景完成
		coroutine.waituntil(function()
			return self.mCurScene:IsComplete()
		end )

		model = model + 0.1
		Messenger:GetInstance():Broadcast(MsgEnum.ui_loading_refresh, UIUtil.GetGameText("lc_common_loading_4"), model)--"完成"
		--coroutine.waitforseconds(0.5)

		if(newScene.mIsShowLoadingUI) then
			UIManager:GetInstance():ClosePanel(UIConfig.ui_loading.name)
		end

		self.mSwitching = false

		Messenger:GetInstance():Broadcast(MsgEnum.GUIDE_ENTER_SCENE, sceneConfig.name)
	end
end

function SceneManager:OnStart(dt)
	if self.mCurScene ~= nil then
		self.mCurScene:OnStart(dt)
	end
end

function SceneManager:OnPlayEntryAnimation()
	self:CheckSwitchAnimation()
	self.mSwitchAnimation.gameObject:SetActive(true)
	self.mSwitchAnimation:Play("map01_cloud_entrance");
end

function SceneManager:OnPlayExitAnimation()
	self:CheckSwitchAnimation()
	self.mSwitchAnimation.gameObject:SetActive(true)
	self.mSwitchAnimation:Play("map01_cloud_exit");
end

function SceneManager:Reset()
	if self.mCurScene ~= nil then
		self.mCurScene:Reset()
	end
end

function SceneManager:CheckSwitchAnimation()
	if self.mSwitchAnimation ~= nil then
		return
	end

	local go = ResourceManager.instance:LoadPrefab("cloud_camera", "cloud_camera")
	self.mSwitchAnimation = go:GetComponent(typeof(CS.UnityEngine.Animation))
end

return SceneManager
