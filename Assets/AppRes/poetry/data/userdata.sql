

CREATE TABLE IF NOT EXISTS "history" (
     "id"	    INTEGER NOT NULL UNIQUE,
     "scoreA"	INTEGER NOT NULL,
     "scoreB"	INTEGER NOT NULL,
     "useTime"	INTEGER NOT NULL,
     "nameA"	TEXT,
     "nameB"	TEXT,
     "date"	    TEXT NOT NULL,
     PRIMARY KEY("id" AUTOINCREMENT)
);

CREATE TABLE IF NOT EXISTS "version" (
    "path"      TEXT NOT NULL UNIQUE,
    "version"   TEXT NOT NULL,
    "date"      TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS "property" (
    "key"      TEXT NOT NULL UNIQUE,
    "value"   TEXT NOT NULL
);