
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2022/09/21 16:17:21
--- Description:
--[[
五子棋对局
]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local Shader = UnityEngine.Shader
local GameObject = UnityEngine.GameObject
local Vector3 = UnityEngine.Vector3
local Vector2 = UnityEngine.Vector2
local EventTrigger= UnityEngine.EventSystems.EventTrigger
local EventTriggerType = UnityEngine.EventSystems.EventTriggerType
local PointerEventData = UnityEngine.EventSystems.PointerEventData
local Input = UnityEngine.Input
local util = require "lua/utility/util"
local xutil = require "utility.xlua.util"

-- match

local print = function ( ... )
    _G.print("match", ...)
    -- _G.print("match", debug.traceback())
end

local Turn = {
    black = 0,
    white = 1
}

local State = {
    waitRival = 0, -- 等待对手入局
    setting = 1, -- 设置
    ready = 2, -- 就绪
    blackTurn = 3, -- 黑手
    whiteTurn = 4, -- 白手
    gameOver = 5, -- 结束
}
local this = {
    matchType = 1, -- 0:主场， 1:客场, 2:观众
    turn = 0,
    N = 5, -- 棋盘阶数，shader 绘制棋盘变量
    D = 500//5, -- 棋盘格子宽度
    O = Vector3(),-- 棋盘原点，左下角
    black = {}, -- 黑方
    white = {},
    state = State.waitRival,
    clientsInfo = {},
    roomInfo = {},
}
local match = this

function match.init(info)
    this.info = info
    this.matchType = info.matchType or 0
    this.hostInfo = info.hostInfo
    this.myname = info.name
end

--AutoGenInit Begin
--[[
请勿手动编辑此函数
手動でこの関数を編集しないでください。
DO NOT EDIT THIS FUNCTION MANUALLY.
لا يدويا تحرير هذه الوظيفة
]]
function this.AutoGenInit()
    this.audio_AudioSource = audio:GetComponent(typeof(CS.UnityEngine.AudioSource))
    this.BackBtn_Button = BackBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.BackBtn_Button.onClick:AddListener(this.BackBtn_OnClick)
    this.blackTmp_RectTransform = blackTmp:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.board_EventTrigger = board:GetComponent(typeof(CS.UnityEngine.EventSystems.EventTrigger))
    this.board_RectTransform = board:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.boardSlider_Slider = boardSlider:GetComponent(typeof(CS.UnityEngine.UI.Slider))
    this.finish_Button = finish:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.finish_Button.onClick:AddListener(this.finish_OnClick)
    this.info_Text = info:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.noteText_Text = noteText:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.result_RectTransform = result:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.setting_RectTransform = setting:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.startBtn_Button = startBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.startBtn_Button.onClick:AddListener(this.startBtn_OnClick)
    this.whiteTmp_RectTransform = whiteTmp:GetComponent(typeof(CS.UnityEngine.RectTransform))
end
--AutoGenInit End

function this.finish_OnClick()
    match.Reset()
    setting:SetActive(true)
end

function this.startBtn_OnClick()
    setting:SetActive(false)
end

function this.BackBtn_OnClick()
    AppGlobal.SceneManager.pop()
end

function match.Awake()
	this.AutoGenInit()
end

-- function match.OnEnable() end
function match.Start()
    match.Reset()

    this.boardSlider_Slider.onValueChanged:AddListener(function(fval)
        this.N = fval//1
        this.D = this.board_RectTransform.sizeDelta.x * this.board_RectTransform.lossyScale.x/this.N
        this.boardSlider_Slider.value = this.N
        Shader.SetGlobalFloat(Shader.PropertyToID("_N"), this.N);
    end)

    local tr = this.board_EventTrigger
    local et = EventTrigger.Entry()
    et.eventID = EventTriggerType.PointerClick,
    et.callback:AddListener(this.OnClick);
    tr.triggers:Add(et);

    this.Client = AppGlobal.Client
    this.Client.AddListeners(this.MessageListeners())

    this.Client.SendMsgt({
        type = "joinRoom",
        roomId = this.info.roomId
    })

end

local function destroyChess(l)
    for _,v in pairs(l)do
        for _,vv in pairs(v) do
            GameObject.DestroyImmediate(vv.gameObject)
        end
    end
end

--- 重置对局
function match.Reset()
    this.turn = 0
    this.state = State.setting
    setting:SetActive(true)
    result:SetActive(false)

    this.boardSlider_Slider.value = this.N
    Shader.SetGlobalFloat(Shader.PropertyToID("_N"), this.N);
    this.D = this.board_RectTransform.sizeDelta.x * this.board_RectTransform.lossyScale.x/this.N


    destroyChess(this.white)
    destroyChess(this.black)
    this.white = {}
    this.black = {}

    local half = this.board_RectTransform.sizeDelta * this.board_RectTransform.lossyScale.x / 2
    local o = this.board_RectTransform.position - Vector3(half.x, half.y)
    this.O = o
    -- xutil.coroutine_call(function()  end)
end

---available
---@param p Vector3
---@return boolean
function available(p)
    if this.white[p.x] and this.white[p.x][p.y] then return false end
    if this.black[p.x] and this.black[p.x][p.y] then return false end
    return true
end

function check5(l)
    -- 横向
    local h = function()
        for i,v in pairs(l)do
            for j,vv in pairs(l[i]) do
                local c = 1
                local t = {l[i][j]}
                for k = i+1,this.N do
                    if l[k] and l[k][j] ~= null then
                        t[#t+1] = l[k][j]
                        c = c+1
                        if c >= 5 then
                            return true, t
                        end
                    else
                        break
                    end
                end
            end
        end
    end
    -- 纵向
    local v = function()
        for i,v in pairs(l)do
            for j,vv in pairs(l[i]) do
                local c = 1
                local t = {l[i][j]}
                for k = j+1,this.N do
                    if l[i][k] ~= null then
                        t[#t+1] = l[i][k]
                        c = c+1
                        if c >= 5 then
                            return true, t
                        end
                    else
                        break
                    end
                end
            end
        end
    end
    -- 左斜线
    local ls = function()
        for i,v in pairs(l)do
            for j,vv in pairs(l[i]) do
                local c = 1
                local t = {l[i][j]}
                for k = 1,this.N-i do
                    if i+k>this.N or j+k>this.N then break end
                    if l[i+k] and l[i+k][j+k] ~= null then
                        t[#t+1] = l[i+k][j+k]
                        c = c+1
                        if c >= 5 then
                            return true, t
                        end
                    else
                        break
                    end
                end
            end
        end
    end
    -- 右斜线
    local rs = function()
        for i,v in pairs(l)do
            for j,vv in pairs(l[i]) do
                local c = 1
                local t = {l[i][j]}
                for k = 1,this.N-i do
                    if i+k>this.N or j-k<0 then break end
                    if l[i+k] and l[i+k][j-k] ~= null then
                        t[#t+1] = l[i+k][j-k]
                        c = c+1
                        if c >= 5 then
                            return true, t
                        end
                    else
                        break
                    end
                end
            end
        end
    end

    local b, t = v(l)
    if b then return b,t end
    local b, t = h(l)
    if b then return b,t end
    local b, t = ls(l)
    if b then return b,t end
    local b, t = rs(l)
    if b then return b,t end
end

---OnClick
---@param eventData PointerEventData
function this.OnClick(data)
    if this.state == State.gameOver then return end

    this.turn = this.turn+1
    local p = (Input.mousePosition - this.O)+Vector3(this.D/2,this.D/2)
    p = Vector3(p.x//this.D, p.y//this.D)

    if available(p)then
        local p2 = Vector3(p.x*this.D,  p.y*this.D)+this.O
        if this.turn % 2 == 0 then
            this.white[p.x] = this.white[p.x] or {}
            local o = GameObject.Instantiate(whiteTmp, board.transform)
            o.name = "white_"..p.x..","..p.y
            o.transform.position = p2
            this.white[p.x][p.y] = o
            local b, t = check5(this.white)
            --print("check white",b, t)
            if b then
                this.whiteWin = t
                for _,i in ipairs(t) do
                    i.transform.localScale = i.transform.localScale *1.3
                end
                this.state = State.gameOver
                match.ShowResult("白方胜")
            end
        else
            this.black[p.x] = this.black[p.x] or {}
            local o = GameObject.Instantiate(blackTmp, board.transform)
            o.name = "black_"..p.x..","..p.y
            o.transform.position = p2
            this.black[p.x][p.y] = o
            local b,t = check5(this.black)
            --print("check black",b, t)
            if b then
                this.blackWin = t
                for _,i in ipairs(t) do
                    i.transform.localScale = i.transform.localScale *1.3
                end
                this.state = State.gameOver
                match.ShowResult("黑方胜")
            end
        end
    end
end

function match.ShowResult(msg)
    result:SetActive(true)
    this.info_Text.text = msg
end

function match.OnDrawGizmos()
    CS.UnityEditor.Handles.Label(blackTmp.transform.position, blackTmp.transform.position:ToString())
    CS.UnityEditor.Handles.Label(whiteTmp.transform.position, whiteTmp.transform.position:ToString())
end

---OnLoginResult
---@param msgt table {success}
local function OnLoginResult(msgt)
    this.loginInfo = msgt
    return true
end

---OnCreateRoomResult
---@param msgt table {roomId, masterId, clients = {}}
local function OnCreateRoomResult(msgt)
    local roomId = msgt.roomId
    this.roomId = roomId
    return true
end

---OnJoinRoomResult
---@param msgt table {roomId, clientId}
local function OnJoinRoomResult(msgt)
    local body = msgt
    this.roomId = body.roomId
    this.Client.roomId = body.roomId
    if this.Client.clientId ~= body.clientId then table.insert(this.clientIds, body.clientId) end
    this.cardsInfo = body.cardsInfo or this.cardsInfo
    this.roomInfo = body.roomInfo or this.roomInfo

    --if this.roomInfo.isNpc or (this.roomInfo.masterId == this.Client.clientId and #this.clientIds > 0) then
    --    startBtn:SetActive(true)
    --    this.noteText_Text.text = "点击 Start 开始"
    --else
    --    this.noteText_Text.text = "准备就绪，等待开局。。。"
    --end

    return true
end

local function OnLeaveRoom(msgt)
    local body = msgt

    if body.clientId == this.clientId and body.roomId == this.roomId then
        this.roomId = nil
        this.room = undef
        this.Client.roomId = undef
    else
        util.removeValue(this.clientIds, body.clientId)
    end

    if body.clientId == this.Client.clientId or #this.clientIds < 2 then
        this.Client.SendMsgt({
            type = "leaveRoom",
            roomId = this.roomId
        })
        AppGlobal.SceneManager.pop()
    end

    return true
end

---同步客户端状态
---@param msgt table body={id, state}
local function OnClientStateChanged(msgt)
    local body = msgt or {}
    this.clientsInfo[body.clientId].state = body.state
    local readyToStart = true
    for clientId, clientInfo in pairs(this.clientsInfo) do
        if clientInfo.state ~= MatchState.readyToStart then
            readyToStart = false
            break
        end
    end
    if readyToStart then
        if body.clientId == this.clientId then -- 房主
            -- show startBtn
            startBtn:SetActive(true)
        end
    end
    return true
end

local OnServerMsgType = {
    ["login"]		= OnLoginResult,
    ["createRoom"]	= OnCreateRoomResult,
    ["joinRoom"] 	= OnJoinRoomResult,
    ["leaveRoom"] 	= OnLeaveRoom,
    ["cStateChange"] = OnClientStateChanged,
    ["startMatch"]  = OnStartMatch,
    ["nextRound"]	= OnNextRound,
    ["gameOver"]    = OnGameOver,
}


function match.MessageListeners()
    return OnServerMsgType
end

function match.OnDestroy()
    if AppGlobal.Client then AppGlobal.Client.RemoveListeners(OnServerMsgType) end
    if this.Client.conn then
        this.Client.SendMsgt({
            type = "bye",
            clientId = this.clientId,
            roomId = this.roomId
        })
        this.Client.conn:close()
    end
end

return match