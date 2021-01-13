
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/08 19:18:14
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"
local socket = require("socket.socket")
local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)
-- match

local conn_stat = {
    offline = 0,
    connecting = 1,
    connected = 2,
}

local print = function ( ... )
    _G.print("[match]", ...)
end

local match = {
    tp = 0, -- 0:主场， 1:客场, 2:观众
}
local this = match

function match.init(info)
	
end

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.playerA_LuaMonoBehaviour = playerA:GetComponent(typeof(CS.LuaMonoBehaviour))
    this.playerB_LuaMonoBehaviour = playerB:GetComponent(typeof(CS.LuaMonoBehaviour))
end
--AutoGenInit End

function match.Awake()
	this.AutoGenInit()
end

-- function match.OnEnable() end

function match.Start()
	--util.coroutine_call(this.coroutine_demo)
    match.ServerStart()
end

function match.sendMsgToPlayer(player, msg)
    player.receiveMsg(msg)
end

---------------------server-------------------{{{{

function match.ServerStart()
    local port= 9990
    local server, err = socket.bind("*", port)
    if err == nil then
        match.server = server
        match.ServerStartAcceptLoop()
        match.ServerStartReceiveLoop()
        print("StartServer ok, listen on:", port)

        -- local test
        match.ClientConnectToServer("10.23.24.239", port)
    else
        print("StartServer failed.", err)
    end
end

function match.ServerStartAcceptLoop()
    util.coroutine_call(function()
        print("waiting for client")
        while true do
            match.server:settimeout(0.01)
            local client, err = match.server:accept()
            if client and not err then
                match.clients = match.clients or {}
                match.clients[1+#match.clients] = client
                print("accept client", client, #match.clients)
            else
                --print("accetp err", err)
            end
            yield_return(UnityEngine.WaitForSeconds(0.3))
        end
    end)
end

function match.ServerStartReceiveLoop()
    util.coroutine_call(function()
        while true do
            local canread, sendt, status = socket.select(this.clients, nil, 0.001)
            -- print("canread", #canread, #this.client)
            for _, c in ipairs(canread) do
                c:settimeout(0.1)
                local line, err = c:receive("*l")
                print("receive", c, #line, line:gsub("[\0-\13]",""), err)

                if not err then
                    -- TODO: pass msg here
                    for i, v in ipairs(this.clients) do
                        if v ~= c then
                            v:send(line.."\n")
                        end
                    end
                elseif(err == "closed")then
                    this.connect_stat = conn_stat.offline
                    c:close()
                    -- this.ondisconnect( c )
                else
                    c:send("<color=red>___ERRORPC"..err.. "<color/>\r\n")
                end
                ::continue::
            end

            yield_return(UnityEngine.WaitForSeconds(0.3))
        end
    end)
end

---------------------server end-------------------}}}}



----------------------client---------------------

function match.ClientConnectToServer(ip, port)
    if this.conn ~= nil and this.conn:getstats() == 1 then
        --https://stackoverflow.com/questions/4160347/close-vs-shutdown-socket
        -- this.conn:shutdown()
        this.conn:close()
        this.conn = nil
    end
    util.coroutine_call(function ()
        this.connect_stat = conn_stat.connecting
        local retrycount = 0
        while (this.connect_stat == conn_stat.connecting and retrycount < 10) do
            print("try connect ...", retrycount)
            retrycount = 1 + retrycount
            local conn, err = socket.connect(ip, port)
            if err == nil and conn then
                this.conn = conn
                this.connect_stat = conn_stat.connected
                print("<color=green>connected to server ok</color>.")
                -- else if err == "connection refused" then
                -- 	print(err)
            else
                print("connect err", err)
            end
            yield_return(UnityEngine.WaitForSeconds(0.3))
        end -- while
        match.ClientStartReceiveLoop()
    end)
end

function match.ClientStartReceiveLoop()
    print('ClientStartReceiveLoop')
    while true
    do
        local canread, sendt, status = socket.select({this.conn}, nil, 0.001)
        -- print("canread", #canread, #this.client)
        for _, c in ipairs(canread) do
            c:settimeout(0.1)
             local line, err = c:receive("*l")
             print("<color=red>client receive</color>", #line, line:gsub("[\0-\13]",""), err)

            if not err then
                -- TODO: pass msg here
            elseif(err == "closed")then
                this.connect_stat = conn_stat.offline
                c:close()
                -- this.ondisconnect( c )
            else
                c:send("<color=red>___ERRORPC"..err.. "<color/>\r\n")
            end
            ::continue::
        end

        yield_return(UnityEngine.WaitForSeconds(0.3))
    end
end

----------------------client end---------------------

function match.OnMouseDown()
    print("OnMouseDown", mouseDeltaWorld)
end

function match.OnDestroy()
    for i, v in ipairs(this.clients) do
        v:close()
    end
    if match.server ~= nil then
        match.server:close()
        print("shutdown server")
    end
end

return match
