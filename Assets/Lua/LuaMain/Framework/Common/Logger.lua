local Logger = BaseClass("Logger")

function Logger.Init()
    Logger.IsOpenLog = Debuger.IsOpenLog
    Logger.Module =
    {
        UI = "UI",
        BATTLE = "BATTLE",
        SCENE = "SCENE",
        COMMON = "COMMON",
        NET = "NET",
        LOGIN = "LOGIN",
        WILDMAP = "WILDMAP",
        HERO = "HERO",
        CONFIG = "CONFIG",
    }

    if(Logger.IsOpenLog) then
        Debuger.SwitchModule(Logger.Module.UI, true)
        Debuger.SwitchModule(Logger.Module.BATTLE, true)
        Debuger.SwitchModule(Logger.Module.SCENE, true)
        Debuger.SwitchModule(Logger.Module.COMMON, true)
        Debuger.SwitchModule(Logger.Module.NET, true)
        Debuger.SwitchModule(Logger.Module.LOGIN, true)
        Debuger.SwitchModule(Logger.Module.WILDMAP, true)
        Debuger.SwitchModule(Logger.Module.HERO, true)
        Debuger.SwitchModule(Logger.Module.CONFIG, true)
    end

    print = function(...)
        Logger.Log(Logger.Module.COMMON, ...)
    end
end

function Logger.ContentWrap(type, ...)
    local msg = string.join(SafePack(...), " ")
    local info = debug.getinfo(3, "Sl")
    local lineinfo = string.format("(%s:%s)", info.short_src, info.currentline)
    return string.format(" %s [%s] %s: %s",
            type,
            os.date("%H:%M:%S"),
            lineinfo,
            msg)
end

function Logger.Log(type, ...)
    if(Logger.IsOpenLog) then
        Debuger.Log(type, Logger.ContentWrap(type, ...))
    end
end

function Logger.LogWarning(type, ...)
    if(Logger.IsOpenLog) then
        Debuger.LogWarning(type, Logger.ContentWrap(type, ...))
    end
end

function Logger.LogError(type, ...)
    if(Logger.IsOpenLog) then
        Debuger.LogError(type, Logger.ContentWrap(type, ...))
    end
end

Logger.Init()
return Logger


