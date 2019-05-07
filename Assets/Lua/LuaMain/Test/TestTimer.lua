local TestTimer = BaseClass("TestTimer")

function TestTimer:OnTimer1()
    Logger.Log(Logger.Module.COMMON, "OnTimer1 is call : " .. Time.realtimeSinceStartup)
end

function TestTimer:OnTimer2()
    Logger.Log(Logger.Module.COMMON, "OnTimer2 is call, cur frame : " .. Time.frameCount)
end

function TestTimer:OnTimer3()
    Logger.Log(Logger.Module.COMMON, "Upadte Timer is call : ")
end

function TestTimer:OnTimer4()
    Logger.Log(Logger.Module.COMMON, "Fixed Timer is call : ")
end

function TestTimer:OnTimer5()
    Logger.Log(Logger.Module.COMMON, "Late Timer is call : ")
end

function TestTimer:OnTimer6()
    if(self.count == 10) then
        self.timer1:Stop()
        return
    end

    self.count = self.count + 1
    Logger.Log(Logger.Module.COMMON, "OnTimer6 is call : " .. self.count)
end

function TestTimer:Test1()
    --无限循环，每隔0.5s调用一次OnTimer1
    local timer = TimerManager:GetInstance():GetTimer(0.5, TestTimer.OnTimer1, self, -1, false, false)
    timer:Start()
end

function TestTimer:Test2()
    --无限循环，每隔10帧调用一次OnTimer1
    local timer = TimerManager:GetInstance():GetTimer(10, TestTimer.OnTimer2, self, -1, true, false)
    timer:Start()
end

function TestTimer:Test3()
    --测试调用的先后顺序
    local timer1 = TimerManager:GetInstance():GetTimer(10, TestTimer.OnTimer3, self, -1, true, false)
    local timer2 = TimerManager:GetInstance():GetFixedTimer(10, TestTimer.OnTimer4, self, -1, true, false)
    local timer3 = TimerManager:GetInstance():GetLateTimer(10, TestTimer.OnTimer5, self, -1, true, false)
    timer1:Start()
    timer2:Start()
    timer3:Start()
end

function TestTimer:Test4()
    self.count = 1
    self.timer1 = TimerManager:GetInstance():GetTimer(1, TestTimer.OnTimer6, self, -1, false, false)
    self.timer1:Start()
end

return TestTimer