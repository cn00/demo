
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local AssetSys = CS.AssetSys

local util = require "xlua.util"

local update = {
	process = 0,
}
local self = update

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

-- update.coroutine_demo = coroutine.create(function()
--     print('update coroutine start!')
--     yield_return(CS.UnityEngine.WaitForSeconds(2))
--     yield_return(CS.AssetSys.GetAsset("ui/login/login.prefab", function(asset)
--         obj = asset
--     end))
-- end

--AutoGenInit Begin
function update.AutoGenInit()
    update.VersionText_Text = VersionText:GetComponent("UnityEngine.UI.Text")
    update.CheckUpdate_Button = CheckUpdate:GetComponent("UnityEngine.UI.Button")
    update.Clean_Button = Clean:GetComponent("UnityEngine.UI.Button")
    update.Slider_Slider = Slider:GetComponent("UnityEngine.UI.Slider")
    update.Back_Button = Back:GetComponent("UnityEngine.UI.Button")
end
--AutoGenInit End

function update.Awake()
	self.AutoGenInit()

	self.CheckUpdate_Button.onClick:AddListener(function()
		print("CheckUpdate")
		assert(coroutine.resume(self.CheckUpdate()))
	end)

	self.Clean_Button.onClick:AddListener(function()
		print("Clean")
		assert(coroutine.resume(self.Clean()))
	end)

	self.Back_Button.onClick:AddListener(function()
		print("Back_Button")
		self.Back()
	end)

	self.VersionText_Text.text = CS.BuildConfig.Instance().Version:ToString()
	self.Slider_Slider.value = 0
end

function update.OnEnable ()
    print("update.OnEnable")

end

function update.Start ()
    print("update.Start")
    --assert(coroutine.resume(update.coroutine_demo))

end

function update.FixedUpdate ()
end

function update.Update ()
	self.process = self.Slider_Slider.value
end

function update.LateUpdate ()
	self.process = self.process + 0.001
    self.Slider_Slider.value = self.process
end

function update.OnDestroy ()
    print("update.OnDestroy")

end

----------------------------------

function update.CheckUpdate()
	return coroutine.create(function()

	    local obj = nil

	    
		yield_return(CS.UpdateSys.Instance:CheckUpdate())
	    
		update.name = "update"
		
		self.Back()
	end)
end

function update.Clean()
	return coroutine.create(
		function()
			print("Delete: " .. CS.AssetSys.CacheRoot)
			CS.Directory.Delete(CS.AssetSys.CacheRoot, true);
		end
	)
end


function update.Back()
    assert(coroutine.resume(coroutine.create(function()
        yield_return(UnityEngine.WaitForSeconds(0.3))
        local obj = nil
        yield_return(CS.AssetSys.GetAsset("ui/test/test.prefab", function(asset)
            obj = asset
        end))
        local gameObj = GameObject.Instantiate(obj)

	    GameObject.DestroyImmediate(mono.gameObject)
    end)))
end

return update
