
local CS = CS
local UnityEngine = CS.UnityEngine

local Input = UnityEngine.Input
local Vector2 = UnityEngine.Vector2
local Vector3 = UnityEngine.Vector3

local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

local canvas = {}
local this = canvas

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)
-- function this.coroutine_demo()
--     util.coroutine_call(function()
--         print('canvas coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.RawImage_RawImage = RawImage:GetComponent(typeof(CS.UnityEngine.UI.RawImage))
end
--AutoGenInit End

function this.Awake()
	this.AutoGenInit()
end

-- function this.OnEnable() end

function this.Start()
	--assert(coroutine.resume(this.coroutine_demo()))
end

-- function this.FixedUpdate() end


local startPos = Vector2()
local direction
function this.Update()

	-- print("update", Input.touchCount, Input.simulateMouseWithTouches)
	if Input.touchCount > 0 then
		local touch = Input.GetTouch(0)
		local case = {
			[UnityEngine.TouchPhase.Began] = function()
				startPos = touch.position
			end,
			[UnityEngine.TouchPhase.Moved] = function()
				direction = touch.position - startPos
				print("moved:", direction)
				RawImage.transform.position = RawImage.transform.position + Vector3(direction.x, direction.y, 0)
			end,
			[UnityEngine.TouchPhase.Ended] = function()
			end
		}
		print("touch:", touch.phrase)
		if case[touch.phrase] ~= nil then
			case[touch.phrase]()
		end
	end
end

-- function this.LateUpdate() end

-- function this.OnDestroy() end

function this.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return canvas
