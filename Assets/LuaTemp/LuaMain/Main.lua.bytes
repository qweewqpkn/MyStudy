--主入口函数。从这里开始lua逻辑

Main = {}
local function Start()					
	print("logic start")	

	local testLua = CS.TestLuaCallCS()
	testLua:ShowSome("xixixixi" .. "it is successful")
	testLua:ShowMyLove()

	CS.TestBase.content = "waht are you doing"
	print(CS.TestBase.content)

	local enumType = CS.TestBase.eDataType.eInt
	if(enumType == CS.TestBase.eDataType.eInt) then
		print("enum is equal")
	end
	
	print(CS.TestBase.eDataType.eInt)
	print(CS.TestBase.eDataType.eFloat)
	print(CS.TestBase.eDataType.eString)
	--for k,v in pairs(package.loaded._G.CS) do
	--	print("name : " .. tostring(k))
	--end
end

local function ShowStr(a, b)
	local c = a + b
	return c, {f1 = a, f2 = b}
end

Main.Start = Start
Main.ShowStr = ShowStr
return Main