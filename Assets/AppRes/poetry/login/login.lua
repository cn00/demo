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
local manager = AppGlobal.manager


local yield_return = xutil.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

-- login

local print = function(...)
    _G.print("poetry/login", ...)
    -- _G.print("[login]", debug.traceback())
end

local login = {
    dbOK = false,
}
local this = login



--AutoGenInit Begin
--[[
请勿手动编辑此函数
手動でこの関数を編集しないでください。
DO NOT EDIT THIS FUNCTION MANUALLY.
لا يدويا تحرير هذه الوظيفة
]]
function this.AutoGenInit()
    this.InputField_InputField = InputField:GetComponent(typeof(CS.UnityEngine.UI.InputField))
    this.loginBtn_Button = loginBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.loginBtn_Button.onClick:AddListener(this.loginBtn_OnClick)
end
--AutoGenInit End

local function junpToIndex()
    local config = require("common.config.config")
    local dburl = "db.db"
    local cachePath = config.dbCachePath -- AssetSys.CacheRoot .. "db.db"
    local fi =  CS.System.IO.FileInfo(cachePath);
    local loadingtasks = {}
    if not fi.Exists or fi.Length < 512  then
        print("download db.db ...")
        table.insert(loadingtasks, CS.AssetSys.Download(dburl, cachePath, function()
            this.dbOK = true
            print("download db.db ok")
        end) )
    else
        print("use cache:", cachePath)
    end
    AppGlobal.SceneManager.push("poetry/index/index.prefab", {
        loadingtasks = loadingtasks
    }, true)
end

function this.loginBtn_OnClick()
    print('loginBtn_OnClick')

    --local jo = this.BSGameSdkCenter
    --local channel = jo:CallStatic("channel") -- return void
    --jo:CallStatic("login") -- return void
    print(string.format("macro and:%s ios:%s editor:%s osx:%s win:%s"
    , UNITY_ANDROID, UNITY_IOS, UNITY_EDITOR, UNITY_EDITOR_OSX, UNITY_EDITOR_WIN))

    if UNITY_EDITOR  then
        junpToIndex()
    elseif UNITY_ANDROID then
        CS.App.JavaUtil.CallStaticVoid("com.bili.a3.BSGameSdkCenter", "login")
        local channel = CS.App.JavaUtil.CallStatic("com.bili.a3.BSGameSdkCenter", "channel")
        print("channel", channel)
    --elseif UNITY_IOS then
    --elseif UNITY_EDITOR then -- uid input
    else
        junpToIndex()
    end
end -- loginBtn_OnClick

function G.OnNativeMessageBLSdk(null, data)
    local callbackType = data.callbackType
    local args = data.data
    print(util.dump(data))
    local actions = {
        ["Login"] = function(argt)
            --[[ argt = {
                type = "BLSdk",
                code = 10010,
                callbackType = "Login",
                data = {
                    access_token = "c417254592631ff61b76701b0b9aee98_sh",
                    username = "Test003",
                    expire_times = "1613475840392", -- ms
                    refresh_token = "c417254592631ff61b76701b0b9aee98_sh",
                    nickname = "Test003",
                    uid = "28387110",
                    result = 1,
                }
            }]]
            junpToIndex()
        end,
        ["Logout"] = function(argt)  end
    }
    if actions[callbackType] then actions[callbackType](args) end
end

function login.Awake()
    this.AutoGenInit()
end

-- function login.OnEnable() end

function login.Start()
    --util.coroutine_call(this.coroutine_demo)
    this.InputField_InputField.onEndEdit:AddListener(function(text)
        print("InputField_InputField.onEndEdit:" .. text)
        AppGlobal.USER_ID = text

        self.url_InputField:Select()
    end)

    --xutil.coroutine_call(function()
    --    local config = require("common.config.config")
    --    local dburl = "db.db"
    --    local cachePath = config.dbCachePath -- AssetSys.CacheRoot .. "db.db"
    --    local fi =  CS.System.IO.FileInfo(cachePath);
    --    if not fi.Exists or fi.Length < 512  then
    --        print("download db.db ...")
    --        yield_return(CS.AssetSys.Download(dburl, cachePath))
    --    else
    --        print("use cache:", cachePath)
    --    end
    --    this.dbOK = true
    --end)

    -- init sdk
    if UNITY_EDITOR then -- uid input

    elseif UNITY_ANDROID then
        local MerchantId, AppId, ServerId, AppKey = "1", "265", "506", "82a737d38acf4bc6abb903ccdbd7a562"
        CS.App.JavaUtil.CallStaticVoid("com.bili.a3.BSGameSdkCenter", "init", true, MerchantId, AppId, ServerId, AppKey)
    elseif UNITY_IOS then
        
    end
end

function login.OnDestroy()
    G.OnNativeMessageBLSdk = undef
end

return login
