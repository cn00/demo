-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt
--- Author: #AuthorName#
--- Email: #AuthorEmail#
--- Date: #CreateTime#
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

-- ___SCRIPTNAME___

local print = function ( ... )
    _G.print("[___SCRIPTNAME___]", ...)
    -- _G.print("[___SCRIPTNAME___]", debug.traceback())
end

local ___SCRIPTNAME___ = {}
local this = ___SCRIPTNAME___

-- example-begin
local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)
function this.coroutine_demo()
    print('coroutine start!')
    yield_return(UnityEngine.WaitForSeconds(1))
    local obj = nil
    yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
        obj = asset
    end))
    local gameObj = GameObject.Instantiate(obj)
end
-- example-end

--AutoGenInit Begin
function ___SCRIPTNAME___.AutoGenInit() end
--AutoGenInit End

function ___SCRIPTNAME___.Awake()
	this.AutoGenInit()
end

-- function ___SCRIPTNAME___.OnEnable() end

function ___SCRIPTNAME___.Start()
	--util.coroutine_call(this.coroutine_demo)
end

-- example-begin
function ___SCRIPTNAME___.FixedUpdate() end

function ___SCRIPTNAME___.OnTriggerEnter(otherCollider) end
function ___SCRIPTNAME___.OnTriggerStay(otherCollider) end
function ___SCRIPTNAME___.OnTriggerExit(otherCollider) end

function ___SCRIPTNAME___.OnCollisionEnter(otherCollision) end

function ___SCRIPTNAME___.OnMouseOver() end
function ___SCRIPTNAME___.OnMouseEnter() end
function ___SCRIPTNAME___.OnMouseDown() end
function ___SCRIPTNAME___.OnMouseDrag() end
function ___SCRIPTNAME___.OnMouseUp() end
function ___SCRIPTNAME___.OnMouseExit() end

function ___SCRIPTNAME___.Update() end

function ___SCRIPTNAME___.LateUpdate() end

function ___SCRIPTNAME___.OnDestroy() end
-- example-end

function ___SCRIPTNAME___.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return ___SCRIPTNAME___
