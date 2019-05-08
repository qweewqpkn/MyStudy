local LuaComponentHelper = BaseClass("LuaComponentHelper", Singleton)

local meta = getmetatable(LuaComponentHelper)
meta.__mode = "k"

local AllGoComponents = {}

function LuaComponentHelper:AddLuaBehaviour(go, luatable)
	if AllGoComponents[go] == nil then
		AllGoComponents[go] = {}
	end

	local lb = luatable.New(go)
	AllGoComponents[go][lb._class_type.__cname] = lb

	return lb
end

function LuaComponentHelper:GetLuaBehaviour(go, cname)
	local all = AllGoComponents[go]
	if all == nil then
		return nil
	end

	return all[cname]
end

function LuaComponentHelper:DestroyImmediate(go)
	if AllGoComponents[go] == nil then
		CS.UnityEngine.GameObject.DestroyImmediate(go)
		go = nil
	else
		for k,v in pairs(AllGoComponents[go]) do
			v:Delete()
			v = nil
		end
		AllGoComponents[go] = nil
		go = nil
	end
end

return LuaComponentHelper