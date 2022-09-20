
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/20 18:40:19
--- Description:
--[[

]]

local G = _G
local CS = CS
local System = CS.System
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.util"
local xutil = require "xlua.util"
local socket = require("socket.socket")
local Vector2 = UnityEngine.Vector2

local yield_return = xutil.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

-- roomList

local print = function ( ... )
    _G.print("roomList", ...)
    -- _G.print("roomList", debug.traceback())
end

local this = {
    broadcastPort = 8800,
    servers = {}, -- {{url, name}, ...}
    newItemCome = false,
    roomList = {},
}
local roomList = this

function roomList.init(info)
    this.info = info
end

--AutoGenInit Begin
--[[
请勿手动编辑此函数
手動でこの関数を編集しないでください。
DO NOT EDIT THIS FUNCTION MANUALLY.
لا يدويا تحرير هذه الوظيفة
]]
function this.AutoGenInit()
    this.autoMatchBtn_Button = autoMatchBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.autoMatchBtn_Button.onClick:AddListener(this.autoMatchBtn_OnClick)
    this.BackBtn_Button = BackBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.BackBtn_Button.onClick:AddListener(this.BackBtn_OnClick)
    this.createBtn_Button = createBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.createBtn_Button.onClick:AddListener(this.createBtn_OnClick)
    this.itemTemplate_RectTransform = itemTemplate:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.scrollContent_RectTransform = scrollContent:GetComponent(typeof(CS.UnityEngine.RectTransform))
end
--AutoGenInit End

function this.createBtn_OnClick()
    print('createBtn_OnClick')
    AppGlobal.SceneManager.load("poetry/room/create/create.prefab", nil, true)
end -- createBtn_OnClick

---自动匹配
function this.autoMatchBtn_OnClick()
    print('autoMatchBtn_OnClick')
    AppGlobal.Client.SendMsgt({
        type = "autoMatch"
    })
end -- autoMatchBtn_OnClick

function this.BackBtn_OnClick()
    print('BackBtn_OnClick')
    AppGlobal.SceneManager.pop()
end -- BackBtn_OnClick

function roomList.Awake()
	this.AutoGenInit()
end

-- function roomList.OnEnable() end

function roomList.Start()
	--util.coroutine_call(this.coroutine_demo)

    itemTemplate:SetActive(false)

    xutil.coroutine_call(function()
        while AppGlobal.Client == nil do yield_return(UnityEngine.WaitForSeconds(0.3)) end

        if AppGlobal.Client.connectStat == AppGlobal.Client.State.offline then
            AppGlobal.Client.ConnectToServer(this.info.serverIp, this.info.serverPort, function(ok)
                if ok then
                end
            end)
        end

        while AppGlobal.Client.connectStat ~= AppGlobal.Client.State.connected do yield_return(UnityEngine.WaitForSeconds(0.3)) end

        AppGlobal.Client.AddListeners(this.OnServerMsg())
        AppGlobal.Client.SendMsgt({
            type = "roomList",
        })
    end)
end

function roomList.OnRoomSelect(roomId)
    AppGlobal.SceneManager.push("poetry/match/match.prefab", {
        --parent = nil,
        roomId = roomId,
        matchType = 1, -- 0:主场， 1:客场, 2:观众
    }, true)
end

function roomList.Update()
    if this.newItemCome then
        for i, v in pairs(this.roomList) do
            if v.obj == nil then
                print("new room", v.name, v.url,v.description)
                v.onClickRoomCallback = this.OnRoomSelect
                local obj = GameObject.Instantiate(itemTemplate, scrollContent.transform)
                obj:SetActive(true)
                local com = obj:GetComponent(typeof(CS.LuaBehaviour)).Lua
                com.init(v)
                v.obj = obj
            end
        end
        local size = this.scrollContent_RectTransform.rect.size
        local h = 205 * (3+#this.roomList)/4 -- (size.x/80)
        this.scrollContent_RectTransform.sizeDelta = Vector2(0, h)
        this.newItemCome = false
    end
end

local function OnAutoMatch(msgt)
    AppGlobal.SceneManager.push("poetry/match/match.prefab", {
        --matchType = "p2c",
        roomId = msgt.roomId,
        autoMatch = true,
        matchType = 1, -- 0:主场， 1:客场, 2:观众
    }, true)
end
---OnRoomListResult
---@param msgt table {{roomId, client={{id,name}, ...}}, ...}
local function OnRoomListResult(msgt)
    for roomId, roomInfo in ipairs(msgt.rooms) do
        if this.roomList[roomId] == nil then
            this.roomList[roomId] = roomInfo
            this.newItemCome = true
        end
    end
    return true
end

local function OnNewRoom(msgt)
    local roomId = msgt.roomId
    if this.roomList[roomId] == nil then
        this.roomList[roomId] = msgt
        this.newItemCome = true
    end
end

local OnServerMsgType = {
    ["roomList"]	= OnRoomListResult,
    ["autoMatch"]   = OnAutoMatch,
    ["createRoom"]  = OnNewRoom,
}

function roomList.OnServerMsg(msgt)
    return OnServerMsgType
end

function this.OnDestroy()
    if AppGlobal.Client then AppGlobal.Client.RemoveListeners(OnServerMsgType) end
end

return roomList