local SceneConfig = {
    --启动场景
    SplashScene = {
        name = "SplashScene",
        type = require "Scene.Splash.SplashScene"
    }
    ,
    --登录场景
    LoginScene = {
        name = "LoginScene",
        type = require "Scene.Login.LoginScene",
    }
    ,
    --大地图
    WildMapScene = {
        name = "WildMapScene",
        type = require "Scene.WildMap.WildMapScene",
    }
    ,
    --战斗场景
    BattleScene = {
        name = "BattleScene",
        type = require "Scene.BattleScene",
    }
    ,
}

return SceneConfig