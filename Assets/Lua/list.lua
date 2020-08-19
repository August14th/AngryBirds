local list = setmetatable({}, {__call = function(l, ...)
    local data = {...}
    return setmetatable(data, {__index = l}) 
end})

function list:filter(f)
    local found = list()
    self:foreach(function(value) 
        if f(value) then
            found:add(vlaue)
        end
    end)
    return found
end

function list:add(...)
    local args = list(...)
    args:foreach(function(arg)
        table.insert(self, arg)
    end)
end

function list:addall(array)
    for index, value in ipairs(array) do
        self:add(value)
    end
end

function list:remove(element)
    for index, value in ipairs(self) do
        if value == element then
            table.remove(self, index)
        end
    end
end

function list:foreach(f)
    for index, value in ipairs(self) do
        f(value)
    end
end

function list:map(f)
    local results = list()
    self:foreach(function(value) 
        results:add(f(value))
    end)
    return results
end

function list:exists(f)
    for index, value in ipairs(self) do
        if f(value) then return true end
    end
    return false
end

function list:contains(element)
    for index, value in ipairs(self) do
        if value == element then return true end
    end
    return false
end

function list:size()
    return #self
end

function list:mkstring(seperator)
    local sb = ''
    for index, value in ipairs(self) do
        if(index ~= 1) then sb = sb .. seperator end 
        sb = sb .. value
    end
    return sb
end

function list:groupby(f)
    local tt = self:map(function(value)
        return {first = f(value), last = value}
    end)
    local groups = {}
    tt:foreach(function(t)
        if(groups[t.first] ~= nil) then groups[t.first]:add(t.last)
        else groups[t.first] = list({t.last}) end
    end)

    return groups
end

function list:sortby(f)
    table.sort(self, f)
end

function list:max(f)
    local t = self
    if f~= nil then
        t = self:map(function(v) f(v) end)
    end
    local max = nil
    t:foreach(function(v)
        if max == nil then max = v
        elseif v > max then max = v end
    end)
    return max
end

function list:min(f)
    local t = self
    if f~= nil then
        t = self:map(function(v) f(v) end)
    end
    local min = nil
    t:foreach(function(v)
        if min == nil then min = v
        elseif v < min then min = v end
    end)
    return min
end

return list