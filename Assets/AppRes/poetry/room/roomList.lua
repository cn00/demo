
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/20 18:40:19
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.util"
local xutil = require "xlua.util"

-- roomList

local print = function ( ... )
    _G.print("roomList", ...)
    -- _G.print("roomList", debug.traceback())
end

local roomList = {
    needQuitUdp = false,
    receiveUdp = nil,
    sendUdp = nil,
    servers = {}, -- {{url, name}, ...}
    newServerCome = false,
}
local this = roomList



--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.roomTemplate_RectTransform = roomTemplate:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.scrollContent_RectTransform = scrollContent:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.SliderV_Slider = SliderV:GetComponent(typeof(CS.UnityEngine.UI.Slider))
    this.SliderVText_Text = SliderVText:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.tableview_TableView = tableview:GetComponent(typeof(CS.TableView.TableView))
    this.tableview_TableViewController = tableview:GetComponent(typeof(CS.TableView.TableViewController))
end
--AutoGenInit End

function roomList.Awake()
	this.AutoGenInit()
end

-- function roomList.OnEnable() end

function roomList.Start()
	--util.coroutine_call(this.coroutine_demo)
    
    roomTemplate:SetActive(false)

    this.needQuitUdp = false
    this.StartUdpReceiveLoop()
    this.StartUdpBroadcastLoop()
end


function roomList.Update()
    if this.newServerCome then
        print("newServerCome")
        for i, v in pairs(this.servers) do
            if v.obj == nil then
                print(v.name, v.url,v.description)
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
        local myip = assert(udp:getsockname())
        print('waiting client connect', host, port, myip)
        while not this.needQuitUdp do
            local msgs,receip,receport = udp:receivefrom()
            if (msgs and receip and receport) then
                print("receive", msgs)
                OnReceiveMsgs(string.sub(msgs,1, 6) == "return" and msgs or string.hexr(msgs))
            else
                --print('waiting client connect ...')
            end
            yield_return(UnityEngine.WaitForSeconds(0.3))
        end
        udp:close()
    end)
end

-- client
function index.StartUdpBroadcastLoop()
    xutil.coroutine_call(function()
        local udp = socket.udp()
        this.sendUdp = udp
        local host ='10.23.24.239', --[[android]] CS.System.Net.IPAddress.Broadcast:ToString(), --[[ok]]  '10.23.25.255' --[[ok]]
        print('broadcastIp', host, CS.System.Net.IPAddress.Broadcast:ToString())
        --CS.App.JavaUtil.Call("com.unity3d.player.UnityPlayerActivity", "RequireWifiLock")
        assert(udp:settimeout(0.1))
        assert(udp:setpeername(host, port))
        assert(udp:setoption('broadcast', true)) -- setsockopt will failed if before setpeername
        --CS.App.JavaUtil.Call("com.unity3d.player.UnityPlayerActivity", "ReleaseWifiLock")
        local myip = assert(udp:getsockname())
        local msgt = {
            url = string.format("%s", myip),
            name = "--name--",
            description = "--description--",
            --action = function()return {3.1415926, 369852}  end
        }
        local msgs = 'return' .. util.dump(msgt, false)
        while not this.needQuitUdp do
            local udpsend, err = udp:send(string.hex(msgs))
            print('broadcast', host, myip, port)
            yield_return(UnityEngine.WaitForSeconds(15))
        end
        print("broadcast udp shutdown")
        udp:close()
    end)
end

function this.OnDestroy()
    this.needQuitUdp = true
    if this.sendUdp then
        print("sendUdp:close")
        this.sendUdp:close()
    end
    if this.receiveUdp then
        print("receiveUdp:close")
        this.receiveUdp:close()
    end
end

return roomList
