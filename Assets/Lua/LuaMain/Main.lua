--主入口函数。从这里开始lua逻辑

Main = {}
local function Start()					
	print("logic start")	

	--for k,v in pairs(package.loaded) do
	--	print("name : " .. k)
	--end
end

Main.Start = Start
return Main