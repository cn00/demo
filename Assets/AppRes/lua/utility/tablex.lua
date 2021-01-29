
local table = table or {}

function table:select(filter)
    assert(type(filter) == "function")
    local t = {}
    for _, v in pairs(self) do
        table.insert(t, filter(v))
    end
    return t
end

function table:where(filter)
    assert(type(filter) == "function")
    local t = {}
    for _, v in pairs(self) do
        local ok , r = filter(v)
        if ok then
            table.insert(t, r)
        end
    end
    return t
end

return table