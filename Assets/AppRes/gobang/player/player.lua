
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/08 19:20:28
--- Description:
--[[
对局双方本地为 playerA，对手为 playerB
观战以房主为 playerA
]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.util"
local xutil = require "utility.xlua.util"
local Vector3 = UnityEngine.Vector3
-- player

local print = function ( ... )
    _G.print("player", ...)
end

local stateIdx = util.newIdx()
local state = {
	locked        = 0,
	idle          = stateIdx(),
	cardPreparing = stateIdx(),
	cardPrepared  = stateIdx(), -- 布战
	remberCard    = stateIdx(), -- 记忆时间
}

local cards = {}
local player = {
	cards = cards,
	state = state.locked,
}
local this = player

function player.swepCard(x0, y0, x1, y1)

end

function player.cardAutoLayout()
	local allcards = cardArea:GetComponentsInChildren(typeof(CS.LuaBehaviour))
	for i = 0, allcards.Length - 1 do
		local c = allcards[i]
		c.transform.localPosition = Vector3()
	end
end

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
end
--AutoGenInit End

function player.Awake()
	this.AutoGenInit()
end

-- function player.OnEnable() end

function player.Start()
	--util.coroutine_call(this.coroutine_demo)
end

---receiveMsg
---@param msg table {type, {args}}
function player.receiveMsg(msg)

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