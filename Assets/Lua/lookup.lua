local _M = {}

_M['UnityEngine.GameObject'] = function(index)
    return function(t, k)
        local v = index(t, k)
            if v then return v 
            else return t:GetComponent(k) 
        end
    end
end

return _M