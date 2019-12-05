

local util = require "xlua.util"

local level_01_01 = {}
local self = level_01_01

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)

-- function level_01_01.coroutine_demo()
--     return coroutine.create(function()
--         print('level_01_01 coroutine start!')
--         yield_return(CS.UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = CS.UnityEngine.GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
function level_01_01.AutoGenInit() end
--AutoGenInit End

function level_01_01.Awake()
	level_01_01.AutoGenInit()
end

function level_01_01.OnEnable()
    print("level_01_01.OnEnable")

end

function level_01_01.Start()
    print("level_01_01.Start")

    --assert(coroutine.resume(level_01_01.coroutine_demo()))

end

function level_01_01.FixedUpdate()

end

function level_01_01.Update()

end

function level_01_01.LateUpdate()

end

function level_01_01.OnDestroy()
    print("level_01_01.OnDestroy")

end
    
return level_01_01
