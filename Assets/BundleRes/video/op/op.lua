-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

local op = {}
local self = op

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

function op.coroutine_back()
    return coroutine.create(function()
        print('op coroutine start!')
        yield_return(UnityEngine.WaitForSeconds(1))
        local obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
            obj = asset
        end))
        local gameObj = GameObject.Instantiate(obj)

	    GameObject.DestroyImmediate(mono.gameObject)
    end)
end

--AutoGenInit Begin
function op.AutoGenInit()
    op.VideoPlayer_VideoPlayer = VideoPlayer:GetComponent("UnityEngine.Video.VideoPlayer")
    op.Camera_Camera = Camera:GetComponent("UnityEngine.Camera")
    op.Back_Button = Back:GetComponent("UnityEngine.UI.Button")
end
--AutoGenInit End

function op.Awake()
	self.AutoGenInit()


	self.Back_Button.onClick:AddListener(function (  )
		assert(coroutine.resume(self.coroutine_back()))
	end)
end

function op.OnEnable()
    print("op.OnEnable")

end

function op.Start()
    print("op.Start")

    --assert(coroutine.resume(op.coroutine_demo()))

end

function op.FixedUpdate()

end

function op.Update()

end

function op.LateUpdate()

end

function op.OnDestroy()
    print("op.OnDestroy")

end
    
return op
