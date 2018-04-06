local util = require "lua.utility.xlua.util"

local update = {
	process = 0,
}
local self = update

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)

-- update.coroutine_demo = coroutine.create(function()
--     print('update coroutine start!')
--     yield_return(CS.UnityEngine.WaitForSeconds(2))
--     yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--         obj = asset
--     end))
-- end

--AutoGenInit Begin
function update.AutoGenInit()
    update.VersionText = VersionText:GetComponent("UnityEngine.UI.Text")
    update.Button = Button:GetComponent("UnityEngine.UI.Button")
    update.Slider = Slider:GetComponent("UnityEngine.UI.Slider")
    update.ButtonClean = ButtonClean:GetComponent("UnityEngine.UI.Button")
end
--AutoGenInit End

function update.Awake()
	update.AutoGenInit()

	update.Button.onClick:AddListener(function()
		print("clicked, you input is '" .. InputField:GetComponent("InputField").text .."'")
		assert(coroutine.resume(Login.CheckUpdate()))
	end)

	update.VersionText.text = CS.BundleConfig.Instance().Version:ToString()
	update.Slider = Slider:GetComponent("Slider")
	update.Slider.value = 0
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
    update.Slider.value = update.Slider.value + 0.001
end

function update.LateUpdate ()

end

function update.OnDestroy ()
    print("update.OnDestroy")

end
    
return update
