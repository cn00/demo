

local util = {}
function util.newIdx(start)
    start = start or 0
    local i = start
    return function()
        i = i + 1
        return i
    end
end

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


---dump
---@param obj table
---@param pretty boolean
---@param prefix string
---@return string
function util.dump(obj, pretty, prefix)
    --breakline = breakline or false
    if type(prefix) ~= "string" then
        prefix = ""
    end

    local getIndent, quoteStr, wrapKey, wrapVal, dumpObj,cleanWrap
    getIndent = function(level)
        if pretty then
            return prefix .. string.rep("\t", level)
        else
            return ""
        end
    end
    quoteStr = function(str)
        return '"' .. string.gsub(str, '"', '\"') .. '"'
    end
    wrapKey = function(val)
        if pretty then
            if type(val) == "number" then
                return "[" .. val .. "] = "
            elseif type(val) == "string" then
                return "[" .. quoteStr(val) .. "] = "
            else
                return "[" .. tostring(val) .. "] = "
            end
        else
            if type(val) == "string" then
                return val .. " = "
            else
                return ""
            end
        end
    end
    wrapVal = function(val, level)
        local tv = type(val)
        if tv == "table" then
            return dumpObj(val, level)
        elseif tv == "number" then
            return val
        elseif tv == "string" then
            return quoteStr(val:gsub("[\0-\15]", ""):gsub("\n", "\\n"):gsub("\r", "\\r"))
        elseif tv == "string" then
            return quoteStr(string.dump(val))
        else
            return quoteStr(tostring(val))
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
                if type(v) ~= "function" then
                    tokens[#tokens + 1] = getIndent(level) .. wrapKey(k) .. wrapVal(v, level) .. ","
                else
                    if type(v) == "table" then
                        if v["__wraped__"] == true then goto continue end
                        v["__wraped__"] = true
                    end
                    tokens[#tokens + 1] = getIndent(level) .. wrapKey(k.. "_func") .. wrapVal(v, level) .. ","
                end
                ::continue::
            end
            local meta = getmetatable(obj)
            if meta ~= nil then tokens[#tokens + 1] = getIndent(level) .. "__meta = " .. wrapVal(meta, level) .. "," end
        else
            tokens[#tokens + 1] = getIndent(level) .. "..."
        end
        tokens[#tokens + 1] = getIndent(level - 1) .. "}"
        if pretty then
            return table.concat(tokens, "\n")
        else
            return table.concat(tokens, "")
        end
    end
    cleanWrap = function(obj)
        if type(obj) ~= "table" then return end
        if obj["__wraped__"] == true then obj["__wraped__"] = undef end
        for k, v in pairs(obj) do
            cleanWrap(v)
        end
    end
    local res = dumpObj(obj, 0)
    cleanWrap(obj)
    return res
end
table.dump = util.dump

-- function table:dump( breakline, prefix )
--     dump(self, breakline, prefix)
-- end

return util
