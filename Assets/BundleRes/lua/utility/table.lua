
local table = table or {}
function table.copy(tab, filter)
    local ntab = {}
    for k, v in tab do
        if v.istable() then
            ntab[k] = table.copy(v)
        elseif v.isstring() or v.isnumber() then
            ntab[k] = v
        end
    end
    return ntab
end

return table