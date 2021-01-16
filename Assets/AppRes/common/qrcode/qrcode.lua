-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local xutil = require "xlua.util"

local qrcode = {
    info = {}, --
}
local this = qrcode

local yield_return = xutil.async_to_sync(function (to_yield, callback)
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
    this.testBtn_Button = testBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.testBtn_Button.onClick:AddListener(this.testBtn_OnClick)
end
--AutoGenInit End

function this.testBtn_OnClick()
    local strResult = this.DecodeResult_Text.text
    this.OnScanResult(strResult)
    this.Encode_QRCodeEncodeController:Encode(strResult)
end -- testBtn_OnClick

function qrcode.Awake()
    this.AutoGenInit()

    this.Decode_QRCodeDecodeController:onQRScanFinished('+', this.OnScanResult)
    --this.Encode_QRCodeEncodeController:onQREncodeFinished('+', function(tex)
    --    this.EncodeImage_RawImage.texture = tex
    --end)

end

function qrcode.init(info)
    this.info = info
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
end

function qrcode.OnScanResult(strResult)
    print("ScanResult: " .. strResult)
    this.DecodeResult_Text.text = strResult
    local head = strResult:sub(1, 7)
    if head == "a3mkgp:" then
        -- callback
        if type(this.info.scanCallback) == "function" then 
            this.info.scanCallback(strResult)
            GameObject.DestroyImmediate(mono.gameObject)
        end
    else
        -- not a valied message, try again
        this.Decode_QRCodeDecodeController:StartWork()
    end
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
