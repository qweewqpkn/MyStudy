local TestCoroutine = BaseClass("TestCoroutine")

function TestCoroutine:OnTest1(id, name)
    Logger.Log(Logger.Module.UI, "协程开始, 参数 : " .. id .. name)
    coroutine.waitforseconds(1)

    Logger.Log(Logger.Module.UI, "当前" .. CS.UnityEngine.Time.frameCount)
    coroutine.waitforframes(5)
    Logger.Log(Logger.Module.UI, "然后" .. CS.UnityEngine.Time.frameCount)
    coroutine.waitforendofframe()
    Logger.Log(Logger.Module.UI, "然后" .. CS.UnityEngine.Time.frameCount)

    local i = 0
    coroutine.waituntil(function()
        Logger.Log(Logger.Module.UI, i)
        i = i + 1
        if(i == 100) then
            return true
        end
        return false
    end)

    local request = ResourceManager.Instance:LoadPrefabRequest("sparkle_green", "sparkle_green")
    coroutine.waitforasyncop(request, function()
        Logger.Log(Logger.Module.UI, "异步加载中.....")
    end)
    local obj = request.Asset
    obj.name = "测试啦"
    Logger.Log(Logger.Module.UI, "结果来了" .. CS.UnityEngine.Time.frameCount)
end

function TestCoroutine:Test1()
    coroutine.start(self.OnTest1, self, 123, "你好")
end

return TestCoroutine