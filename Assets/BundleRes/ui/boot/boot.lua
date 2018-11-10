
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"
require("lua.utility.BridgingClass")

local boot = {
    1, 2, 3, 4,
    s = "sssss",
    {5, 6, 7, 8, 9,},
    num = 9999,
    hahahaah = {
        dd = 999,
        11,22,33,44
    }
}
local self = boot

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

-- local printbak = _G.print
-- _G.print = function(...)
--     printbak(table.unpack({...}), debug.traceback())
-- end

function boot.coroutine_boot(first, ...)
    -- local args = {...}
    util.coroutine_call(function(...)
        print(debug.traceback("test traceback"))
        -- print(table.unpack({...}), debug.traceback( "coroutine_boot "..tostring({...})  ))
        -- yield_return(CS.UnityEngine.WaitForSeconds(1))
        local obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset("ui/loading/loading.prefab", function(asset)
            obj = asset
        end))
        local loading = CS.UnityEngine.GameObject.Instantiate(obj)

		yield_return(CS.UpdateSys.Instance:CheckUpdate())
        print("UpdateSys.CheckUpdate 1")
        
        obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset("common/manager/manager.prefab", function(asset)
            obj = asset
        end))
        local manager = CS.UnityEngine.GameObject.Instantiate(obj)

		yield_return(CS.NetSys.Instance:Init())
        print("NetSys 1")

        obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset("data/fb/monsterdata_txt.mon.txt", function(asset)
            obj = asset.Data
        end))
        boot.fbtestdata = obj
        print("fbtestdata:", obj)
        boot.FlatbuffersTest(obj)

	    obj = nil
	    yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
	        obj = asset;
	    end))
	    print("lua login 1", obj);
	    local login = CS.UnityEngine.GameObject.Instantiate(obj);

        -- yield_return(CS.UnityEngine.WaitForSeconds(3))

	    loading:SetActive(false)

    end)
end

--AutoGenInit Begin
function boot.AutoGenInit()
end
--AutoGenInit End

function boot.Awake()
	boot.AutoGenInit()
end

-- function boot.OnEnable()
--     print("boot.OnEnable")

-- end

function boot.Start()
    print("boot.Start")
    boot.coroutine_boot(1,2,2,4)
end

function boot.FlatbuffersTest(buffer)
    local fb = require("flatbuffers")
    boot.flatbuffers = assert(require("flatbuffers"))
    monster = assert(require("proto.MyGame.Example.Monster"))  
    test = assert(require("proto.MyGame.Example.Equipment"))
    vec3 = assert(require("proto.MyGame.Example.Vec3"))

    boot.fbbuf = fb.binaryArray.New(buffer)
    boot.monsterN = monster.New()
    boot.monster = monster.GetRootAsMonster(boot.fbbuf, 0)
    boot.monstert = {
        Hp = boot.monster:Hp(),
        mana = boot.monster:Mana(),
        color = boot.monster:Color(),
    }
    print("Mana():", boot.monster:Mana())
    -- for k,v in pairs(getmetatable(boot.monster)["__index"]) do 
    --     print("boot.monster", k, v)
    -- end
end
-- function boot.FixedUpdate()

-- end

-- function boot.Update()

-- end

-- function boot.LateUpdate()

-- end

-- function boot.OnDestroy()
--     print("boot.OnDestroy")

-- end
    
return boot
