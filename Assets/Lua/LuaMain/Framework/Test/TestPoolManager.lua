local TestPoolManager = BaseClass("TestPoolManager")

function TestPoolManager:__init()
    self.mList = {}
end

function TestPoolManager:Test1()
    local go = SmartGOManager:GetInstance():Spawn("cube", "cube", true)
    table.insert(self.mList, go)
    go.transform.localPosition = Vector3.New(0.0, 1.0, 0.0)
    go.name = "blood"

    local go = SmartGOManager:GetInstance():Spawn("c_female_01_ani", "c_female_01_ani", true)
end

function TestPoolManager:Test2()
    Logger.LogError(Logger.Module.COMMON, "TestPoolManager:Test2() : " .. #self.mList)
    SmartGOManager:GetInstance():Release("cube", "cube")

    SmartGOManager:GetInstance():Release("c_female_01_ani", "c_female_01_ani")


    PoolManager:GetInstance():Delete()
    --for k,v in ipairs(self.mList) do
    --    SmartGOManager:GetInstance():DeSpawn(v)
    --end
end

return TestPoolManager