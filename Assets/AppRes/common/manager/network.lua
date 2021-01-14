
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject

--require "init"
local util = require "xlua.util"
local socket = require "socket.socket"
local sprotoparser = require "sprotoparser"

local http = require "socket.http"
local ltn12 = require "socket.ltn12"

local lfb = require("lfb")()
local proto, ptid, bfbs_names = require ("proto") ()
print("require proto", proto, ptid, bfbs_names)

local logtag = "[network]"
local print = function ( ... )
    _G.print(logtag, ...)
    -- _G.print(table.dump({...}, true, logtag))
end

-- local response_body = {}
-- local post_data = "post_data"
-- -- local res, code, headers, status = http.request "http://localhost:8008"
-- local res, code, headers, status = http.request
-- {  
-- 	url = "http://anbolihua.iteye.com/blog/2316423",
-- 	-- url = "http://localhost:8008/index.html",
-- 	method = "GET",
-- 	headers =
-- 	{
-- 		["Content-Type"] = "text/html; charset=utf-8",
-- 		-- ["Content-Length"] = #post_data,
-- 	},
-- 	source = ltn12.source.string(post_data),
-- 	sink = ltn12.sink.table(response_body)
-- }
-- print("http.request", table.dump {res, code, headers, status, response_body=response_body})
local protopath = "proto.bfbs.txt"
local Tag = "[network]"
local network = {
	connect_stat = false,
	--Ip = "10.23.22.233",
	--Port = "8001",
	socket = socket,
	proto = {proto = proto, ptid = ptid, bfbs_names = bfbs_names}
}
local this = network
local coroutine_call = util.coroutine_call

local conn_stat = {
	offline = 0,
	connecting = 1,
	connected = 2,
}

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
end
--AutoGenInit End

function this.Awake()
	this.AutoGenInit()

	this.Connect()
end

function this.co_init_lfbs()
	-- for k,v in pairs(lfb) do
	-- 	print(k, v)
	-- end
	this.lfb = lfb
	for i,v in ipairs(bfbs_names) do
		local obj
		yield_return(CS.AssetSys.Instance:GetAsset(v, function(asset)
			obj = asset.bytes
			-- print(v, (asset:GetType()), #obj, obj:gsub("[\0-\13]",""))
		end))
		-- local ok, content = assert(lfb:load_bfbs_file("Assets/BundleRes/" .. bfbs_name))
		-- print("sample.bfbs.txt 2", #content, #obj, content:gsub("[\0-\13]",""))

		local ok = assert(lfb:load_bfbs(v, #obj, obj))
		print(i,v, ok)
	end
end

local Action_Move = {
	head = {
		id = "10000",
	},
	from = {x = 11,  y = 22,  z = 33},
	to   = {x = 911, y = 922, z = 933},
}
this.Action_Move = Action_Move

function this.lfb_test()
	coroutine_call(function()
		while(waitfor_lfb_init)do
			yield_return(UnityEngine.WaitForSeconds(0.1))
		end

		local protoid = ptid.Action_Move_c2s
		local lfb = this.lfb
		this.loaded_bfbs = lfb:loaded()
		local buf
		print("this.loaded_bfbs", protoid, table.dump(this.loaded_bfbs))
		-- local ex = os.clock()
		-- for i = 1,10000 do
			buf = assert(lfb:encode(protopath, proto[protoid], this.Action_Move))
		-- end
		-- local ey = os.clock()
		-- print("encode 10000 times", ey-ex, #buf, buf:gsub("[\0-\13]",""))
			
		this.conn:send(protoid)
		this.conn:send(10000000 + #buf)
		-- this.conn:send(1000+offset)
		this.conn:send(buf)

		local ey = os.clock()
		local t  
		for i = 1, 10000 do
			t = assert(lfb:decode(protopath, proto[protoid], buf))
		end
		local ez = os.clock()
		print("decode 10000 times:", ez - ey, #t, table.dump(t, true, logtag))
	end)
end

function this.coroutine_start_receive()
	print('coroutine_start_receive')
	while true 
	do
		local canread, sendt, status = socket.select({this.conn}, nil, 0.001)
		-- print("canread", #canread, #this.client)
		-- local liner, err = c:receive("*l")
		-- print("read line", #liner, liner:gsub("[\0-\13]",""), err)
		for _, c in ipairs(canread) do
			c:settimeout(0.1)
			local protoid, err = c:receive(8)
			if protoid == nil then 
				print("goto continue", err)
				this.connect_stat = conn_stat.offline
				c:close()
				goto continue 
			end
			protoid = tonumber(protoid, 10)
			local size = tonumber(c:receive(8), 10) - 10000000
			local data = c:receive(size)
			-- local data, err = c:receive()
			print("receive", #data, data:gsub("[\0-\13]",""), err)

			local res_cb = this.wait_for_res[protoid - 10000000]
			if res_cb ~= nil then
				local lfb = this.lfb
				local t = assert(lfb:decode(protopath, proto[protoid], data))
				res_cb(t)
				this.wait_for_res[protoid - 10000000] = nil
			end 

			if not err then
				--print()
			elseif(err == "closed")then
				this.connect_stat = conn_stat.offline
				c:close()
				-- this.ondisconnect( c )
			else
				c:send("<color=red>___ERRORPC"..err.. "<color/>\r\n")
			end
			::continue::
		end

		yield_return(UnityEngine.WaitForSeconds(1))
	end
end


this.wait_for_res = {}
function this.send(protoid, dt, res_cb)
	local buf = assert(lfb:encode(protopath, proto[protoid], dt))
	this.conn:send(protoid)
	this.conn:send(10000000 + #buf)
	this.conn:send(buf)
	if res_cb ~= nil then
		this.wait_for_res[protoid] = res_cb
	end
end

function this.Connect()
	if this.conn ~= nil and this.conn:getstats() == 1 then 
		--https://stackoverflow.com/questions/4160347/close-vs-shutdown-socket
		-- this.conn:shutdown()
		this.conn:close()
		this.conn = nil
	end
	this.connect_stat = conn_stat.connecting
	coroutine_call(function ()
		local retrycount = 0
		while (this.connect_stat == conn_stat.connecting and retrycount < 10) do
			print("try connect ...", retrycount)
			retrycount = 1 + retrycount
			local conn, err = socket.connect(Ip, Port)
			if err == nil and conn then
				this.conn = conn
				this.connect_stat = conn_stat.connected
				print("<color=green>connected to server ok</color>.")
			-- else if err == "connection refused" then
			-- 	print(err)
			else
				print("connect err", err)
			end
			yield_return(UnityEngine.WaitForSeconds(1))
		end
	end)
end

function this.Start()
	--coroutine_call(this.co_init_lfbs)

	this.Connect()

	--this.Button_Button.onClick:AddListener(this.lfb_test)
	-- fb_test()

	util.coroutine_call(this.coroutine_start_receive)
end

-- function this.FixedUpdate() end

function this.Update() 
	if (this.connect_stat == conn_stat.offline)then
		print("reconnect ...")
		this.Connect()
	end
end

-- function this.LateUpdate() end

function this.OnDestroy()
	if this.conn ~= nil and this.conn:getstats() == 1 then 
		this.conn:close()
	end
end

function this.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return network
