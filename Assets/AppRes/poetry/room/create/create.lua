--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/20 15:44:34
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "util"
local xutil = require "xlua.util"
local manager = AppGlobal.manager
local Vector2 = UnityEngine.Vector2
-- create

local print = function(...)
    _G.print("create", ...)
    -- _G.print("create", debug.traceback())
end

local this = {
    selectTags = {}
}
local create = this

local suzukiLimitTag = "思琪限定"

--AutoGenInit Begin
--[[
请勿手动编辑此函数
手動でこの関数を編集しないでください。
DO NOT EDIT THIS FUNCTION MANUALLY.
لا يدويا تحرير هذه الوظيفة
]]
function this.AutoGenInit()
    this.BackBtn_Button = BackBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.BackBtn_Button.onClick:AddListener(this.BackBtn_OnClick)
    this.okBtn_Button = okBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.okBtn_Button.onClick:AddListener(this.okBtn_OnClick)
    this.roomDescription_Text = roomDescription:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.roomName_Text = roomName:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.tagContentRoot_RectTransform = tagContentRoot:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.tagItemTemplate_LuaMonoBehaviour = tagItemTemplate:GetComponent(typeof(CS.LuaMonoBehaviour))
    this.testBtn_Button = testBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.testBtn_Button.onClick:AddListener(this.testBtn_OnClick)
    this.testContent_Text = testContent:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.testContent_RectTransform = testContent:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.testRoot_Image = testRoot:GetComponent(typeof(CS.UnityEngine.UI.Image))
end
--AutoGenInit End

local function getData(callback)
    xutil.coroutine_call(function()
        -- get poetry
        local condition = string.format("where tags like '%%%s%%' limit 40", table.concat(this.selectTags, "%' or tags like '%"))
        local dbtable = "poetryAuthor"
        local poetry = AppGlobal.Datasys.getdata(dbtable, { "id", "title", "author", "tags", "content" }, condition)
        if callback then callback(poetry) end
    end)
end

function this.testBtn_OnClick()
    print('testBtn_OnClick')
    -- refresh ui
    if (not testRoot.activeSelf and #this.selectTags > 0) then
        getData(function(data)
            local idx = 0
            local st = table.select(data, function(o)
                idx = idx + 1;
                return string.format("%02d:<color=red>%s</color>|<color=green>%s</color>|%s", idx, o.title, o.tags, o.content)
            end)
            this.testContent_Text.text = table.concat(st, "\n")
            this.testContent_RectTransform.sizeDelta = Vector2(0, this.testContent_Text.preferredHeight)
        end)
    end
    testRoot:SetActive(not testRoot.activeSelf)
end -- textBtn_OnClick

function this.BackBtn_OnClick()
    AppGlobal.SceneManager.push("poetry/index/index.prefab", function()
        GameObject.DestroyImmediate(gameObject)
    end, true)
end -- BackBtn_OnClick

function this.okBtn_OnClick()
    print('okBtn_OnClick')
    getData(function(data)
        local cardIds = table.select(data, function(o) return o.id  end)
        this.cardIds = cardIds
       AppGlobal.Client.SendMsgt({
            type = "createRoom",
            cardIds = this.cardIds,
            masterName = AppGlobal.USER_ID or "不知名客户"
        })
    end)
end -- okBtn_OnClick

function create.Awake()
    this.AutoGenInit()
end

-- function create.OnEnable() end

local function tagOnClick(tag, isAdd, tagItem)
    if isAdd then
        if tag == suzukiLimitTag then
            this.selectTags = { tag }
        else
            if this.selectTags[1] == suzukiLimitTag then
                return false
            else
                if #this.selectTags > 3 then
                    return false
                end
                table.insert(this.selectTags, tag)
            end
        end
    else
        util.removeValue(this.selectTags, tag)
    end
    okBtn:SetActive(#this.selectTags>0)
    testBtn:SetActive(#this.selectTags>0)
    return true
end

function create.Start()
    -- load tags
    okBtn:SetActive(false)
    testBtn:SetActive(false)
    xutil.coroutine_call(function()
        local tags = AppGlobal.Datasys.getdata("tags", { "tag" }, "") -- limit 100
        this.tags = tags
        local size = this.tagContentRoot_RectTransform.rect.size
        local h = 131 * #tags / 13 -- (size.x/80)
        print("hhh", h, size)
        this.tagContentRoot_RectTransform.sizeDelta = Vector2(0, h)

        AppGlobal.Client.ConnectToServer("localhost", 9990, function(ok)
            if ok then
                AppGlobal.Client.AddListeners(this.OnServerMsgType)
            end
        end)

        -- 思琪限定
        local go = GameObject.Instantiate(tagItemTemplate, this.tagContentRoot_RectTransform)
        local lua = go:GetComponent(typeof(CS.LuaMonoBehaviour)).Lua
        local info = {
            text = suzukiLimitTag,
            clickCallback = tagOnClick
        }
        lua.init(info)

        for _, v in pairs(tags) do
            local info = {
                text = v.tag,
                clickCallback = tagOnClick
            }
            local go = GameObject.Instantiate(tagItemTemplate, this.tagContentRoot_RectTransform)
            local lua = go:GetComponent(typeof(CS.LuaMonoBehaviour)).Lua
            lua.init(info)
        end
    end)
end

local function OnCreateRoom(msgt)
    AppGlobal.SceneManager.push("poetry/match/match.prefab", {
        --parent = nil,
        roomId = msgt.roomId,
        matchType = 0, -- 0:主场， 1:客场, 2:观众
    }, true)
    GameObject.DestroyImmediate(gameObject)

    --AppGlobal.SceneManager.push("poetry/roomList/roomList.prefab", {
    --    --parent = nil,
    --    serverIp = "localhost",
    --    serverPort = 99990,
    --    autoMatch = false,
    --    matchType = 0, -- 0:主场， 1:客场, 2:观众
    --}, true)
end

create.OnServerMsgType = {
    ["createRoom"] = OnCreateRoom,
}

function create.OnDestroy()
    if AppGlobal.Client then AppGlobal.Client.RemoveListeners(create.OnServerMsgType) end
end
return create
