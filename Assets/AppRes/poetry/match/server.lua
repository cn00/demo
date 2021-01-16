
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
	tcpClients = {}, -- tcpClients
	clientsInfo = {}, -- clients info: client = {status, id, tcp}
	conn_stat = {
		offline = 0,
		connecting = 1,
		connected = 2,
	},
}
local this = server

---ServerStart 
function server.ServerStart()
	local port= server.ServerPort
	print("<color=red>ServerStart", port)
	local tcp, err = socket.bind("*", port)
	if err == nil then
		server.tcpServer = tcp
		server.ServerStartAcceptLoop()
		server.ServerStartReceiveLoop()
		print("StartServer ok, listen on:", port)
	else
		print("StartServer failed.", err)
	end
end

function server.ServerStartAcceptLoop()
	local clientIdx = util.newIdx(1)
	xutil.coroutine_call(function()
		print("waiting for client")
		local errmsg = nil
		while true do
			server.tcpServer:settimeout(0.01)
			local client, err = server.tcpServer:accept()
			if client and not err then
				server.tcpClients = server.tcpClients or {}
				server.clientsInfo = server.clientsInfo or {}
				server.tcpClients[1+#server.tcpClients] = client
				local cinfo =  {
					status = 0,
					id = clientIdx(),
					tcp = client
				}
				server.clientsInfo[client] = cinfo
				print("accept client", client, #server.tcpClients)
				--client:send("wellcom "..tostring(client).."\n")
				client:send("return ")
				client:send(xutil.dump({ type="login", body = cinfo }))
				client:send("\n")
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
				local line, err = c:receive("*l")

				if not err then
					server.ServerOnReceiveMsg(c, line)
				elseif(err == "closed")then
					c:close()
					-- this.ondisconnect( c )
					this.connect_stat = this.conn_stat.offline
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
---@param c tcpclient
---@param msgs string
function server.ServerOnReceiveMsg(c, msgs)
	print("ServerOnReceiveMsg", c, msgs)
	local function selectClient(filter)
		filter = filter or function() return true  end
		local cgroup = {}
		for k, v in pairs(server.clientsInfo) do
			if filter(v) then cgroup[1+#cgroup] = v.tcp end
		end
		return cgroup
	end

	local f = load(msgs)
	if f then
		local msgt = f()
		local type = msgt.type
		local body = msgt.body

		-- TODO: do server things here
		-- singleton and group msg
		local msgt = {type = type, body = body}
		local cgroup = selectClient(function(i)return i.tcp ~= c  end)
		print("ServerOnReceiveMsg", type, #cgroup)
		server.ServerBroadcastMsg(xutil.dump(msgt), cgroup)
		goto finaly

		-- broadcast msg
		--match.ServerBroadcastMsg(msgt)
	else
		print("unrecognised msg", server.clientsInfo[c].id)
	end
	::finaly::

end

function server.ServerBroadcastMsg(msgs, cgroup)
	cgroup = cgroup or server.tcpClients
	for i, v in ipairs(cgroup) do
		v:send("return")
		v:send(msgs)
		v:send("\n")
	end
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

return server
