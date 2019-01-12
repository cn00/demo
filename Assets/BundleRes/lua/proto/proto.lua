

-- s2c = c2s + 10000000
local ptid = {
	Action_Move_c2s = 10000001,
	Action_Move_s2c = 20000001,
}

local bfbs_names = {
	"proto/bfbs/proto.bfbs.txt",
	-- "proto/bfbs/common_obj.bfbs.txt",
}

local proto = {
	[ptid.Action_Move_c2s] = "Action.Move",
	[ptid.Action_Move_s2c] = "Action.Move",
}

setmetatable(proto, {__index = function(t, k)
	if ptid[k] then 
		return t[ptid[k]]
	end
end})

return function ()
	return proto, ptid, bfbs_names
end 
