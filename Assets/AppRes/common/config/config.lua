
local CS = CS
local dir
if UNITY_EDITOR then
    dir = "ab/"
else
    dir = CS.AssetSys.CacheRoot
end
local config = {
    dbCachePath = dir .. "db.db",
    userDbPath = dir.. "user.db",
}
print("dbCachePath", config.dbCachePath)
return config