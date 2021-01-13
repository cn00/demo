
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/12 11:48:34
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"
local Vector3 = UnityEngine.Vector3

-- card

local print = function ( ... )
    _G.print("[card]", ...)
end

local card = {
    id = "", -- question id
    qt = "", -- question type: ab/ba
    q = "", -- question string
    a = "", -- answer string
}
local this = card

---init
---@param info table
function card.init(info)
    card.id = info.id
    card.qt = info.qt
    card.q = info.q
    card.a = info.a
end

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.AnswerBtn_Button = AnswerBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.AnswerBtn_Button.onClick:AddListener(this.AnswerBtn_OnClick)
    this.Text_TextVirtical = Text:GetComponent(typeof(CS.TextVirtical))
end
--AutoGenInit End

function this.AnswerBtn_OnClick()
    print('AnswerBtn_OnClick')
    local answer
    if card.qt == "ab" then
        answer = string.format("%s\n<color=red>%s</color>", card.q, card.a)
    else
        answer = string.format("<color=red>%s</color>\n%s", card.a, card.q)
    end
    answer = string.gsub(answer, "(,)%s*", "%1\n")
    answer = string.gsub(answer, "(，)%s*", "%1\n")
    answer = string.gsub(answer, "(%.)%s*", "%1\n")
    answer = string.gsub(answer, "(。)%s*", "%1\n")
    
    this.Text_TextVirtical.text = answer
end -- ShowAnswerBtn_OnClick

function card.Awake()
	this.AutoGenInit()
end

local mousePreviousWorld, mouseCurrentWorld, mouseDeltaWorld;
local mainCamera;
function card.Start()
    mainCamera = UnityEngine.Camera.current;
    card.camera = mainCamera
    print("mainCamera", mainCamera)
end

function card.Update()
    local mouseCurrent = UnityEngine.Input.mousePosition;
    this.mouseCurrent = mouseCurrent
    this.mouseCurrentWorld = mainCamera:ScreenToWorldPoint(Vector3(mouseCurrent.x, mouseCurrent.y, -mainCamera.transform.position.z));

    this.mouseDeltaWorld = this.mouseCurrentWorld - this.mousePreviousWorld;
    this.mousePreviousWorld = this.mouseCurrentWorld;
end

function card.OnMouseDrag()
    local d = this.mouseDeltaWorld * 120
    gameObject.transform.localPosition = gameObject.transform.localPosition + d
    --gameObject.transform:Translate(d);
    print("OnMouseDrag", gameObject.name, this.mouseDeltaWorld, gameObject.transform.localPosition, d)
end

function card.OnMouseUp()
    local x0 = -384.1555 --40  -- local px = 1   w = 182*0.4 = 73,    xi =  40+(1+73)*i
    local y0 = 180 -- -57 --  p = 16, h = 256*0.4 = 102.4, yi = -57+(16+102)*i
    local p = gameObject.transform.localPosition
    local ix = math.ceil(((p.x-x0 - 37)/74))
    local iy = math.ceil(((p.y-y0 - 59)/118))
    local x, y = x0+74*ix,y0+118*iy
    gameObject.transform.localPosition = Vector3(x, y, p.z)
    print("OnMouseUp", p, ix, iy, x, y, gameObject.transform.localPosition)
end

return card
