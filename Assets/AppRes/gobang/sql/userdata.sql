CREATE TABLE IF NOT EXISTS "gobang_history"
(
    "id"      INTEGER NOT NULL UNIQUE,
    "scoreA"  INTEGER NOT NULL,
    "scoreB"  INTEGER NOT NULL,
    "useTime" INTEGER NOT NULL,
    "nameA"   TEXT,
    "nameB"   TEXT,
    "date"    TEXT    NOT NULL,
    PRIMARY KEY ("id" AUTOINCREMENT)
);