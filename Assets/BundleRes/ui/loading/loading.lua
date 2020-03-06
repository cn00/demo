
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "xlua.util"

--local LoadingValue = LoadingValue
--local LoadingString = LoadingString

-- loading

local print = function ( ... )
    _G.print("[loading]", ...)
end

local this = {}

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.Slider_Slider = Slider:GetComponent(typeof(CS.UnityEngine.UI.Slider))
    this.Text_Text = Text:GetComponent(typeof(CS.UnityEngine.UI.Text))
end
--AutoGenInit End

function this.Awake()
	this.AutoGenInit()
end

function this.Update()
	if(type(LoadingValue) == "number")then this.Slider_Slider.value = LoadingValue end
	if(type(LoadingString) == "string")then this.Text_Text.text = LoadingString end
end

return this
