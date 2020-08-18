local class = function(class, super)
    local mt = {
        __call = function(class, ...)
            local instance = {}
            if(class.ctor) then
                instance = class:ctor(...) 
            end
            return setmetatable(instance, {__indx = class})
        end
    }
    if(super) then 
        mt.__index = super 
        class.super = super
    end
    return setmetatable(class, mt)
end

return class