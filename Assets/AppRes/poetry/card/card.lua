
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
local xutil = require "utility.xlua.util"
local Vector3 = UnityEngine.Vector3
local EventTrigger= UnityEngine.EventSystems.EventTrigger
local EventTriggerType = UnityEngine.EventSystems.EventTriggerType
local AssetSys = CS.AssetSys

-- card

local yield_return = xutil.async_to_sync(function(to_yield, callback) mono:YieldAndCallback(to_yield, callback) end)

local print = function ( ... )
    _G.print("card", ...)
end

local this = {
    isShowAnswer = false,
    lock = 0,
    owner = 1, -- 1: 自己的， 2：对方的
    id = "", -- question id
    qt = "", -- question type: ab/ba
    q = "云想衣裳花想容,春风拂槛露华浓.", -- info.q 云想衣裳花想容春风拂槛露华浓.<color=red>若非群玉山头见,会向瑶台月下逢.</color>
    a = "若非群玉山头见,会向瑶台月下逢.", -- info.a
}
local card = this

---init
---@param info table
function card.init(info)
    card.lock = 0 -- 0:free, 1:owner free 2:all lock
    card.id = info.id
    card.qt = info.qt
    --card.q = "云想衣裳花想容,春风拂槛露华浓." -- info.q 云想衣裳花想容春风拂槛露华浓.<color=red>若非群玉山头见,会向瑶台月下逢.</color>解决，。
    --card.a = "若非群玉山头见,会向瑶台月下逢." -- info.a
end


--AutoGenInit Begin
--[[
请勿手动编辑此函数
手動でこの関数を編集しないでください。
DO NOT EDIT THIS FUNCTION MANUALLY.
لا يدويا تحرير هذه الوظيفة
]]
function this.AutoGenInit()
    this.AnswerBtn_Button = AnswerBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.AnswerBtn_Button.onClick:AddListener(this.AnswerBtn_OnClick)
    this.Animator = gameObject:GetComponent(typeof(CS.UnityEngine.Animator))
    this.contentRoot_RectTransform = contentRoot:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.Text_TextVirtical = Text:GetComponent(typeof(CS.TextVirtical))
end
--AutoGenInit End

function this.AnswerBtn_OnClick()
    print('AnswerBtn_OnClick')
    if this.isShowAnswer then
        this.showAnswer()
    else
        this.hidAnswer()
    end
end -- ShowAnswerBtn_OnClick

function card.showAnswer()
    this.isShowAnswer = true
    local info = card.info
    local text
    if info.qi < info.ai then
        text = string.format("%s<color=red>%s</color>", info.content[info.qi], info.content[info.ai])
    else
        text = string.format("<color=red>%s</color>%s", info.content[info.ai], info.content[info.qi])
    end
    text = string.gsub(text,"(,)%s*", "%1\n")
                 :gsub("(%.)%s*", "%1\n")
                 :gsub("(%?)%s*", "%1\n")
                 :gsub("(!)%s*", "%1\n")
                 :gsub("(，)%s*", "%1\n")
                 :gsub("(。)%s*", "%1\n")
                 :gsub("(？)%s*", "%1\n")
                 :gsub("(！)%s*", "%1\n")
                 :trim()

    this.Text_TextVirtical.text = text
end

function card.hidAnswer()
    this.isShowAnswer = false
    local info = card.info
    local text = string.gsub(info.content[info.qi], "(,)%s*", "%1\n")
        :gsub("(%.)%s*", "%1\n")
        :gsub("(%?)%s*", "%1\n")
        :gsub("(!)%s*", "%1\n")
        :gsub("(，)%s*", "%1\n")
        :gsub("(。)%s*", "%1\n")
        :gsub("(？)%s*", "%1\n")
        :gsub("(！)%s*", "%1\n")
        :trim()
    this.Text_TextVirtical.text = text
end

function card.Awake()
	this.AutoGenInit()
end

