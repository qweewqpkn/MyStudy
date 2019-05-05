local TestBaseClass = BaseClass("TestBaseClass")

function TestBaseClass:__init(...)
    self.testValue = 10
    local t = SafePack(...)
    for k,v in ipairs(t) do
        Logger.Log(Logger.Module.COMMON, string.format("参数%s是%s", k, v))
    end
    Logger.Log(Logger.Module.COMMON, "TestBaseClass __init")
end

function TestBaseClass:__delete()
    Logger.Log(Logger.Module.COMMON, "TestBaseClass __delete")
end

function TestBaseClass:Test()
    self.testValue = 100
    Logger.Log(Logger.Module.COMMON, "TestBaseClass Test")
end

return TestBaseClass