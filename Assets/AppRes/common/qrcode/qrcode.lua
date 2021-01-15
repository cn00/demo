-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "xlua.util"

local qrcode = {}
local this = qrcode

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.Decode_QRCodeDecodeController = Decode:GetComponent(typeof(CS.QRCodeDecodeController))
    this.DecodeResult_Text = DecodeResult:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.DeviceCamera_DeviceCameraController = DeviceCamera:GetComponent(typeof(CS.DeviceCameraController))
    this.Encode_QRCodeEncodeController = Encode:GetComponent(typeof(CS.QRCodeEncodeController))
    this.EncodeImage_RawImage = EncodeImage:GetComponent(typeof(CS.UnityEngine.UI.RawImage))
    this.EncodeImage_RectTransform = EncodeImage:GetComponent(typeof(CS.UnityEngine.RectTransform))
end
--AutoGenInit End

function qrcode.Awake()
    this.AutoGenInit()

    this.Decode_QRCodeDecodeController:onQRScanFinished('+', this.OnScanResult)
    this.Encode_QRCodeEncodeController:onQREncodeFinished('+', function(tex)
        this.EncodeImage_RawImage.texture = tex
    end)

	--this.Back_Button.onClick:AddListener(
	--	function()
	--		assert(coroutine.resume(this.Back()))
	--	end
	--)
end

function qrcode.AddDecodeCallback(fun)
    this.Decode_QRCodeDecodeController:onQRScanFinished('+', fun)
end

function qrcode.RmDecodeCallback(fun)
    this.Decode_QRCodeDecodeController:onQRScanFinished('-', fun)
end

function qrcode.AddEncodeCallback(fun)
    this.Encode_QRCodeEncodeController:onQREncodeFinished('+', fun)
end

function qrcode.RmEncodeCallback(fun)
    this.Encode_QRCodeEncodeController:onQREncodeFinished('-', fun)
end

function qrcode.OnEnable()
    print("qrcode.OnEnable")
end

function qrcode.Start()
    print("qrcode.Start")

    this.Decode_QRCodeDecodeController:StartWork()

    --assert(coroutine.resume(qrcode.coroutine_demo()))
    this.OnScanResult("http://cn.test.qrcode")

end

function qrcode.OnScanResult(strResult)
    print("ScanResult: " .. strResult)
    this.DecodeResult_Text.text = strResult
    if #strResult > 7 then -- http://
        -- assert(coroutine.resume(this.Back()))
        this.Encode_QRCodeEncodeController:Encode(strResult)

    end
    this.Decode_QRCodeDecodeController:StartWork()
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
