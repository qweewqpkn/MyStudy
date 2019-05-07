local TestUI = BaseClass("TestUI")

function TestUI:Test1()
    UIManager:GetInstance():OpenPanel(UIConfig.ui_test.name)
    UIManager:GetInstance():OpenPanel(UIConfig.ui_test.name)
    UIManager:GetInstance():OpenPanel(UIConfig.ui_test.name)
end

function TestUI:Test2()
    UIManager:GetInstance():OpenPanel(UIConfig.ui_test.name)
    UIManager:GetInstance():OpenPanel(UIConfig.ui_mail.name)
end

function TestUI:Test3()
    UIManager:GetInstance():OpenPanel(UIConfig.ui_test.name)
    UIManager:GetInstance():OpenPanel(UIConfig.ui_mail.name)
    UIManager:GetInstance():CloseAllPanel()
    UIManager:GetInstance():OpenPanel(UIConfig.ui_test.name)
    UIManager:GetInstance():OpenPanel(UIConfig.ui_mail.name)
end

function TestUI:Test4()
    UIManager:GetInstance():OpenPanel(UIConfig.ui_test.name)
    UIManager:GetInstance():OpenPanel(UIConfig.ui_mail.name)
    UIManager:GetInstance():CloseAllPanel()
    UIManager:GetInstance():OpenPanel(UIConfig.ui_mail.name)
end

function TestUI:Test5()
    UIManager:GetInstance():OpenPanel(UIConfig.ui_test.name)
    UIManager:GetInstance():OpenPanel(UIConfig.ui_mail.name)

    UIManager:GetInstance():ClosePanel(UIConfig.ui_test.name)
    UIManager:GetInstance():ClosePanel(UIConfig.ui_mail.name)

    UIManager:GetInstance():OpenPanel(UIConfig.ui_test.name)
    UIManager:GetInstance():OpenPanel(UIConfig.ui_mail.name)
end

function TestUI:Test6()
    UIManager:GetInstance():OpenPanel(UIConfig.ui_test.name)
    UIManager:GetInstance():OpenPanel(UIConfig.ui_mail.name)

    UIManager:GetInstance():ClosePanel(UIConfig.ui_test.name)

    UIManager:GetInstance():OpenPanel(UIConfig.ui_test.name)
    UIManager:GetInstance():OpenPanel(UIConfig.ui_mail.name)
end

function TestUI:Test7()
    UIManager:GetInstance():OpenPanel(UIConfig.ui_test.name)
    UIManager:GetInstance():ClosePanel(UIConfig.ui_test.name)
    UIManager:GetInstance():OpenPanel(UIConfig.ui_test.name)
    UIManager:GetInstance():ClosePanel(UIConfig.ui_test.name)
    UIManager:GetInstance():OpenPanel(UIConfig.ui_test.name)
end

return TestUI