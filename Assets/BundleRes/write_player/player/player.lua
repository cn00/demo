
local CS = CS
local AssetSys = CS.AssetSys
local UnityEngine = CS.UnityEngine
local Input = UnityEngine.Input
local Vector2 = UnityEngine.Vector2
local Vector3 = UnityEngine.Vector3

local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

local player = {}
local this = player

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)
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
end
--AutoGenInit End

function this.Awake()
	this.AutoGenInit()
end

-- function this.OnEnable() end

function this.Start()
	--assert(coroutine.resume(this.coroutine_demo()))
	local text = '豫章故郡，洪都新府。星分翼轸，地接衡庐。襟三江而带五湖，控蛮荆而引瓯越。物华天宝，龙光射牛斗之墟；人杰地灵，徐孺下陈蕃之榻。雄州雾列，俊采星驰。台隍枕夷夏之交，宾主尽东南之美。都督阎公之雅望，棨戟遥临；宇文新州之懿范，襜帷暂驻。十旬休假，胜友如云；千里逢迎，高朋满座。腾蛟起凤，孟学士之词宗；紫电青霜，王将军之武库。家君作宰，路出名区；童子何知，躬逢胜饯。'
	local count = 0
	local wcount = 0
	local btns = {}
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
			btns[1+#btns] = {
				c = i,
				frame = 0,
				btn = item:GetComponent(typeof(CS.UnityEngine.UI.Button))
			}
		end
	end
	this.count = count
	this.wcount = wcount
	this.btns = btns

	-- -- this is ok
	-- local clip = UnityEngine.Resources.Load("1-滕王阁序-960")
	-- print("Resources.Load(\"1-滕王阁序-960.mp4\"", clip)
	-- this.op_movie_VideoPlayer.clip = clip
	local videoname = "1-滕王阁序-640.mp4"
	local cachePath = AssetSys.CacheRoot .. videoname
	this.op_movie_VideoPlayer.url = "file://" .. cachePath

	this.op_movie_VideoPlayer:Play()

	for i,v in ipairs(btns) do
		local frame = math.ceil(1.0*(i-1)/wcount*this.op_movie_VideoPlayer.frameCount)
		v.frame = frame

		v.btn.onClick:AddListener(function()
			if this.ToggleEdit_Toggle.isOn then
				v.frame = this.op_movie_VideoPlayer.frame
			else
				print("op_movie_VideoPlayer.time", v.btn, frame, this.op_movie_VideoPlayer.frame, this.op_movie_VideoPlayer.frameCount)
				-- this.op_movie_VideoPlayer:Pause()
				-- this.op_movie_VideoPlayer:Play()
				this.op_movie_VideoPlayer.frame = frame
			end
		end)
	end

	-- RectTransform 是 transform 的别名
	Content.transform.sizeDelta = UnityEngine.Vector2(count * 67, 60)

	-- -- UnityEngine.VideoPlayer ？？？ GetComponent("UnityEngine.Video.VideoPlayer") 取不到？
    -- player.op_movie_VideoPlayer = op_movie:GetComponent("UnityEngine.Video.VideoPlayer") -- 不行
    -- player.op_movie_VideoPlayer = op_movie:GetComponent(typeof(UnityEngine.Video.VideoPlayer)) -- 可以

	print("this.op_movie_VideoPlayer", op_movie, this.op_movie_VideoPlayer)

	this.playbackSpeedText_Text.text = this.op_movie_VideoPlayer.playbackSpeed
    this.Slider_Slider.onValueChanged:AddListener(function(fval)
        print("onValueChanged", fval, this.Slider_Slider.value, op_movie, this.op_movie_VideoPlayer)
		this.op_movie_VideoPlayer.playbackSpeed = math.exp(this.Slider_Slider.value) - 1
		this.playbackSpeedText_Text.text = string.format("%.2f", this.op_movie_VideoPlayer.playbackSpeed)
    end)
end

-- function this.FixedUpdate() end

-- function this.LateUpdate() end

-- function this.OnDestroy() end

function this.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return player
