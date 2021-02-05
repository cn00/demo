package.path = package.path .. ";/Volumes/Data/test/Assets/AppRes/lua/utility/?.lua"
local sqlite = require "lsqlite3"
local util = require "util"
local db = sqlite.open("/Volumes/Data/test/ab/Android/db.db")

local function upTags3457()
    local tags = {
        [3] = "三言",
        [4] = "四言",
        [5] = "五言",
        [7] = "七言",
    }
    local rows = {}
    for row in db:nrows("select id, title, content from poetry") do
        rows[1+#rows] = row
    end
    print("rows", #rows)
    --db:close()
    ----sqlite.complete(db)
    --db = sqlite.open("/Volumes/Data/test/ab/Android/db.db")
    for _, row in ipairs(rows) do
        local id, title, content= row.id, row.title, row.content
        content = string.gsub(content, "，", "|")
                        :gsub("。", "|")
                        :gsub("？", "|")
                        :gsub("！", "|")
        local splits= string.split(content, "|")
        local length = utf8.len(splits[1])
        --print(id, title, #splits, length)
        for i= 2, #splits do
            if length ~= utf8.len(splits[i]) then
                length = 0
                break
            end
        end
        --if length > 0 then
        local tag = tags[length]
        if tag then
            local sql = string.format([[update poetry set tags = tags || '|%s' where id = %s and not tags like '%%%s%%']], tag, id, tag)
            print(id, title, tag)
            assert(sqlite.OK == db:exec(sql), db:errmsg())
        end
        --end
    end
end
--upTags3457()

--- 17s
local function collectTags0()

    local vm, ok = db:prepare([[insert or ignore into tags (tag) VALUES (?)]])
    print(vm, ok, db:errmsg())
    for row in db:nrows("select tags from poetry group by tags") do
        local tags = row["tags"] or ""
        local ts = string.split(tags, "|")
        print(table.unpack(ts))
        for i, t in pairs(ts) do
            assert(sqlite.OK==vm:bind_values(t))
            assert(vm:step() == sqlite.DONE)
            assert(vm:reset() == sqlite.OK)
        end
    end
    assert(vm:finalize() == sqlite.OK)
end
collectTags0()

--- 18s
local function collectTags1()
    local tags = {}
    for row in db:nrows([[select tags tag from poetry group by tag]]) do
        tags[1+#tags] = row.tag
    end
    print("tags", #tags)
    for k, v in ipairs(tags) do
        local ts = string.split(v, "|")
        for i, vv in ipairs(ts) do
            local sql = string.format([[INSERT or IGNORE into tags (tag) values ('%s')]], vv)
            assert(sqlite.OK == db:exec(sql), db:errmsg() .. sql)
            print(v, sql)
        end
    end
end
--collectTags1()