--[[
	游戏日志工具
]]

------------- Class -------------
local Logger = {
    _type = "Log",
    outfile = nil,
    level = "Log",
}

-------------- private vars  & funs ----------
local modes = {
    { name = "Log"},
    { name = "LogWarning"},
    { name = "LogError"},
}

local levels = {}
for i, v in ipairs(modes) do
    levels[v.name] = i
end

local _tostring = tostring

local tostring = function(...)
    local t = {}
    for i = 1, select('#', ...) do
        local x = select(i, ...)
        t[#t + 1] = _tostring(x)
    end
    return table.concat(t, " ")
end

for i, x in ipairs(modes) do
    local nameupper = x.name:upper()
    Logger[x.name] = function(...)

        if i < levels[Logger.level] then
            return
        end

        local msg = tostring(...)
        local info = debug.getinfo(2, "Sl")
        local lineinfo = string.format("(%s:%s)", info.short_src, info.currentline)

        -- Output to console
        print(string.format("[%-6s%s] %s: %s",
                nameupper,
                os.date("%H:%M:%S"),
                lineinfo,
                msg))

        if x.name == "LogError" then
            error(msg)
        end

        if Logger.outfile then
            local fp = io.open(Logger.outfile, "a")
            local str = string.format("[%-6s%s] %s: %s\n",
                    nameupper, os.date(), lineinfo, msg)
            fp:write(str)
            fp:close()
        end

    end
end

return Logger


