local _M = { _engine = nil}

function _M:engine()
    if not _M._engine then
        _M._engine = CS.UnityEngine.GameObject.FindWithTag("GameEngine")
    end
    return _M._engine
end

function _M:Assets()
    return self:engine():GetComponent("Assets")        
end

function _M:Scenes()
    return self:engine():GetComponent("Scenes")        
end

function _M:Canvas()
    return CS.UnityEngine.GameObject.Find("Canvas")
end

return _M