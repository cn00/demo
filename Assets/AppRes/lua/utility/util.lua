
require("stringx")
require("tablex")

local util = {}
function util.newIdx(start)
    start = start or 0
    local i = start
    return function()
        i = i + 1
        return i
    end
end

---clean temp flag named `flag`
---@param tab table
---@param flag string
function util.cleanFlag(tab, flag)
    if type(tab) ~= "table" then return end
    tab[flag] = undef
    for i, v in pairs(tab) do
        if type(v) == "table" and v[flag] then 
            util.cleanFlag(v, flag)
        end
    end
end

local table = table
function util.copy(tab, filter)
    local ntab = {}
    for k, v in tab do
        if type(v) == "table" 
           and not v.__copyed__  -- nested table
        then
            ntab[k] = util.copy(v)
        else -- if v.isstring() or v.isnumber() then
            ntab[k] = v
        end
    end
    util.cleanFlag(tab, "__copyed__")
    return ntab
end
table.copy  = util.copy
table.clone = util.copy

---dump
---@overload fun(obj:table)
---@param obj table
---@param pretty boolean
---@param prefix string
---@return string
function util.dump(obj, pretty, prefix)
    --breakline = breakline or false
    if type(prefix) ~= "string" then
        prefix = ""
    end

    local getIndent, quoteStr, wrapKey, wrapVal, dumpObj
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
        elseif tv == "function" then
            return quoteStr(string.xxd(string.dump(val)))
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
                if k == "__wraped__" then goto continue end
                if type(v) == "function" then
                    tokens[#tokens + 1] = getIndent(level) .. wrapKey(k.. "_func") .. wrapVal(v, level) .. ","
                else
                    if type(v) == "table" then
                        if v["__wraped__"] == true then goto continue end
                        v["__wraped__"] = true
                    end
                    tokens[#tokens + 1] = getIndent(level) .. wrapKey(k) .. wrapVal(v, level) .. ","
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
    local res = dumpObj(obj, 0)
    util.cleanFlag(obj, "__wraped__")
    return res
end
table.dump = util.dump

-- function table:dump( breakline, prefix )
--     dump(self, breakline, prefix)
-- end

function util.removeValue(t, vv)
    for i, v in ipairs(t) do
        if v == vv then table.remove(t, i) return true end
    end
    return false
end

function util.StrTable2CSArray( tab )
    local list = CS.System.Collections.Generic["List`1[System.String]"]()
    for i,v in ipairs(tab) do
        -- print(i,v)
        list:Add(v)
    end
    return list --:ToArray()
end
    
return util
