local list = setmetatable({}, {__call = function(l, ...)
    local data = {...}
    return setmetatable(data, {__index = l}) 
end})

function list.filter(data, f)
    local found = list()
    data:foreach(function(value) 
        if f(value) then
            found:add(vlaue)
        end
    end)
    return found
end

function list.add(data, ...)
    local args = list(...)
    args:foreach(function(arg)
        table.insert(data, arg)
    end)
end

function list.addAll(data, array)
    for index, value in ipairs(array) do
        data:add(value)
    end
end

function list.remove(data, element)
    for index, value in ipairs(data) do
        if value == element then
            table.remove(data, index)
        end
    end
end

function list.foreach(data, f)
    for index, value in ipairs(data) do
        f(value)
    end
end

function list.map(data, f)
    local results = list()
    data:foreach(function(value) 
        results:add(f(value))
    end)
    return results
end

function list.exists(data, f)
    for index, value in ipairs(data) do
        if f(value) then return true end
    end
    return false
end

function list.contains(data, element)
    for index, value in ipairs(data) do
        if value == element then return true end
    end
    return false
end

function list.size(data)
    return #data
end

function list.mkstring(data, seperator)
    local sb = ''
    for index, value in ipairs(data) do
        if(index ~= 1) then sb = sb .. seperator end 
        sb = sb .. value
    end
    return sb
end

function list.groupby(data, f)
    local tt = data:map(function(value)
        return {first = f(value), last = value}
    end)
    local groups = {}
    tt:foreach(function(t)
        if(groups[t.first] ~= nil) then groups[t.first]:add(t.last)
        else groups[t.first] = list({t.last}) end
    end)

    return groups
end

function list.sortby(data, f)
    table.sort(data, f)
end

function list.max(data, f)
    local t = data
    if f~= nil then
        t = data:map(function(v) f(v) end)
    end
    local max = nil
    t:foreach(function(v)
        if max == nil then max = v
        elseif v > max then max = v end
    end)
    return max
end

function list.min(data, f)
    local t = data
    if f~= nil then
        t = data:map(function(v) f(v) end)
    end
    local min = nil
    t:foreach(function(v)
        if min == nil then min = v
        elseif v < min then min = v end
    end)
    return min
end

return list