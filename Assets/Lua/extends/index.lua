local _M = {}

_M['CS.UnityEngine.GameObject'] = function(index)
    return function(t, k)
        if k == 'lua' then
            return function(t, comstring)
                local com = require(comstring)        
                return com(t)
            end
        end
        local v = index(t, k)
            if v then return v 
            else return t:GetComponent(k) 
        end
    end
end

return _M