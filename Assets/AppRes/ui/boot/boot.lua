local util = require "util"
local xutil = require "xlua.util"
require("lua.utility.BridgingClass")
--local lpeg = require "lpeg"
local mobdebug = require('ui.boot.mobdebug')

local CS = CS
local AssetSys = CS.AssetSys
local File = CS.System.IO.File
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local UnityWebRequest = UnityEngine.Networking.UnityWebRequest

local AppGlobal = _G.AppGlobal or {}
_G.AppGlobal = AppGlobal

local print = function(...)
    _G.print("[boot]", ...)
    -- _G.print("[boot]", debug.traceback())
end

local function mysqlTest()

    local luasql = require "luasql.mysql"
    print("luasql=",luasql)
    local mysql = luasql.mysql()
    local db, err = mysql:connect("a3_m_305", "a3", "654123", "10.23.24.239")
    if err ~= nil then print("mysql", err, debug.traceback()) return end
    -- local sql = "show tables;"
    local sql = "select s.jp_name, c.* from a3_m_305.m_card c left join a3_m_305.m_string_item s on c.card_name_id = s.string_id limit 20;"
    local res, err = db:execute(sql)
    if err ~= nil then print(err) return end
    print("colnames", res:numrows(), util.dump(res:getcolnames(false)))
    for i=0,res:numrows()-1 do
        --local t = {res:fetch()}
         local t = {}
         res:fetch(t, "a")
         print("mysql", i, util.dump(t, false))
    end
    db:close()
end
--mysqlTest()

local function p7zipTest()
    require ("p7zip")
    local stderr = p7zip.g_StdErr:Open("luastderr.txt", "a")
    local stdout = p7zip.g_StdOut:Open("luastdout.txt", "a")
    p7zip.g_StdErr:Print("stderr from lua\n")
    local r, e = p7zip.execute("7z l Assets/TitleInfo.xlsx -y")
    print("7z a doc.7z doc -y:", r, e)
    p7zip.g_StdErr:Flush()
    p7zip.g_StdOut:Flush()
end
--p7zipTest()

local sqlite = require("lsqlite3")
print("sqlite", sqlite)
package.cpath = package.cpath..";./Assets/XLua/Plugins/OSX/lib?.dylib"
local lfs = require "lfs"
print("lfs", lfs)

local boot = {}
local this = boot

local yield_return = xutil.async_to_sync(function(to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

-- local printbak = _G.print
-- _G.print = function(...)
--     printbak(table.unpack({...}), debug.traceback())
-- end

function boot.coroutine_boot(first, ...)
    -- local args = {...}
    xutil.coroutine_call(function(...)
        print("boot.coroutine_boot 0")
        -- print(debug.traceback("test traceback"))
        -- print(table.unpack({...}), debug.traceback( "coroutine_boot "..tostring({...})  ))
        -- yield_return(UnityEngine.WaitForSeconds(1))


        -- yield_return(CS.AssetSys.Instance:GetBundle("lua/socket.bd", function ( bundle )
        --     print(bundle)
        -- end))

        local obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset("common/manager/manager.prefab", function(asset)
            obj = asset
        end))
        obj = GameObject.Instantiate(obj)
        this.manager = obj:GetComponent("LuaMonoBehaviour").Lua
        yield_return(UnityEngine.WaitForSeconds(0.3))
        this.manager.Scene.layer = {
            front = this.front_Transform,
            middle = this.middle_Transform,
            back = this.back_Transform
        }
        AppGlobal.manager = this.manager

        print("boot.coroutine_boot 1")
        
        yield_return(CS.AssetSys.Instance:GetAsset("ui/dialog/dialog01.prefab"))
        
        yield_return(CS.AssetSys.Instance:GetAsset("font/fzxz/方正小篆体.ttf"))

        print("boot.coroutine_boot 2 方正小篆体")

        -- yield_return(UnityEngine.WaitForSeconds(0.01)) -- wait for child object awaked

        -- this.msgmanager = this.manager.message

        -- this.msgmanager.AddListener("test001", function ( data )
        --     print("test001", data)
        -- end)
        -- this.msgmanager.AddListener("test001", function ( data )
        --     print("test001 11", data)
        -- end)
        -- this.msgmanager.AddListener("test002", function ( data )
        --     print("test002", data)
        -- end)
        -- print("AddListener test001")


        yield_return(CS.UpdateSys.Instance:CheckUpdate())
        print("boot.coroutine_boot 3 CheckUpdate")

        -- yield_return(CS.NetSys.Instance:Init())
        -- print("NetSys 1")

        --obj = nil
        --yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
        --    obj = asset;
        --end))
        --print("lua login 1", obj);
        --local login = GameObject.Instantiate(obj);


        -- yield_return(CS.AssetSys.Instance:GetAsset("video/op/op.prefab", function(asset)
        --     obj = asset
        -- end))
        -- print(obj)
        -- local write_player = GameObject.Instantiate(obj)

        --manager.Scene.push("index/index.prefab")
        this.manager.Scene.push("poetry/index/index.prefab")
        print("boot.coroutine_boot 3 poetry/index/index")
        --this.manager.Scene.push("poetry/match/match.prefab")
        --manager.Scene.push("don-quixote/index/index.prefab")
        --manager.Scene.push("don-quixote/linkText/linkText.prefab")
        
        --obj = nil
        --yield_return(CS.AssetSys.Instance:GetAsset("writeplayer/index/index.prefab", function(asset)
        --    obj = asset
        --    print("get index", obj)
        --end))
        --print(obj)
        --local go = GameObject.Instantiate(obj)
        --print("index", go)

        -- yield_return(UnityEngine.WaitForSeconds(9))
        -- this.msgmanager.Trigger("test001", {k1 = 1, k2 = 2, k3 = "asdfg"})
        
        print("boot.coroutine_boot end")
    end)
end

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.back_Transform = back:GetComponent(typeof(CS.UnityEngine.Transform))
    this.front_Transform = front:GetComponent(typeof(CS.UnityEngine.Transform))
    this.middle_Transform = middle:GetComponent(typeof(CS.UnityEngine.Transform))
end
--AutoGenInit End

function boot.Awake()
    boot.AutoGenInit()

end

-- function boot.OnEnable()
--     print("boot.OnEnable")

-- end

function boot.Start()
    mobdebug.start("localhost", 8173)
    print("boot.Start mobdebug 8173", mobdebug)

    --print("UnityEditor.EditorApplication.applicationContentsPath", CS.UnityEditor.EditorApplication.applicationContentsPath)
    --print("UnityEditor.EditorApplication.applicationPath", CS.UnityEditor.EditorApplication.applicationPath)

    boot.mobdebug = mobdebug

    boot.coroutine_boot(1, 2, 2, 4)
    -- boot.breakInfoFun,boot.xpcallFun = require("luadebug.LuaDebug")("localhost", 7003)
end

--local Time = UnityEngine.Time
--local lastGCTime = 0
--local GCInterval = 1
--function boot.FixedUpdate()
--    if (Time.time - lastGCTime > GCInterval) then
--         boot.breakInfoFun()
--        lastGCTime = Time.time
--    end
--end



return boot
