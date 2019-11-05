local CountDownTimer = BaseClass("CountDownTimer")

--构造函数
function CountDownTimer:__init(...)
	self.m_oritotalTime = 0
	self.m_totalTime = 0
	self.m_elapsedTime = 0
end

function CountDownTimer:Delete()
	self.m_textMc = nil
	self.m_callBack = nil
	self.m_intervalCb = nil
end

function CountDownTimer:IntervalCallBack()
	self.m_elapsedTime = self.m_elapsedTime + Time.deltaTime * 1000

	if self.m_elapsedTime >= self.m_oritotalTime then
		if self.m_callBack ~= nil then
			self.m_callBack()
			self.m_callBack = nil
		end
		if self.m_timer ~= nil then
			self.m_timer:Stop()
			self.m_timer = nil
		end
	else
		if self.m_intervalCb ~= nil then
			self.m_intervalCb()
		end		
	end
	
	if self.m_textMc ~= nil then
		self.m_textMc.text = TimeStrUtil.TimeToString2(math.round(math.max(0, self.m_totalTime - self.m_elapsedTime)/1000))
	end
end

function CountDownTimer:SetUp(totalTime, textMc, cb, intervealCb, caller, ...) 
	self.m_oritotalTime = totalTime
	self.m_totalTime = totalTime
	self.m_textMc = textMc
	
	self.params = SafePack(...)
	self.m_callBack = cb and (function() cb(caller, SafeUnpack(self.params)) end)
	self.m_intervalCb = intervealCb and (function() intervealCb(caller, SafeUnpack(self.params)) end)

	self.m_timer = TimerManager:GetInstance():GetTimer(1, self.IntervalCallBack, self, -1, true, false)
	self.m_timer:Start()

	if self.m_textMc ~= nil then
		self.m_textMc.text = TimeStrUtil.TimeToString2(math.round(self.m_totalTime/1000))
	end
end

function CountDownTimer:ReduceTime(t)
	self.m_totalTime = self.m_oritotalTime - t
end
	
function CountDownTimer:ReduceTimeReal(t)
	self.m_oritotalTime = self.m_oritotalTime - t
	self.m_totalTime = self.m_oritotalTime
end

function CountDownTimer:IsTimeOut() 
	return self.m_elapsedTime >= self.m_totalTime
end
	
function CountDownTimer:GetTimeLeft()
	return self.m_totalTime - self.m_elapsedTime 
end

function CountDownTimer:Stop()
	if self.m_timer ~= nil then
		self.m_timer:Stop()
		self.m_timer = nil
	end
	self:Delete()
end

return CountDownTimer