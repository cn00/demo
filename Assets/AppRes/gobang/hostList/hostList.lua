
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/20 18:40:19
--- Description:
--[[

]]

local G = _G
local CS = CS
local System = CS.System
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.util"
local xutil = require "xlua.util"
local socket = require("socket.socket")

local yield_return = xutil.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

-- roomList

local print = function ( ... )
    _G.print("hostList", ...)
    -- _G.print("roomList", debug.traceback())
end

local this = {
    needQuitUdp = false,
    receiveUdp = nil,
    broadcastPort = 8800,
    servers = {}, -- {{url, name}, ...}
    newServerCome = false,
}
local hostList = this

--AutoGenInit Begin
--[[
请勿手动编辑此函数
手動でこの関数を編集しないでください。
DO NOT EDIT THIS FUNCTION MANUALLY.
لا يدويا تحرير هذه الوظيفة
]]
function this.AutoGenInit()
    this.BackBtn_Button = BackBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.BackBtn_Button.onClick:AddListener(this.BackBtn_OnClick)
    this.itemTemplate_RectTransform = itemTemplate:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.scrollContent_RectTransform = scrollContent:GetComponent(typeof(CS.UnityEngine.RectTransform))
end
--AutoGenInit End

function this.BackBtn_OnClick()
    print('BackBtn_OnClick')
    AppGlobal.SceneManager.pop()
end -- BackBtn_OnClick

function hostList.Awake()
	this.AutoGenInit()
end

-- function roomList.OnEnable() end

function hostList.Start()
	--util.coroutine_call(this.coroutine_demo)

    itemTemplate:SetActive(false)

    this.needQuitUdp = false
    this.StartUdpReceiveLoop()
end


function hostList.Update()
    if this.newServerCome then
        print("newServerCome")
        for i, v in pairs(this.servers) do
            if v.obj == nil then -- 未创建
                v.onClickRoomCallback = function(ip, port)
                    print("connect", ip, port)
                    AppGlobal.SceneManager.push("poetry/roomList/roomList.prefab", {
                        --parent = nil,
                        serverIp = ip,
                        serverPort = port,
                        autoMatch = true,
                        matchType = 1, -- 0:主场， 1:客场, 2:观众
                    }, true)
                end
                local obj = GameObject.Instantiate(itemTemplate, scrollContent.transform)
                obj:SetActive(true)
                local com = obj:GetComponent(typeof(CS.LuaBehaviour)).Lua
                com.init(v)
                v.obj = obj
            end
        end
        this.newServerCome = false
    end
end

local function OnReceiveMsgs(msgs)
    print("OnReceiveMsgs", msgs)
    if string.sub(msgs, 1,6) == "return" then
        local f = load(msgs)
        if f then
            local msgt = f() -- {id, name, ip, port, description}
            if msgt.ip and this.servers[msgt.ip] == nil then
                this.servers[msgt.ip] = msgt
                this.newServerCome = true
            end
        end
    end
end

---接收 udp 广播
function hostList.StartUdpReceiveLoop()
    xutil.coroutine_call(function()
        local port = this.broadcastPort
        local udp = socket.udp()
        this.receiveUdp = udp
        local host = '0.0.0.0' -- '10.23.24.239'
        udp:settimeout(0.1)
        udp:setsockname(host, tostring(port))
        assert(udp:setoption('broadcast', true)) -- setsockopt will failed if before setpeername
        local myip = assert(udp:getsockname())
        print('waiting client connect', host, port, myip)
        while not this.needQuitUdp do
            local msghex,receip,receport = udp:receivefrom()
            if (msghex and receip and receport) then
                local msgs = string.hexr(msghex)
                OnReceiveMsgs(msgs)
            else
                --print('waiting client connect ...')
            end
            yield_return(UnityEngine.WaitForSeconds(0.3))
        end
        udp:close()
    end)
end

local function sendMsgt(msgt)
    local msgs = 'return' .. util.dump(msgt, false)
    local udpsend, err = udp:send(string.hex(msgs))
    print('broadcast', udpsend, err)
end

function this.OnDestroy()
    this.needQuitUdp = true
    if this.receiveUdp then
        print("receiveUdp:close")
        this.receiveUdp:close()
    end
end

return hostList