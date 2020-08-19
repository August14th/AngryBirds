local index = require("extends.index")
local newindex = require("extends.newindex")
local list = require("list")

local keys = list()
for key, _ in pairs(index) do
    keys:add(key)
end
for key, _ in pairs(newindex) do
    keys:add(key)
end
load("local _ = {" .. keys:mkstring(",") .."}")()