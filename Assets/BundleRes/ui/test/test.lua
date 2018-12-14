local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local AssetSys = CS.AssetSys
local sqlite = CS.SQLite.SQLite3

local ios = UnityEngine.RuntimePlatform.IPhonePlayer
local os = UnityEngine.Application.platform --"iOS" --"OSX Editor"
if os == ios then
    require "nslua"
end

local util = require "utility.xlua.util"
local coroutine_call = util.coroutine_call
local test = {}
local this = test

local print = function ( ... )
    _G.print("[test]", ... )
    -- _G.print("[this]", debug.traceback())
end

function YieldAndCallback(to_yield, callback)
	mono:YieldAndCallback(to_yield, callback)
end
local yield_return = util.async_to_sync(YieldAndCallback)

--[[
	        
        if os == ios then
            octest()

            oc_blsdk_init()
            yield_return(UnityEngine.WaitForSeconds(0.5))
            oc_blsdk_login()
        end

        -- ffitest()

]]
function this.nslua_test()
    print"nslua_test begin"

    -- [[NSBundle mainBundle] pathForResource:@"LuaBridge" ofType:@"lua"];
    print("get objc path1:OCLuaBridge.lua")
    local path1 = OC.NSBundle.mainBundle:pathForResource_ofType_("LuaBridge", "lua")
    print("objc path_1:", path1)
    local f = io.open(path1)
    local c = f:read("*a")
    f:close()
    print("objc file content:", c:gsub("\n", "\\n"))

    print"nslua_test end"
end

local function didLoginSuccessWithAccessKey( accessKey, uid )
    print("didLoginSuccessWithAccessKey", accessKey, uid)
end
_G.didLoginSuccessWithAccessKey = didLoginSuccessWithAccessKey

function this.oc_blsdk_init()
    print("oc_blsdk_login begin")
    --[[ -- init sdk
        [[BLGameSdk defaultGameSdk] initWithGameid:@"85"
                                            cpId:@"2"
                                        serverid:@"159"
                                            appKey:@"bcf9f03f94234804a2aa11f6c9f4ccf0"
                                        sandboxKey:@"abc123"
                                        delegate:this];
    ]]
    -- BLSdkInit(AppId, "1", ServerId, AppKey, SandBoxKey);
    OC.BLGameSdk.defaultGameSdk:initWithGameid_cpId_serverid_appKey_sandboxKey_delegate_(
         "85"
        ,"2"
        ,"159"
        ,"bcf9f03f94234804a2aa11f6c9f4ccf0"
        ,"abc123"
        ,OC.BLGameSdkDelegateApp.Instance
    )
end

function this.oc_blsdk_login()
    print("oc_blsdk_login begin")
    --[[ [[BLGameSdk defaultGameSdk] showLoginView];]]
    OC.BLGameSdk.defaultGameSdk:showLoginView()
    print("oc_blsdk_login end")
end
function this.oc_blsdk_logout()
    print("oc_blsdk_logout begin")
    --[[ [[BLGameSdk defaultGameSdk] logout];]]
    OC.BLGameSdk.defaultGameSdk:logout()
    print("oc_blsdk_logout end")
end

function this.ffitest()
    print("ffitest began")

    local ffi = require "ffi"
    this.ffi = ffi
    -- for k,v in pairs(ffi) do
    --     print("ffi", k,v)
    -- end

    ffi.cdef [[
        typedef struct {
            int fake_id;
            unsigned int len;
        } CSSHeader;
        typedef struct {
            CSSHeader header;
            float x;
            float y;
            float z;
            float w;
        } Vector4;
    ]]

	
    local vector = ffi.typeof('Vector4 *')
	local v = CS.UnityEngine.Vector4(12.3, 23.4, 34.5, 45.6)
	--[[
		Vector4 v1 = new Vector4(), v2 = new Vector4();
		v1.Scale(v2);
	]]
	local v2 = CS.UnityEngine.Vector4(0.3, 0.4, 0.5, 0.6)
	CS.UnityEngine.Vector4.Scale(v, v2);

    local vn = ffi.cast(vector, v)
    this.vn = vn
    print("vector.dump", ffi.typeof(vn), ffi.type(vn))
    if vn.header.fake_id == -1 then
        print('vector { ', vn.x, vn.y, vn.z, vn.w, '}')
    else
        print('please gen code')
    end

	do
		-- -- printf only ok in simulator and editor why
		-- ffi.cdef [[
		-- 	int printf(const char * __restrict, ...) __printflike(1, 2);
		-- ]]
		-- print("before ffi.C.printf")
		-- local ffir = ffi.C.printf("testffi: %s\n", "fooffffffffff")
		-- print("ffir", ffir)
		
		ffi.cdef [[
			/*int fprintf(void * , const char * , ...); ok in sim*/ 
			int fprintf(void *, const char *, ...);
			int test_pow(int v);
		]]
		local tp = ffi.C.test_pow(66)
		print("test_pow", tp)
		local f = io.open(CS.AssetSys.CacheRoot .. "ffi.test.txt", "a")
		print("fprintf", f)
		local fout = ffi.C.fprintf(f, "test: %s\n", "foo")
		print("fprintf", fout)
	end
    print("ffitest end")
