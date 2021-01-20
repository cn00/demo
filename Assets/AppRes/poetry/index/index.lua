
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/12 12:07:03
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "util"
local xutil = require "xlua.util"
local sqlite3 = require("lsqlite3")
local manager = AppGlobal.manager
local config = require("config.config.config")
local socket = require("socket.core")

local dbpath = config.dbCachePath;
local userDbpath = config.userDbPath;

local yield_return = xutil.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

-- index

local print = function ( ... )
    _G.print("[index]", ...)
end

local index = {
    needQuitUdp = false,
    receiveUdp = nil, 
    sendUdp = nil,
    servers = {}, -- {{url, name}, ...}
    newServerCome = false,
}
local this = index



--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.bg_Image = bg:GetComponent(typeof(CS.UnityEngine.UI.Image))
    this.bottom_RectTransform = bottom:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.history_Button = history:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.history_Button.onClick:AddListener(this.history_OnClick)
    this.nameInput_Text = nameInput:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.qingdong_Button = qingdong:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.qingdong_Button.onClick:AddListener(this.qingdong_OnClick)
    this.roomList_LuaMonoBehaviour = roomList:GetComponent(typeof(CS.LuaMonoBehaviour))
    this.roomTemplate_RectTransform = roomTemplate:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.scrollContent_RectTransform = scrollContent:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.wangzhe_Button = wangzhe:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.wangzhe_Button.onClick:AddListener(this.wangzhe_OnClick)
end
--AutoGenInit End

function this.qingdong_OnClick()
    print('qingdong_OnClick')
end -- qingdong_OnClick

function this.wangzhe_OnClick()
    print('wangzhe_OnClick')
end -- wangzhe_OnClick

function this.history_OnClick()
    manager.Scene.push("poetry/history/history.prefab", nil, true)
end -- history_OnClick

function index.Awake()
    this.AutoGenInit()
end

function index.Start()
    --xutil.coroutine_call(function()
    --    local sql
    --    yield_return(CS.AssetSys.GetAsset("poetry/data/userdata.sql", function(asset)
    --        sql = asset.text
    --        print("userdata.sql", sql)
    --    end))
    --    local db = sqlite3.open(userDbpath);
    --    assert(sqlite3.OK == db:exec(sql), db:errmsg())
    --    db:close()
    --end)

    roomTemplate:SetActive(false)

    this.needQuitUdp = false
    this.StartUdpReceiveLoop()
    this.StartUdpBroadcastLoop()
end

function index.Update()
    if this.newServerCome then
        print("newServerCome")
        for i, v in pairs(this.servers) do
            if v.obj == nil then
                local obj = GameObject.Instantiate(roomTemplate, scrollContent.transform)
                obj:SetActive(true)
                local com = obj:GetComponent(typeof(CS.LuaMonoBehaviour)).Lua
                com.init(v)
                v.obj = obj
            end
        end
        this.newServerCome = false
    end
end

function this.joinGame_OnClick()
    local name = this.nameInput_Text.text
    uiroot:SetActive(false)
    local openMatch = function(url)
        require("stringx")
        local hostInfo = string.split(url, ":")
        print("joinGame", url, hostInfo)
        local args = {
            tp = 1, -- 0:主场， 1:客场, 2:观众, 暂定为客场，建立连接后再判定是否为观众
            hostInfo = hostInfo,
            name = name,
        }
        manager.Scene.push("poetry/match/match.prefab", args, true)
    end
    manager.Scene.push("common/qrcode/qrcode.prefab", { scanCallback = openMatch }, true)
end -- joinGame_OnClick

function this.newGame_OnClick()
    local name = this.nameInput_Text.text
    uiroot:SetActive(false)
    this.needQuitUdp = true
    manager.Scene.push("poetry/match/match.prefab", {
        tp = 0,
        name = name
    }, true)
end -- newGame_OnClick

local function OnReceiveMsgs(msgs)
    if string.sub(msgs, 1,6) == "return" then
        local f = load(msgs)
        if f then
            local msgt = f() -- {id, name, url, description}
            if msgt.url and this.servers[msgt.url] == nil then
                this.servers[msgt.url] = msgt
                this.newServerCome = true
            end
        end
    end
end

local port = 8800

--server
function index.StartUdpReceiveLoop()
    xutil.coroutine_call(function()
        local udp = socket.udp()
        this.receiveUdp = udp
        local host = '0.0.0.0' -- '10.23.24.239'
        udp:settimeout(0.1)
        udp:setsockname(host, tostring(port))
        assert(udp:setoption('broadcast', true)) -- setsockopt will failed if before setpeername
        print('waiting client connect', host, port)
        while not this.needQuitUdp do
            local msgs,receip,receport = udp:receivefrom()
            if (msgs and receip and receport) then
                print(receip,receport, msgs)
                OnReceiveMsgs(msgs)
            else
                --print('waiting client connect ...')
            end
            yield_return(UnityEngine.WaitForSeconds(3))
        end
        udp:close()
    end)
end

-- client
function index.StartUdpBroadcastLoop()
    xutil.coroutine_call(function()
        local udp = socket.udp()
        local host = '10.23.25.255'
        assert(udp:settimeout(0.1))
        assert(udp:setpeername(host, port))
        assert(udp:setoption('broadcast', true)) -- setsockopt will failed if before setpeername
        print('waiting client connect', host, port)
        local myip = assert(udp:getsockname())
        local msgt = {
            url = string.format("%s", myip),
            name = "--name--", 
            description = "",
            --action = function()return {3.1415926, 369852}  end
        }
        local msgs = 'return' .. util.dump(msgt, false)
        while not this.needQuitUdp do
            local udpsend, err = udp:send(msgs)
            yield_return(UnityEngine.WaitForSeconds(15))
        end
        print("broadcast udp shutdown")
        udp:close()
    end)
end

function this.OnDestroy()
    this.needQuitUdp = true
    if this.sendUdp then this.sendUdp:close() end
    if this.receiveUdp then this.receiveUdp:close() end
end

return index
