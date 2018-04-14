

local util = require "lua.utility.xlua.util"

local boot = {}
local self = boot

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

function boot.coroutine_demo()
    return coroutine.create(function()
        print('boot coroutine start!')
        yield_return(CS.UnityEngine.WaitForSeconds(1))
        local obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
            obj = asset
        end))
        local gameObj = CS.UnityEngine.GameObject.Instantiate(obj)

		print("UpdateSys 0")
		yield_return(CS.UpdateSys.Instance:Init())
		print("UpdateSys 1")

	    obj = nil
	    print("lua login 0", obj);
	    yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
	        obj = asset;
	    end))
	    print("lua login 1", obj);
	    local login = CS.UnityEngine.GameObject.Instantiate(obj);

	    loading:SetActive(false)

    end)
end

--AutoGenInit Begin
function boot.AutoGenInit()
end
--AutoGenInit End

function boot.Awake()
	boot.AutoGenInit()
end

function boot.OnEnable()
    print("boot.OnEnable")

end

function boot.Start()
    print("boot.Start")

    assert(coroutine.resume(boot.coroutine_demo()))

end

function boot.FixedUpdate()

end

function boot.Update()

end

function boot.LateUpdate()

end

function boot.OnDestroy()
    print("boot.OnDestroy")

end
    
return boot
