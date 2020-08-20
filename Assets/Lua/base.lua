local _M = { _engine = nil, _asset = nil, _scenes = nil}

function _M:engine()
    if not _M._engine then
        _M._engine = CS.UnityEngine.GameObject.FindWithTag("GameEngine")
    end
    return _M._engine
end

function _M:Assets()
    if not _M._asset then
        local asset = self:engine():GetComponent("Resources") 
        if asset then
            _M._asset = asset
        else
            _M._asset = self:engine():GetComponent("Bundles") 
        end
    end
    return _M._asset       
end

function _M:Scenes()
    if not _M._scenes then
        _M._scenes = self:engine():GetComponent("Scenes") 
    end
    return _M._scenes 
end

function _M:Canvas()
    return CS.UnityEngine.GameObject.Find("Canvas")
end

return _M