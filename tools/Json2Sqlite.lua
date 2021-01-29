local util = require "util"
local json = require("json")
local sqlite = require("lsqlite3")

local function Poetry2Sqlite(path, db)
    if (path:sub(-5) ~= ".json") then
        return
    end
    local f = io.open(path, "r")
    local s = f:read("*a")
    local t = json.decode(s)
    for k, v in pairs(t) do
        local tp = type(v)
        --local n = t == "number" and v or #v
        if tp == "string" then
            t[k] = string.gsub(v, "'", "''")
        end
        --print(k, n, t)
    end
    local sql = string.format("insert into poetry (id , dynasty , about , poet_id, poet_name , star , name , tags , fanyi , content , shangxi) "
            .. "VALUES ('%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s')"
    , t.id, t.dynasty
    , t.about --and t.about:gsub("<[^>]+>", "")
    , t.poet.id, t.poet.name, t.star, t.name, table.concat(t.tags, "|")
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
    local f = io.open(path .. ".sql", "w+")
    f:write(errms .. "\n")
    f:write(sql)
    f:close()
end

local function Poet2Db(path, db)
    if (path:sub(-5) ~= ".json") then
        return
    end
    local f = io.open(path, "r")
    local s = f:read("*a")
    local t = json.decode(s)
    for k, v in pairs(t) do
        local tp = type(v)
        --local n = t == "number" and v or #v
        if tp == "string" then
            t[k] = string.gsub(v, "'", "''")
        end
        --print(k, n, t)
    end
    local sql = string.format("insert into poet (id ,name ,image ,content ,desc ,dynasty ,star) "
            .. "VALUES ('%s' ,'%s' ,'%s' ,'%s' ,'%s' ,'%s' ,'%s')"
        ,t.id
        ,t.name
        ,t.image
        ,t.content and t.content:gsub("<[^>]+>", "")
        ,t.desc and t.desc:gsub("<[^>]+>", "")
        ,t.dynasty and t.dynasty:gsub("<[^>]+>", "")
        ,t.star
    )
    local ok = db:exec(sql)
    local errms = ""
    if ok ~= sqlite.OK then
        errms = db:errmsg()
        print("sql error", errms)
    end
    local f = io.open(path .. ".sql", "w+")
    f:write(errms .. "\n")
    f:write(sql)
    f:close()
end

local dbp = "/Volumes/Data/test/ab/db.db"
local createPoetrySql = [[
    CREATE TABLE IF NOT EXISTS "poetry" (
        "id"	INTEGER NOT NULL UNIQUE,
        "name"	TEXT NOT NULL,
        "dynasty"	TEXT,
        "content"	TEXT NOT NULL,
        "tags"	TEXT NOT NULL,
        "fanyi"	TEXT,
        "shangxi"	TEXT,
        "about"	TEXT,
        "poet_id"	INTEGER,
        "poet_name"	TEXT,
        "star"	INTEGER,
        PRIMARY KEY("id" AUTOINCREMENT)
    );
    CREATE INDEX IF NOT EXISTS "poetry_index" ON "poetry" (
        "id",
        "name",
        "poet_name",
        "dynasty",
        "tags",
        "star"
    );
]]
local db = sqlite.open(dbp)
db:exec(createPoetrySql)
local createPoetSql = [[
    CREATE TABLE IF NOT EXISTS "poet" (
        "id"	    INTEGER NOT NULL UNIQUE,
        "name"	    TEXT NOT NULL,
        "content"	TEXT,
        "desc"	    TEXT,
        "dynasty"   TEXT,
        "star"	    INTEGER,
        "image"	    TEXT,
        PRIMARY KEY("id" AUTOINCREMENT)
    );
    CREATE INDEX IF NOT EXISTS "poet_index" ON "poet" (
        "id",
        "name",
        "desc"
    );
]]
db:exec(createPoetSql)

local count = 0
util.GetFiles("/Volumes/Data/test/poetry/poet", function(p)
    count = 1 + count
    print(count, p)
    Poet2Db(p, db)
end, "json")


--local count = 0
--util.GetFiles("/Volumes/Data/test/poetry/poetry", function(p)
--    count = 1 + count
--    print(count, p)
--    Poetry2Sqlite(p, db)
--end, "json")
db:close()
