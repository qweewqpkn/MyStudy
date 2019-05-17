local UIConfig = {
    ui_main = {
        name = "ui_main",
        type = require "UI.View.Main.ui_main",
    }
    ,
    ui_loading = {
        name = "ui_loading",
        type = require "UI.View.Loading.ui_loading",
    }
    ,
    ui_mail = {
        name = "ui_mail",
        type = require "UI.View.Mail.ui_mail",
    }
    ,
    ui_test = {
        name = "ui_test",
        type = require "UI.View.Test.ui_test",
    }    ,
}

return UIConfig