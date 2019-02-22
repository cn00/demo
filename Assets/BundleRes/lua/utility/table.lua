
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


local function dump(obj, breakline, prefix)
    breakline = breakline == nil or breakline == true
    if type(prefix) ~= "string" then
        prefix = ""
    end

    local getIndent, quoteStr, wrapKey, wrapVal, dumpObj
    getIndent = function(level)
        if breakline then
            return prefix .. string.rep("\t", level)
        else
            return ""
        end
    end
    quoteStr = function(str)
        return '"' .. string.gsub(str, '"', '\"') .. '"'
    end
    wrapKey = function(val)
        if breakline then
            if type(val) == "number" then
                return "[" .. val .. "] = "
            elseif type(val) == "string" then
                return "[" .. quoteStr(val) .. "] = "
            else
                return "[" .. tostring(val) .. "] = "
            end
        else
            if type(val) == "string" then
                return quoteStr(val) .. " = "
            else
                return ""
            end
        end
    end
    wrapVal = function(val, level)
        if type(val) == "table" then
            return dumpObj(val, level)
        elseif type(val) == "number" then
            return val
        elseif type(val) == "string" then
            return quoteStr(val:gsub("[\0-\15]", ""):gsub("\n", "\\n"):gsub("\r", "\\r"))
        else
            return tostring(val)
        end
    end
    dumpObj = function(obj, level)
        if type(obj) ~= "table" then
            return wrapVal(obj)
        end
        level = level + 1

        local tokens = {}
        tokens[#tokens + 1] = "{"
        if level < 5 then 
            for k, v in pairs(obj) do
                tokens[#tokens + 1] = getIndent(level) .. wrapKey(k) .. wrapVal(v, level) .. ","
            end
            local meta = getmetatable(obj)
            if meta ~= nil then tokens[#tokens + 1] = getIndent(level) .. "__meta = " .. wrapVal(meta, level) .. "," end
        else
            tokens[#tokens + 1] = getIndent(level) .. "..."
        end
        tokens[#tokens + 1] = getIndent(level - 1) .. "}"
        if breakline then
            return table.concat(tokens, "\n")
        else
            return table.concat(tokens, " ")
        end
    end
    return dumpObj(obj, 0)
end
table.dump = dump

-- function table:dump( breakline, prefix )
--     dump(self, breakline, prefix)
-- end

return table