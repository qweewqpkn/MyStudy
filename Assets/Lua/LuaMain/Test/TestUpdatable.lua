local TestUpdatable = BaseClass("TestUpdatable", Updatable)

function  TestUpdatable:Update()
    Logger.Log(Logger.Module.COMMON, "TestUpdatable Update")
end

function  TestUpdatable:LateUpdate()
    Logger.Log(Logger.Module.COMMON, "TestUpdatable LateUpdate")
end

function  TestUpdatable:FixedUpdate()
    Logger.Log(Logger.Module.COMMON, "TestUpdatable FixedUpdate")
end

return TestUpdatable