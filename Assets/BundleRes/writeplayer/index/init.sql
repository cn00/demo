-- CREATE TABLE IF NOT EXISTS "program"
-- (
--     "id"   INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
--     "name" TEXT,
--     "type" TEXT
-- );


CREATE TABLE IF NOT EXISTS "item"
(
    "id"             INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    "name"           TEXT,
    "url"            TEXT,
    "cpath"          TEXT,
    "tpath"          TEXT,
    "text"           TEXT
);

CREATE TABLE IF NOT EXISTS "cache_info"
(
    path   text not null primary key,
    etag   text,
    length int
);

-- CREATE TABLE IF NOT EXISTS "text"
-- (
--     id integer default 1 not null
--         constraint text_pk
--             primary key autoincrement,
--     text text not null
-- );


-- -- view
-- drop view if exists "item_view";
-- create view "item_view" AS
-- select 
--        i.*, 
--        t.text,
--        p.name,
--        p.type
-- from 
--     "item" as i
-- left join 
--     "text" as t
-- left join
--     program as p
-- ON 
--     i.tid = t.id 
--     and i.pid = p.id;
