local util = require "lua.utility.xlua.util"

local boot = {
	
}
local self = boot
    
-- local function async_yield_return(to_yield, cb)
--     mono:YieldAndCallback(to_yield, cb)
-- end
-- local yield_return = util.async_to_sync(async_yield_return)
-- local print = CS.AppLog.d
local yield_return = util.async_to_sync(function (to_yield, callback)
	print("yield_return: {0}, {1}", to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

boot.coroutine1 = coroutine.create(function()
    print('coroutine start!')
    local s = os.time()

    local obj = nil
    CS.AppLog.d("lua AppBoot.Start {0}", obj);
    yield_return(CS.AssetSys.Instance:GetAsset("ui/loading/loading.prefab", function(asset)
        obj = asset;
    end))
    CS.AppLog.d("lua AppBoot.Start {0}", obj);
    local loading = CS.UnityEngine.GameObject.Instantiate(obj);

    print("UpdateSys 0")
    yield_return(CS.UpdateSys.Instance:Init())
    print("UpdateSys 1")

    yield_return(CS.UnityEngine.WaitForSeconds(3))
    print('wait interval:', os.time() - s)

    obj = nil
    CS.AppLog.d("lua login {0}", obj);
    yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
        obj = asset;
    end))
    CS.AppLog.d("lua login {0}", obj);
    local login = CS.UnityEngine.GameObject.Instantiate(obj);
    loading:SetActive(false)

    yield_return(CS.UnityEngine.WaitForSeconds(3))
    loading:SetActive(true)

    local www = CS.UnityEngine.WWW('http://10.23.114.141:8008')
    yield_return(www)
	if not www.error then
		local wwws = www.text
		-- local s = CS.System.Text.Encoding.UTF8:GetString(www.bytes)
  --       print("utf8 s: " .. s)
  --       print("text count: " .. #wwws)
  --       print("text: " .. wwws)
        print("bytes count: " .. #www.bytes)
        print("bytes: " .. www.bytes)
	else
	    print('error:', www.error)
	end
end)

function boot.Start ()
    print("boot.Start")
	assert(coroutine.resume(boot.coroutine1))
end

function boot:FixedUpdate ()

end

function boot.Update ()

end

function boot:LateUpdate ()

end

function boot.OnDestroy ()
    print("boot.OnDestroy")

end

return boot
