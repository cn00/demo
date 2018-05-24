-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

local qrcode = {}
local self = qrcode

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

-- function qrcode.coroutine_demo()
--     return coroutine.create(function()
--         print('qrcode coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
function qrcode.AutoGenInit()
    qrcode.DeviceCamera_DeviceCameraController = DeviceCamera:GetComponent("DeviceCameraController")
    qrcode.QRController_QRCodeDecodeController = QRController:GetComponent("QRCodeDecodeController")
    qrcode.Back_Button = Back:GetComponent("UnityEngine.UI.Button")
end
--AutoGenInit End

function qrcode.Awake()
    self.AutoGenInit()

    self.QRController_QRCodeDecodeController:onQRScanFinished('+', self.OnScanResult)

	self.Back_Button.onClick:AddListener(
		function()
			assert(coroutine.resume(self.Back()))
		end
	)
end

function qrcode.OnEnable()
    print("qrcode.OnEnable")
end

function qrcode.Start()
    print("qrcode.Start")

    self.QRController_QRCodeDecodeController:StartWork()

    --assert(coroutine.resume(qrcode.coroutine_demo()))

end

function qrcode.FixedUpdate()

end

function qrcode.Update()

end

function qrcode.LateUpdate()

end

function qrcode.OnDestroy()
    print("qrcode.OnDestroy")

end

function qrcode.OnScanResult(strResult)
    print("ScanResult: " .. strResult)
    if #strResult > 7 then -- http://
        assert(coroutine.resume(self.Back()))
    end
end


function qrcode.Back()
    return coroutine.create(function()
        yield_return(UnityEngine.WaitForSeconds(1))
        local obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset("ui/test/test.prefab", function(asset)
            obj = asset
        end))
        local gameObj = GameObject.Instantiate(obj)

	    GameObject.DestroyImmediate(mono.gameObject)
    end)
end

return qrcode
