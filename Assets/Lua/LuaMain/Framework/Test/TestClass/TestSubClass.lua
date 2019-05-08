local TestBaseClass = require "Test.TestClass.TestBaseClass"
local TestSubClass = BaseClass("TestSubClass", TestBaseClass)

function TestSubClass:__init(...)
    Logger.Log(Logger.Module.COMMON, "TestSubClass __init")
end

function TestSubClass:__delete()
    Logger.Log(Logger.Module.COMMON, "TestSubClass __delete")
end

function TestSubClass:Test()
    self.super.Test(self)
    Logger.Log(Logger.Module.COMMON, "TestSubClass Test" .. self.testValue)
end

return TestSubClass