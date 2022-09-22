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
local util = require "utility.util"
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
local _print = _G.print
local print = function(...)
    _print("<color=yellow>server</color>", os.date(), ...)
end
local this = {
    AI_delay = 1, -- ai 等待时间
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
    print("db.path:", config.dbCachePath)
    local db = sqlite.open(config.dbCachePath)
    -- print("getDbData", sql)
    local ret = {}
    for row in db:nrows(sql) do
        table.insert(ret, row)
    end
    -- print("getDbData", #ret)
    return ret
end

local function getCardsInfo(idlist)
    local sql = string.format([[select id, content from poetry where id in (%s)]], table.concat(idlist, ","))
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

local function getRandomCard(n)
    local count = n
    --local filter = "WHERE tags like '%思琪限定%'"
    local filter = "WHERE true"
    local sql = string.format([[
			SELECT a.id, a.content, a.author
			FROM `poetryAuthor` a
			JOIN (
				SELECT ROUND(substr(random(), 2, 4) / 10000.0 * (( SELECT MAX(id) FROM `poetry` %s)
				- ( SELECT MIN(id) FROM `poetry` %s) - %d) + (SELECT MIN(id) FROM `poetry` %s)) AS id
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
            level = 0 ,-- math.random(0, 9),
            --masterId = body.clientId, -- poetry.author.id
            --note = body.note,			-- poetry.author.desc
            isNpc = true,
            userAnswers = {}, -- {[idx] = {clientId, idx}, ...}
            description="npc",
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
    local retry = 0
    xutil.coroutine_call(function()
        while retry < 4 and this.tcpServer == nil do
            retry = retry + 1
            local ip = this.ServerIP
            local port = this.ServerPort
            print("<color=red>ServerStart_1", ip, port)
            local tcp = assert(socket.bind(ip, port))
            print("<color=red>ServerStart_2", tcp)
            if tcp then
                this.tcpServer = tcp

                createNpc(5)

                this.ServerStartAcceptLoop()
                this.ServerStartReceiveLoop()
                print("StartServer ok, listen on *:", port)

                this.StartUdpBroadcastLoop()
            else
                this.ServerPort = this.ServerPort + 1
                print(err, "try next port", this.ServerPort)
            end
        end

    end)
end

---广播服务器地址
function server.StartUdpBroadcastLoop()
    xutil.coroutine_call(function()
        local udp = socket.udp()
        this.sendUdp = udp
        local host = CS.System.Net.IPAddress.Broadcast:ToString() -- [[desktop ok]]
        -- local host = '10.23.25.255' --[[desktop ok]]
        if UNITY_ANDROID and not UNITY_EDITOR then
            --host = "10.23.15.255" --[[android ok]]
            host = "10.23.255.255" --[[android ok]]
            --host = "255.255.255.255" --[[ desktop ok, android permission denied]]
        end
        local port = this.broadcastPort
        print('broadcastIp', host, port)
        CS.App.JavaUtil.CallStaticVoid("com.unity3d.player.UnityPlayerActivity", "RequireWifiLock")
            assert(udp:settimeout(0.1))
            assert(udp:setpeername(host, port))
            assert(udp:setoption('broadcast', true)) -- setsockopt will failed if before setpeername
        CS.App.JavaUtil.CallStaticVoid("com.unity3d.player.UnityPlayerActivity", "ReleaseWifiLock")
        local myip = assert(udp:getsockname())
        local hostname = System.Net.Dns.GetHostName()
        local count = 0
        while not this.needQuitUdp do
            local msgt = {
                ip = string.format("%s", myip),
                port = this.ServerPort,
                name = hostname,
                description = "wellcome",
            }
            local msgs = 'return' .. util.dump(msgt, false)
            CS.App.JavaUtil.CallStaticVoid("com.unity3d.player.UnityPlayerActivity", "RequireWifiLock")
            local udpsend, err = udp:send(string.hex(msgs))
            CS.App.JavaUtil.CallStaticVoid("com.unity3d.player.UnityPlayerActivity", "ReleaseWifiLock")
            count = count + 1
            if count%10 == 1 then print('broadcast', count, host, myip, port)end
            yield_return(UnityEngine.WaitForSeconds(count%10))
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

                this.clientsInfo = this.clientsInfo or {}
                local clientId = newClientId()
                print("accept client", clientId, #this.tcpClients)
                print("getpeername", client:getpeername())
                print("getsockname", client:getsockname())
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
                    error("server receive __ERROR__ " .. err .. c:getpeername())
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
        if(this.OnClientMsgType[type]) then
            this.OnClientMsgType[type](msgt)
        else
            print("no listener for type: " .. type)
        end
    else
        print("unrecognised msg", tcp:getpeername())
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
    local level = msgt.level or 0
    local roomId = newRoomId()
    local room = {
        roomId = roomId,
        masterId = body.clientId,
        note = body.note,
        masterName = "client:" .. msgt.clientId,
    }
    local cardsInfo = getCardsInfo(body.cardIds)
    table.insert(this.levels[level], 1, roomId)
    this.roomsInfo[roomId] = room
    this.cardsInfo[roomId] = cardsInfo
    this.clientIds[roomId] = { }
    local availableIdxs = {}
    for i, v in ipairs(cardsInfo) do
        v.idx = i
        table.insert(availableIdxs, i)
    end
    this.availableIdxs[roomId] = availableIdxs

    local rt = {
        type = "createRoom",
        success = true,
        roomId = roomId,
        masterId = body.clientId,
        level = body.level or 0,
        description="nnpc",
    }
    for i, tcp in ipairs(this.tcpClients) do
        this.SendMsgtToClient(rt, tcp)
    end
    return true
end

---OnRoomList
---@param msgt table {}
local function OnRoomList(msgt)
    local level = msgt.level or 0
    local perpage = 100
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
    local roomId = body.roomId
    local room = this.roomsInfo[roomId]
    if room == nil then return end --
    local rt = {
        type = "leaveRoom",
        roomId = body.roomId,
        clientId = body.clientId,
    }
    this.SendMsgtToRoom(rt, body.roomId)

    util.removeValue(this.clientIds[body.roomId], body.clientId)
    if #this.clientIds[roomId] < 1 then
        print("delete roomId", roomId)
        this.clientIds[roomId] = undef
        this.cardsInfo[roomId] = undef
        this.roomsInfo[roomId] = undef
        util.removeValue(this.levels[room.level or 0], roomId)
    end
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

local function nextRound(roomId)
    local room = this.roomsInfo[roomId]
    local availableIdxs = this.availableIdxs[roomId]
    if #availableIdxs < 1 then
        local ret = { type = "gameOver", }
        this.SendMsgtToRoom(ret, roomId)
        return
    end

    local currentIdx = availableIdxs[math.random(1, #availableIdxs)] -- first round
    room.currentIdx = currentIdx
    util.removeValue(availableIdxs, currentIdx)
    local ret = {
        type = "nextRound",
        currentIdx = currentIdx,
    }
    this.SendMsgtToRoom(ret, roomId)

    -- NPC
    room.userAnswers = room.userAnswers or {}
    local userAnswers = room.userAnswers[currentIdx] or {}
    if room.isNpc and #userAnswers == 0 then
        xutil.coroutine_call(function()
            yield_return(UnityEngine.WaitForSeconds(this.AI_delay))
            local userAnswers = room.userAnswers[currentIdx] or {}
            if currentIdx == room.currentIdx and #userAnswers == 0 then -- player answered within 5 seconds?
                --yield_return(UnityEngine.WaitForSeconds(0.01*math.random(200, 500)))
                print("npc answer", currentIdx)
                this.SendMsgtToRoom({
                    type = "answer",
                    roomId = roomId,
                    currentIdx = currentIdx,
                    userAnswer = currentIdx,
                }, roomId)

                this.endRound(roomId)
            end
        end)
    --else
    --    nextRound(roomId)
    end
end

--- 房主发起开始游戏, 默记阶段
---@param msgt table
local function OnStartMatch(msgt)
    local body = msgt or {}
    local roomId = body.roomId
    local memoryTime = msgt.memoryTime or 5
    --TODO: 验证房主
    --local client = this.clientsInfo[body.clientId]
    --client.state = body.state
    for _, clientId in ipairs(this.clientIds[roomId]) do
        this.SendMsgtToClient(msgt, this.clientsInfo[clientId].tcp)
    end
    local cardIds = {}
    xutil.coroutine_call(function()
        yield_return(UnityEngine.WaitForSeconds(memoryTime)) -- TODO: 默记时间⌚️
        nextRound(roomId)
    end)
    return true
end

local function OnHeartbeat(msgt)
    if msgt.clientId > 0 and this.clientsInfo[msgt.clientId] then
        this.clientsInfo[msgt.clientId].lastActive = os.clock()
        this.SendMsgtToClient({ type = "heartbeat", "my heart will go on.", serverTime = os.time() }, this.clientsInfo[msgt.clientId].tcp)
    end
    return true
end
local function OnChat(msgt)
    local roomId = msgt.roomId
    print("OnChat", roomId, this.clientIds[roomId])
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
    local room = this.roomsInfo[roomId]
    msgt.currentIdx = room.currentIdx

    room.userAnswers = room.userAnswers or {}
    local userAnswers = room.userAnswers[room.currentIdx] or {}
    table.insert(userAnswers, msgt.userAnswer)
    room.userAnswers[room.currentIdx] = userAnswers

    this.SendMsgtToRoom(msgt, roomId)

    this.endRound(roomId)
    return true
end

function server.endRound(roomId)
    xutil.coroutine_call(function()
        yield_return(UnityEngine.WaitForSeconds(4)) -- show answer time

        this.SendMsgtToRoom({
            type = "endRound",
            roomId = roomId
        }, roomId)

        local availableIdxs = this.availableIdxs[roomId]
        if #availableIdxs > 0 then
            nextRound(roomId)
        else
            local ret = {
                type = "gameOver",
            }
            this.SendMsgtToRoom(ret, roomId)
            util.removeValue(this.levels[0], roomId) --TODO: fixed level
            this.clientIds[roomId] = undef
            this.cardsInfo[roomId] = undef
            this.roomsInfo[roomId] = undef
        end    end)

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
    ["answer"]    = OnAnswer,
    --["endRound"]    = OnEndRound,
    ["heartbeat"] = OnHeartbeat,
    ["chat"] = OnChat,
    ["bye"] = OnBye,
}

---ServerSendMsgTo
---@param msgt table
---@param client tcp
function server.SendMsgtToClient(msgt, client)
    --print("SendMsgtToClient", msgt.clientId, client)
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
    --print("SendMsgtToRoom", msgt.type, roomId)
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