end

-- print("this:", util.dump(this))

function this.CheckUpdate()
	coroutine_call(function()
		local obj = nil
		yield_return(CS.AssetSys.Instance:GetAsset(
		"ui/loading/loading.prefab", function(asset)
			obj = asset
		end))
		
		local loading = GameObject.Instantiate(obj)
		loading.name = "loading"
		local oldLoading = GameObject.Find("loading")
		GameObject.DestroyImmediate(oldLoading)
		
		obj = nil
		yield_return(CS.AssetSys.Instance:GetAsset(
		"ui/update/update.prefab", function(asset)
			obj = asset
		end))
		local update = GameObject.Instantiate(obj)
		update.name = "update"
		
		this.Destroy()
	end)
end

function this.Qrcode()
	coroutine_call(function()
		local obj = nil
		yield_return(
		CS.AssetSys.Instance:GetAsset(
		"ui/loading/loading.prefab",
		function(asset)
			obj = asset
		end))
		local loading = GameObject.Instantiate(obj)
		loading.name = "loading"
		local oldLoading = GameObject.Find("loading")
		GameObject.DestroyImmediate(oldLoading)
		
		obj = nil
		yield_return(CS.AssetSys.Instance:GetAsset(
		"ui/qrcode/qrcode.prefab",
		function(asset)
			obj = asset
		end))
		local qrcode = GameObject.Instantiate(obj)
		qrcode.name = "qrcode"
		
		this.Destroy()
	end)
end

function this.Op()
	coroutine_call(function()
		print("open Op")
		local obj = nil
		yield_return(CS.AssetSys.Instance:GetAsset(
		"video/op/op.prefab",
		function(asset)
			obj = asset
		end))
		local loading = GameObject.Instantiate(obj)
		loading.name = "op"
		local oldLoading = GameObject.Find("loading")
		GameObject.DestroyImmediate(oldLoading)
		
		this.Destroy()
	end)
end

function this.TableViewTest()
	coroutine_call(function()
		print("open TableViewTest")
		
		-- local subpath = "../Streaming/Excel.tmp/EtudeLessonInfo.xlsx"
		-- local localPath = CS.AssetSys.CacheRoot .. "download/" .. subpath
		-- local endCallback = function(www)
		-- 	if www.progress >= 1 and (www.error == nil or www.error == "") then
		-- 		print("download ok", subpath)
		-- 		CS.AssetSys.AsyncSave(localPath, www.bytes)
		-- 	else
		-- 		-- print(www.error)
		-- 	end
		-- end
		-- local progressCallback = function(progress)
		-- 	print("downloading " .. progress)
		-- end
		-- local url = CS.AssetSys.HttpRoot .. subpath
		-- yield_return(CS.AssetSys.Www(url, endCallback, progressCallback))
		
		local obj = nil
		yield_return(CS.AssetSys.Instance:GetAsset(
		"ui/excel_viewer/excel_view.prefab",
		function(asset)
			obj = asset
		end))
		local go = GameObject.Instantiate(obj)
		go.name = "table_view"
		local oldLoading = GameObject.Find("loading")
		GameObject.DestroyImmediate(oldLoading)
		
		this.Destroy()
	end)
end

