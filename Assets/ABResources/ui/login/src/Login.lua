local Login = {

	Start = function ()
		print("lua Login.start..."..this.transform.position:ToString())

		Button:GetComponent("Button").onClick:AddListener(function()
			print("clicked, you input is '" .. InputField:GetComponent("InputField").text .."'")
		end)

	end,

	Update = function ()

	end,

	OnDestroy = function ()

	end,
    
}

return Login
