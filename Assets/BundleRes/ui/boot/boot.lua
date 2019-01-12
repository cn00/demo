local util = require "utility.xlua.util"
require("lua.utility.BridgingClass")
local lpeg = require "lpeg"
local mobdebug = require('ui.boot.mobdebug')

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

local Game = _G.Game or {}
_G.Game = Game

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
        print("boot.coroutine_boot")
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
        Game.manager = manager
        this.manager = manager:GetComponent("LuaMonoBehaviour").luaTable

        yield_return(UnityEngine.WaitForSeconds(0.01)) -- wait for child object awaked

        this.msgmanager = this.manager.message

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


		yield_return(CS.UpdateSys.Instance:CheckUpdate())
        print("UpdateSys.CheckUpdate 1")

		yield_return(CS.NetSys.Instance:Init())
        print("NetSys 1")

	    obj = nil
	    yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
	        obj = asset;
	    end))
	    print("lua login 1", obj);
	    local login = GameObject.Instantiate(obj);

        loading:SetActive(false)

        yield_return(UnityEngine.WaitForSeconds(9))
        this.msgmanager.Trigger("test001", {k1 = 1, k2 = 2, k3 = "asdfg"})
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
    
    print("UnityEditor.EditorApplication.applicationContentsPath", CS.UnityEditor.EditorApplication.applicationContentsPath)
    print("UnityEditor.EditorApplication.applicationPath", CS.UnityEditor.EditorApplication.applicationPath)

    boot.mobdebug = mobdebug

    boot.coroutine_boot(1,2,2,4)
    -- boot.breakInfoFun,boot.xpcallFun = require("luadebug.LuaDebug")("localhost", 7003)
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
