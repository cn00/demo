

_G.FBSizePrefix = true

-- c2s 10000001 ~ 19999999
-- s2c 20000001 ~ 29999999
local ptid = {
	monsterc2s = 10000001,
	monsters2c = 20000001,
}

local bfbs_names = {
	"proto/bfbs/sample.bfbs.txt",
	"proto/bfbs/common_obj.bfbs.txt",
}

local proto = {
	[ptid.monsterc2s] = "Sample.Monster_c2s",
	[ptid.monsters2c] = "Sample.Monster_s2c",
}

setmetatable(proto, {__index = function(t, k)
	if ptid[k] then 
		return t[ptid[k]]
	end
end})

return function ()
	return proto, ptid, bfbs_names
end 
