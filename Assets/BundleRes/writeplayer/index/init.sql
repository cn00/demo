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

-- cache_info
CREATE TABLE IF NOT EXISTS "cache_info"
(
    "id"     INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
    "path"   text    NOT NULL UNIQUE,
    "etag"   text,
    "length" int,
    "utime"  TEXT CHECK (datetime())
);
-- DROP TRIGGER if EXISTS update_cache_info_utime;
CREATE TRIGGER IF NOT EXISTS update_cache_info_utime
    AFTER UPDATE
    ON cache_info
BEGIN
    UPDATE cache_info
    SET utime = datetime('now', 'localtime')
    WHERE id = old.id;
END;

-- DROP TRIGGER if EXISTS insert_cache_info_utime;
CREATE TRIGGER insert_cache_info_utime
    AFTER INSERT
    ON cache_info
BEGIN
    UPDATE cache_info
    SET utime = datetime('now', 'localtime')
    WHERE new.id = id;
END;

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

-- -- update via view https://www.sqlitetutorial.net/sqlite-instead-of-triggers/
-- 
-- CREATE VIEW AlbumArtists(
--                          AlbumTitle,
--                          ArtistName
--     ) AS
-- SELECT Title,
--        Name
-- FROM Albums
--          INNER JOIN Artists USING (ArtistId);
-- 
-- CREATE TRIGGER insert_artist_album_trg
--     INSTEAD OF INSERT
--     ON AlbumArtists
-- BEGIN
--     -- insert the new artist first
--     INSERT INTO Artists(Name)
--     VALUES (NEW.ArtistName);
-- 
--     -- use the artist id to insert a new album
--     INSERT INTO Albums(Title, ArtistId)
--     VALUES (NEW.AlbumTitle, last_insert_rowid());
-- END;
-- INSERT INTO AlbumArtists(AlbumTitle, ArtistName)
-- VALUES ('Who Do You Trust?', 'Papa Roach');