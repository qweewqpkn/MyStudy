local SingletonManager = BaseClass("SingletonManager", Singleton)

function SingletonManager:__init()
    self.mInstList = {}
end

function SingletonManager:__delete()
    self.mInstList = nil
end

function SingletonManager:Register(inst)
    if(self.mInstList[inst] == nil) then
        self.mInstList[inst] = true
    else
        Logger.LogError(Logger.Module.COMMON, "SingletonManager Register same inst")
    end
end

function SingletonManager:Release()
    for k,_ in pairs(self.mInstList) do
        k:Delete()
    end

    self:Delete()
end

return SingletonManager