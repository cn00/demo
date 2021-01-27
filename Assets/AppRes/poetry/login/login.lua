--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/27 18:37:36
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "util"
local xutil = require "xlua.util"
require("json")

-- login

local print = function(...)
    _G.print("[login]", ...)
    -- _G.print("[login]", debug.traceback())
end

local login = {}
local this = login



--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.loginBtn_Button = loginBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.loginBtn_Button.onClick:AddListener(this.loginBtn_OnClick)
end
--AutoGenInit End

function this.loginBtn_OnClick()
    print('loginBtn_OnClick')

    CS.App.JavaUtil.Call("com.bili.a3.BSGameSdkCenter", "login")
end -- loginBtn_OnClick

function G.OnNativeMessageBLSdk(null, data)
    local callbackType = data.callbackType
    local userInfo = data.data
    local example = {
        data = { __wraped__ = "true",
                 access_token = "c417254592631ff61b76701b0b9aee98_sh",
                 expire_times = "1613475840392",
                 nickname = "Test003",
                 refresh_token = "c417254592631ff61b76701b0b9aee98_sh",
                 username = "Test003",
                 uid = "28387110", },
        callbackType = "Login",
        code = 10010,
        type = "BLSdk", }
    print(util.dump(data))
end

function login.Awake()
    this.AutoGenInit()
end

-- function login.OnEnable() end

function login.Start()
    --util.coroutine_call(this.coroutine_demo)
    local MerchantId, AppId, ServerId, AppKey = "1", "265", "506", "82a737d38acf4bc6abb903ccdbd7a562"
    CS.App.JavaUtil.Call("com.bili.a3.BSGameSdkCenter", "init", true, MerchantId, AppId, ServerId, AppKey)
end

function login.OnDestroy()
    G.OnNativeMessageBLSdk = undef
end

return login
