local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local AssetSys = CS.AssetSys
local sqlite = CS.SQLite.SQLite3

local util = require "lua.utility.xlua.util"

local test = {}
local self = test

local yield_return =
	util.async_to_sync(
	function(to_yield, callback)
		mono:YieldAndCallback(to_yield, callback)
	end
)

function test.CheckUpdate()
	return coroutine.create(
		function()
			local obj = nil
			yield_return(
				CS.AssetSys.Instance:GetAsset(
					"ui/loading/loading.prefab",
					function(asset)
						obj = asset
					end
				)
			)
			local loading = GameObject.Instantiate(obj)
			loading.name = "loading"
			local oldLoading = GameObject.Find("loading")
			GameObject.DestroyImmediate(oldLoading)

			obj = nil
			yield_return(
				CS.AssetSys.Instance:GetAsset(
					"ui/update/update.prefab",
					function(asset)
						obj = asset
					end
				)
			)
			local update = GameObject.Instantiate(obj)
			update.name = "update"

			self.Destroy()
		end
	)
end

function test.Op(...)
	return coroutine.create(
		function()
			print("open Op")
			local obj = nil
			yield_return(
				CS.AssetSys.Instance:GetAsset(
					"video/op/op.prefab",
					function(asset)
						obj = asset
					end
				)
			)
			local loading = GameObject.Instantiate(obj)
			loading.name = "op"
			local oldLoading = GameObject.Find("loading")
			GameObject.DestroyImmediate(oldLoading)

			self.Destroy()
		end
	)
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
		-- local rowsAffected = sqlite.Changes(db)
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
	ScreenCapture.CaptureScreenshot(screenshoot, 1)
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
	return coroutine.create(
		function()
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
				-- local rowsAffected = sqlite.Changes(db)
				-- print("rowsAffected", rowsAffected)
			elseif result2 == sqlite.Result.Busy then
				print("db busy")
			end
			print()

			self.Insert_10000_Button.interactable = true
			local lastid = sqlite.LastInsertRowid(db)
			print(result2, "lastid" .. lastid, "coast:" .. os.time() - t)
		end
	)
end

--AutoGenInit Begin
function test.AutoGenInit()
	test.Insert_10000_Button = Insert_10000:GetComponent("UnityEngine.UI.Button")
	test.ix_InputField = ix:GetComponent("UnityEngine.UI.InputField")
	test.iy_InputField = iy:GetComponent("UnityEngine.UI.InputField")
	test.Insertxy_Button = Insertxy:GetComponent("UnityEngine.UI.Button")
	test.OpenOP_Button = OpenOP:GetComponent("UnityEngine.UI.Button")
	test.CheckUpdate_Button = CheckUpdate:GetComponent("UnityEngine.UI.Button")
end
--AutoGenInit End

function test.Awake()
	self.AutoGenInit()

	self.Insertxy_Button.onClick:AddListener(
		function()
			self.InsertOnClick()
		end
	)

	self.Insert_10000_Button.onClick:AddListener(
		function()
			self.Insert_10000_Button.interactable = false
			assert(coroutine.resume(test.coroutine_insert_10000()))
		end
	)

	self.OpenOP_Button.onClick:AddListener(
		function()
			assert(coroutine.resume(self.Op()))
		end
	)

	self.CheckUpdate_Button.onClick:AddListener(
		function()
			assert(coroutine.resume(self.CheckUpdate()))
		end
	)

	self.ix_InputField.onEndEdit:AddListener(
		function(text)
			self.iy_InputField:Select()
			print("x_InputField.onEndEdit:" .. text)
		end
	)
	self.iy_InputField.onEndEdit:AddListener(
		function(text)
			print("y_InputField.onEndEdit:" .. text)
			self.InsertOnClick()
		end
	)

	local sqlpath = CS.AssetSys.CacheRoot .. "test.sqlite3"
	print(sqlpath)
	local result, db = sqlite.Open(sqlpath)
	print("test.Awake sqlite.Open", result, db)

	if result == sqlite.Result.OK then
		self.db = db
		local sql =
			"create table if not exists `test_map` (\n" ..
			"`id` integer primary key autoincrement not null,\n" .. "`x` integer not null,\n" .. "`y` integer not null\n" .. ")"
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

function test.OnEnable()
	print("test.OnEnable")
end

function test.Start()
	print("test.Start")
end

function test.FixedUpdate()
end

function test.Update()
end

function test.LateUpdate()
end

function test.OnDestroy()
	print("test.OnDestroy")
	sqlite.Close(self.db)
end

function test.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return test
