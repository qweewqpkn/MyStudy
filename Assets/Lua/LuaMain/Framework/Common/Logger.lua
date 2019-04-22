local Logger = BaseClass("Logger")

function Logger.Init()
    Logger.IsOpenLog = Debuger.IsOpenLog
    Logger.Module =
    {
        UI = "UI",
        BATTLE = "BATTLE",
        SCENE = "SCENE",
        COMMON = "COMMON",
    }

    if(Logger.IsOpenLog) then
        Debuger.SwitchModule(Logger.Module.UI, true)
        Debuger.SwitchModule(Logger.Module.BATTLE, true)
        Debuger.SwitchModule(Logger.Module.SCENE, true)
        Debuger.SwitchModule(Logger.Module.COMMON, true)
    end
end

function Logger.ContentWrap(...)
    local msg = string.join(SafePack(...), " ")
    local info = debug.getinfo(3, "Sl")
    local lineinfo = string.format("(%s:%s)", info.short_src, info.currentline)
    return string.format("[%s] %s: %s",
            os.date("%H:%M:%S"),
            lineinfo,
            msg)
end

function Logger.Log(type, ...)
    if(Logger.IsOpenLog) then
        Debuger.Log(type, Logger.ContentWrap(...))
    end
end

function Logger.LogWarning(type, ...)
    if(Logger.IsOpenLog) then
        Debuger.LogWarning(type, Logger.ContentWrap(...))
    end
end

function Logger.LogError(type, ...)
    if(Logger.IsOpenLog) then
        Debuger.LogError(type, Logger.ContentWrap(...))
    end
end

Logger.Init()
return Logger