function this.SqliteInsert(x, y)
	local db = this.db
	local sql = "INSERT INTO `test_map` (x,y) VALUES " .. "(" .. x .. "," .. y .. ");" .. "COMMIT;"
	local stmt = sqlite.Prepare2(db, sql)
	local result2 = sqlite.Step(stmt)
	local result3 = sqlite.Finalize(stmt)
	if result2 == sqlite.Result.Error then
		local errmsg = sqlite.GetErrmsg(db)
		print(errmsg)
	elseif result2 == sqlite.Result.Done then
		local rowsAffected = sqlite.Changes(db)
		-- print("rowsAffected", rowsAffected)
	elseif result2 == sqlite.Result.Busy then
		print("db busy")
	end
	-- local lastid = sqlite.LastInsertRowid(db)
	-- print(sql, result2, result3, lastid)
end

function this.SqliteSelect()
	local sql = "SELECT * FROM `test_map`;"
	local stmt = sqlite.Prepare2(db, sql)
	local count = 0
	local result2 = sqlite.Step(stmt)
	local collect = {}
	while result2 == sqlite.Result.Row do
		count = count + 1
		local c0 = sqlite.ColumnString(stmt, 0)
		local c1 = sqlite.ColumnString(stmt, 1)
		local c2 = sqlite.ColumnString(stmt, 2)
		collect[count] = table.concat({tostring(result2), c0, c1, c2}, ",")
		
		result2 = sqlite.Step(stmt)
	end
	sqlite.Finalize(stmt)
	print(table.concat(collect, "\n"))
end

function this.CaptureScreenshot()
	local screenshoot = CS.AssetSys.CacheRoot .. "Screenshot.lua.png"
	CS.UnityEngine.ScreenCapture.CaptureScreenshot(screenshoot, 1)
end

function this.GetIpAddresses()
	-- string[]
	local ips = CS.NetSys.LocalIpAddressStr()
	for i = 0, ips.Length - 1 do
		print("arr", i, ips[i])
	end
	
	-- List<string>
	local ipl = CS.NetSys.LocalIpAddressStrList()
	for i = 0, ipl.Count - 1 do
		print("List", i, ipl[i])
	end
end

function this.UpdateOnClick()
	this.Update()
end

function this.InsertOnClick()
	local input0 = this.ix_InputField.text
	local input1 = this.iy_InputField.text
	
	print("clicked, you input is [" .. input0 .. input1 .. "]")
	this.SqliteInsert(input0, input1)
end

function this.coroutine_insert_10000()
	this.Insert_10000_Button.interactable = false
	coroutine_call(function()
		print("coroutine_insert_10000 coroutine start!")
		local t = os.time()
		-- insert 10000 records coast 83 second
		local values = {}
		for i = 1, 10000 do
			-- this.SqliteInsert(math.random(1,100000), math.random(20000,100000))
			values[i] = "(" .. math.random(1, 100000) .. "," .. math.random(20000, 100000) .. ")"
			-- print("idx:",i)
			-- yield_return(CS.UnityEngine.WaitForSeconds(0))
		end
		local sql = "INSERT INTO `test_map` (x,y) VALUES " .. table.concat(values, ",") .. ";" .. "COMMIT;"
		local db = this.db
		local stmt = sqlite.Prepare2(db, sql)
		local result2 = sqlite.Step(stmt)
		local result3 = sqlite.Finalize(stmt) -- 清理前一个任务
		if result2 == sqlite.Result.Error then
			local errmsg = sqlite.GetErrmsg(db)
			print(errmsg)
		elseif result2 == sqlite.Result.Done then
			local rowsAffected = sqlite.Changes(db)
			print("rowsAffected", rowsAffected)
		elseif result2 == sqlite.Result.Busy then
			print("db busy")
		end
		print()
		
		this.Insert_10000_Button.interactable = true
		local lastid = sqlite.LastInsertRowid(db)
		print(result2, "lastid" .. lastid, "coast:" .. os.time() - t)
	end)
end

