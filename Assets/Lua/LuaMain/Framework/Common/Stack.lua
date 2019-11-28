local Stack = BaseClass("Stack")

function Stack:__init(...)
    self.list = {}
end

function Stack:__delete()
    self.list = nil
end

function Stack:Clear()
    self.list = {}
end

function Stack:Contains(t)
    for i,v in ipairs(self.list) do
        if(v == t) then
            return true, i
        end
    end

    return false, -1
end

function Stack:Peek()
    local count = self:Count()
    if(count > 0) then
        return self.list[count]
    else
        return nil
    end
end

function Stack:Pop()
    local count = self:Count()
    if(count> 0) then
        return table.remove(self.list, count)
    end

    return nil
end

function Stack:Push(t)
    table.insert(self.list, t)
end

function Stack:Count()
    return #self.list
end

function Stack:MoveToTop(t)
    local isHave, index = self:Contains(t)
    if(isHave) then
        for i = index, #self.list do
            if(i == #self.list) then
                table.remove(self.list, i)
            else
                self.list[i] = self.list[i+1]
            end
        end

        self:Push(t)
    end
end

return Stack