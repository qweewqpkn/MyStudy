local TestUpdate = BaseClass("TestUpdate", Updatable)

function TestUpdate:Update()
    Logger.Log(Logger.Module.COMMON, "Update is call hahaha")
end

return TestUpdate