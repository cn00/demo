

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local AssetSys = CS.AssetSys
local sqlite = CS.SQLite.SQLite3

local util = require "lua.utility.xlua.util"

local login = {}
local self = login

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)


function login.CheckUpdate()
	return coroutine.create(function()
		yield_return(CS.UpdateSys.Instance:CheckUpdate())

	    local obj = nil
		yield_return(CS.AssetSys.Instance:GetAsset("ui/loading/loading.prefab", function(asset)
	        obj = asset;
	    end))
	    local loading = CS.UnityEngine.GameObject.Instantiate(obj)
	    loading.name = "loading"
	    local oldLoading = CS.UnityEngine.GameObject.Find("loading")
	    CS.UnityEngine.GameObject.DestroyImmediate(oldLoading)
	    

	    obj = nil
	    yield_return(CS.AssetSys.Instance:GetAsset("ui/update/update.prefab", function(asset)
	        obj = asset;
	    end))
	    local update = CS.UnityEngine.GameObject.Instantiate(obj)
	    update.name = "update"

	end)
end

function login.SqliteInsert(x, y)
	local db = self.db
	local sql = "INSERT INTO `test_map` (x,y) VALUES "
			.. "(".. x ..",".. y ..");"
			.. "COMMIT;"
	local stmt = sqlite.Prepare2(db, sql)
	local result2 = sqlite.Step(stmt)
	local result3 = sqlite.Finalize(stmt)
	if result2 == sqlite.Result.Error then
		local errmsg = sqlite.GetErrmsg(db)
		print(errmsg)
	elseif result2 == sqlite.Result.Done then
		-- local rowsAffected = sqlite.Changes(db)
		-- print("rowsAffected", rowsAffected)
	elseif result2== sqlite.Result.Busy then
		print("db busy")
	end
	-- local lastid = sqlite.LastInsertRowid(db)
	-- print(sql, result2, result3, lastid)
end

function login.SqliteSelect()
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
		collect[count] = table.concat({tostring(result2), c0, c1, c2}, ',')

		result2 = sqlite.Step(stmt)
	end
	sqlite.Finalize(stmt)
	print(table.concat(collect, "\n"))
end

function login.CaptureScreenshot()
	local screenshoot = CS.AssetSys.CacheRoot .. "Screenshot.lua.png"
    CS.UnityEngine.ScreenCapture.CaptureScreenshot(screenshoot, 1);
end

--AutoGenInit Begin
function login.AutoGenInit()
    login.Button = Button:GetComponent("UnityEngine.UI.Button")
    login.InputField = InputField:GetComponent("UnityEngine.UI.InputField")
    login.InputField_1 = InputField_1:GetComponent("UnityEngine.UI.InputField")
end
--AutoGenInit End

function login.ButtonOnClick()
	local input0 = self.InputField.text
	local input1 = self.InputField_1.text

	print("clicked, you input is [" .. InputField:GetComponent("InputField").text .."]")
	-- assert(coroutine.resume(login.CheckUpdate()))
	self.SqliteInsert(input0, input1)
end

function login.coroutine_insert()
	return coroutine.create(function()
	    print('coroutine_insert coroutine start!')
	    local t = os.time()
		-- insert 10000 records coast 83 second
		local values = {}
		for i = 1, 10000 do
			-- self.SqliteInsert(math.random(1,100000), math.random(20000,100000))
			values[i] = "(" .. math.random(1,100000)..",".. math.random(20000,100000) .. ")"
			-- print("idx:",i)
			-- yield_return(CS.UnityEngine.WaitForSeconds(0))
		end
		local sql = "INSERT INTO `test_map` (x,y) VALUES "
				.. table.concat(values, ",") .. ";"
				.. "COMMIT;"
		local db = self.db
		local stmt = sqlite.Prepare2(db, sql)
		local result2 = sqlite.Step(stmt)
		local result3 = sqlite.Finalize(stmt)
		if result2 == sqlite.Result.Error then
			local errmsg = sqlite.GetErrmsg(db)
			print(errmsg)
		elseif result2 == sqlite.Result.Done then
			-- local rowsAffected = sqlite.Changes(db)
			-- print("rowsAffected", rowsAffected)
		elseif result2== sqlite.Result.Busy then
			print("db busy")
		end
		print("coast: ", os.time()-t)

		self.Button.interactable = true
		local lastid = sqlite.LastInsertRowid(db)
		print(result2, result3, lastid, #sql)
	end)
end

function login.Awake()
	self.AutoGenInit()
	self.Button.onClick:AddListener(function (  )
		-- self.ButtonOnClick()
		self.Button.interactable = false
		assert(coroutine.resume(login.coroutine_insert()))
	end)

	self.InputField.onEndEdit:AddListener(function ( text )
		self.InputField_1:Select()
		print("InputField.onEndEdit:"..text)
	end)
	self.InputField_1.onEndEdit:AddListener(function ( text )
		print("InputField_1.onEndEdit:"..text)
		self.ButtonOnClick()
	end)

	-- self.InputField_1.onValidateInput=function ( text, charIndex, addedChar )
	-- 	print("InputField_1.onValidateInput:", text, charIndex, addedChar)
	-- 	-- self.ButtonOnClick()
	-- end

	local sqlpath = CS.AssetSys.CacheRoot .. "test.sqlite3"
	print(sqlpath)
	local result, db = sqlite.Open(sqlpath)
	print("login.Awake sqlite.Open", result, db)

	if result == sqlite.Result.OK then
		self.db = db
		local sql = "create table if not exists `test_map` (\n"
				 .. "`id` integer primary key autoincrement not null,\n"
				 .. "`x` integer not null,\n"
				 .. "`y` integer not null\n"
				 .. ")"
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

function login.OnEnable()
    print("login.OnEnable")

end

function login.Start()
    print("login.Start")
end

function login.FixedUpdate()

end

function login.Update()

end

function login.LateUpdate()

end

function login.OnDestroy()
    print("login.OnDestroy")
	sqlite.Close(self.db)
end
    
return login
