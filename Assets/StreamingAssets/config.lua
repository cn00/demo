local CS = CS
local AssetSys = CS.AssetSys
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local Directory = CS.System.IO.Directory
local File = CS.System.IO.File

-- config

local print = function ( ... )
    _G.print("config", ...)
end

local oss = "http://192.168.31.154/ab/"
AssetSys.WebRoot = oss -- "http://10.23.24.239/assets/test/ab/"

print(AssetSys.WebRoot)

--Directory.Delete(AssetSys.CacheRoot .. "ui", true)
--Directory.Delete(AssetSys.CacheRoot .. "writeplayer", true)