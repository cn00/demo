
local util = require "lua.utility.xlua.util"

local playground = {}
local self = playground

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     print("yield_return: ", to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)

-- playground.coroutine_demo = coroutine.create(function()
--     print('playground coroutine start!')
--     yield_return(CS.UnityEngine.WaitForSeconds(2))
--     yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--         obj = asset
--     end))
-- end)

--AutoGenInit Begin
function playground.AutoGenInit()
    playground.Terrain0 = Terrain0:GetComponent("UnityEngine.TerrainCollider")
end
--AutoGenInit End

function playground.Awake()
	playground.AutoGenInit()
end

function playground.OnEnable ()
    print("playground.OnEnable")

end

function playground.Start ()
    print("playground.Start")

    --assert(coroutine.resume(playground.coroutine_demo))

end

function playground.FixedUpdate ()

end

function playground.Update ()

end

function playground.LateUpdate ()

end

function playground.OnDestroy ()
    print("playground.OnDestroy")

end
    
return playground