--AutoGenInit Begin
function this.AutoGenInit()
    this.Insert_10000_Button = Insert_10000:GetComponent("UnityEngine.UI.Button")
    this.ix_InputField = ix:GetComponent("UnityEngine.UI.InputField")
    this.iy_InputField = iy:GetComponent("UnityEngine.UI.InputField")
    this.Insertxy_Button = Insertxy:GetComponent("UnityEngine.UI.Button")
    this.OpenOP_Button = OpenOP:GetComponent("UnityEngine.UI.Button")
    this.CheckUpdate_Button = CheckUpdate:GetComponent("UnityEngine.UI.Button")
    this.QRCode_Button = QRCode:GetComponent("UnityEngine.UI.Button")
    this.IpAddress_Button = IpAddress:GetComponent("UnityEngine.UI.Button")
    this.tableview_Button = tableview:GetComponent("UnityEngine.UI.Button")
    this.nslua_Button = nslua:GetComponent("UnityEngine.UI.Button")
    this.blsdk_init_Button = blsdk_init:GetComponent("UnityEngine.UI.Button")
    this.blsdk_login_Button = blsdk_login:GetComponent("UnityEngine.UI.Button")
    this.blsdk_logout_Button = blsdk_logout:GetComponent("UnityEngine.UI.Button")
    this.ffi_test_Button = ffi_test:GetComponent("UnityEngine.UI.Button")
end
--AutoGenInit End
function this.Awake()
	this.AutoGenInit()
end

function this.Start()
	local qrOnClick = function()
		-- CS.QRCode.OpenQRScanner()
		-- CS.JavaUtil.CallJavaApi("com.game.qrview.Interface", "OpenQRScanner");
		-- CS.JavaUtil.CallJavaApi("com.unity3d.player.UnityPlayer", "OpenQRScanner");
		assert(coroutine.resume(this.Qrcode()))
	end
	this.QRCode_Button.onClick:AddListener(this.Qrcode )
	this.Insertxy_Button.onClick:AddListener(this.InsertOnClick )
	this.Insert_10000_Button.onClick:AddListener(this.coroutine_insert_10000 )
	this.OpenOP_Button.onClick:AddListener(this.Op)
	this.CheckUpdate_Button.onClick:AddListener(this.CheckUpdate)
	this.tableview_Button.onClick:AddListener(this.TableViewTest)

	this.nslua_Button.onClick:AddListener(this.nslua_test)
	this.blsdk_init_Button.onClick:AddListener(this.oc_blsdk_init)
	this.blsdk_login_Button.onClick:AddListener(this.oc_blsdk_login)
	this.blsdk_logout_Button.onClick:AddListener(this.oc_blsdk_logout)
	this.ffi_test_Button.onClick:AddListener(this.ffitest)

	local ixonEndEdit = function(text)
		this.iy_InputField:Select()
		print("x_InputField.onEndEdit:" .. text)
	end
	local iyonEndEdit = function(text)
		print("y_InputField.onEndEdit:" .. text)
		this.InsertOnClick()
	end
	this.ix_InputField.onEndEdit:AddListener(ixonEndEdit)
	this.iy_InputField.onEndEdit:AddListener(iyonEndEdit)
	
	this.IpAddress_Button.onClick:AddListener(this.octest)
	
	local sqlpath = CS.AssetSys.CacheRoot .. "this.sqlite3"
	print(sqlpath)
	local result, db = sqlite.Open(sqlpath)
	print("this.Awake sqlite.Open", result, db)
	
	if result == sqlite.Result.OK then
		this.db = db
		local sql = [[create table if not exists `test_map` (
            `id` integer primary key autoincrement not null,
            `x` integer not null,
            `y` integer not null
            )]]
		-- local sql = "create table if not exists `test_map` (\n"
		-- .. "`id` integer primary key autoincrement not null,\n"
		-- .. "`x` integer not null,\n"
		-- .. "`y` integer not null\n"
		-- .. ")"
		local stmt = sqlite.Prepare2(db, sql)
		local result2 = sqlite.Step(stmt)
		local result3 = sqlite.Finalize(stmt)
		if result2 == sqlite.Result.Error then
			local errmsg = sqlite.GetErrmsg(db)
			print(errmsg)
		elseif result2 == sqlite.Result.Done then
			local rowsAffected = sqlite.Changes(db)
			print("rowsAffected", rowsAffected)
		end
	end
end

function this.OnDestroy()
	print("this.OnDestroy")
	sqlite.Close(this.db)
end

function this.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return test 