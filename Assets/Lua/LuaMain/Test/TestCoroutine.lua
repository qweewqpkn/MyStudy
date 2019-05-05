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

function TestCoroutine:OnTest2(a, b, c)
    Logger.Log(Logger.Module.COMMON, string.format("TestCoroutine OnTest2 , testValue:%s a:%s, b:%s, c:%s", self.testValue, a, b, c))
    local value = coroutine.yield(2*a)
    return 1,"3", value
end

--测试原生lua协程
function TestCoroutine:Test2()
    self.testValue = 10
    local co = coroutine.create(self.OnTest2)
    local v1,v2,v3 = coroutine.resume(co, self, 1, "test")
    Logger.Log(Logger.Module.COMMON, string.format("TestCoroutine:Test2 v1:%s, v2:%s, v3:%s", v1, v2, v3))
    local v1,v2,v3,v4 = coroutine.resume(co, "第二次resume")
    Logger.Log(Logger.Module.COMMON, string.format("TestCoroutine:Test2 v1:%s, v2:%s, v3:%s v4:%s", v1, v2, v3, v4))
end

return TestCoroutine