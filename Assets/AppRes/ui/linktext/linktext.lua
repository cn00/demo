
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "xlua.util"

-- linktext

local print = function ( ... )
    _G.print("[linktext]", ...)
end

local this = {}

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)
-- function this.coroutine_demo()
--     print('coroutine start!')
--     yield_return(UnityEngine.WaitForSeconds(1))
--     local obj = nil
--     yield_return(CS.AssetSys.GetAsset("ui/login/login.prefab", function(asset)
--         obj = asset
--     end))
--     local gameObj = GameObject.Instantiate(obj)
-- end

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.TextLI_LinkImageText = TextLI:GetComponent(typeof(CS.LinkImageText))
end
--AutoGenInit End

function this.Awake()
	this.AutoGenInit()
end

-- function this.OnEnable() end

function this.Start()
	--util.coroutine_call(this.coroutine_demo)
end

-- function this.FixedUpdate() end

-- function this.OnTriggerEnter(otherCollider) end
-- function this.OnTriggerStay(otherCollider) end
-- function this.OnTriggerExit(otherCollider) end

-- function this.OnCollisionEnter(otherCollision) end

-- function this.OnMouseOver() end
-- function this.OnMouseEnter() end
-- function this.OnMouseDown() end
-- function this.OnMouseDrag() end
-- function this.OnMouseUp() end
-- function this.OnMouseExit() end

-- function this.Update() end

-- function this.LateUpdate() end

-- function this.OnDestroy() end

function this.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return this
