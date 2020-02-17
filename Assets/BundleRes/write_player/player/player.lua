
local CS = CS
local File = CS.System.IO.File
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
--         yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.Slider_Slider = Slider:GetComponent(typeof(CS.UnityEngine.UI.Slider))
    this.op_movie_VideoPlayer = op_movie:GetComponent(typeof(CS.UnityEngine.Video.VideoPlayer))
    this.sv_item_tem_Button = sv_item_tem:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.Content_HorizontalLayoutGroup = Content:GetComponent(typeof(CS.UnityEngine.UI.HorizontalLayoutGroup))
    this.ToggleEdit_Toggle = ToggleEdit:GetComponent(typeof(CS.UnityEngine.UI.Toggle))
    this.playbackSpeedText_Text = playbackSpeedText:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.RawImage_RawImage = RawImage:GetComponent(typeof(CS.UnityEngine.UI.RawImage))
    this.Save_Button = Save:GetComponent(typeof(CS.UnityEngine.UI.Button))
end
--AutoGenInit End

function this.Awake()
	this.AutoGenInit()
end

-- function this.OnEnable() end

function this.Start()

	this.videoname = "1-滕王阁序-640.mp4"
	this.timeline = AssetSys.CacheRoot .. this.videoname .. ".lua"
	local cachePath = AssetSys.CacheRoot .. this.videoname
	this.op_movie_VideoPlayer.url = "file://" .. cachePath

	local btns = {}
	if(File.Exists(this.timeline))then
		btns = dofile(this.timeline)
		btns.old = true
	end

	--assert(coroutine.resume(this.coroutine_demo()))
	local text = '豫章故郡，洪都新府。星分翼轸，地接衡庐。襟三江而带五湖，控蛮荆而引瓯越。物华天宝，龙光射牛斗之墟；人杰地灵，徐孺下陈蕃之榻。雄州雾列，俊采星驰。台隍枕夷夏之交，宾主尽东南之美。都督阎公之雅望，棨戟遥临；宇文新州之懿范，襜帷暂驻。十旬休假，胜友如云；千里逢迎，高朋满座。腾蛟起凤，孟学士之词宗；紫电青霜，王将军之武库。家君作宰，路出名区；童子何知，躬逢胜饯。'
	local count = 0
	local wcount = 0
	-- for i in string.gmatch(text, "[%z\1-\127\194-\244][\128-\191]*") do
	for i in string.gmatch(text, "[%z\194-\244][\128-\191]*") do
		-- print(i)
		local item = GameObject.Instantiate(sv_item_tem, Content.transform)
		item:SetActive(true)
		local t = item.transform:Find("Text"):GetComponent("UnityEngine.UI.Text")
		t.text = i
		item.name = i
		count = count + 1
		-- if i:gmatch("[%z，。；？“”]") == nil then
		if i ~= "，"
		and i ~= "。" 
		and i ~= "；" 
		and i ~= "？" 
		and i ~= "“" 
		and i ~= "”" 
		then
			wcount = wcount + 1
			btns[wcount] = btns[wcount] or {}
			local btni = btns[wcount] or {}
			btni.c = i
			btni.btn = item:GetComponent(typeof(CS.UnityEngine.UI.Button))
			
		end
	end
	this.count = count
	this.wcount = wcount
	this.btns = btns

	this.init()

end

function this.init()
    util.coroutine_call(function()
		while(not this.op_movie_VideoPlayer.isPrepared)do
			print('waiting for video prepared ...')
			yield_return(UnityEngine.WaitForSeconds(0.3))
		end
        this.op_movie_VideoPlayer:Play()

		local wcount = this.wcount
		for i,v in ipairs(this.btns) do
			if not this.btns.old then
				v.frame = math.ceil(1.0*(i-1)/wcount*this.op_movie_VideoPlayer.frameCount)
			end
			v.btn.onClick:AddListener(function()
				if this.ToggleEdit_Toggle.isOn then
					v.frame = this.op_movie_VideoPlayer.frame
					v.modified = true
				else
					-- print("op_movie_VideoPlayer.time", v.btn, v.frame, this.op_movie_VideoPlayer.frame, this.op_movie_VideoPlayer.frameCount)
					-- this.op_movie_VideoPlayer:Pause()
					-- this.op_movie_VideoPlayer:Play()
					this.op_movie_VideoPlayer.frame = v.frame
				end
			end)
		end

		-- RectTransform 是 transform 的别名
		Content.transform.sizeDelta = UnityEngine.Vector2(this.count * 67, 60)

		-- -- UnityEngine.VideoPlayer ？？？ GetComponent("UnityEngine.Video.VideoPlayer") 取不到？
		-- player.op_movie_VideoPlayer = op_movie:GetComponent("UnityEngine.Video.VideoPlayer") -- 不行
		-- player.op_movie_VideoPlayer = op_movie:GetComponent(typeof(UnityEngine.Video.VideoPlayer)) -- 可以

		this.playbackSpeedText_Text.text = this.op_movie_VideoPlayer.playbackSpeed
		this.Slider_Slider.onValueChanged:AddListener(function(fval)
			print("onValueChanged", fval, this.Slider_Slider.value, op_movie, this.op_movie_VideoPlayer)
			this.op_movie_VideoPlayer.playbackSpeed = math.exp(this.Slider_Slider.value) - 1
			this.playbackSpeedText_Text.text = string.format("%.2f", this.op_movie_VideoPlayer.playbackSpeed)
		end)

		this.Save_Button.onClick:AddListener(function()
			local luas = "return " .. dump(this.btns)
			File.WriteAllText(this.timeline, luas)
		end)
    end)
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
	print("rabc:", a, b, c, frame, t[c].frame)
	return a
end

this.currentidx = 1
function this.Update()
	if Time.frameCount % 128 == 0 then
		local lastidx = this.currentidx
		this.currentidx = findidx(this.btns, this.op_movie_VideoPlayer.frame)
		if(lastidx ~= this.currentidx)then
			print("current:", this.btns[this.currentidx].c)
			this.btns[lastidx].btn:GetComponent(typeof(CS.UnityEngine.UI.Image)).color = Color.white
			this.btns[this.currentidx].btn:GetComponent(typeof(CS.UnityEngine.UI.Image)).color = Color.red
		end
	end
end


-- function this.FixedUpdate() end

-- function this.LateUpdate() end

-- function this.OnDestroy() end

function this.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return player
