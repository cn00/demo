
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "utility.xlua.util"
local socket = require "socket.socket"
local sprotoparser = require "utility.sprotoparser"

local http = require "socket.http"
local ltn12 = require "socket.ltn12"

local lfb = require "lfb"
local flatbuffers = require "flatbuffers.flatbuffers"
local proto, ptid, bfbs_names = require ("proto.proto") ()

local print = function ( ... )
    _G.print("[network]", ...)
end

print(proto, ptid)

local response_body = {}
local post_data = "post_data"
-- local res, code, headers, status = http.request "http://localhost:8008"
local res, code, headers, status = http.request
{  
	url = "http://anbolihua.iteye.com/blog/2316423",
	-- url = "http://localhost:8008/index.html",
	method = "GET",
	headers =
	{
		["Content-Type"] = "text/html; charset=utf-8",
		-- ["Content-Length"] = #post_data,
	},
	source = ltn12.source.string(post_data),
	sink = ltn12.sink.table(response_body)
}
print("http.request", util.dump {res, code, headers, status, response_body=response_body})

local Tag = "[network]"
local network = {
	connect_stat = false,
	Ip = "10.23.22.233",
	Port = "8001",
	socket = socket,
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
function this.coroutine_demo()
	print('network coroutine start!')
	
end

--AutoGenInit Begin
function this.AutoGenInit()
    this.Button_Button = Button:GetComponent("UnityEngine.UI.Button")
end
--AutoGenInit End

function this.Awake()
	this.AutoGenInit()
end

-- function this.OnEnable() end
local function fb_test( ... )
	local monster = proto[ptid.monsterc2s]
	local vec3 = require "common.Vec3"
	local shareT = require "Sample.shareT"

	local b = flatbuffers.Builder(1)
    local name = b:CreateString("MyMonster")
	
	-- all children should build before parent
	shareT.Start(b)
	local pos = vec3.CreateVec3(b, 1999.0, 2.0, 3.0)
	shareT.AddPos(b, pos)
    shareT.AddMana(b, 8888)
	local st = shareT.End(b)
	
	monster.Start(b)
	
	local pos2 = vec3.CreateVec3(b, 2.0, 22.0, 23.0)
	monster.AddPos(b, pos2)

    monster.AddHp(b, 9980)
    monster.AddMana(b, 9999)
	monster.AddName(b, name)
	
	monster.AddSt(b, st)
	
    local mon = monster.End(b)
	
    if FBSizePrefix then
        b:FinishSizePrefixed(mon)
    else
        b:Finish(mon)
    end
	local buf, offset = b:Output(true), b:Head()
	local size = #buf
	print("offset:" .. offset, "size:"..size)
	
	this.conn:send(ptid.monsterc2s)
	this.conn:send(10000000+size)
	this.conn:send(1000+offset)
	this.conn:send(buf)

	-- buf = flatbuffers.binaryArray.New(buf)
	-- offset = 0
	-- offset = offset + flatbuffers.N.Int32.bytewidth
	-- local size = flatbuffers.N.Int32:Unpack(buf, offset)
	-- local c2s = proto[ptid.monsterc2s].GetRoot(buf, offset)
	-- print(offset, size, c2s:Name(), c2s:Hp())
end
-- print("this:", util.dump(this))

function this.co_init_lfbs()
	-- for k,v in pairs(lfb) do
	-- 	print(k, v)
	-- end
	local lfb = lfb()
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

local Monster_c2s = {
	-- pos:common.Vec3;
	-- mana:short = 150;
	-- hp:short = 100;
	-- name:string;
	-- st:shareT;
	pos = {x = 1100000, y = 22,z = 33},
	mana = 989,
	hp = 89,
	name = "name Monster_c2s",
	st = {
		-- pos:common.Vec3;
		-- mana:short = 150;
		pos = {x = 99, y = 88, z = 77},
		mana = 456,
	}
}
this.Monster_c2s = Monster_c2s

local Monster_s2c = {
	-- pos:common.Vec3;
	-- mana:short = 150;
	-- hp:short = 100;
	-- name:string;
	-- // friendly:bool = false (deprecated);
	-- inventory:[ubyte];
	-- color:common.Color = Blue;
	-- weapons:[common.Weapon];
	-- equipped:common.Equipment;
	-- st:shareT;
	pos = {x = 11, y = 22,z = 33},
	mana = 989,
	hp = 89,
	name = "name Monster_c2s",
	st = {
		-- pos:common.Vec3;
		-- mana:short = 150;
		pos = {x = 99, y = 88, z = 77},
		mana = 456,
	}
}
this.Monster_s2c = Monster_s2c

function this.lfb_test()
	coroutine_call(function()
		while(waitfor_lfb_init)do
			yield_return(UnityEngine.WaitForSeconds(0.1))
		end

		local protoid = ptid.monsterc2s
		local lfb = this.lfb
		this.loaded_bfbs = lfb:loaded()
		local buf
		print("this.loaded_bfbs", util.dump(this.loaded_bfbs))
		local ex = os.clock()
		for i = 1,10000 do
			buf = assert(lfb:encode("sample.bfbs.txt", proto[protoid], this.Monster_c2s))
		end
		local ey = os.clock()
		print("encode 10000s", ey-ex, #buf, buf:gsub("[\0-\13]",""))
			
		this.conn:send(protoid)
		this.conn:send(10000000+ #buf)
		-- this.conn:send(1000+offset)
		this.conn:send(buf)

		local ey = os.clock()
		local t  
		for i = 1, 10000 do
			t = assert(lfb:decode("sample.bfbs.txt", proto[protoid], buf))
		end
		local ez = os.clock()
		print("decode 10000s:", ez - ey, #t, util.dump(t))
	end)
end

function this.coroutine_start_receive()
	print('coroutine_start_receive')
	while true 
	do
		local canread, sendt, status = socket.select({this.conn}, nil, 0.001)
		-- print("canread", #canread, #this.client)
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

			if not err then
				print()
			elseif(err == "closed")then
				this.connect_stat = conn_stat.offline
				c:close()
				-- this.ondisconnect( c )
			else
				c:send("___ERRORPC"..err.. "\r\n")
			end
			::continue::
		end

		yield_return(UnityEngine.WaitForSeconds(1))
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
		while (this.connect_stat == conn_stat.connecting) do
			print("try connect ...")
			local conn, err = socket.connect(this.Ip, this.Port)
			print("connect", conn, err)
			if err == nil and conn then
				this.conn = conn
				this.connect_stat = conn_stat.connected
			-- else if err == "connection refused" then
			-- 	print(err)
			else
				print("unknow err", err)
			end
			yield_return(UnityEngine.WaitForSeconds(1))
		end
	end)
end

function this.Start()
	coroutine_call(this.co_init_lfbs)

	this.Connect()

	this.Button_Button.onClick:AddListener(this.lfb_test)
	-- fb_test()

	util.coroutine_call(this.coroutine_demo)
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
