local Singleton = Class()

function Singleton:ctor(...)

end

function Singleton:GetInstance()
	if(self.mInstance == nil) then
		self.mInstance = self.new()
	end

	return self.mInstance
end

function Singleton:Dispose()
	self.mInstance = nil
end

return Singleton



