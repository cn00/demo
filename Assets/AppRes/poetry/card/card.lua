
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
local EventTrigger= UnityEngine.EventSystems.EventTrigger
local EventTriggerType = UnityEngine.EventSystems.EventTriggerType
-- card

local print = function ( ... )
    _G.print("[card]", ...)
end

local card = {
    lock = 0,
    id = "", -- question id
    qt = "", -- question type: ab/ba
    q = "云想衣裳花想容,春风拂槛露华浓.", -- info.q 云想衣裳花想容春风拂槛露华浓.<color=red>若非群玉山头见,会向瑶台月下逢.</color>
    a = "若非群玉山头见,会向瑶台月下逢.", -- info.a
}
local this = card

---init
---@param info table
function card.init(info)
    card.lock = 0 -- 0:free, 1:owner free 2:all lock
    card.id = info.id
    card.qt = info.qt
    card.q = "云想衣裳花想容,春风拂槛露华浓." -- info.q 云想衣裳花想容春风拂槛露华浓.<color=red>若非群玉山头见,会向瑶台月下逢.</color>解决，。
    card.a = "若非群玉山头见,会向瑶台月下逢." -- info.a
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
function card.Start()
    local tr = gameObject:GetComponent(typeof(EventTrigger)) 
    if tr == nil then tr = gameObject:AddComponent(typeof(EventTrigger)) end
    local et = EventTrigger.Entry()
    et.eventID = EventTriggerType.Drop,
    et.callback:AddListener(this.OnDrop);
    tr.triggers:Add(et);

    local mainCamera= UnityEngine.Camera.main;
    print("mainCamera", mainCamera, transform, tr, et)
    this.camera = mainCamera
end

--[[ evendData:UnityEngine.EventSystems.PointerEventData: 1832248400
    Position: (252.0, 377.0)
    delta: (0.0, 0.0)
    eligibleForClick: True
    pointerEnter: Text (UnityEngine.GameObject)
    pointerPress: card (UnityEngine.GameObject)
    lastPointerPress: 
    pointerDrag: card (UnityEngine.GameObject)
    Use Drag Threshold: True
    Current Raycast:
    Name: Text (UnityEngine.GameObject)
    module: Name: match(Clone) (UnityEngine.GameObject)
    eventCamera: 
    sortOrderPriority: 0
    renderOrderPriority: 1
    distance: 0
    index: 0
    depth: 17
    worldNormal: (0.0, 0.0, -1.0)
    worldPosition: (0.0, 0.0, 0.0)
    screenPosition: (252.0, 377.0)
    module.sortOrderPriority: 0
    module.renderOrderPriority: 1
    sortingLayer: 0
    sortingOrder: 0
    Press Raycast:
    Name: Text (UnityEngine.GameObject)
    module: Name: match(Clone) (UnityEngine.GameObject)
    eventCamera: 
    sortOrderPriority: 0
    renderOrderPriority: 1
    distance: 0
    index: 0
    depth: 13
    worldNormal: (0.0, 0.0, -1.0)
    worldPosition: (0.0, 0.0, 0.0)
    screenPosition: (84.0, 377.0)
    module.sortOrderPriority: 0
    module.renderOrderPriority: 1
    sortingLayer: 0
    sortingOrder: 0
    : -595727872
]]
function card.OnDrop(evendData)
    print("OnDrop", evendData:GetType(), evendData)
    local dropObj = evendData.pointerDrag
end

local isDraging = false
function card.Update()
    if isDraging and this.camera then
        local mouseCurrent = UnityEngine.Input.mousePosition;
        this.mouseCurrent = mouseCurrent
        this.mouseCurrentWorld = this.camera:ScreenToWorldPoint(Vector3(mouseCurrent.x, mouseCurrent.y, -this.camera.transform.position.z));

        this.mouseDeltaWorld = this.mouseCurrentWorld - this.mousePreviousWorld;
        this.mousePreviousWorld = this.mouseCurrentWorld;
    end
end

function card.OnMouseDrag()
    if isDraging then
        local d = this.mouseDeltaWorld * 120
        --gameObject.transform.localPosition = gameObject.transform.localPosition + d
        transform.position = this.mouseCurrentWorld
        --gameObject.transform:Translate(d);
        --print("OnMouseDrag", gameObject.name, this.mouseDeltaWorld, gameObject.transform.localPosition, d)
    end
    isDraging = true
end

function card.OnMouseUp()
    isDraging = false
    local x0 = -458.1555 --40  -- local px = 1   w = 182*0.4 = 73,    xi =  40+(1+73)*i
    local y0 = 180 -- -57 --  p = 16, h = 256*0.4 = 102.4, yi = -57+(16+102)*i
    local p = gameObject.transform.localPosition
    local ix = math.ceil(((p.x-x0 - 37)/74))
    local iy = math.ceil(((p.y-y0 - 59)/118))
    local x, y = x0+74*ix,y0+118*iy
    gameObject.transform.localPosition = Vector3(x, y, p.z)
    print("OnMouseUp", p, ix, iy, x, y, gameObject.transform.localPosition)
end

return card
