
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"
require("lua.utility.BridgingClass")


-- _G.dump = dump

local ffi = require "ffi"

local testffi = ffi.cdef [[
    int printf ( const char * format, ... );
    int fprintf ( int * stream, const char * format, ... );
]]
print("testffi", util.dump(testffi))
local ffir = ffi.C.printf("testffi: %s\n", "fooffffffffff")
print("ffir", ffir)
local f = io.open("ffi.test.txt", "a")
ffi.C.fprintf(f, "test: %s\n", "foo")
ffi.C.fprintf(f, "test: %s\n", "foo")

local lpeg = require "lpeg"

local boot = {
    G = _G,
    Name = "boot",
    1, 2, 3, 4,
    s = "sssss",
    {5, 6, 7, 8, 9,},
    num = 9999,
    hahahaah = {
        dd = 999,
        11,22,33,44
    }
}
local this = boot
boot.ffi = ffi
boot.testffi = testffi

print("boot:", boot)

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
        -- yield_return(UnityEngine.WaitForSeconds(1))
        local obj = nil
        -- yield_return(CS.AssetSys.Instance:GetAsset("ui/loading/loading.prefab", function(asset)
        --     obj = asset
        -- end))
        -- local loading = GameObject.Instantiate(obj)
        
        obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset("common/manager/manager.prefab", function(asset)
            obj = asset
        end))
        local manager = GameObject.Instantiate(obj)

        this.msgmanager = _G.manager.message_sys_LuaMonoBehaviour.luaTable

        this.msgmanager.AddListener("test001", function ( data )
            print("test001", data)
        end)
        this.msgmanager.AddListener("test001", function ( data )
            print("test001 11", data)
        end)
        this.msgmanager.AddListener("test002", function ( data )
            print("test002", data)
        end)
        print("AddListener test001")

        yield_return(UnityEngine.WaitForSeconds(1))
        this.msgmanager.Trigger("test001", {k1 = 1, k2 = 2, k3 = "asdfg"})


		yield_return(CS.UpdateSys.Instance:CheckUpdate())
        print("UpdateSys.CheckUpdate 1")

		yield_return(CS.NetSys.Instance:Init())
        print("NetSys 1")

        obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset("data/fb/monsterdata_txt.mon.txt", function(asset)
            print(asset)
            -- obj = asset.text
            obj = asset.bytes
        end))
        boot.fbtestdata = obj
        print("fbtestdata:", #obj, obj)
        boot.FlatbuffersTest(obj)

	    obj = nil
	    yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
	        obj = asset;
	    end))
	    print("lua login 1", obj);
	    local login = GameObject.Instantiate(obj);


	    -- loading:SetActive(false)

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
    boot.flatbuffers = assert(require("flatbuffers.flatbuffers"))
    local fb = boot.flatbuffers
    boot.monsterc2s = assert(require("Sample.Monster_c2s"))
    boot.monsters2c = assert(require("Sample.Monster_s2c"))

    -- boot.fbbuf = fb.binaryArray.New(buffer)
    -- boot.monster = monster.GetRootAsMonster(boot.fbbuf, 0)
    -- boot.monstert = {
    --     Hp = boot.monster:Hp(),
    --     mana = boot.monster:Mana(),
    --     color = boot.monster:Color(),
    -- }
    -- for k, v in pairs(getmetatable(boot.monster)["__index"])do
    --     if k ~= "Init" and type(v) == "function" then
    --         print("---" .. k, v, v(boot.monster, 0))
    --         -- assert((function (  )
    --             -- boot.monstert["----" .. k] = v(boot.monster, 1)
    --         -- end)())
    --     end
    -- end

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
