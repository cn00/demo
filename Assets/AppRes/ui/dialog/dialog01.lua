
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "utility.xlua.util"

-- dialog01

local print = function ( ... )
    _G.print("dialog01", ...)
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
    this.CancelBtn_Button = CancelBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.ConfirmBtn_Button = ConfirmBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.ContentText_Text = ContentText:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.TitleText_Text = TitleText:GetComponent(typeof(CS.UnityEngine.UI.Text))
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