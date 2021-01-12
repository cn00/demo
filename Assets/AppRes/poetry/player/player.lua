
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/08 19:20:28
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"
local Vector3 = UnityEngine.Vector3
-- player

local print = function ( ... )
    _G.print("[player]", ...)
end

local cards = {}
local player = {
	cards = cards
}
local this = player

function player.cardAutoLayout()
	local allcards = cardArea:GetComponentsInChildren(typeof(CS.LuaMonoBehaviour))
	for i = 0, allcards.Length - 1 do
		local c = allcards[i]
		c.transform.localPosition = Vector3()
	end
end

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.cardArea_RectTransform = cardArea:GetComponent(typeof(CS.UnityEngine.RectTransform))
end
--AutoGenInit End

function player.Awake()
	this.AutoGenInit()
end

-- function player.OnEnable() end

function player.Start()
	--util.coroutine_call(this.coroutine_demo)
end

---AddCard
---@param card table
function player.AddCard(card)
	table.insert(cards, card)
end

function player.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return player
