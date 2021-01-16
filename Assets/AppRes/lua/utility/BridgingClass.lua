local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject

BridgingClass = {}

function BridgingClass.GetMetaTable( tab )
	local mt = getmetatable(tab)
	if mt ~= nil then
		mt.Name = "__meta";
		print("BridgingClass.GetMetaTable", tab, mt)
	end
	return mt
end

function BridgingClass.ToStringLua( tab )
	return tostring(tab)
end

local function GetFuncId( func )
	local info = debug.getinfo(func, "S")
	local id = info.source..":"..info.linedefined
	return id
end
BridgingClass.GetFuncId = GetFuncId

function BridgingClass.RawSet( tab, t )
	rawset(tab, t.k, t.v)
end

local EditorGUI = CS.UnityEditor.EditorGUI
local EditorGUILayout = CS.UnityEditor.EditorGUILayout
local Draw
---Draw
---@param self table
---@param opt table
local function _Draw(self, opt)
	opt = opt or {ltname = "t", indent = 0}
	if self == nil then
		return;
	--elseif(type(self) ~= "table")then
	end
	
	local grep = opt.grep

	if (EditorGUI.indentLevel > 5) then
		return;
	end
	EditorGUI.indentLevel = EditorGUI.indentLevel + opt.indent;
	local name = (self["Name"] or opt.ltname);
	local Foldout = self["Foldout"] or false;
	if(type(Foldout) ~= "boolean")then Foldout = false end
	Foldout = EditorGUILayout.Foldout(Foldout, name .. ": " .. tostring(self), true);
	rawset(self, "Foldout", Foldout);
	local isdrawed = rawget(self, "__Drawed");
	if (Foldout and not isdrawed) then
		 rawset(self, "__Drawed", true);
		
		local drawv = function(k, v)
			-- print("drawv_" .. tostring(k), type(v))
			local typev = type(v)
			if (typev == "boolean") then
				local tmp = EditorGUILayout.Toggle(v);
				self[k] = tmp;
			elseif (typev == "number") then
				local tmp = EditorGUILayout.LongField(v);
				self[k] = tmp;
			elseif (typev == "string") then
				local tmp = EditorGUILayout.TextField(v);
				self[k] = tmp;
			elseif (typev == "function") then
				EditorGUILayout.TextField(GetFuncId(v));
			elseif (typev == "userdata") and type(v.GetType) == "function" then
				local ct = v:GetType()
				if ct and ct:IsSubclassOf(typeof(UnityEngine.Object)) then
					local o = EditorGUILayout.ObjectField(v, typeof(CS.UnityEngine.Object), true);
					--local com = o:GetComponent(ct)
					--if o.gameObject ~= v.gameObject  and com ~= nil then
					--	rawset(self, k, com) -- useless?
					--end
				else
					EditorGUILayout.LabelField(tostring(v));
				end

				-- -- print("typeof", v:GetType())
				-- if (string.match(typeof(v), "CS.System.Enum")) then
				-- 		local tmp = EditorGUILayout.EnumPopup(v);
				-- 		self[k] = tmp;
				-- elseif (string.match(typeof(v), "CS.UnityEngine.Component")) then
				-- 	EditorGUILayout.ObjectField(v.gameObject, CS.UnityEngine.Object, true);
				-- else
				-- 	EditorGUILayout.LabelField(tostring(v));
				-- end
			end
			return v;
		end

		-- local verticalScope = EditorGUILayout.VerticalScope("box")
		do
			-- local k,v = next(self)
			-- while k ~= nil do
			for k, v in pairs(self) do
				-- print(k, v, type(v))
				local kk = tostring(k);
				local vtype = type(v)
				 if (kk == "_G") then
					goto continue;
				 end
				if (type(k) == "number") then
					kk =  "[" .. kk .. "]";
				end
				-- value
				if (vtype == "table") then
					local t = v;
					rawset(t, "Name", kk);
					local isdrawed2 = rawget(t, "__Drawed");
					if(not isdrawed2) then
						--  t.RawSet("Drawed", true);
						opt.ltname = kk
						opt.indent = 1
						Draw(t , opt);
					else
					 	EditorGUILayout.LabelField(kk .. "->" .. tostring(t));
					end
				else -- number string boolean function userdata
					if (kk == "Foldout" or kk == "Name") or (grep ~= nil and kk:find(grep) == nil and tostring(v):find(grep) == nil) then
						goto continue;
					end
					EditorGUILayout.BeginHorizontal()
					do
						local svtype = vtype
						if vtype == "userdata" and type(v.GetType) == "function" then
							local ct = v:GetType()
							if ct then
								svtype = tostring(ct):gsub('.*%.(.*):.*', '<%1>')
							end
						end
						EditorGUILayout.LabelField(kk .. ":" .. svtype); -- draw key
						drawv(k,v);
					end
					EditorGUILayout.EndHorizontal()
				end
				if (vtype == "userdata") then
					local umeta = getmetatable(v)
					if (string.match(tostring(v), "LuaMonoBehaviour")) then
						local t = v.Lua;
						opt.ltname = ""
						opt.indent = 1
						Draw(t , opt);
					else
						if umeta ~= nil then
						--	if type(umeta) == "table" then 
						--		opt.ltname = "_ud_meta"
						--		opt.indent = 1
						--		_Draw(umeta , opt);
						--	end
						--else
							EditorGUILayout.BeginHorizontal()
							do
								EditorGUILayout.LabelField("_ud_meta");
								EditorGUILayout.LabelField(tostring(umeta));
							end
							EditorGUILayout.EndHorizontal()
						end
					end
				end
				::continue::
			end -- for

			-- metatable
			local meta = getmetatable(self);
			if (meta ~= nil) then
				if type(meta) == "table" then
					opt.ltname = "__meta"
					opt.indent = 1
					Draw(meta, opt);
				else
					EditorGUILayout.BeginHorizontal()
					do
						EditorGUILayout.LabelField("__meta:" .. type(meta));
						EditorGUILayout.LabelField(tostring(meta));
					end
					EditorGUILayout.EndHorizontal()
				end
			end
		end -- do
		-- rawset(self, "Drawed", false);
	end
	EditorGUI.indentLevel = EditorGUI.indentLevel - opt.indent;
