CREATE TABLE IF NOT EXISTS "program"
(
    "id"   INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    "name" INTEGER,
    "type" TEXT
);
CREATE TABLE IF NOT EXISTS "item"
(
    "id"             INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    "pid"            INTEGER NOT NULL,
    "url"            TEXT    NOT NULL,
    "cpath"          TEXT    NOT NULL,
    "tpath"          TEXT,
    "tid"            INTEGER
);
CREATE TABLE IF NOT EXISTS "text"
(
    id integer default 1 not null
        constraint text_pk
            primary key autoincrement,
    text text not null
);

-- view
create view if not exists "item_view" AS
select 
       i.*, 
       t.text as text 
from 
    "item" as i
left join 
    "text" as t
ON 
    i.tid = t.id;