local mousePreviousWorld, mouseCurrentWorld, mouseDeltaWorld;
function card.Start()
    this.Text_TextVirtical.text = card.info.content[card.info.qi]:gsub("(，)%s*", "%1\n"):gsub("(？)%s*", "%1\n")
    --[[public enum EventTriggerType
    {
            PointerEnter = 0,/// Intercepts a IPointerEnterHandler.OnPointerEnter.
            PointerExit = 1,/// Intercepts a IPointerExitHandler.OnPointerExit.
            PointerDown = 2,/// Intercepts a IPointerDownHandler.OnPointerDown.
            PointerUp = 3,/// Intercepts a IPointerUpHandler.OnPointerUp.
            PointerClick = 4,/// Intercepts a IPointerClickHandler.OnPointerClick.
            Drag = 5,/// Intercepts a IDragHandler.OnDrag.
            Drop = 6,/// Intercepts a IDropHandler.OnDrop.
            Scroll = 7,/// Intercepts a IScrollHandler.OnScroll.
            UpdateSelected = 8,/// Intercepts a IUpdateSelectedHandler.OnUpdateSelected.
            Select = 9,/// Intercepts a ISelectHandler.OnSelect.
            Deselect = 10,/// Intercepts a IDeselectHandler.OnDeselect.
            Move = 11,/// Intercepts a IMoveHandler.OnMove.
            InitializePotentialDrag = 12,/// Intercepts IInitializePotentialDrag.InitializePotentialDrag.
            BeginDrag = 13,/// Intercepts IBeginDragHandler.OnBeginDrag.
            EndDrag = 14,/// Intercepts IEndDragHandler.OnEndDrag.
            Submit = 15,/// Intercepts ISubmitHandler.Submit.
            Cancel = 16 /// Intercepts ICancelHandler.OnCancel.
    }]]
     local tr = gameObject:GetComponent(typeof(EventTrigger))
     if tr == nil then tr = gameObject:AddComponent(typeof(EventTrigger)) end
     local et = EventTrigger.Entry()
     et.eventID = EventTriggerType.PointerClick,
     et.callback:AddListener(this.OnClick);
     tr.triggers:Add(et);

    if this.info.die == true then this.hid() end

    local mainCamera= UnityEngine.Camera.main;
    --print("mainCamera", mainCamera, transform, tr, et)
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

--local isDraging = false
--function card.Update()
--    if isDraging and this.camera then
--        local mouseCurrent = UnityEngine.Input.mousePosition;
--        this.mouseCurrent = mouseCurrent
--        this.mouseCurrentWorld = this.camera:ScreenToWorldPoint(Vector3(mouseCurrent.x, mouseCurrent.y, -this.camera.transform.position.z));
--
--        this.mouseDeltaWorld = this.mouseCurrentWorld - this.mousePreviousWorld;
--        this.mousePreviousWorld = this.mouseCurrentWorld;
--    end
--end

---OnAnimationEvent
---@param name string
function card.OnAnimationEvent(name)
    print("OnAnimationEvent", name)
    if name == "tremble_out_end" then
        contentRoot:SetActive(false)
    end
end

function card.OnMouseDrag()
    --if isDraging then
    --    local d = this.mouseDeltaWorld * 120
    --    --gameObject.transform.localPosition = gameObject.transform.localPosition + d
    --    transform.position = this.mouseCurrentWorld
    --    --gameObject.transform:Translate(d);
    --    --print("OnMouseDrag", gameObject.name, this.mouseDeltaWorld, gameObject.transform.localPosition, d)
    --end
    --isDraging = true
end

function card.OnClick(eventData)
    -- callback to match
    if this.info.onClickCallback then
        this.info.onClickCallback(this.info.idx)
    end

    --print("OnClick", p, eventData)
end

function card.hid()
    --this.Animator:ResetTrigger("tremble_out")
    this.Animator:Play("tremble_out")
end

function card.OnMouseUp()
    --isDraging = false
    --local x0 = -458.1555 --40  -- local px = 1   w = 182*0.4 = 73,    xi =  40+(1+73)*i
    --local y0 = 180 -- -57 --  p = 16, h = 256*0.4 = 102.4, yi = -57+(16+102)*i
    local p = gameObject.transform.localPosition
    --local ix = math.ceil(((p.x-x0 - 37)/74))
    --local iy = math.ceil(((p.y-y0 - 59)/118))
    --local x, y = x0+74*ix,y0+118*iy
    --gameObject.transform.localPosition = Vector3(x, y, p.z)
    print("OnMouseUp", p, gameObject.transform.localPosition)
end

return card