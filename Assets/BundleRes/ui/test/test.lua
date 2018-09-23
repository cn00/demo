local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local AssetSys = CS.AssetSys
local sqlite = CS.SQLite.SQLite3

local util = require "lua.utility.xlua.util"

local test = {}
local self = test

function YieldAndCallback(to_yield, callback)
	mono:YieldAndCallback(to_yield, callback)
end
local yield_return = util.async_to_sync(YieldAndCallback)

function test.CheckUpdate()
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
		
		self.Destroy()
	end)
end

function test.Qrcode()
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
		
		self.Destroy()
	end)
end

function test.Op()
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
		
		self.Destroy()
	end)
end

function test.TableViewTest()
	return coroutine.create(function()
		print("open TableViewTest")
		
		local subpath = "StreamingAssets/Excel.tmp/EtudeLessonInfo.xlsx"
		local localPath = CS.AssetSys.CacheRoot .. "download/" .. subpath
		local endCallback = function(www)
			if www.progress >= 1 and (www.error == nil or www.error == "") then
				print("download ok", subpath)
				CS.AssetSys.AsyncSave(localPath, www.bytes)
			else
				-- print(www.error)
			end
		end
		local progressCallback = function(progress)
			print("downloading " .. progress)
		end
		local url = CS.AssetSys.HttpRoot .. subpath
		yield_return(CS.AssetSys.Www(url, endCallback, progressCallback))
		
		local obj = nil
		yield_return(CS.AssetSys.Instance:GetAsset(
		"ui/test/table_view.prefab",
		function(asset)
			obj = asset
		end))
		local table_view = GameObject.Instantiate(obj)
		table_view.name = "table_view"
		local oldLoading = GameObject.Find("loading")
		GameObject.DestroyImmediate(oldLoading)
		
		self.Destroy()
	end)
end

function test.SqliteInsert(x, y)
	local db = self.db
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

function test.SqliteSelect()
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

function test.CaptureScreenshot()
	local screenshoot = CS.AssetSys.CacheRoot .. "Screenshot.lua.png"
	CS.UnityEngine.ScreenCapture.CaptureScreenshot(screenshoot, 1)
end

function test.GetIpAddresses()
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

function test.UpdateOnClick()
	test.Update()
end

function test.InsertOnClick()
	local input0 = self.ix_InputField.text
	local input1 = self.iy_InputField.text
	
	print("clicked, you input is [" .. input0 .. input1 .. "]")
	self.SqliteInsert(input0, input1)
end

function test.coroutine_insert_10000()
	self.Insert_10000_Button.interactable = false
	coroutine_call(function()
		print("coroutine_insert_10000 coroutine start!")
		local t = os.time()
		-- insert 10000 records coast 83 second
		local values = {}
		for i = 1, 10000 do
			-- self.SqliteInsert(math.random(1,100000), math.random(20000,100000))
			values[i] = "(" .. math.random(1, 100000) .. "," .. math.random(20000, 100000) .. ")"
			-- print("idx:",i)
			-- yield_return(CS.UnityEngine.WaitForSeconds(0))
		end
		local sql = "INSERT INTO `test_map` (x,y) VALUES " .. table.concat(values, ",") .. ";" .. "COMMIT;"
		local db = self.db
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
		
		self.Insert_10000_Button.interactable = true
		local lastid = sqlite.LastInsertRowid(db)
		print(result2, "lastid" .. lastid, "coast:" .. os.time() - t)
	end)
end

--AutoGenInit Begin
function test.AutoGenInit()
	test.Insert_10000_Button = Insert_10000:GetComponent("UnityEngine.UI.Button")
	test.ix_InputField = ix:GetComponent("UnityEngine.UI.InputField")
	test.iy_InputField = iy:GetComponent("UnityEngine.UI.InputField")
	test.Insertxy_Button = Insertxy:GetComponent("UnityEngine.UI.Button")
	test.OpenOP_Button = OpenOP:GetComponent("UnityEngine.UI.Button")
	test.CheckUpdate_Button = CheckUpdate:GetComponent("UnityEngine.UI.Button")
	test.QRCode_Button = QRCode:GetComponent("UnityEngine.UI.Button")
	test.IpAddress_Button = IpAddress:GetComponent("UnityEngine.UI.Button")
	test.tableview_Button = tableview:GetComponent("UnityEngine.UI.Button")
end
--AutoGenInit End
function test.Awake()
	self.AutoGenInit()
	local qrOnClick = function()
		-- CS.QRCode.OpenQRScanner()
		-- CS.JavaUtil.CallJavaApi("com.game.qrview.Interface", "OpenQRScanner");
		-- CS.JavaUtil.CallJavaApi("com.unity3d.player.UnityPlayer", "OpenQRScanner");
		assert(coroutine.resume(self.Qrcode()))
	end
	self.QRCode_Button.onClick:AddListener(self.Qrcode )
	self.Insertxy_Button.onClick:AddListener(self.InsertOnClick )
	self.Insert_10000_Button.onClick:AddListener(self.coroutine_insert_10000 )
	self.OpenOP_Button.onClick:AddListener(self.Op)
	self.CheckUpdate_Button.onClick:AddListener(self.CheckUpdate)
	test.tableview_Button.onClick:AddListener(function()
		assert(coroutine.resume(self.TableViewTest()))
	end)
	local ixOnEdit = function(text)
		self.iy_InputField:Select()
		print("x_InputField.onEndEdit:" .. text)
	end
	local iyOnEdit = function(text)
		print("y_InputField.onEndEdit:" .. text)
		self.InsertOnClick()
	end
	self.ix_InputField.onEndEdit:AddListener(ixOnEdit)
	self.iy_InputField.onEndEdit:AddListener(iyOnEdit)
	
	self.IpAddress_Button.onClick:AddListener(
	function()
		self.GetIpAddresses()
	end
	)
	
	local sqlpath = CS.AssetSys.CacheRoot .. "test.sqlite3"
	print(sqlpath)
	local result, db = sqlite.Open(sqlpath)
	print("test.Awake sqlite.Open", result, db)
	
	if result == sqlite.Result.OK then
		self.db = db
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

function test.OnDestroy()
	print("test.OnDestroy")
	sqlite.Close(self.db)
end

function test.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return test 