local util = require "lua.utility.xlua.util"

local login_helper = {
	
}
local self = login_helper

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     print("yield_return: ", to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)

-- login_helper.coroutine1 = coroutine.create(function()
--     print('login_helper coroutine start!')
--     yield_return(CS.UnityEngine.WaitForSeconds(2))
--     yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--         obj = asset;
--     end))
-- end

function login_helper.OnEnable ()
    print("login_helper.OnEnable")

end

function login_helper.Start ()
    print("login_helper.Start")

    --assert(coroutine.resume(login_helper.coroutine1))

end

function login_helper.FixedUpdate ()

end

function login_helper.Update ()

end

function login_helper.LateUpdate ()

end

function login_helper.OnDestroy ()
    print("login_helper.OnDestroy")

end
    
return login_helper
