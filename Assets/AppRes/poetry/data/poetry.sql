
-- table
CREATE TABLE IF NOT EXISTS "poetry" (
    "id"	    INTEGER NOT NULL UNIQUE,
    "star"	    INTEGER,
    "authorId"	INTEGER,
    "title"	    TEXT NOT NULL,
    "content"	TEXT NOT NULL,
    "tags"	    TEXT,
    "fanyi"	    TEXT,
    "shangxi"	TEXT,
    "about"	    TEXT,
    PRIMARY KEY("id" AUTOINCREMENT)
);

-- index
CREATE INDEX "poetry_index" ON "poetry" (
    "id",
    "title",
    "tags",
    "star",
    "authorId"
);
CREATE INDEX IF NOT EXISTS "author_index" ON "author" (
    "id",
    "name",
    "star",
    "dynasty"
);

-- view
CREATE VIEW IF NOT EXISTS poetryAuthor AS
SELECT
	p.id, p.title, a.dynasty, a.name author, p.content, p.tags, p.star pstar,p.about,a.id aid
FROM poetry p 
LEFT JOIN author a ON p.authorId = a.id;

CREATE VIEW poetry100_57_author as
SELECT a.id xh5, b.id xh7, a.author author5, b.author author7, a.content content5, b.content content7
FROM
(
        SELECT *
        FROM poetry100 p1
        WHERE tags like '%五言'
) a
LEFT JOIN
(
    SELECT *
    FROM poetry100 p1
    WHERE tags like '%七言'
) b ON a.author = b.author;