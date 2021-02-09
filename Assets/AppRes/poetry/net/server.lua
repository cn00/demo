--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/13 13:01:28
--- Description: 
--[[
-[ ] android 广播
-[x] 生成机器人
]]

local G = _G
local CS = CS
local System = CS.System
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.util"
local xutil = require "xlua.util"
local socket = require("socket.socket")
local config = require("common.config.config")
local sqlite = require("lsqlite3")

local this = AppGlobal.Server
if this then return this end

local yield_return = xutil.async_to_sync(function(to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

-- server

local print = function(...)
    _G.print("<color=yellow>server</color>", ...)
end
local this = {
    socket= socket,
    ServerIP = "*",
    ServerPort = Port, -- from mono inject values
    tcpServer = nil, -- game Server
    sendUdp = nil, -- broadcast server
    broadcastPort = BroadcastPort, 

    tcpClients = {}, -- tcpClients -- for socket.select
    clientsInfo = {}, -- {[clientId] = {status, userId, tcp}, ...}

    -- room
    roomsInfo = {},     -- {[roomId]={clientId, ...},...}
    levels = {},        -- {[level]={roomId, ...}, ...}
    cardsInfo = {},     -- {[roomId]={cardId, ...},...}
    availableIdxs = {}, -- {[roomId]={idx, ...},...}
    clientIds = {}, -- {[roomId]={clientId, ...},...}
}
local server = this
AppGlobal.Server = this

local conn_stat = {
    offline = 0,
    connecting = 1,
    connected = 2,
}

local newRoomId = util.newIdx(1)
local newClientId = util.newIdx(1)

local function getDbData(sql)
    local db = sqlite.open(config.dbCachePath)
    local ret = {}
    for row in db:nrows(sql) do
        table.insert(ret, row)
    end
    return ret
end

local function getRandomCard(n)
    local count = n
    local filter = "WHERE tags like '%唐诗%'"
    local sql = string.format([[
			SELECT a.id, a.content, a.author
			FROM `poetryAuthor` a
			JOIN (
				SELECT ROUND(substr(random(), 2, 4) / 10000.0 * (( SELECT MAX(id) FROM `poetry` %s) - ( SELECT MIN(id) FROM `poetry` %s) - %d) + (SELECT MIN(id) FROM `poetry` %s)) AS id
			) AS b
			%s and a.id >= b.id
			LIMIT %d;]], filter, filter, count, filter, filter, count):gsub("\n\t\t", " ")
    --print(sql:gsub("[\n\t ]+", " "))
    local rit = getDbData(sql)
    for i, row in ipairs(rit) do
        row.content = string.gsub(row.content
        ,"(。)%s*", "%1|")
                            :gsub("(？)%s*", "%1|")
                            :gsub("(！)%s*", "%1|")
                            :split("|")
        if this.hardModel then
            -- 困难模式
            row.qi = math.random(1, #row.content)
            local ai = row.qi == #row.content and row.qi - 1 or row.qi + 1
            if row.qi > 1 and row.qi < #row.content and math.random(0, 100) > 50 then
                ai = row.qi - 1
            end -- 前一句
            row.ai = ai
        else
            row.qi = math.random(1, #row.content - 1)
            row.ai = row.qi + 1
        end
        row.content = undef
    end
    return rit
end

---createNpc roomId [1, 9]
local function createNpc(num)
    local t0 = os.clock()
    
    --print("rit", util.dump(rit))
    for i = 1, num do
        local roomId = newRoomId()
        local room = {
            roomId = roomId,
            level = math.random(0, 9),
            --masterId = body.clientId, -- poetry.author.id
            --note = body.note,			-- poetry.author.desc
            isNpc = true,
        }
        this.roomsInfo[roomId] = room

        this.clientIds[roomId] = {}

        local roomgroup = this.levels[room.level] or {}
        table.insert(roomgroup, roomId)
        this.levels[room.level] = roomgroup

        local cardsInfo = getRandomCard(40)
        this.cardsInfo[roomId] = cardsInfo
        room.masterName = cardsInfo[1].author
        local availableIdxs = {}
        for i, v in ipairs(cardsInfo) do
            v.idx = i
            table.insert(availableIdxs, i)
        end
        this.availableIdxs[roomId] = availableIdxs
    end
    print("createNpc 100 use", t0, os.clock() - t0)
end

---ServerStart 
function server.Start()
    local ip = this.ServerIP
    local port = this.ServerPort
    print("<color=red>ServerStart", port)
    local tcp, err = assert(socket.bind(ip, port))
    if tcp then
        this.tcpServer = tcp

        createNpc(100)

        this.StartUdpBroadcastLoop()
        this.ServerStartAcceptLoop()
        this.ServerStartReceiveLoop()
        print("StartServer ok, listen on *:", port)
    else
        assert(false, err, port)
    end
end

---广播服务器地址
function server.StartUdpBroadcastLoop()
    xutil.coroutine_call(function()
        local udp = socket.udp()
        this.sendUdp = udp
        local host = '10.23.25.255' --[[ok]] -- '10.23.24.239' --[[android]]-- CS.System.Net.IPAddress.Broadcast:ToString() -- [[desktop ok]] -- 
        local port = this.broadcastPort
        print('broadcastIp', host, host)
        --CS.App.JavaUtil.Call("com.unity3d.player.UnityPlayerActivity", "RequireWifiLock")
        assert(udp:settimeout(0.1))
        assert(udp:setpeername(host, port))
        assert(udp:setoption('broadcast', true)) -- setsockopt will failed if before setpeername
        --CS.App.JavaUtil.Call("com.unity3d.player.UnityPlayerActivity", "ReleaseWifiLock")
        local myip = assert(udp:getsockname())
        local hostname = System.Net.Dns.GetHostName()
        local msgt = {
            ip = string.format("%s", myip),
            port = this.ServerPort,
            name = hostname,
            description = "wellcome",
            --action = function()return {3.1415926, 369852}  end
        }
        local msgs = 'return' .. util.dump(msgt, false)
        local count = 0
        while not this.needQuitUdp do
            local udpsend, err = udp:send(string.hex(msgs))
            print('broadcast', count, host, myip, port)
            count = count + 1
            yield_return(UnityEngine.WaitForSeconds(count%20))
        end
        print("broadcast udp shutdown")
        udp:close()
    end)
end

local function selectClient(filter)
    filter = filter or function()
        return true
    end
    local cgroup = {}
    for k, v in pairs(server.clientsInfo) do
        if filter(v) then
            cgroup[1 + #cgroup] = v.tcp
        end
    end
    return cgroup
end

function server.ServerStartAcceptLoop()
    xutil.coroutine_call(function()
        print("waiting for client")
        local errmsg = nil
        while true do
            server.tcpServer:settimeout(0.01)
            local client, err = server.tcpServer:accept()
            if client and not err then
                this.tcpClients = this.tcpClients or {}
                this.tcpClients[1 + #this.tcpClients] = client
                print("accept client", client, #this.tcpClients)

                this.clientsInfo = this.clientsInfo or {}
                local clientId = newClientId()
                local cinfo = {
                    type = "connect",
                    status = 0,
                    clientId = clientId,
                }
                server.clientsInfo[clientId] = cinfo
                this.SendMsgtToClient(cinfo, client)
                cinfo.tcp = client
            else
                if errmsg ~= err then
                    errmsg = err
                    print("accetp err", err)
                end
            end
            yield_return(UnityEngine.WaitForSeconds(0.3))
            --coroutine.yield()
        end
    end)
end

function server.ServerStartReceiveLoop()
    xutil.coroutine_call(function()
        print("serving for clients ...")
        while true do
            local canread, sendt, status = socket.select(this.tcpClients, nil, 0.001)
            -- print("canread", #canread, #this.client)
            for _, c in ipairs(canread) do
                c:settimeout(0.1)
                local line, err = c:receive("*l") -- default="*l", "*a"

                if not err then
                    server.ServerOnReceiveMsgs(line, c)
                elseif (err == "closed") then
                    c:close()
                    -- this.ondisconnect( c )
                    this.connect_stat = conn_stat.offline
                else
                    c:send("server receive __ERROR__" .. err .. tostring(c))
                end
                :: continue ::
            end

            yield_return(UnityEngine.WaitForSeconds(0.3))
        end
    end)
end

---ServerOnReceiveMsg
---@param tcp tcpclient
---@param msgs string
---@param tcp tcpClient
function server.ServerOnReceiveMsgs(msgs, tcp)
    print("OnReceive", msgs)
    local f = load(msgs)
    if f then
        local msgt = f()
        local type = msgt.type
        local body = msgt or {}
        body.tcp = tcp
        assert(this.OnClientMsgType[type] and this.OnClientMsgType[type](msgt))
    else
        print("unrecognised msg", server.clientsInfo[tcp].id)
    end
end

---OnLogin
---@param msgt table {userId, userName, clientId}
local function OnLogin(msgt)
    local body = msgt or {}
    local playerId = body.playerId
    local passwd = body.passwd
    local success = true
    local loginMsg = "welcome"
    local ret = {
        type = "login",
            success = success,
            playerId = playerId,
            loginMsg = loginMsg
    }
    this.SendMsgtToClient(ret, body.tcp)
    return true
end

--- 房主选择诗集范围，用 id 列表创建房间
---@param msgt table {userId, cardsInfo = {}}
local function OnCreateRoom(msgt)
    local body = msgt or {}
    local roomId = newRoomId()
    local room = {
        roomId = roomId,
        masterId = body.clientId,
        note = body.note,
    }
    this.roomsInfo[roomId] = room
    this.cardsInfo[roomId] = body.cardIds
    this.clientIds[roomId] = { body.clientId }

    local rt = {
        type = "createRoom",
            success = true,
            roomId = roomId,
            cardIds = body.cardIds,
            level = body.level or 0
    }
    this.SendMsgtToClient(rt, body.tcp)
    return true
end

---OnRoomList
---@param msgt table {}
local function OnRoomList(msgt)
    local level = msgt.level or 0
    local perpage = 10
    local page = math.min(#this.roomsInfo, msgt.page or 0)
    local rooms = {}
    for i = page * perpage, math.min((1 + page) * perpage, #this.levels[level]) do
        local roomid = this.levels[level][i]
        local room = this.roomsInfo[roomid]
        table.insert(rooms, room)
    end
    local rt = {
        type = "roomList",
        rooms = rooms, -- TODO: only send simple room info
    }
    this.SendMsgtToClient(rt, msgt.tcp)
    return true
end

---原有成员只发送 clientId， 新成员发送 this.clientIds[body.roomId]
---@param msgt table body = 
local function OnJoinRoom(msgt)
    local body = msgt or {}
    local roomId = body.roomId
    local room = this.roomsInfo[body.roomId]

    local rt = {
        type = "joinRoom",
        roomId = body.roomId, 
        clientId = body.clientId,
    }

    -- 原有成员
    for _, clientId in pairs(this.clientIds[roomId]) do
        this.SendMsgtToClient(rt, this.clientsInfo[clientId].tcp)
    end

    -- 新成员
    table.insert(this.clientIds[roomId], body.clientId)
    rt.clientIds = this.clientIds[roomId]
    rt.cardsInfo = this.cardsInfo[roomId]
    rt.roomInfo  = this.roomsInfo[roomId]
    this.SendMsgtToClient(rt, msgt.tcp)
    return true
end

---OnLeaveRoom
---@param msgt table {clientId, roomId}
local function OnLeaveRoom(msgt)
    local body = msgt or {}
    local room = this.roomsInfo[body.roomId]
    local rt = {
        type = "leaveRoom",
        roomId = body.roomId, 
        clientId = body.clientId,
    }
    for _, clientId in pairs(this.clientIds[body.roomId]) do
        this.SendMsgtToClient(rt, this.clientsInfo[clientId].tcp)
    end
    util.removeValue(this.clientIds[body.roomId], body.clientId)
    return true
end

---OnPrepared
---@param msgt table {clientId, roomId, state} -- state = 0:free 1:prepared 2: ...
local function OnClientStateChanged(msgt)
    local body = msgt or {}
    local client = this.clientsInfo[body.clientId]
    client.state = body.state
    local room = this.roomsInfo[body.roomId]
    for _, clientId in ipairs(this.clientIds[body.roomId]) do
        this.SendMsgtToClient(msgt, this.clientsInfo[clientId].tcp)
    end
    return true
end

--- 房主发起开始游戏, 默记阶段
---@param msgt table
local function OnStartMatch(msgt)
    local body = msgt or {}
    local roomId = body.roomId
    --TODO: 验证房主
    --local client = this.clientsInfo[body.clientId]
    --client.state = body.state
    local room = this.roomsInfo[body.roomId]
    for _, clientId in ipairs(this.clientIds[body.roomId]) do
        this.SendMsgtToClient(msgt, this.clientsInfo[clientId].tcp)
    end
    local cardIds = {}
    xutil.coroutine_call(function()
        yield_return(UnityEngine.WaitForSeconds(5)) -- TODO: 默记时间⌚️
        local availableIdxs = this.availableIdxs[roomId]
        local currentIdx = availableIdxs[math.random(1, #availableIdxs)] -- first round
        util.removeValue(availableIdxs, currentIdx)
        local ret = {
            type = "nextRound",
                currentIdx = currentIdx,
        }
        for _, clientId in ipairs(this.clientIds[body.roomId]) do
            this.SendMsgtToClient(ret, this.clientsInfo[clientId].tcp)
        end
    end)
    return true
end

---OnUserAction
---@param msgt table {}
local function OnUserAction(msgt)
    local body = msgt or {}
    local roomId = body.roomId
    local room = this.roomsInfo[body.roomId]
    local availableIdxs = this.availableIdxs[roomId]
    local currentIdx = availableIdxs[math.random(1, #availableIdxs)] -- currentIdx
    util.removeValue(availableIdxs, currentIdx)
    local ret = {
        type = "nextRound",
            currentIdx = currentIdx,
    }
    for _, clientId in ipairs(this.clientIds[body.roomId]) do
        this.SendMsgtToClient(ret, this.clientsInfo[clientId].tcp)
    end
    return true
end

local function OnHeartbeat(msgt)
    this.clientsInfo[msgt.clientId].lastActive = os.clock()
    this.SendMsgtToClient({ type = "heartbeat", "my heart will go on.", serverTime = os.time() }, this.clientsInfo[msgt.clientId].tcp)
    return true
end
local function OnChat(msgt)
    local roomId = msgt.roomId
    for _, clientId in ipairs(this.clientIds[roomId]) do
        this.SendMsgtToClient(msgt, this.clientsInfo[clientId].tcp)
    end
    return true
end

local function OnAutoMatch(msgt)
    local body = msgt or {}
    local clientId = body.clientId
    local level = body.level or 0
    local roomId = this.levels[level][math.random(1, #this.levels[level])]
    local rt = {
        type = "autoMatch",
        roomId = roomId,
    }
    this.SendMsgtToClient(rt, body.tcp)
    return true
end

local function OnAnswer(msgt)
    local body = msgt or {}
    local roomId = body.roomId
    this.SendMsgtToRoom(msgt, roomId)
    return true
end

local function OnEndRound(msgt)
    local body = msgt or {}
    local clientId = body.clientId
    local roomId = body.roomId
    local room = this.roomsInfo[roomId]

    local availableIdxs = this.availableIdxs[roomId]
    if #availableIdxs > 0 then
        local currentIdx = availableIdxs[math.random(1, #availableIdxs)] -- currentIdx
        util.removeValue(availableIdxs, currentIdx)
        local ret = {
            type = "nextRound",
                currentIdx = currentIdx,
        }
        this.SendMsgtToRoom(ret, roomId)
    else
        local ret = {
            type = "gameOver",
        }
        this.SendMsgtToRoom(ret, roomId)
    end
    return true
end

local function OnBye(msgt)

    return true
end

server.OnClientMsgType = {
    ["login"] = OnLogin, --> login
    ["createRoom"] = OnCreateRoom, --> createRoom
    ["roomList"] = OnRoomList,
    ["autoMatch"] = OnAutoMatch,
    ["joinRoom"] = OnJoinRoom,
    ["leaveRoom"] = OnLeaveRoom,
    ["cStateChange"] = OnClientStateChanged,
    ["startMatch"] = OnStartMatch,
    ["nextRound"] = OnUserAction,
    ["answer"]    = OnAnswer,
    ["endRound"]    = OnEndRound,
    ["heartbeat"] = OnHeartbeat,
    ["chat"] = OnChat,
    ["bye"] = OnBye,
}

---ServerSendMsgTo
---@param msgt table
---@param client tcp
function server.SendMsgtToClient(msgt, client)
    this.SendMsgsToClient(util.dump(msgt), client)
end

function server.SendMsgsToGroup(msgs, cgroup)
    cgroup = cgroup or server.tcpClients
    for i, v in ipairs(cgroup) do
        this.SendMsgsToClient(msgs, v)
    end
end

function server.SendMsgsToClient(msgs, tcp)
    tcp:send("return ")
    tcp:send(msgs)
    tcp:send("\n")
end

---------------------server end-------------------}}}}

---SendMsgtToRoom
---@param msgt table
---@param roomId number
function server.SendMsgtToRoom(msgt, roomId)
    print("SendMsgtToRoom", msgt.type, roomId, #this.clientIds[roomId])
    for _, clientId in ipairs(this.clientIds[roomId]) do
        this.SendMsgtToClient(msgt, this.clientsInfo[clientId].tcp)
    end
end

function server.OnDestroy()
    if this.tcpClients and #this.tcpClients > 0 then
        for i, v in ipairs(this.tcpClients) do
            v:close()
        end
    end
    if this.sendUdp then
        print("sendUdp:close")
        this.sendUdp:close()
    end
    if this.tcpServer ~= nil then
        this.tcpServer:close()
        print("shutdown server")
    end
    AppGlobal.Server = undef
end

return server
