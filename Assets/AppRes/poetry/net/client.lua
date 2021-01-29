
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
local util = require "lua.utility.util"
local xutil = require "xlua.util"
local socket = require("socket.socket")
local manager = G.AppGlobal.manager

local yield_return = xutil.async_to_sync(function (to_yield, callback)
	mono:YieldAndCallback(to_yield, callback)
end)

-- client

local useUdp = true

local print = function ( ... )
    _G.print("<color=green>client</color>", ...)
    -- _G.print("client", debug.traceback())
end

local client = {
	lastActive = 0,
	conn_stat = {
		offline = 0,
		connecting = 1,
		connected = 2,
	},
	conn = nil, -- tcpClient
	cinfo = nil, -- client info 
	callback = {}, -- regist callback
}
local this = client
local conn_stat = client.conn_stat

---ClientConnectToServer
---@param ip string
---@param port number
---@param callback function
function client.ClientConnectToServer(ip, port, callback)
	table.insert(this.callback, callback)
	if this.conn ~= nil and this.conn:getstats() == 1 then
		--https://stackoverflow.com/questions/4160347/close-vs-shutdown-socket
		-- this.conn:shutdown()
		this.conn:close()
		this.conn = undef
	end
	xutil.coroutine_call(function ()
		this.connect_stat = conn_stat.connecting
		local retrycount = 0
		print("ClientConnectToServer", ip, port)
		while (this.connect_stat == conn_stat.connecting and retrycount < 3) do
			print("try connect ...", retrycount)
			retrycount = 1 + retrycount
			local tcp = socket.tcp()
			tcp:settimeout(2)
			local   msg, err = tcp:connect(ip, port)
			print("conn", tcp, err, msg)
			if err == nil then
				this.conn = tcp
				this.connect_stat = conn_stat.connected
				print("<color=green>connected to server ok</color>.")
				-- else if err == "connection refused" then
				-- 	print(err)
			else
				print("connect err", err, msg)
			end
			yield_return(UnityEngine.WaitForSeconds(0.3))
		end -- while
		if this.conn ~= nil then
			this.ClientStartReceiveLoop()
			this.StartHeartbeatLoop()
		else
			print("can not connect the host", ip, port)
			
			manager.Scene.push("poetry/index/index.prefab", nil, true)
		end
	end)
end

function client.ClientStartReceiveLoop()
	print('ClientStartReceiveLoop')
	xutil.coroutine_call(function ()
		while true do
			local canread, sendt, status = socket.select({this.conn}, nil, 0.001)
			-- print("canread", #canread, #this.client)
			for _, c in ipairs(canread) do
				c:settimeout(0.1)
				local line, err = c:receive("*l")

				if not err then
					client.ClientOnReceiveMsg(line)
				elseif(err == "closed")then
					this.connect_stat = conn_stat.offline
					c:close()
					manager.Scene.push("poetry/index/index.prefab", nil, true)
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
	local deltas = 10
	xutil.coroutine_call(function ()
		while true do
			if os.time() - this.lastActive > deltas then
				this.SendMsgt({ type = "heartbeat", body = {"do you know my heart?", clientId = this.clientId} })
			end
			yield_return(UnityEngine.WaitForSeconds(deltas))
		end
	end)
end

function client.ClientOnReceiveMsg(msgs)
	print("ClientOnReceiveMsg", msgs)
	this.lastActive = os.time()
	local f = load(msgs)
	if f then
		local msgt = f()
		for _, callback in ipairs(this.callback) do 
			callback(msgt)
		end
	end
end

---ClientSend
---@param msgt table
function client.SendMsgt(msgt)
	if this.conn ~= nil and type(msgt) == "table" then
		local msgs = util.dump(msgt,false)
		print("ClientSend", msgs)
		this.SendMsgs(msgs)
	end
end

function client.SendMsgs(msgs)
	this.conn:send("return ")
	this.conn:send(msgs)
	this.conn:send("\n")
end
----------------------client end---------------------
local function broadcastTest()
	--ok
	local d_host ='10.23.25.255' -- '127.0.0.1'
	local d_port = 8800
	local udp = socket.udp()
	udp:settimeout(1)
	assert(udp:setpeername(d_host, d_port))
	assert(udp:setoption('broadcast', true)) -- setsockopt will failed if before setpeername
	assert(udp:setoption('dontroute', true))
	-- print(res, errmsg)
	local function rec_msg()
		local recudp = udp:receive()
		if (recudp) then
			print('recudp data:' .. recudp)
		else
			print('recudp data nil')
		end
	end
	while 1 do
		local udpsend, err = udp:send('hello i am lua client')
		if (udpsend) then
			print('udpsend ok', udpsend, err, d_host, d_port)
			rec_msg()
			break
		else
			print('udpsend err', err, d_host, d_port)
		end
	end
	udp:close()
end


return client
