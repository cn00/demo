
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

-- index

local print = function ( ... )
    _G.print("index", ...)
end

local index = {}
local this = index
-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)
-- function this.coroutine_demo()
--     print('coroutine start!')
--     yield_return(UnityEngine.WaitForSeconds(1))
--     local obj = nil
--     yield_return(CS.AssetSys.GetAsset("ui/login/login.prefab", function(asset)
--         obj = asset
--     end))
--     local gameObj = GameObject.Instantiate(obj)
-- end

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.Button_2_Button = Button_2:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.Button_2_Button.onClick:AddListener(this.Button_2_OnClick)
    this.Button_3_Button = Button_3:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.Button_3_Button.onClick:AddListener(this.Button_3_OnClick)
    this.contentRoot_RectTransform = contentRoot:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.qread_Button = qread:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.qread_Button.onClick:AddListener(this.qread_OnClick)
    this.templateBtn_Button = templateBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.templateBtn_Button.onClick:AddListener(this.templateBtn_OnClick)
end
--AutoGenInit End

--- qread
--- UnityEvent that is triggered when the button is pressed.
--- Triggered on MouseUp after MouseDown on the same object.
function this.qread_OnClick()
    print('qread_OnClick')
    AppGlobal.SceneManager.push("qread/qread.prefab")
end -- qread_OnClick

function this.Button_3_OnClick()
    print('Button_3_OnClick')
end -- Button_3_OnClick

function this.Button_2_OnClick()
    print('Button_2_OnClick')
end -- Button_2_OnClick

--- template button
function this.templateBtn_OnClick()
    print('templateBtnOnClick')
    -- empty
end -- templateBtnOnClick()

function index.Awake()
	this.AutoGenInit()
end

-- function index.OnEnable() end

function index.Start()
	--util.coroutine_call(this.coroutine_demo)
end

-- function index.FixedUpdate() end

-- function index.OnTriggerEnter(otherCollider) end
-- function index.OnTriggerStay(otherCollider) end
-- function index.OnTriggerExit(otherCollider) end

-- function index.OnCollisionEnter(otherCollision) end

-- function index.OnMouseOver() end
-- function index.OnMouseEnter() end
-- function index.OnMouseDown() end
-- function index.OnMouseDrag() end
-- function index.OnMouseUp() end
-- function index.OnMouseExit() end

-- function index.Update() end

-- function index.LateUpdate() end

-- function index.OnDestroy() end

function index.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return index
