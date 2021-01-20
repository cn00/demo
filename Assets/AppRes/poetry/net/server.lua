
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/13 13:01:28
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.util"
local xutil = require "xlua.util"
local socket = require("socket.socket")

local yield_return = xutil.async_to_sync(function (to_yield, callback)
	mono:YieldAndCallback(to_yield, callback)
end)

-- server

local print = function ( ... )
    _G.print("[server]", ...)
end
local server = {
	ServerPort = 9990,
	tcpServer = nil, -- tcpServer
	
	tcpClients = {}, -- tcpClients -- for socket.select
	clientsInfo = {},    -- {[clientId] = {status, userId, tcp}, ...}

	-- room
	rooms = {}, 	-- {[roomId]={clientId, ...},...}
	cardIds = {},	-- {[roomId]={cardId, ...},...}
	clientIds = {}, -- {[roomId]={clientId, ...},...}
}
local this = server

local conn_stat = {
	offline = 0,
	connecting = 1,
	connected = 2,
}

local newRoomId = util.newIdx(1)
local newClientId = util.newIdx(1)

---ServerStart 
function server.ServerStart()
	local port= server.ServerPort
	print("<color=red>ServerStart", port)
	local tcp, err = socket.bind("*", port)
	if err == nil then
		this.tcpServer = tcp
		this.ServerStartAcceptLoop()
		this.ServerStartReceiveLoop()
		print("StartServer ok, listen on:", port)
	else
		print("StartServer failed.", err)
	end
end

