local TestUI = BaseClass("TestUI")

function TestUI:Test1()
    UIManager:GetInstance():OpenPanel("ui_test")
    UIManager:GetInstance():OpenPanel("ui_test")
    UIManager:GetInstance():OpenPanel("ui_test")
end

function TestUI:Test2()
    UIManager:GetInstance():OpenPanel("ui_test")
    UIManager:GetInstance():OpenPanel("ui_mail")
end

function TestUI:Test3()
    UIManager:GetInstance():OpenPanel("ui_test")
    UIManager:GetInstance():OpenPanel("ui_mail")
    UIManager:GetInstance():CloseAllPanel()
    UIManager:GetInstance():OpenPanel("ui_test")
    UIManager:GetInstance():OpenPanel("ui_mail")
end

function TestUI:Test4()
    UIManager:GetInstance():OpenPanel("ui_test")
    UIManager:GetInstance():OpenPanel("ui_mail")
    UIManager:GetInstance():CloseAllPanel()
    UIManager:GetInstance():OpenPanel("ui_mail")
end

function TestUI:Test5()
    UIManager:GetInstance():OpenPanel("ui_test")
    UIManager:GetInstance():OpenPanel("ui_mail")

    UIManager:GetInstance():ClosePanel("ui_test")
    UIManager:GetInstance():ClosePanel("ui_mail")

    UIManager:GetInstance():OpenPanel("ui_test")
    UIManager:GetInstance():OpenPanel("ui_mail")
end

function TestUI:Test6()
    UIManager:GetInstance():OpenPanel("ui_test")
    UIManager:GetInstance():OpenPanel("ui_mail")

    UIManager:GetInstance():ClosePanel("ui_test")

    UIManager:GetInstance():OpenPanel("ui_test")
    UIManager:GetInstance():OpenPanel("ui_mail")
end

function TestUI:Test7()
    UIManager:GetInstance():OpenPanel("ui_test")
    UIManager:GetInstance():ClosePanel("ui_test")
    UIManager:GetInstance():OpenPanel("ui_test")
    UIManager:GetInstance():ClosePanel("ui_test")
    UIManager:GetInstance():OpenPanel("ui_test")
end

return TestUI