local Messenger = BaseClass("Messager", Singleton)

MsgEnum = setmetatable({MaxIndex = 100001},{__index = function(tab,key )
	local val = rawget(tab, key)
	if(val ~= nil) then
		Logger.E("MsgEnum have same key : " .. key)
		return nil
	else
		local newIndex = MsgEnum.MaxIndex
		rawset(tab,key,newIndex)
		MsgEnum.MaxIndex = newIndex + 1
		return newIndex
	end
end })

function Messenger:__init(...)
	self.mNotifies = {}
end

function Messenger:Register(id, func, target, ...)
	if self.mNotifies[id] == nil then
		self.mNotifies[id] = {}
	end

	local bFind = false
	for _,v in pairs(self.mNotifies[id]) do
		if(v.func == func and v.target == target) then
			bFind = true
			break
			--Logger.LogError(Logger.Module.COMMON, "Messenger have same func and target, check!!! id : " .. id .. table.dump(target, nil, 2))
		end
	end

	if(not bFind) then
		local item = {target = target, id = id, func = func, args = SafePack(...)}
		table.insert(self.mNotifies[id], item)
	end
end

function Messenger:Broadcast(id, ...)
	if(self.mNotifies[id] ~= nil) then
		--for _,v in pairs(self.mNotifies[id]) do
		--	if(v ~= nil and v.func ~= nil) then
		--		local args = ConcatSafePack(v.args, SafePack(...))
		--		if(v.target ~= nil) then
		--			v.func(v.target, SafeUnpack(args))
		--		else
		--			v.func(SafeUnpack(args))
		--		end
		--	end
		--end

		--chen hang 采用倒序遍历，防止第一个ntf处理后，第二个ntf被跳过的BUG
		for i = #self.mNotifies[id], 1, -1 do
			if(self.mNotifies[id] ~= nil)then
				local v = self.mNotifies[id][i]
				if(v ~= nil and v.func ~= nil) then
					local args = ConcatSafePack(v.args, SafePack(...))
					if(v.target ~= nil) then
						v.func(v.target, SafeUnpack(args))
					else
						v.func(SafeUnpack(args))
					end
				end
			end
		end
	end
end

function Messenger:RemoveByTarget(target, id)
	if(target == nil) then
		Logger.E("Messenger:RemoveByTarget target is nil")
		return
	end

	for k,v in pairs(self.mNotifies) do
		if(v ~= nil) then
			local count = #v
			for i = count, 1, -1 do
				if(id == nil) then
					if(v[i].target == target) then
						table.remove(v, i)
					end
				else
					if(v[i].target == target and v[i].id == id) then
						table.remove(v, i)
					end
				end
			end

			if(#v == 0) then
				self.mNotifies[k] = nil
			end
		end
	end
end

function Messenger:RemoveByFunc(id, func)
	local v = self.mNotifies[id]
	if(v ~= nil) then
		local count = #v
		for i = count, 1, -1 do
			if(v[i].func == func) then
				table.remove(v, i)
				break
			end
		end

		if(#v == 0) then
			self.mNotifies[id] = nil
		end
	end
end

function Messenger:RemoveByID(id)
	self.mNotifies[id] = nil
end

return Messenger

