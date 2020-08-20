local base = require "base"
local class = require "class"

local _M = class({prefab_name = "prefab/LosePanel"}, base)

function _M:ctor()
    local assets = self:Assets()
    local canvas = self:Canvas()
    local scenes = self:Scenes()
    local panel = assets:NewUI(self.prefab_name, canvas.transform)
    panel:child("Home").Button.click = function()
        scenes:GotoScene("Main")
    end
    return {panel = panel}
end

return _M


