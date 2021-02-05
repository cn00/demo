
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

local print = function ( ... )
    _G.print("create", ...)
    -- _G.print("create", debug.traceback())
end

local this = {
    selectTags = {}
}
local create = this



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

function this.testBtn_OnClick()
    print('testBtn_OnClick')
    -- refresh ui
    if(not testRoot.activeSelf) then
        xutil.coroutine_call(function()
            -- get poetry
            local condition = string.format("where tags like '%%%s%%' limit 10", table.concat(this.selectTags, "%' or tags like '%"))
            local poetry = AppGlobal.Datasys.getdata("poetryAuthor", {"title", "author", "tags", "content"}, condition)
            local st = table.select(poetry, function(o) return string.format("<color=red>%s</color>|<color=green>%s</color>|%s", o.title, o.tags, o.content) end)
            this.testContent_Text.text = table.concat(st, "\n")
    
            this.testContent_RectTransform.sizeDelta = Vector2(0, this.testContent_Text.preferredHeight)
        end)
    end
    testRoot:SetActive(not testRoot.activeSelf)
end -- textBtn_OnClick

function this.BackBtn_OnClick()
    AppGlobal.SceneManager.push("poetry/index/index.prefab", nil, true)
end -- BackBtn_OnClick

function this.okBtn_OnClick()
    print('okBtn_OnClick')
end -- okBtn_OnClick

function create.Awake()
	this.AutoGenInit()
end

-- function create.OnEnable() end

local function tagOnClick(tag, isAdd)
    if isAdd then
        if #this.selectTags > 3 then return false end
        table.insert(this.selectTags, tag)
    else
        util.removeValue(this.selectTags, tag)
    end
    return true
end

function create.Start()
    -- load tags
	xutil.coroutine_call(function()
        local tags = AppGlobal.Datasys.getdata("tags", {"tag"}, "limit 40")
        this.tags = tags
        local count = 0
        for _, v in pairs(tags) do
            count = count + 1
            local info = {
                text = v.tag,
                clickCallback = tagOnClick
            }
            local go = GameObject.Instantiate(tagItemTemplate, this.tagContentRoot_RectTransform)
            local lua = go:GetComponent(typeof(CS.LuaMonoBehaviour)).Lua
            lua.init(info)
        end
        local size = this.tagContentRoot_RectTransform.sizeDelta
        local h = 140 * count/(size.x/80)
    end)
end

return create
