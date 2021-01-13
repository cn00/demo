

local util = require "util"
local json = require("json")
local sqlite = require("lsqlite3")

local function Json2Sqlite(path, db)
    if(path:sub(-5) ~= ".json" )then return end
    local f = io.open(path, "r")
    local s = f:read("*a")
    local t = json.decode(s)
    for k, v in pairs(t) do
        local t = type(v)
        --local n = t == "number" and v or #v
        if t == "string" then v = string.gsub(v, "'", "''")end
        --print(k, n, t)
    end
    local sql = string.format("insert into poetry (id , dynasty , about , poet_id, poet_name , star , name , tags , fanyi , content , shangxi) "
    .."VALUES ('%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s')"
    , t.id , t.dynasty , t.about , t.poet.id, t.poet.name , t.star , t.name , table.concat(t.tags, "|") 
    , t.fanyi and t.fanyi:gsub("<[^>]+>", "") --:gsub("[ \t]+", "") 
    , t.content:gsub("<[^>]+>", "") --:gsub("%s*", "") 
    , t.shangxi and t.shangxi:gsub("<[^>]+>", "") --:gsub("[ \t]+", "")
    )
    local ok = db:exec(sql)
    local errms = ""
    if ok ~= sqlite.OK then 
        errms = db:errmsg()
        print("sql error", errms)
    end
    local f = io.open(path..".sql", "w+")
    f:write(errms .. "\n")
    f:write(sql)
    f:close()
end

local dbp = "/Volumes/Data/test/ab/Android/db.db"
local csql = [[
CREATE TABLE "poetry" (
	"id"	INTEGER NOT NULL UNIQUE,
	"name"	TEXT NOT NULL,
	"dynasty"	TEXT,
	"content"	TEXT NOT NULL,
	"tags"	TEXT NOT NULL,
	"fanyi"	TEXT,
	"shangxi"	TEXT,
	"about"	TEXT,
	"poet_name"	TEXT,
	"star"	INTEGER,
	PRIMARY KEY("id" AUTOINCREMENT)
);]]
local db = sqlite.open(dbp)
local p = "/Volumes/Data/test/poetry/poetry/poetry_1.json"
local count = 0
util.GetFiles("/Volumes/Data/test/poetry/poetry",function(p)
    count = 1+count
    print(count, p)
    Json2Sqlite(p, db)
end, "json")
db:close()
return Json2Sqlite