local TestWeakTable = BaseClass("TestWeakTable")

function TestWeakTable:Test1()
    self.mStrongTable = {}
    self.mStrongTable[1] = {1, 2, 3}
    self.mStrongTable.obj = function() Logger.E("123213") end
    self.mStrongTable[3] = 5
    --self.mRef = self.mStrongTable[1]
    self.mRef1 = self.mStrongTable.obj
    setmetatable(self.mStrongTable, {__mode = "v"})
    Logger.E("TestWeakTable:Test1() mStrongTable num : " .. table.count(self.mStrongTable))
    collectgarbage("collect")
    Logger.E("TestWeakTable:Test1() mStrongTable num : " .. table.count(self.mStrongTable))
    Logger.E(table.dump(self.mStrongTable))
end

return TestWeakTable