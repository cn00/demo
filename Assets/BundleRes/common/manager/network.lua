
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"
local socket = require "socket"
print("--------")
    for k, v in pairs(socket) do
        print(k, v)
    end
    --[[
        dns	table: 0x14d1024c0
        skip	function: 0x139fafc90
        _SETSIZE	1024.0
        protect	function: 0x139faead0
        select	function: 0x139fb2cf0
        udp6	function: 0x139fb4e00
        sleep	function: 0x139fb42f0
        _VERSION	LuaSocket 3.0-rc1
        gettime	function: 0x139fb42a0
        __unload	function: 0x139fafcd0
        tcp	function: 0x139fb3c30
        tcp6	function: 0x139fb3c40
        connect	function: 0x139fb3c50
        udp	function: 0x139fb4df0
        newtry	function: 0x139faea70
    ]]
print("--------")

local Tag = "[network]"
local network = {
	Ip = "127.0.0.1",
	Port = "8888"
}
local this = network

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)
-- function this.coroutine_demo()
--     util.coroutine_call(function()
--         print('network coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
function this.AutoGenInit() end
--AutoGenInit End

function this.Awake()
	this.AutoGenInit()
end

-- function this.OnEnable() end

local fd = assert(socket.connect(this.Ip, this.Port))
function this.Start()
	--assert(coroutine.resume(this.coroutine_demo()))
end

-- function this.FixedUpdate() end

-- function this.Update() end

-- function this.LateUpdate() end

-- function this.OnDestroy() end

function this.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end
local function send_package(fd, pack)
	local package = string.pack(">s2", pack)
	socket.send(fd, package)
end

local function unpack_package(text)
	local size = #text
	if size < 2 then
		return nil, text
	end
	local s = text:byte(1) * 256 + text:byte(2)
	if size < s+2 then
		return nil, text
	end

	return text:sub(3,2+s), text:sub(3+s)
end

local function recv_package(last)
	local result
	result, last = unpack_package(last)
	if result then
		return result, last
	end
	local r = socket.recv(fd)
	if not r then
		return nil, last
	end
	if r == "" then
		error "Server closed"
	end
	return unpack_package(last .. r)
end

local session = 0

local function send_request(name, args)
	session = session + 1
	local str = request(name, args, session)
	send_package(fd, str)
	print("Request:", session)
end

local last = ""

local function print_request(name, args)
	print("REQUEST", name)
	if args then
		for k,v in pairs(args) do
			print(k,v)
		end
	end
end

local function print_response(session, args)
	print("RESPONSE", session)
	if args then
		for k,v in pairs(args) do
			print(k,v)
		end
	end
end

local function print_package(t, ...)
	if t == "REQUEST" then
		print_request(...)
	else
		assert(t == "RESPONSE")
		print_response(...)
	end
end
local function dispatch_package()
	while true do
		local v
		v, last = recv_package(last)
		if not v then
			break
		end

		print_package(host:dispatch(v))
	end
end

send_request("handshake")
send_request("set", { what = "hello", value = "world" })
while true do
	dispatch_package()
	local cmd = socket.readstdin()
	if cmd then
		if cmd == "quit" then
			send_request("quit")
		else
			send_request("get", { what = cmd })
		end
	else
		socket.usleep(100)
	end
end

return network
