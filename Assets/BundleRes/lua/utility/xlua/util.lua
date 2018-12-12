-- Tencent is pleased to support the open source community by making xLua available.
-- Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
-- Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
-- http://opensource.org/licenses/MIT
-- Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

local unpack = unpack or table.unpack

local function async_to_sync(async_func, callback_pos)
    return function(...)
        local _co = coroutine.running() or error ('this function must be run in coroutine')
        local rets
        local waiting = false
        local function cb_func(...)
            if waiting then
                assert(coroutine.resume(_co, ...))
            else
                rets = {...}
            end
        end
        local params = {...}
        table.insert(params, callback_pos or (#params + 1), cb_func)
        async_func(unpack(params))
        if rets == nil then
            waiting = true
            rets = {coroutine.yield()}
        end
        
        return unpack(rets)
    end
end

local function coroutine_call(func, ...)
    -- return function(...)
        local co = coroutine.create(func)
        assert(coroutine.resume(co, ...))
    -- end
end

local move_end = {}

local generator_mt = {
    __index = {
        MoveNext = function(self)
            self.Current = self.co()
            if self.Current == move_end then
                self.Current = nil
                return false
            else
                return true
            end
        end,
        Reset = function(self)
            self.co = coroutine.wrap(self.w_func)
        end
    }
}

local function cs_generator(func)
    local generator = setmetatable({
        w_func = function()
            func()
            return move_end
        end
    }, generator_mt)
    generator:Reset()
    return generator
end

local function loadpackage(...)
    for _, loader in ipairs(package.searchers) do
        local func = loader(...)
        if type(func) == 'function' then
            return func
        end
    end
end

local function auto_id_map()
    local hotfix_id_map = require 'hotfix_id_map'
    local org_hotfix = xlua.hotfix
    xlua.hotfix = function(cs, field, func)
        local map_info_of_type = hotfix_id_map[typeof(cs):ToString()]
        if map_info_of_type then
            if func == nil then func = false end
            local tbl = (type(field) == 'table') and field or {[field] = func}
            for k, v in pairs(tbl) do
                local map_info_of_methods = map_info_of_type[k]
                local f = type(v) == 'function' and v or nil
                for _, id in ipairs(map_info_of_methods or {}) do
                    CS.XLua.HotfixDelegateBridge.Set(id, f)
                end
                --CS.XLua.HotfixDelegateBridge.Set(
            end
        else
            return org_hotfix(cs, field, func)
        end
    end
end

--和xlua.hotfix的区别是：这个可以调用原来的函数
local function hotfix_ex(cs, field, func)
    assert(type(field) == 'string' and type(func) == 'function', 'invalid argument: #2 string needed, #3 function needed!')
    local function func_after(...)
        xlua.hotfix(cs, field, nil)
        local ret = {func(...)}
        xlua.hotfix(cs, field, func_after)
        return unpack(ret)
    end
    xlua.hotfix(cs, field, func_after)
end

local function bind(func, obj)
    return function(...)
        return func(obj, ...)
    end
end

--为了兼容luajit，lua53版本直接用|操作符即可
local enum_or_op = debug.getmetatable(CS.System.Reflection.BindingFlags.Public).__bor
local enum_or_op_ex = function(first, ...)
    for _, e in ipairs({...}) do
        first = enum_or_op(first, e)
    end
    return first
end

-- description: 直接用C#函数创建delegate
local function createdelegate(delegate_cls, obj, impl_cls, method_name, parameter_type_list)
    local flag = enum_or_op_ex(CS.System.Reflection.BindingFlags.Public, CS.System.Reflection.BindingFlags.NonPublic, 
        CS.System.Reflection.BindingFlags.Instance, CS.System.Reflection.BindingFlags.Static)
    local m = parameter_type_list and typeof(impl_cls):GetMethod(method_name, flag, nil, parameter_type_list, nil)
             or typeof(impl_cls):GetMethod(method_name, flag)
    return CS.System.Delegate.CreateDelegate(typeof(delegate_cls), obj, m)
end

local function dump(obj, breakline)
    breakline = breakline == nil or breakline == true
    local getIndent, quoteStr, wrapKey, wrapVal, dumpObj
    getIndent = function(level)
        if breakline then
            return string.rep("\t", level)
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
            return quoteStr(val)
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

local function isnilgo(go)
    if go == nil then
		return true
	end
	
	if type(go) == "userdata" and go.IsNull ~= nil then
		return go:IsNull()
	end
	
    return false
end

return {
    isnilgo = isnilgo,
    async_to_sync = async_to_sync,
    coroutine_call = coroutine_call,
    cs_generator = cs_generator,
    loadpackage = loadpackage,
    auto_id_map = auto_id_map,
    hotfix_ex = hotfix_ex,
    bind = bind,
    createdelegate = createdelegate,
    dump = dump,
}
