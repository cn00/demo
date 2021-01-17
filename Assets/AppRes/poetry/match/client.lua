
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

local print = function ( ... )
    _G.print("[client]", ...)
    -- _G.print("[client]", debug.traceback())
end

local client = {
	conn_stat = {
		offline = 0,
		connecting = 1,
		connected = 2,
	},
	conn = nil, -- tcpClient
	cinfo = nil, -- client info 
}
local this = client
local conn_stat = client.conn_stat

---ClientConnectToServer
---@param ip string
---@param port number
---@param callback function
function client.ClientConnectToServer(ip, port, callback)
	client.callback = callback
	if this.conn ~= nil and this.conn:getstats() == 1 then
		--https://stackoverflow.com/questions/4160347/close-vs-shutdown-socket
		-- this.conn:shutdown()
		this.conn:close()
		this.conn = undef
	end
	xutil.coroutine_call(function ()
		this.connect_stat = conn_stat.connecting
		local retrycount = 0
		while (this.connect_stat == conn_stat.connecting and retrycount < 3) do
			print("try connect ...", retrycount)
			retrycount = 1 + retrycount
			local tcp = socket.tcp()
			tcp:settimeout(2)
			local conn, err = tcp:connect(ip, port)
			print("conn", tcp, conn, err)
			if err == nil and conn then
				this.conn = tcp
				this.connect_stat = conn_stat.connected
				print("<color=green>connected to server ok</color>.")
				-- else if err == "connection refused" then
				-- 	print(err)
			else
				print("connect err", err)
			end
			yield_return(UnityEngine.WaitForSeconds(0.3))
		end -- while
		if this.conn ~= nil then
			client.ClientStartReceiveLoop()
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
				--print("<color=red>client receive</color>", #line, line:gsub("[\0-\13]",""), err)

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

function client.ClientOnReceiveMsg(msgs)
	print("ClientOnReceiveMsg", msgs)
	local f = load(msgs)
	if f then
		local msgt = f()
		if client.callback then client.callback(msgt)end
	end
end

---ClientSend
---@param msg table
function client.ClientSend(msg)
	if this.conn ~= nil and type(msg) then
		local msgs = util.dump(msg,false)
		print("ClientSend", msgs)
		this.conn:send("return ")
		this.conn:send(msgs)
		this.conn:send("\n")
	end
end
----------------------client end---------------------


function client.OnDestroy()
	if this.conn then
		this.conn:close()
	end
end

return client
