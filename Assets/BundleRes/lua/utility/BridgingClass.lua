BridgingClass = {};

function BridgingClass.GetMetaTable( tab )
	local mt = getmetatable(tab)
	-- print("BridgingClass.GetMetaTable", tab, mt)
	-- if mt ~= nil then mt.Name = "__meta" end
	return mt
end

function BridgingClass.GetTable(name, id, key)
	if     Lua_Table[name] == nil 
		or Lua_Table[name][id] == nil
		or Lua_Table[name][id][key] == nil
	then
		print("table["+name+"]["+tostring(id)+"]["+key+"] not found")
		return nil
	else
		return Lua_Table[name][id][key]
	end
end

function BridgingClass.reload( tbName )
	package.loaded[tbName] = nil;
    require (tbName)
end

return BridgingClass