local ui_main = BaseClass("ui_main", UIBase)

function ui_main:__init(...)
	self.mAbPath = "ui_main"
end

function ui_main:OnShow(data)
	print("ui_main onshow")
end

function ui_main:OnClose()
	print("ui_main onshow")
end

local function Register()
	UIManager:GetInstance():RegisterCreateFunc("ui_main", ui_main.New)
end
Register()
return ui_main