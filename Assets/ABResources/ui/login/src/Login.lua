local util = require "lua.utility.xlua.util"
--local LoginHelper = require 'LoginHelper'

local Login = {

	Update = function ()

	end,

	OnDestroy = function ()

	end,

   
}
	Login.Start = function ()
		print("lua Login.start..."..self.transform.position:ToString())

		Button:GetComponent("Button").onClick:AddListener(function()
			print("clicked, you input is '" .. InputField:GetComponent("InputField").text .."'")
		end)

	end

local yield_return = util.async_to_sync(function (to_yield, cb)
	self:YieldAndCallback(to_yield, cb)
end)

return Login
