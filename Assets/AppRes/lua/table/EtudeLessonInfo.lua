-- usage: 
--	EtudeLessonInfo[id][ColumnName] 
--	EtudeLessonInfo[id].ColumnName

local ColumnIdx={
	id=1,
	hdid=2,
	pid=3,
	Text=4,
}

local EtudeLessonInfo={
	[1]={1,	9,	1,	"常夏的海滩",	},
	[2]={2,	9,	2,	"灼热的海滩",	},
	[3]={3,	9,	3,	"业火的海滩",	},
	[4]={4,	13,	1,	"悠闲的万圣节",	},
	[5]={5,	13,	2,	"兴奋的万圣节",	},
}

for k,v in pairs(EtudeLessonInfo) do
	if type(v) == "table" then
		setmetatable(v,{
		__newindex=function(t,kk) print("warning: attempte to change a readonly table") end,
		__index=function(t,kk)
			if ColumnIdx[kk] ~= nil then
				return t[ColumnIdx[kk]]
			else
				print("err: \"EtudeLessonInfo\" have no field ["..kk.."]")
				return nil
			end
		end})
	end
end
return EtudeLessonInfo
