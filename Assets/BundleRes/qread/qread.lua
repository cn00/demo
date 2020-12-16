--- quick read trainging


local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"
local num2zh = require "lua.utility.num2zh"
-- qread

local print = function ( ... )
    _G.print("[qread]", ...)
end

local qread = {}
local this = qread

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)
-- function qread.coroutine_demo()
--     print('coroutine start!')
--     yield_return(UnityEngine.WaitForSeconds(1))
--     local obj = nil
--     yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--         obj = asset
--     end))
--     local gameObj = GameObject.Instantiate(obj)
-- end

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function qread.AutoGenInit()
    this.contentRoot_Transform = contentRoot:GetComponent(typeof(CS.UnityEngine.Transform))
    this.Panel_Image = Panel:GetComponent(typeof(CS.UnityEngine.UI.Image))
    this.StartBtn_Button = StartBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    --this.StartBtn_Button.onClick:AddListener(function()end) -- StartBtn
    this.StartBtnText_Text = StartBtnText:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.template_Text = template:GetComponent(typeof(CS.UnityEngine.UI.Text))
end
--AutoGenInit End

function qread.QuestOne(n, a, b)
    local q =  qread.question(n, a, b)
    local x = -1000
    for i, v in ipairs(q) do
        if(type(v) == "number")then
            v = num2zh(v)
        end
        local c = GameObject.Instantiate(template, this.contentRoot_Transform)
        c:SetActive(true);
        c.name = v
        x = x + math.random(100, math.ceil(2000/(2*n-1)))
        c.transform.localPosition = UnityEngine.Vector3(x, 0--[[math.random(-300, 300)]], 0)
        local l = c:GetComponent(typeof(CS.UnityEngine.UI.Text))
        l.text = v
    end
    -- result 
    local c = GameObject.Instantiate(template, this.contentRoot_Transform)
    local s = table.concat(q)
    local v = load("return " .. s)()
    c:SetActive(true);
    c.name = s .. "=" .. v
    c.transform.localPosition = UnityEngine.Vector3(760, -450, 0)
    local l = c:GetComponent(typeof(CS.UnityEngine.UI.Text))
    l.text = s .. "=" .. v
    l.alignment = UnityEngine.TextAnchor.MiddleRight
    l.resizeTextForBestFit = true
    l.rectTransform.pivot = UnityEngine.Vector2(1, 0.5);
    l.rectTransform.sizeDelta = UnityEngine.Vector2(700, 180)
end

local runing = false
function qread.Awake()
	this.AutoGenInit()

    this.StartBtn_Button.onClick:AddListener(function()
        --if runing then
        --    runing = false
        --    this.StartBtnText_Text.text = "Start"
            local children = this.contentRoot_Transform:GetComponentsInChildren(typeof(CS.UnityEngine.Transform))
            for i = 0, children.Length - 1 do
                if(children[i].gameObject ~= contentRoot)then GameObject.DestroyImmediate(children[i].gameObject)end
            end
        --else
            runing = true
            --this.StartBtnText_Text.text = "Stop"
            this.StartBtnText_Text.text = "Refresh"
            local n = math.random(3, 9)
            qread.QuestOne(n, 3, 9)
        --end 
    end)
end


function Node(v, p, l, r)
    local n = {
        v = v,
        p = p,
        l = l,
        r = r,
        c = {}, -- conflict
    }
    return n
end

--- @param root Node
--- @param n Node
--- @return Node
function addNode(root, n)
    local i = root
    while (i.v > n.v and i.l ~= nil) or (i.v < n.v and i.r ~= nil) do
        if i.v > n.v then
            i = i.l
        elseif(i.v < n.v ) then
            i = i.r
        end
    end
    if i.v > n.v then
        i.l = n
    elseif i.v < n.v then
        i.r = n
    else -- conflict: i.v == n.v
        table.insert(i.c, n)
    end
end

---@param n number
---@param rangeA number
---@param rangeB number
---@return table "1+2-3*4/5" = {1, 2, '+', 3, 4, '*', 5 '/', '-'}
function qread.question(n, rangeA, rangeB)
    if rangeA == nil then rangeA = 0 end
    if rangeB == nil then rangeB = 100 end
    local operator = {'+', '-', '*', '/'}

    local t = {}
    local kh = 0
    for i = 1, n-1 do
        if(math.random(0, 100) < 30) then
            table.insert(t, '(')
            kh = kh + 1
        end
        table.insert(t, math.random(rangeA, rangeB))
        
        if(kh > 0 and math.random(0, 100) < 30) then
            table.insert(t, ')')
            kh = kh - 1
        end
        table.insert(t, operator[math.random(1, #operator)])
    end
    table.insert(t, math.random(rangeA, rangeB))

    while kh > 0 do
        table.insert(t, ')')
        kh = kh - 1
    end
    
    local s = table.concat(t)
    print(s)
    print(load("return " .. s)())
    
    return t
end

-- function qread.OnEnable() end

function qread.Start()
	--util.coroutine_call(this.coroutine_demo)
end

-- function qread.FixedUpdate() end

-- function qread.OnTriggerEnter(otherCollider) end
-- function qread.OnTriggerStay(otherCollider) end
-- function qread.OnTriggerExit(otherCollider) end

-- function qread.OnCollisionEnter(otherCollision) end

-- function qread.OnMouseOver() end
-- function qread.OnMouseEnter() end
-- function qread.OnMouseDown() end
-- function qread.OnMouseDrag() end
-- function qread.OnMouseUp() end
-- function qread.OnMouseExit() end

-- function qread.Update() end

-- function qread.LateUpdate() end

-- function qread.OnDestroy() end

function qread.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return this
