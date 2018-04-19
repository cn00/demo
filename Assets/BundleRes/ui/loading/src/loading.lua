
local util = require "lua.utility.xlua.util"
local loading = {}
local self = loading
local GameObject = CS.UnityEngine.GameObject

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

function loading.reload()
    return coroutine.create(function()
    	CS.AssetSys.Instance:UnloadBundle("ui/loading.bd", true)
    	local obj
    	yield_return("ui/loading/loading.prefab", function ( asset )
    		obj = asset
    	end)
    	local ui = GameObject.Instantiate(obj)
    end)
end

--AutoGenInit Begin
function loading.AutoGenInit()
    self.Cube = Cube:GetComponent("UnityEngine.MeshRenderer")
    self.Sphere = Sphere:GetComponent("UnityEngine.MeshRenderer")
end
--AutoGenInit End

function loading.Start ()
    print("loading.Start")

end

function loading.FixedUpdate ()

end

function loading.Update ()

end

function loading.LateUpdate ()

end

function loading.OnDestroy ()
    print("loading.OnDestroy")

end
    
return loading
