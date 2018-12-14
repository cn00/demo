-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "utility.xlua.util"

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
    qrcode.Decode_QRCodeDecodeController = Decode:GetComponent("QRCodeDecodeController")
    qrcode.Back_Button = Back:GetComponent("UnityEngine.UI.Button")
    qrcode.Encode_QRCodeEncodeController = Encode:GetComponent("QRCodeEncodeController")
    qrcode.EncodeImage_RectTransform = EncodeImage:GetComponent("UnityEngine.RectTransform")
    qrcode.EncodeImage_RawImage = EncodeImage:GetComponent("UnityEngine.UI.RawImage")
    qrcode.DecodeResult_Text = DecodeResult:GetComponent("UnityEngine.UI.Text")
end
--AutoGenInit End

function qrcode.Awake()
    self.AutoGenInit()

    self.Decode_QRCodeDecodeController:onQRScanFinished('+', self.OnScanResult)
    self.Encode_QRCodeEncodeController:onQREncodeFinished('+', function(tex)
        self.EncodeImage_RawImage.texture = tex
    end)

	self.Back_Button.onClick:AddListener(
		function()
			assert(coroutine.resume(self.Back()))
		end
	)
end

function qrcode.AddDecodeCallback(fun)
    self.Decode_QRCodeDecodeController:onQRScanFinished('+', fun)
end

function qrcode.RmDecodeCallback(fun)
    self.Decode_QRCodeDecodeController:onQRScanFinished('-', fun)
end

function qrcode.AddEncodeCallback(fun)
    self.Encode_QRCodeEncodeController:onQREncodeFinished('+', fun)
end

function qrcode.RmEncodeCallback(fun)
    self.Encode_QRCodeEncodeController:onQREncodeFinished('-', fun)
end

function qrcode.OnEnable()
    print("qrcode.OnEnable")
end

function qrcode.Start()
    print("qrcode.Start")

    self.Decode_QRCodeDecodeController:StartWork()

    --assert(coroutine.resume(qrcode.coroutine_demo()))
    self.OnScanResult("http://cn.test.qrcode")

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
    self.DecodeResult_Text.text = strResult
    if #strResult > 7 then -- http://
        -- assert(coroutine.resume(self.Back()))
        self.Encode_QRCodeEncodeController:Encode(strResult)

    end
    self.Decode_QRCodeDecodeController:StartWork()
end


function qrcode.Back()
    return coroutine.create(function()
        yield_return(CS.UnityEngine.WaitForSeconds(0.3))
        local obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset("ui/test/test.prefab", function(asset)
            obj = asset
        end))
        local gameObj = GameObject.Instantiate(obj)

	    GameObject.DestroyImmediate(mono.gameObject)
    end)
end

return qrcode
