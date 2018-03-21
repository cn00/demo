local util = require "lua.utility.xlua.util"
--local LoginHelper = require 'LoginHelper'

local Login = {
	date1=123456
}

-- local self = Login
function Login:init( ... )
	print("Login.init" .. self.date1)
	-- body
end

function Login.Start()
	print("lua Login.start..."..mono.transform.position:ToString())
	Login:init()
	
	Button:GetComponent("Button").onClick:AddListener(function()
		print("clicked, you input is '" .. InputField:GetComponent("InputField").text .."'")
	end)

end

function Login.Update ()
	-- print("Login.Update")
end

function Login.OnDestroy ()

end

   

local yield_return = util.async_to_sync(function (to_yield, cb)
	self:YieldAndCallback(to_yield, cb)
end)

return Login
