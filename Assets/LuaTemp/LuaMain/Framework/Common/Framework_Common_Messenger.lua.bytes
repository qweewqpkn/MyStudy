local Messenger = BaseClass("Messager", Singleton)

MsgEnum = setmetatable({MaxIndex = 100001},{__index = function(tab,key )
	local newIndex = MsgEnum.MaxIndex
	rawset(tab,key,newIndex)
	MsgEnum.MaxIndex = newIndex + 1
	return newIndex
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
		if(v.func == func) then
			bFind = true
			Logger.LogError("Messenger have same func, check!!!")
		end
	end

	if(not bFind) then
		local item = {target = target, id = id, func = func, args = SafePack(...)}
		table.insert(self.mNotifies[id], item)
	end
end

function Messenger:Broadcast(id, ...)
	if(self.mNotifies[id] ~= nil) then
		for _,v in pairs(self.mNotifies[id]) do
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

function Messenger:RemoveByTarget(target)
	for k,v in pairs(self.mNotifies) do
		local count = #v
		for i = count, 1, -1 do
			if(v[i].target == target) then
				table.remove(v, i)
			end
		end

		if(#v == 0) then
			self.mNotifies[k] = nil
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
	end
end

function Messenger:RemoveByID(id)
	self.mNotifies[id] = nil
end

return Messenger

