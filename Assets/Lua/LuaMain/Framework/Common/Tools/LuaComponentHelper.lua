local LuaComponentHelper = BaseClass("LuaComponentHelper", Singleton)

local meta = getmetatable(LuaComponentHelper)
meta.__mode = "k"

local AllGoComponents = {}

local function GetCompleteClassName(t)
	if t.super == nil then
		return t.__cname
	end

	return GetCompleteClassName(t.super).."."..t.__cname
end

function LuaComponentHelper:AddLuaBehaviour(go, luamonoclass)
	if AllGoComponents[go] == nil then
		AllGoComponents[go] = {}
	end

	local luamono = luamonoclass.New(go)
	local key = GetCompleteClassName(luamonoclass)
	AllGoComponents[go][key] = luamono

	return luamono
end

function LuaComponentHelper:GetLuaBehaviour(go, luamonoclass)
	local all = AllGoComponents[go]
	if all == nil then
		return nil
	end

	local key = GetCompleteClassName(luamonoclass)
	for k,v in pairs(all) do
		local b, _, endpos = string.startswith(k, key)
		if b and (endpos == string.len(k) or string.sub(k, endpos, endpos + 1) == ".") then
			return v
		end
	end

	return nil
end

function LuaComponentHelper:DestroyImmediate(go)
	if AllGoComponents[go] == nil then
		SmartGOManager:GetInstance():DeSpawn(go)
		go = nil
	else
		for k,v in pairs(AllGoComponents[go]) do
			v:Delete()
			v = nil
		end
		AllGoComponents[go] = nil
		SmartGOManager:GetInstance():DeSpawn(go)
		go = nil
	end
end

return LuaComponentHelper