end
local function CleanDrawedFlag(self)
	if type(self) == "table" then
		if rawget(self, "__Drawed") then
			rawset(self, "__Drawed", undef)
		end
	end
end
function Draw(self, opt)
	_Draw(self, opt)
	CleanDrawedFlag(self)
end
BridgingClass.Draw = Draw

function DrawObj(name, obj, begincb, endcb)
--[[
	if(begin ~= null)begin()end
	if (obj is bool) then
		obj = EditorGUILayout.Toggle(name, (bool)obj);
	elseif (obj is Enum) then
		if (name.ToLower().Contains("flag")) then
			obj = EditorGUILayout.EnumFlagsField(name, (Enum)obj);
		else
			obj = EditorGUILayout.EnumPopup(name, (Enum)obj);
	elseif (obj is int) then
		obj = EditorGUILayout.IntField(name, (int)obj);
	elseif(obj is uint) then
		int vv = Convert.ToInt32((object)obj);
		obj = EditorGUILayout.IntField(name, vv);
	elseif ( obj is long) then
		if (name.ToLower().Contains("time")) then
			EditorGUILayout.LabelField(name, DateTime.FromFileTime((long)obj).ToString());
		else
			obj = EditorGUILayout.LongField(name, (long)obj);
	elseif (obj is double or obj is float) then
		obj = EditorGUILayout.FloatField(name, (float)obj);
	elseif (obj is string) then
		if (name.ToLower().EndsWith("s")) then
			EditorGUILayout.LabelField(name);
			obj = EditorGUILayout.TextArea((string)obj)
				.Replace("\r", "")
				.Replace("\n\n", "\n")
				.TrimStart(' ', '\n', '\t')
				.TrimEnd(' ', '\n', '\t');
		else
			obj = EditorGUILayout.TextField(name, (string)obj);
		end
	elseif (obj is IList) then
		var il = obj as IList;
		var foldouti = tmpFoldout[name];
		DrawList(name, il, ref foldouti);
		tmpFoldout[name] = foldouti;
	elseif (obj is IDictionary) then
		var f = tmpFoldout[name];
		DrwaDic(name, obj as IDictionary, ref f);
		tmpFoldout[name] = f;
	elseif (obj is XLua.LuaTable) then
		(obj as XLua.LuaTable).Draw(name); then
	elseif (obj is UnityEngine.Object) then
		obj = EditorGUILayout.ObjectField(name, obj as UnityEngine.Object, typeof(UnityEngine.Object), true);
	else
		DrawComObj(name, obj);
	end
	if(end~= null)end();
]]
end

function BridgingClass.GetTable(name, id, key)
	if     Lua_Table[name] == nil 
		or Lua_Table[name][id] == nil
		or Lua_Table[name][id][key] == nil
	then
		print("table["..name.."]["..tostring(id).."]["..key.."] not found")
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