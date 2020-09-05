local class = require "core.class"

local _M = class({})

function _M:ctor(go)
    local homeBtn = go:child("Home").Button
    homeBtn.click = function()
        scenes:GotoScene("Main")
    end
    return {go = go}
end

return _M


