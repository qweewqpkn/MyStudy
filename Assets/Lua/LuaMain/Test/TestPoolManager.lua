local TestPoolManager = BaseClass("TestPoolManager")

function TestPoolManager:Test1()
    ResourceManager.Instance:LoadPrefabAsync("sparkle_green", "sparkle_green", function(obj)
        local poolGO = PoolManager:GetInstance():GetPoolGO(obj, 10, function(obj1)
            obj1.transform.localPosition = Vector3.New(0.0, 1.0, 0.0)
            obj1.name = "blood"
        end)
        local item = poolGO:Spawn()
    end)
end

return TestPoolManager