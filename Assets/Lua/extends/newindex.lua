local _M = {}

_M['CS.UnityEngine.UI.Button'] = function(newindex)
    return function(t, k, v)
        if k == 'click' then
            local onclick = t.onClick
            if v then
                onclick:RemoveAllListeners()
                onclick:AddListener(v)
            else
                onclick:RemoveAllListeners()
            end    
        else 
            newindex(t, k, v)
        end 
    end
end

return _M