local function selectClient(filter)
	filter = filter or function() return true  end
	local cgroup = {}
	for k, v in pairs(server.clientsInfo) do
		if filter(v) then cgroup[1+#cgroup] = v.tcp end
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
				this.tcpClients[1+#this.tcpClients] = client
				print("accept client", client, #this.tcpClients)
				
				this.clientsInfo = this.clientsInfo or {}
				local clientId = newClientId()
				local cinfo =  
				{
					status = 0,
					clientId = clientId,
				}
				server.clientsInfo[clientId] = cinfo
				this.SendMsgtToClient({ type = "connect", body = cinfo }, client)
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
				elseif(err == "closed")then
					c:close()
					-- this.ondisconnect( c )
					this.connect_stat = conn_stat.offline
				else
					c:send("server receive __ERROR__".. err .. tostring(c))
				end
				::continue::
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
	print("ServerOnReceiveMsgs", tcp, msgs)
	local f = load(msgs)
	if f then
		local msgt = f()
		local type = msgt.type
		local body = msgt.body
		body.tcp = tcp
		assert(this.OnClientMsgType[type] and this.OnClientMsgType[type](msgt))
	else
		print("unrecognised msg", server.clientsInfo[tcp].id)
	end
end

---OnLogin
---@param msgt table body = {userId, userName, clientId}
local function OnLogin(msgt)
	local body = msgt.body
	local playerId = body.playerId
	local passwd = body.passwd
	local success = true
	local loginMsg = "welcome"
	local ret = { 
		type = "login", 
		body = {
			success = success, 
			playerId = playerId, 
			loginMsg = loginMsg
		} 
	}
	this.SendMsgtToClient(ret, body.tcp)
	return true
end

--- 房主选择诗集范围，用 id 列表创建房间
---@param msgt table body = {userId, cardsInfo = {}}
local function OnCreateRoom(msgt)
	local body = msgt.body
	local roomId = newRoomId()
	local room = {
		roomId = roomId,
		masterId = body.clientId,
		note = body.note,
	}
	this.rooms[roomId]   = room
	this.cardIds[roomId] = body.cardIds
	this.clientIds[roomId] = {body.clientId}
	
	local rt = {
		type = "createRoom",
		body = {
			success = true
		}
	}
	this.SendMsgtToClient(rt, body.tcp)
	return true
end

---OnRoomList
---@param msgt table body = {}
local function OnRoomList(msgt)
	local rt = {
		type = "roomList",
		body = this.rooms, -- TODO: only send simple room info
	}
	this.SendMsgtToClient(rt, msgt.body.tcp)
	return true
end

---原有成员只发送 clientId， 新成员发送 this.clientIds[body.roomId]
---@param msgt table body = 
local function OnJoinRoom(msgt)
	local body= msgt.body
	local roomId = body.roomId
	local room = this.rooms[body.roomId]
	
	local rt = {
		type = "joinRoom",
		body = { roomId = body.roomId, clientId = body.clientId },
	}
	
	-- 原有成员
	for _, clientId in pairs(this.clientIds[roomId]) do
		this.SendMsgtToClient(rt, this.clientsInfo[clientId].tcp)
	end
	
	-- 新成员
	table.insert(this.clientIds[roomId], body.clientId)
	rt.body.clientIds = this.clientIds[roomId]
	rt.body.cardIds = this.cardIds[roomId]
	this.SendMsgtToClient(rt, msgt.body.tcp)
	return true
end

---OnLeaveRoom
---@param msgt table body = {clientId, roomId}
local function OnLeaveRoom(msgt)
	local body= msgt.body
	local room = this.rooms[body.roomId]
	local rt = {
		type = "leaveRoom",
		body = { roomId = body.roomId, clientId = body.clientId },
	}
	for _, clientId in pairs(this.clientIds[body.roomId]) do
		this.SendMsgtToClient(rt, this.clientsInfo[clientId].tcp)
	end
	util.removeValue(this.clientIds[body.roomId], body.clientId)
	return true
end

---OnPrepared
---@param msgt table body = {clientId, roomId, state} -- state = 0:free 1:prepared 2: ...
local function OnClientStateChanged(msgt)
	local body = msgt.body
	local client = this.clientsInfo[body.clientId]
	client.state = body.state
	local room = this.rooms[body.roomId]
	for _, clientId in ipairs(this.clientIds[body.roomId]) do
		this.SendMsgtToClient(msgt, this.clientsInfo[clientId].tcp)
	end
end

--- 房主发起开始游戏
---@param msgt table
local function OnStartMatch(msgt)
	local body = msgt.body
	local client = this.clientsInfo[body.clientId]
	client.state = body.state
	local room = this.rooms[body.roomId]
	for _, clientId in ipairs(this.clientIds[body.roomId]) do
		this.SendMsgtToClient(msgt, this.clientsInfo[clientId].tcp)
	end
	local cardIds = {}
	return true
end


---OnUserAction
---@param msgt table body = {}
local function OnUserAction(msgt)
	local body= msgt.body
	local room = this.rooms[body.roomId]
	local currentIdx = 0 -- TODO:currentIdx
	local ret = {
		type = "nextRound",
		body = {
			currentIdx = currentIdx,
		}
	}
	for _, clientId in ipairs(this.clientIds[body.roomId]) do
		this.SendMsgtToClient(ret, this.clientsInfo[clientId].tcp)
	end
	return true
end

local function OnHeartbeat(msgt)
	this.clientsInfo[msgt.body.clientId].lastActive = os.clock()
	this.SendMsgtToClient({ type = "heartbeat", body = {"my heart will go on."} }, this.clientsInfo[msgt.body.clientId].tcp)
end

server.OnClientMsgType = {
	["login"] 		= OnLogin, --> login
	["createRoom"] 	= OnCreateRoom, --> createRoom
	["roomList"]	= OnRoomList,
	["joinRoom"] 	= OnJoinRoom,
	["leaveRoom"] 	= OnLeaveRoom,
	["cStateChange"] = OnClientStateChanged,
	["startMatch"]	= OnStartMatch,
	["userAction"]	= OnUserAction,
	["heartbeat"] 	= OnHeartbeat,
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


function server.OnDestroy()
	if this.tcpClients and #this.tcpClients > 0 then
		for i, v in ipairs(this.tcpClients) do
			v:close()
		end
	end
	if server.tcpServer ~= nil then
		server.tcpServer:close()
		print("shutdown server")
	end
end


local function udpServerReceiveBroadcastTest() -- ok
	local socket = require("socket.core")
	local udp = socket.udp()
	local host = '0.0.0.0' -- '10.23.24.239'
	local port = '8800'
	udp:settimeout(10)
	udp:setsockname(host, port)
	assert(udp:setoption('broadcast', true)) -- setsockopt will failed if before setpeername
	--assert(udp:setoption('dontroute', true))
	print('waiting client connect', host, port)
	while 1 do
		local revbuff,receip,receport = udp:receivefrom()
		if (revbuff and receip and receport) then
			print('revbuff:['..revbuff..'],receip:'..receip..',receport:'..receport)
			local sendcli = udp:sendto('hello i am lua server',receip,receport)
			if(sendcli) then
				print('sendcli ok')
			else
				print('sendcli error')
			end
		else
			print('waiting client connect ...')
		end
	end
	udp:close()
end
return server
