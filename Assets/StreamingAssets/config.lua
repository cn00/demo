
local CS = CS
local AssetSys = CS.AssetSys
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject

-- config

local print = function ( ... )
    _G.print("[config]", ...)
end

AssetSys.WebRoot = "http://10.23.22.233/assets/test/ab/"

print(AssetSys.WebRoot)
