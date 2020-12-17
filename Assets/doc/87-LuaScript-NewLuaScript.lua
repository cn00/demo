-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

-- #SCRIPTNAME#

local print = function ( ... )
    _G.print("[#SCRIPTNAME#]", ...)
end

local #SCRIPTNAME# = {}
local this = #SCRIPTNAME#
-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)
-- function this.coroutine_demo()
--     print('coroutine start!')
--     yield_return(UnityEngine.WaitForSeconds(1))
--     local obj = nil
--     yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--         obj = asset
--     end))
--     local gameObj = GameObject.Instantiate(obj)
-- end

--AutoGenInit Begin
function #SCRIPTNAME#.AutoGenInit() end
--AutoGenInit End

function #SCRIPTNAME#.Awake()
	this.AutoGenInit()
end

-- function #SCRIPTNAME#.OnEnable() end

function #SCRIPTNAME#.Start()
	--util.coroutine_call(this.coroutine_demo)
end

-- function #SCRIPTNAME#.FixedUpdate() end

-- function #SCRIPTNAME#.OnTriggerEnter(otherCollider) end
-- function #SCRIPTNAME#.OnTriggerStay(otherCollider) end
-- function #SCRIPTNAME#.OnTriggerExit(otherCollider) end

-- function #SCRIPTNAME#.OnCollisionEnter(otherCollision) end

-- function #SCRIPTNAME#.OnMouseOver() end
-- function #SCRIPTNAME#.OnMouseEnter() end
-- function #SCRIPTNAME#.OnMouseDown() end
-- function #SCRIPTNAME#.OnMouseDrag() end
-- function #SCRIPTNAME#.OnMouseUp() end
-- function #SCRIPTNAME#.OnMouseExit() end

-- function #SCRIPTNAME#.Update() end

-- function #SCRIPTNAME#.LateUpdate() end

-- function #SCRIPTNAME#.OnDestroy() end

function #SCRIPTNAME#.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return #SCRIPTNAME#
