local TestMessenger = BaseClass("TestMessenger")

function TestMessenger:OnProcess1(...)
    Logger.Log(Logger.Module.COMMON, "TestMessenger:OnProcess1")
    local args = SafePack(...)
    for k,v in ipairs(args) do
        Logger.Log(Logger.Module.COMMON, v)
    end
end

function TestMessenger:OnProcess2(...)
    Logger.Log(Logger.Module.COMMON, "TestMessenger:OnProcess2")
end

function TestMessenger:OnProcess3()
    Logger.Log(Logger.Module.COMMON, "TestMessenger:OnProcess3")
end

function TestMessenger:Test1()
    Messenger:GetInstance():Register(MsgEnum.UI_LOADINING_REFRESH, TestMessenger.OnProcess1, self)
    Messenger:GetInstance():Register(MsgEnum.UI_LOADINING_REFRESH, TestMessenger.OnProcess2, self)
    Messenger:GetInstance():Broadcast(MsgEnum.UI_LOADINING_REFRESH, "数据啦1", 123)
    Messenger:GetInstance():RemoveByTarget(self)
    Messenger:GetInstance():Broadcast(MsgEnum.UI_LOADINING_REFRESH, "数据啦2", 456)
    Messenger:GetInstance():Register(MsgEnum.UI_LOADINING_REFRESH, TestMessenger.OnProcess1, self)
    Messenger:GetInstance():Broadcast(MsgEnum.UI_LOADINING_REFRESH, "数据啦3", 456)
end

function TestMessenger:Test2()
    Messenger:GetInstance():Register(MsgEnum.UI_LOADINING_REFRESH, TestMessenger.OnProcess1, self)
    Messenger:GetInstance():Broadcast(MsgEnum.UI_LOADINING_REFRESH, "数据啦1", 123)
    Messenger:GetInstance():RemoveByID(MsgEnum.UI_LOADINING_REFRESH)
    Messenger:GetInstance():Broadcast(MsgEnum.UI_LOADINING_REFRESH, "数据啦1", 123)
end

function TestMessenger:Test3()
    Messenger:GetInstance():Register(MsgEnum.UI_LOADINING_REFRESH, TestMessenger.OnProcess1, self)
    Messenger:GetInstance():Register(MsgEnum.UI_LOADINING_REFRESH, TestMessenger.OnProcess2, self)
    Messenger:GetInstance():Broadcast(MsgEnum.UI_LOADINING_REFRESH, "数据啦1", 123)
    Messenger:GetInstance():RemoveByFunc(MsgEnum.UI_LOADINING_REFRESH, TestMessenger.OnProcess1)
    Messenger:GetInstance():Broadcast(MsgEnum.UI_LOADINING_REFRESH, "数据啦1", 123)
    Messenger:GetInstance():RemoveByFunc(MsgEnum.UI_LOADINING_REFRESH, TestMessenger.OnProcess2)
    Messenger:GetInstance():Broadcast(MsgEnum.UI_LOADINING_REFRESH, "数据啦1", 123)
end

return TestMessenger