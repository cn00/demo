
local CS = CS
local File = CS.System.IO.File
local Directory = CS.System.IO.Directory
local AssetSys = CS.AssetSys
local UnityEngine = CS.UnityEngine
local Time = UnityEngine.Time
local Input = UnityEngine.Input
local Vector2 = UnityEngine.Vector2
local Vector3 = UnityEngine.Vector3
local Color = UnityEngine.Color


local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"
local dump = require "lua.utility.dump"

local print = function(...)
	_G.print("writeplayer/player", ...)
	-- _G.print("boot", debug.traceback())
end
local player = {}
local this = player

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)
-- function this.coroutine_demo()
--     util.coroutine_call(function()
--         print('player coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.BackBtn_Button = BackBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.mi_Image = mi:GetComponent(typeof(CS.UnityEngine.UI.Image))
    this.op_movie_VideoPlayer = op_movie:GetComponent(typeof(CS.UnityEngine.Video.VideoPlayer))
    this.playbackSpeedText_Text = playbackSpeedText:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.RawImage_RawImage = RawImage:GetComponent(typeof(CS.UnityEngine.UI.RawImage))
    this.Save_Button = Save:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.SliderV_Slider = SliderV:GetComponent(typeof(CS.UnityEngine.UI.Slider))
    this.tableview_TableViewController = tableview:GetComponent(typeof(CS.TableView.TableViewController))
    this.tableview_TableView = tableview:GetComponent(typeof(CS.TableView.TableView))
    this.ToggleEdit_Toggle = ToggleEdit:GetComponent(typeof(CS.UnityEngine.UI.Toggle))
    this.ToggleLoop_Toggle = ToggleLoop:GetComponent(typeof(CS.UnityEngine.UI.Toggle))
    this.ToggleMi_Toggle = ToggleMi:GetComponent(typeof(CS.UnityEngine.UI.Toggle))
end
--AutoGenInit End

function this.Awake()
	this.AutoGenInit()
end

-- function this.OnEnable() end

function this.Start()

end

function this.SetData(data)
	this.ToggleMi_Toggle.onValueChanged:AddListener(function(b)
		mi:SetActive(b)
	end)

	this.init(data)
end

--local sync_get_asset = util.async_to_sync(AssetSys.Instance.GetAsset)
--local get_asset = function(path)
--	return sync_get_asset(AssetSys.Instance, path)
--end
function this.init(data)
    util.coroutine_call(function()
	    AppGlobal.SceneManager.openloading()
	    
	    local videourl = data.url
		
        local cachePath = AssetSys.CacheRoot .. data.cpath
        if not File.Exists(cachePath) then
            yield_return(AssetSys.Download(videourl, cachePath))
        else
            print("use cache:", cachePath)
        end

		this.videoname = videoname
		this.timeline = AssetSys.CacheRoot .. data.cpath .. ".lua"
		local tdir = this.timeline:sub(0, this.timeline:find("/[^/]*$"))
		Directory.CreateDirectory(tdir)
		this.op_movie_VideoPlayer.url = "file://" .. cachePath

		while(not this.op_movie_VideoPlayer.isPrepared)do
			print('waiting for video prepared ...')
			yield_return(UnityEngine.WaitForSeconds(0.3))
		end
		
		this.op_movie_VideoPlayer.waitForFirstFrame = true
		--this.op_movie_VideoPlayer:Play()

		local citems = {}
		if(File.Exists(this.timeline))then
			citems = dofile(this.timeline)
			citems.old = true
			print("use cache:", this.timeline)
		end

		--assert(coroutine.resume(this.coroutine_demo()))
		local text = data.text or ""
		local wcount = 0
		-- for i in string.gmatch(text, "[%z\1-\127\194-\244][\128-\191]*") do
		for i in string.gmatch(text, "[%z\194-\244][\128-\191]*") do
			--print("拆字:", i)
			---- if i:gmatch("[%z，。；？“”]") == nil then
			--if i ~= "，"
			--and i ~= "。" 
			--and i ~= "；" 
			--and i ~= "？" 
			--and i ~= "“" 
			--and i ~= "”" 
			--then
				wcount = wcount + 1
				local btni = citems[wcount] or { frame = 0}
				btni.c = i
				citems[wcount] = btni
			--end
		end
		this.wcount = wcount
		this.DataSource = citems

	    this.InitTableViewData()
	    this.tableview_TableView:ReloadData()

	    for i,v in ipairs(this.DataSource) do
			if not this.DataSource.old then
				v.frame = math.ceil(1.0*(i-1)/wcount*this.op_movie_VideoPlayer.frameCount)
			end
			--print(i, v.c, v.frame)
		end
		
		citems[1+#citems] = { c="", frame = this.op_movie_VideoPlayer.frameCount}

		-- -- UnityEngine.VideoPlayer ？？？ GetComponent("UnityEngine.Video.VideoPlayer") 取不到？
		-- player.op_movie_VideoPlayer = op_movie:GetComponent("UnityEngine.Video.VideoPlayer") -- 不行
		-- player.op_movie_VideoPlayer = op_movie:GetComponent(typeof(UnityEngine.Video.VideoPlayer)) -- 可以

		this.playbackSpeedText_Text.text = this.op_movie_VideoPlayer.playbackSpeed
		this.SliderV_Slider.onValueChanged:AddListener(function(fval)
			--print("onValueChanged", fval, this.SliderV_Slider.value, op_movie, this.op_movie_VideoPlayer)
			this.op_movie_VideoPlayer.playbackSpeed = math.exp(this.SliderV_Slider.value) - 1
			this.playbackSpeedText_Text.text = string.format("%.2f", this.op_movie_VideoPlayer.playbackSpeed)
		end)

		this.Save_Button.onClick:AddListener(function()
			local luas = "return " .. dump(this.DataSource)
			File.WriteAllText(this.timeline, luas)
		end)

	    this.BackBtn_Button.onClick:AddListener(function()
		    AppGlobal.SceneManager.pop();
	    end)

		this.proc = this.mUpdate

	    AppGlobal.SceneManager.closeloading()
    end)
end

local function new(o)
	return o
end

function this.InitTableViewData()

	this.tableview_TableViewController.GetDataCount = function(table)
		return #this.DataSource
	end
	this.tableview_TableViewController.GetCellSize = function(table, row)
		local size = 80;
		return size
	end
	this.tableview_TableViewController.CellAtRow = function(tb, row)
		local celltypenumber = this.tableview_TableViewController.prefabCells.Length
		local cellidentifier = "cell" -- string.format("cell", 1 + (row % celltypenumber))
		--print("cellidentifier", cellidentifier, row, celltypenumber)
		local cell = tb:ReusableCellForRow(cellidentifier, row)
		cell.name = "lua-Cell-" .. (row)
		local ct = cell:GetComponent("LuaMonoBehaviour").Lua
		local cdata = this.DataSource[row + 1]
		cdata.resetui = function()
			local cell = tb:CellForRow(row)
			if cell == nil then return end
			print("----resetui", row)
			cell:GetComponent(typeof(CS.UnityEngine.UI.Image)).color = new Color(1.0, 1.0, 1.0, 1.0)
		end
		cdata.resetui()
		cdata.updateui = function()
			local cell = tb:CellForRow(row)
			if cell == nil then return end
			print("----updateui", row)
			cell:GetComponent(typeof(CS.UnityEngine.UI.Image)).color = new Color(0.6, 0.4, 0.2, 0.5)
		end
		ct.SetCellData(cdata, this.ColumnIdxA, this.ColumnPerPage) 
		ct.Text_Text.text = cdata.c
		ct.TableViewCell:DidPointClickEvent("+", function(row2)
			local v = this.DataSource[row2+1]
			if this.ToggleEdit_Toggle.isOn then
				local delta = this.op_movie_VideoPlayer.frame - v.frame
				print("delta:", delta)
				v.frame = this.op_movie_VideoPlayer.frame
				v.modified = true
				for ii = row2 + 1, #this.DataSource do
					local bii = this.DataSource[ii]
					if not bii.modified then
						bii.frame = bii.frame + delta
					end
					if bii.frame > this.op_movie_VideoPlayer.frameCount then
						bii.frame = this.op_movie_VideoPlayer.frameCount
					end
				end
			else
				-- print("op_movie_VideoPlayer.time", v.btn, v.frame, this.op_movie_VideoPlayer.frame, this.op_movie_VideoPlayer.frameCount)
				-- this.op_movie_VideoPlayer:Pause()
				this.op_movie_VideoPlayer.frame = v.frame
				this.op_movie_VideoPlayer:Play()
			end

		end)

		-- print("CellAtRow", row)
		return cell
	end

end

local function findidx(t, frame)
	local l = #t
	local a = 1
	local b = l
	local c = (a+b)//2
	while(a < c and b > c )do
		-- print("abc:", a, b, c, frame, t[c].frame)
		if     frame > t[c].frame then
			a = c
		elseif frame < t[c].frame then
			b = c
		else
			return c
		end
		c = (a+b)//2
	end
	-- print("rabc:", a, b, c, frame, t[c].frame)
	return a
end

this.currentidx = 1
function this.mUpdate()

	--if Time.frameCount % 32 == 0 then
		if this.ToggleLoop_Toggle.isOn then
			local cf = this.op_movie_VideoPlayer.frame
			local nidx = 1+this.currentidx
			if nidx <= #this.DataSource and cf > this.DataSource[nidx].frame or cf >= this.op_movie_VideoPlayer.frameCount - 1 then
				this.op_movie_VideoPlayer.frame = this.DataSource[this.currentidx].frame
			end
		end
		local lastidx = this.currentidx
		this.currentidx = findidx(this.DataSource, this.op_movie_VideoPlayer.frame)
		if(lastidx ~= this.currentidx)then
			print("current:", this.DataSource[this.currentidx].c)
			local last = this.DataSource[lastidx]
			if last ~= nil then last.resetui();end
			local current = this.DataSource[this.currentidx];
			if current ~= nil then current.updateui();end
			--this.DataSource[lastidx].btn:GetComponent(typeof(CS.UnityEngine.UI.Image)).color = Color(0, 0, 0, 0)
			--this.DataSource[this.currentidx].btn:GetComponent(typeof(CS.UnityEngine.UI.Image)).color = Color(0.6, 0.4, 0.2, 0.5)
		end
	--end
end
function this.Update()
	if this.proc then
		this.proc()
	end
end

function this.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return player
