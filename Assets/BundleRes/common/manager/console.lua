
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "xlua.util"
local socket = require "socket.socket"
local loadstring = loadstring
local lpeg = require "lpeg"
local P = lpeg.P
local S = lpeg.S
local R = lpeg.R
local match = lpeg.match

local print = function ( ... )
    _G.print("[console]", ...)
end

local this = {
	client = {},
	Ip = "*",
	Port = "9988",
	server = nil,
}
--local this = console

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

function this.coroutine_start_accept()
	print('coroutine_start_accept')
	while this.listing 
	do
		this.server:settimeout(0.01)
		local c, err = this.server:accept()
		-- print("accept", c, err)
		if (not err) then
			-- c:settimeout(0.1)
			local clientname = tostring(c) .. "="
			.. tostring(c:getpeername()) 
			-- .. ", sock:" .. tostring(c:getsockname()) --c:receive()
			print(clientname .. " connect at " .. os.date("%m/%d/%Y %H:%M:%S"))
			c:send(tostring("[" .. clientname .. "], wellcom!!!\r\n"))
			this.onconnect( c )
		end

		yield_return(UnityEngine.WaitForSeconds(1))
	end
end

function this.coroutine_start_receive()
	print('coroutine_start_receive')
	while this.listing 
	do
		local canread, sendt, status = socket.select(this.client, nil, 0.001)
		-- print("canread", #canread, #this.client)
		for _, c in ipairs(canread) do
			-- c:settimeout(0.01)
			local line, err = c:receive()
			print("receive", tostring(c), line, err)
			if not err then
				-- c:send(string.upper(line) .. ": SERVER_SIDE\r\n")
				this.passercmd( c, line )
			elseif(err == "closed")then
				this.ondisconnect( c )
			else
				c:send("___ERRORPC"..err.. "\r\n")
			end
		end

		yield_return(UnityEngine.WaitForSeconds(1))
	end
end

local man = 
[[
	lua:		execute lua code
	bc:			broadcast msg
	h/help:		show this help
]]
local function logcreate(c)
	return function ( condition, stackTrace, type )
		c:send(condition)
		c:send("\n")
	end
end
local LogType = UnityEngine.LogType
function this.passercmd( c, cmd )
	local pidx = match(S' '^0 * (R'az'^0 * R'09'^0), cmd)
	local exe = string.sub(cmd, 1, pidx)
	local params = string.sub( cmd, pidx+1 )
	exe = string.gsub( exe, ' ', '' )
	local commands = {
		help = function ()
			c:send(man)
		end,
		lua = function ( code )
			local f = load( "return function()" .. code .. " end")
			if f then f() end
		end,
		bc = function( msg )
			this.broadcast(c, tostring(c) .. ":" .. msg)
		end,
		exit = function (  )
			this.ondisconnect( c )
		end,
		ls = function ( prm )
			local msg = "all clients:\n"
			for i,v in ipairs(this.client) do
				msg = msg .. i .. " " .. tostring(v) .. ":" .. v:getpeername() ..(c == v and "(me)\n" or "\n")
			end
			c:send(msg)
		end,
		log = function (p)
			this.logl = this.logl or function( condition, stackTrace, type )
				c:send(tostring(type) .. condition)
				if(type == LogType.Error or type == LogType.Exception)then
					c:send(string.gsub(stackTrace, "\n", "\n[App]\t"))
				end
				c:send("\n")
			end
			if(p == "off")then 
				UnityEngine.Application.logMessageReceived("-", this.logl)
				c:send("remote log off\n")
			else
				UnityEngine.Application.logMessageReceived("+", this.logl)
				c:send("remote log on\n")
			end
		end
	}
	commands.h = commands.help
	commands.l = commands.log
	this.commands = commands
	local command = this.commands[exe]
	if(command ~= nil)then command(params)end
	-- c:send(exe .. " done\r\n")
end

function this.broadcast(c, msg )
	for _, cc in ipairs(this.client) do
		if cc ~= c then cc:send(msg .. "\n") end
	end
end

function this.onconnect( c )
	local idx = -1
	for i,v in ipairs(this.client) do
		if v == c then idx = i end
	end
	if idx == -1 then
		table.insert( this.client, c )
		-- this.client[#this.client] = c
		-- this.client[c:getfd()] = c
		this.broadcast(c, "welcom " .. tostring(c:getpeername()))
	end
end

function this.ondisconnect( c )
	local idx = -1
	for i,v in ipairs(this.client) do
		if v == c then idx = i end
	end
	if idx ~= -1 then
		c:close()
		table.remove( this.client,idx )
		this.broadcast(c, "byebye " .. tostring(c:getpeername()))
		-- this.client[c:getfd()] = nil
	end
end

--AutoGenInit Begin
function this.AutoGenInit() end
--AutoGenInit End

function this.Awake()
	this.AutoGenInit()
end

-- function this.OnEnable() end

function this.Start()
	local server, err = socket.bind(this.Ip, this.Port, 1)
	print("server:", server, err)
	this.listing = true
	-- server:listen(0)
	local count = 0
	this.server = server
	util.coroutine_call(this.coroutine_start_accept)
	util.coroutine_call(this.coroutine_start_receive)
end

-- function this.FixedUpdate() end

-- function this.Update() end

-- function this.LateUpdate() end

function this.OnDestroy()
	print("console OnDestroy")
	this.listing = false
	this.server:close()
	do
		for i,v in pairs(this.client) do
			print("shutdown", i, tostring(v))
			v:shutdown()
		end
	end
end

function this.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return this
