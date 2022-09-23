
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/15 18:28:48
--- Description:
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "utility.util"
local xutil = require "utility.xlua.util"
local socket = require("utility.socket.socket")

local this = AppGlobal.Client
if this then return this end

local yield_return = xutil.async_to_sync(function (to_yield, callback)
	mono:YieldAndCallback(to_yield, callback)
end)

-- client

local useUdp = true

local print = function ( ... )
    _G.print("<color=green>client</color>", ...)
     _G.print("client", debug.traceback())
end

local this = {
	connectStat = 0,
	lastActive = 0,
	State = {
		offline = 0,
		connecting = 1,
		connected = 2,
	},
	tcpClient = nil, -- tcpClient
	cinfo = nil, -- client info
	listeners = {}, -- regist callback
	clientId = -1,
	roomId = -1,
}
local client = this
AppGlobal.Client =  this

local State = this.State

---ConnectToServer
---@param ip string
---@param port number
---@param callback function
function client.ConnectToServer(ip, port, callback)
	if this.connectStat == State.connected then
		if type(callback) == "function" then callback(true) end
		assert("already connected a server")
		return
	end

	if this.tcpClient ~= nil and this.tcpClient:getstats() == 1 then
		--https://stackoverflow.com/questions/4160347/close-vs-shutdown-socket
		-- this.conn:shutdown()
		this.tcpClient:close()
		this.tcpClient = undef
		if type(callback) == "function" then callback(false) end
	end
	xutil.coroutine_call(function ()
		this.connectStat = State.connecting
		local retrycount = 0
		print("ConnectToServer", ip, port)
		while (this.connectStat == State.connecting and retrycount < 3) do
			print("try connect ...", retrycount)
			retrycount = 1 + retrycount
			local tcp = socket.tcp()
			tcp:settimeout(2)
			local   msg, err = tcp:connect(ip, port)
			print("conn", tcp, err, msg)
			if err == nil then
				this.tcpClient = tcp
				this.connectStat = State.connected
				print("<color=green>connected to server ok</color>.")
				-- else if err == "connection refused" then
				-- 	print(err)

				this.AddListener("connect", function(msgt)
					this.clientId = msgt.clientId
				end)

				this.AddListener("heartbeat", function(msgt)  end)

				if type(callback) == "function" then callback(true) end
			else
				print("connect err", err, msg)
			end
			yield_return(UnityEngine.WaitForSeconds(0.3))
		end -- while
		if this.tcpClient ~= nil then
			this.StartReceiveLoop()
			this.StartHeartbeatLoop()
		else
			print("can not connect the host", ip, port)
			if type(callback) == "function" then callback(false) end
			AppGlobal.SceneManager.pop()
		end
	end)
end

function client.StartReceiveLoop()
	print('StartReceiveLoop')
	xutil.coroutine_call(function ()
		while true do
			local canread, sendt, status = socket.select({this.tcpClient }, nil, 0.001)
			-- print("canread", #canread, #this.client)
			for _, c in ipairs(canread) do
				c:settimeout(0.1)
				local line, err = c:receive("*l")

				if not err then
					this.OnReceiveMsg(line)
				elseif(err == "closed")then
					this.connectStat = State.offline
					c:close()
					AppGlobal.SceneManager.push("poetry/index/index.prefab", nil, true)
				else
					c:send("___ERRORPC "..err..tostring(c)..  "\n")
				end
				::continue::
			end
			yield_return(UnityEngine.WaitForSeconds(0.3))
		end
	end)
end

function client.StartHeartbeatLoop()
	print('StartHeartbeatLoop')
	local deltas = 30
	xutil.coroutine_call(function ()
		while true do
			if os.time() - this.lastActive > deltas then
				this.SendMsgt({ type = "heartbeat", "do you fell my heart?", clientId = this.clientId })
			end
			yield_return(UnityEngine.WaitForSeconds(deltas))
		end
	end)
end

function client.OnReceiveMsg(msgs)
	print("OnReceiveMsg", msgs)
	this.lastActive = os.time()
	local f = load(msgs)
	if f then
		local msgt = f()
		local listener = this.listeners[msgt.type]
		if type(listener) ==  "function" then
			listener(msgt)
		elseif type(listener) == "table" then
			for _, v in ipairs(listener) do
				v(msgt)
			end
		else
			print("no listener for type: " .. msgt.type)
		end
	end
end

---AddListener
---@param msgtype string
---@param callback function
function client.AddListener(msgtype, callback)
	local listener = this.listeners[msgtype]
	if listener == nil then
		this.listeners[msgtype] = callback
	else
		if type(listener) == "function" then
			this.listeners[msgtype] = {listener, callback}
		elseif type(listener) == "table" then
			table.insert(listener, callback)
		end
	end
end

---AddListeners
---@param listeners table
function client.AddListeners(listeners)
	for k, v in pairs(listeners) do
		this.AddListener(k, v)
	end
end

---注销消息监听
---@param msgtype string
---@param _listener function
function client.RemoveListener(msgtype, _listener)
	local listener = this.listeners[msgtype]
	if  type(listener) == "function" then
		this.listeners[msgtype] = undef
	elseif  type(listener) == "table" then
		util.removeValue(listener, _listener)
	end
end

function client.RemoveListeners(listeners)
	for k, v in pairs(listeners) do
		this.RemoveListener(k, v)
	end
end

---ClientSend
---@param msgt table
function client.SendMsgt(msgt)
	if this.tcpClient ~= nil and type(msgt) == "table" then
		msgt = msgt or {}
		msgt.clientId = this.clientId
		msgt.roomId = msgt.roomId or this.roomId
		local msgs = util.dump(msgt,false)
		print("ClientSend", msgs)
		this.SendMsgs(msgs)
	else
		error("tcp client not ready")
	end
end

function client.SendMsgs(msgs)
	this.tcpClient:send("return ")
	this.tcpClient:send(msgs)
	this.tcpClient:send("\n")
end
----------------------client end---------------------

function client.closeConnect()
	if this.tcpClient ~= nil then
		this.tcpClient:close()
		print("shutdown client")
		this.tcpClient = nil
	end

	-- TODO: clean ...
end

function client.OnDestroy()
	this.closeConnect()
	AppGlobal.Client = undef
end

return client