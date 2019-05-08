--[[
-- added by wsh @ 2017-11-30
-- Lua全局配置
--]]

local Config = {}

-- 调试模式：真机出包时关闭
Config.Debug = true

Config.AuthAddr = 
{
    DefaultAddrIndex = 1,
    list = 
    {
        "172.10.1.7:2021",  --dev
    },
    GetAuthAddr = function (self)
    	return self.list[self.DefaultAddrIndex]
    end
}

return Config