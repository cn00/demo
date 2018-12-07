local util = require "lua.utility.xlua.util"
require("lua.utility.BridgingClass")
local lpeg = require "lpeg"
local ffi = require "ffi"
local mobdebug = require('ui.boot.mobdebug')

require "nslua"

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject

local print = function ( ... )
    _G.print("[boot]", ... )
    -- _G.print("[boot]", debug.traceback())
end

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

function octest()
    print"objc test begin"

    -- local fileurl = NSURL:fileURLWithPath_("LuaBridge.lua")
    -- print("objc fileurl:", fileurl)

    -- [[NSBundle mainBundle] pathForResource:@"LuaBridge" ofType:@"lua"];
    print("get objc path1:OCLuaBridge.lua")
    local path1 = OC.NSBundle.mainBundle:pathForResource_ofType_("OCLuaBridge", "lua")
    print("objc path_1:", path1)
    local f = io.open(path1)
    local c = f:read("*a")
    f:close()
    print("objc file content:", c:gsub("\n", "\\n"))

    print"objc test end"
end

local function didLoginSuccessWithAccessKey( accessKey, uid )
    print("didLoginSuccessWithAccessKey", accessKey, uid)
end
_G.didLoginSuccessWithAccessKey = didLoginSuccessWithAccessKey

local function oc_blsdk_init()
    print("oc_blsdk_login begin")
    --[[ -- init sdk
        [[BLGameSdk defaultGameSdk] initWithGameid:@"85"
                                            cpId:@"2"
                                        serverid:@"159"
                                            appKey:@"bcf9f03f94234804a2aa11f6c9f4ccf0"
                                        sandboxKey:@"abc123"
                                        delegate:self];
    ]]
    -- BLSdkInit(AppId, "1", ServerId, AppKey, SandBoxKey);
    OC.BLGameSdk.defaultGameSdk:initWithGameid_cpId_serverid_appKey_sandboxKey_delegate_(
         "85"
        ,"2"
        ,"159"
        ,"bcf9f03f94234804a2aa11f6c9f4ccf0"
        ,"abc123"
        ,OC.BLGameSdkDelegateApp.Instance
    )
end

local function oc_blsdk_login()
    print("oc_blsdk_login begin")
    --[[ [[BLGameSdk defaultGameSdk] showLoginView];]]
    OC.BLGameSdk.defaultGameSdk:showLoginView()
    print("oc_blsdk_login end")
end

local function ffitest()
    local testffi = ffi.cdef [[
        int printf ( const char * format, ... );
        int fprintf ( int * stream, const char * format, ... );
    ]]
    boot.testffi = testffi

    print("testffi", util.dump(testffi))
    local ffir = ffi.C.printf("testffi: %s\n", "fooffffffffff")
    print("ffir", ffir)
    local f = io.open(CS.AssetSys.CacheRoot .. "ffi.test.txt", "a")
    ffi.C.fprintf(f, "test: %s\n", "foo")
    ffi.C.fprintf(f, "test: %s\n", "foo")

    ffi.cdef [[
        typedef struct {
            int fake_id;
            unsigned int len;
        } CSSHeaderee;
        typedef struct {
            CSSHeaderee header;
            float x;
            float y;
            float z;
            float w;
        } Vector4;
    ]]

    local vector = ffi.typeof('Vector4 *')
    local v = CS.UnityEngine.Vector4(12.3, 23.4, 34.5, 45.6)
    local vn = ffi.cast(vector, v)
    boot.vn = vn
    print("vector.dump", ffi.typeof(vn), ffi.type(vn))
    if vn.header.fake_id == -1 then
        print('vector { ', vn.x, vn.y, vn.z, vn.w, '}')
    else
        print('please gen code')
    end
end

-- print("boot:", util.dump(boot))

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
        -- print(debug.traceback("test traceback"))
        -- print(table.unpack({...}), debug.traceback( "coroutine_boot "..tostring({...})  ))
        -- yield_return(UnityEngine.WaitForSeconds(1))
        local obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset("ui/loading/loading.prefab", function(asset)
            obj = asset
        end))
        print(obj)
        local loading = GameObject.Instantiate(obj)

        yield_return(CS.AssetSys.Instance:GetBundle("lua/socket.bd", function ( bundle )
            print(bundle)
        end))
        
        obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset("common/manager/manager.prefab", function(asset)
            obj = asset
        end))
        local manager = GameObject.Instantiate(obj)
        this.msgmanager = manager:GetComponent("LuaMonoBehaviour").luaTable

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

        -- yield_return(UnityEngine.WaitForSeconds(1))
        this.msgmanager.Trigger("test001", {k1 = 1, k2 = 2, k3 = "asdfg"})


		yield_return(CS.UpdateSys.Instance:CheckUpdate())
        print("UpdateSys.CheckUpdate 1")

		yield_return(CS.NetSys.Instance:Init())
        print("NetSys 1")

        obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset("data/fb/monsterdata_txt.mon.txt", function(asset)
            print("monsterdata_txt", (asset:GetType()))
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

        loading:SetActive(false)
        
        ffitest()
        
        octest()

        oc_blsdk_init()
        yield_return(UnityEngine.WaitForSeconds(0.5))
        oc_blsdk_login()
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
    mobdebug.start("localhost", 8172)
    print("boot.Start mobdebug", mobdebug)

    boot.mobdebug = mobdebug

    boot.coroutine_boot(1,2,2,4)
    -- boot.breakInfoFun,boot.xpcallFun = require("luadebug.LuaDebug")("localhost", 7003)
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

local Time = UnityEngine.Time
local lastGCTime = 0
local GCInterval = 1
function boot.FixedUpdate()
    if (Time.time - lastGCTime > GCInterval) then
        -- boot.breakInfoFun()
        lastGCTime = Time.time
    end
end

-- function boot.Update()

-- end

-- function boot.LateUpdate()

-- end

-- function boot.OnDestroy()
--     print("boot.OnDestroy")

-- end
    
return boot
