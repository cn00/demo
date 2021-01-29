
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/12 12:07:03
--- Description: 
--[[
-[ ] 人机对战
-[ ] 自动匹配
-[ ] 创建对局
-[ ] 选择对局:对战/旁观
]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "util"
local xutil = require "xlua.util"
local sqlite3 = require("lsqlite3")
local manager = AppGlobal.manager
local config = require("common.config.config")
local socket = require("socket.core")

local dbpath = config.dbCachePath;
local userDbpath = config.userDbPath;

local yield_return = xutil.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

-- index

local print = function ( ... )
    _G.print("poetry/index", ...)
end

local index = {
}
local this = index

function index.stateToSave()
    
end

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.bg_Image = bg:GetComponent(typeof(CS.UnityEngine.UI.Image))
    this.bottom_RectTransform = bottom:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.createRoom_Button = createRoom:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.createRoom_Button.onClick:AddListener(this.createRoom_OnClick)
    this.gameGround_Button = gameGround:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.gameGround_Button.onClick:AddListener(this.gameGround_OnClick)
    this.history_Button = history:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.history_Button.onClick:AddListener(this.history_OnClick)
    this.nameInput_Text = nameInput:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.p2cPlay_Button = p2cPlay:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.p2cPlay_Button.onClick:AddListener(this.p2cPlay_OnClick)
    this.startMatch_Button = startMatch:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.startMatch_Button.onClick:AddListener(this.startMatch_OnClick)
end
--AutoGenInit End

---对局大厅
function this.gameGround_OnClick()
    print('gameGround_OnClick')
end -- gameGround_OnClick

---人机对战
function this.p2cPlay_OnClick()
    print('p2cPlay_OnClick')
    -- start a local server
    local server = GameObject.Instantiate(CS.AssetSys.GetAssetSync("poetry/net/server.prefab"), manager.Scene.layer.back)
    manager.Scene.push("poetry/match/match.prefab", {
        --parent = nil,
        matchType = "p2c",
        server = server
    }, true)
end -- p2cPlay_OnClick

function this.createRoom_OnClick()
    print('createRoom_OnClick')
    manager.Scene.push("poetry/room/create.prefab", nil, true)
end -- createRoom_OnClick

---自动匹配
function this.startMatch_OnClick()
    print('startMatch_OnClick')
    manager.Scene.push("poetry/match/match.prefab", nil, true)
end -- startMatch_OnClick

---历史战绩
function this.history_OnClick()
    print('history_OnClick')
    manager.Scene.push("poetry/history/history.prefab", nil, true)
end -- history_OnClick

function index.Awake()
    this.AutoGenInit()
end

function index.Start()
    xutil.coroutine_call(function()
        -- init userdata db
        local sql
        yield_return(CS.AssetSys.GetAsset("poetry/data/userdata.sql", function(asset)
            sql = asset.text
            print("userdata.sql", sql)
        end))
        local db = sqlite3.open(userDbpath);
        assert(sqlite3.OK == db:exec(sql), db:errmsg())
        db:close()
    end)
end

function this.joinGame_OnClick()
    local name = this.nameInput_Text.text
    uiroot:SetActive(false)
    local openMatch = function(url)
        require("stringx")
        local hostInfo = string.split(url, ":")
        print("joinGame", url, hostInfo)
        local args = {
            tp = 1, -- 0:主场， 1:客场, 2:观众, 暂定为客场，建立连接后再判定是否为观众
            hostInfo = hostInfo,
            name = name,
        }
        manager.Scene.push("poetry/match/match.prefab", args, true)
    end
    manager.Scene.push("common/qrcode/qrcode.prefab", { scanCallback = openMatch }, true)
end -- joinGame_OnClick

function this.newGame_OnClick()
    local name = this.nameInput_Text.text
    uiroot:SetActive(false)
    this.needQuitUdp = true
    manager.Scene.push("poetry/match/match.prefab", {
        tp = 0,
        name = name
    }, true)
end -- newGame_OnClick


return index
