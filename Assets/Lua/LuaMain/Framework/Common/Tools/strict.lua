local mt = getmetatable(_G)
if mt == nil then
    mt = {}
    setmetatable(_G, mt)
end

__STRICT = true
mt.__declared = {}

mt.__newindex = function (t, n, v)
    if __STRICT and not mt.__declared[n] then
        local info = debug.getinfo(2, "S")
        local w = info.what

        if w ~= "main" and w ~= "C" then
            error("assign to undeclared variable '"..n.."'", 2)
        end
        mt.__declared[n] = true
    end
    rawset(t, n, v)
end

mt.__index = function (t, n)
    if not mt.__declared[n] and debug.getinfo(2, "S").what ~= "C" then
        error("variable '"..n.."' is not declared", 2)
    end
    return rawget(t, n)
end

function global(...)
    for _, v in ipairs{...} do mt.__declared[v] = true end
end

--定义全局变量
function declareG(name, initval)
    --rawset(_G, name, initval or false)
    if rawget(_G, name) ~= nil then
        error("_G var already declared!"..name)
    end
    mt.__declared[name] = true
    rawset(_G, name, initval or false)
end
