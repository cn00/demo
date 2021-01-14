
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "xlua.util"

-- camera

local print = function ( ... )
    _G.print("[camera]", ...)
end

local this = {}

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.PostProcessLayer = gameObject:GetComponent(typeof(CS.UnityEngine.Rendering.PostProcessing.PostProcessLayer))
    this.PostProcessVolume = gameObject:GetComponent(typeof(CS.UnityEngine.Rendering.PostProcessing.PostProcessVolume))
    this.Camera = gameObject:GetComponent(typeof(CS.UnityEngine.Camera))
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
