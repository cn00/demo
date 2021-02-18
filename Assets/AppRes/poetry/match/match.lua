
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/08 19:18:14
--- Description: 
--[[
-[ ] 单人模式
-[ ] 团队模式
]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local AssetSys = CS.AssetSys
local File = CS.System.IO.File
local Vector2 = UnityEngine.Vector2

local stringx = require("stringx") -- split
local xutil = require "xlua.util"
local util = require "lua.utility.util"
local socket = require("socket.socket")
local sqlite = require("lsqlite3")
local manager = G.AppGlobal.manager
local config = require("common.config.config")

local dbpath = config.dbCachePath;
local userDbpath = config.userDbPath;

local yield_return = xutil.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

-- match

local MatchState ={
    idle = 0,
    waitingForPlayerB = 1,
    cardSending = 2,
    playerPreparing = 3,
    fighting = 4,
    countScore = 5,
    readyToStart = 6,
    penaltySelect = 7,-- 罚牌选择
    memoryTime = 8,
}

local print = function ( ... )
    _G.print("match", os.date(), ...)
end

local this = {
    ServerHost = "localhost",
    ServerPort = 9990,
    --matchTypes = "p2c", -- p2c, p2p, visitor
    matchType = 0, -- 0:主场， 1:客场, 2:观众
    matchState = MatchState.idle,
    stateStartTime = 0, -- 状态开始时间
    playerPreparingCountdown = 10, -- 状态读秒
    round = -1, -- 回合
    startTime = -1, -- 
    roundStartTime = 0, -- 回合开始时间
    roundCountdown = 6, -- 回合读秒
    userAnswer = -1, -- 当前双方选手的答案
    scoreA = 0, -- 得分
    scoreB = 0,
    availableIdxs = {}, -- available questions index
    availableIdxsA = {}, -- 
    availableIdxsB = {}, -- 
    currentIdx = -1, -- current question idx [1 - #cqs]
    myname = "libai",
    loginInfo = nil,
    clientId = -1, -- 每次登录服务器时分配的 id 会变
    clientIds = {}, -- 房间成员
    cardsInfo = nil, -- 房间卡牌id
    clientName = "",
    cinfo = nil,  -- client info {myname ,id}
    roomId = nil, -- 加入的房间信息
    clientsInfo = {}, -- {[clientId] = {}}
    roomList={},
    memoryTime = 3, -- 默记时间(秒)
    memoryTimeCount = -1,
}
local match = this

function match.init(info)
    this.info = info
    this.matchType = info.matchType or 0
    this.hostInfo = info.hostInfo
    this.myname = info.name
end

---removeValueFromArray
---@param val any
---@param arr table
local function removeValueFromArray(val, arr)
    --print("removeValueFromArray", val)
    for i, v in ipairs(arr) do
        if v == val then table.remove(arr, i) return true end
    end
    return false
end

---myAnswer
---@param idx number
function match.onCardClick(idx)
    if this.userAnswer > 0 or this.matchState ~= MatchState.fighting then 
        print("game not start yet.", idx)
        return 
    end

    if this.userAnswer == idx then print("慢了一步") return end

    this.userAnswer = idx

    xutil.coroutine_call(function()
        local card = this.cardsInfo[idx]
        card.Lua.hid()
        card.die = true
        local msg = {
            type = "answer", -- or "card_action_b",
                roomId = this.roomId,
                userAnswer = idx,
        }
        this.Client.SendMsgt(msg)
    end)
end

function match.saveResult()
    local db = sqlite.open(userDbpath)
    local nameA, scoreA, nameB, scoreB, useTime, date 
        = this.myname, this.scoreA, this.clientName, this.scoreB, UnityEngine.Time.time - this.startTime, os.date("%Y-%m-%d %H:%M:%S")
    local sql = string.format([[insert into "history" 
        (nameA, scoreA, nameB, scoreB, useTime, date) 
        VALUES ('%s', '%s', '%s', '%s', '%s', '%s')]]
    , nameA, scoreA, nameB, scoreB, useTime, date) --os.date("%Y-%m-%d %H:%M:%S",os.time())
    local errn = db:exec(sql)
    if errn ~= sqlite.OK then
        print("saveResult err", db:errmsg(), sql)
    else
        print("saveResult ok",  nameA, scoreA, nameB, scoreB, useTime, date)
    end
end

function match.nextRound()
    this.matchState = MatchState.fighting
    if #this.availableIdxsA < 1 or #this.availableIdxsB < 1 then
        this.question_Text.text = string.format("%s:%s 你%s啦", this.scoreA, this.scoreB, (this.scoreA > this.scoreB and '赢' or '输'))
        xutil.coroutine_call(function()
            yield_return(UnityEngine.WaitForSeconds(10))
            AppGlobal.SceneManager.pop()
        end)
    else
        match.roundStartTime = os.time()
        this.round = #this.cardsInfo - #this.availableIdxs
        local card = this.cardsInfo[this.currentIdx]
        this.say(card.id, card.qi)
        card.Lua.showAnswer()
        this.question_Text.text = card.content[card.qi]
        
        ----print("host tts",  string.gsub(card.content[card.qi], "\n", "\\n"))
        --CS.App.JavaUtil.CallStaticVoid("com.unity3d.player.TTS", "Say", card.content[card.qi])
    end
end

---throwCard 罚牌, 将一张牌调换阵营
function match.throwCard(role)

end

---getPoetryIds
---@param count number
---@param filter string sql conditions where where
function match.getPoetryIds(count, filter)
    count = count or 1000
    local sql = string.format([[select id from poetry %s limit %s;]], filter, count)
    print("getPoetryIds", sql)
    local db = sqlite.open(dbpath)
    local res = {}
    for row in db:nrows(sql) do
        res[1+#res] = row.id
    end
    db:close()
    return res
end

---@param id table
---@return table
function match.mergeCardsInfo()
    local ids =  table.select(this.cardsInfo, function(o) return o.id end)
    local sids = table.concat(ids, ",")
    local db = sqlite.open(dbpath)
    local sql = string.format([[select id, content from poetry where id in (%s)]], sids)
    local res = {}
    for row in db:nrows(sql) do
        row.content = row.content
                :gsub("(。)%s*", "%1|")
                :gsub("(？)%s*", "%1|")
                :gsub("(！)%s*", "%1|")
                :split("|")
        res[row.id] = row.content
    end
    for i, v in ipairs(this.cardsInfo) do
        v.content = res[v.id]
    end
    db:close()
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
    this.cardTemplate_RectTransform = cardTemplate:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.Chat_RectTransform = Chat:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.ChatBtn_Button = ChatBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.ChatBtn_Button.onClick:AddListener(this.ChatBtn_OnClick)
    this.noteText_Text = noteText:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.playerA_RectTransform = playerA:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.playerB_RectTransform = playerB:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.QRcode_QRCodeEncodeController = QRcode:GetComponent(typeof(CS.QRCodeEncodeController))
    this.question_Text = question:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.scoreText_Text = scoreText:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.ShowQRcodeBtn_Button = ShowQRcodeBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.ShowQRcodeBtn_Button.onClick:AddListener(this.ShowQRcodeBtn_OnClick)
    this.startBtn_Button = startBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.startBtn_Button.onClick:AddListener(this.startBtn_OnClick)
end
--AutoGenInit End

function this.startBtn_OnClick()
    print('startBtn_OnClick')
    startBtn:SetActive(false)
    this.Client.SendMsgt({
        type = "startMatch",
        clientId = this.clientId,
        roomId = this.roomId,
        memoryTime = this.memoryTime
    })
end -- startBtn_OnClick

function this.ChatBtn_OnClick()
    print('ChatBtn_OnClick')
    Chat:SetActive(not Chat.activeSelf)
end -- ChatBtn_OnClick

function this.ShowQRcodeBtn_OnClick()
    QRcode:SetActive(not QRcode.activeSelf)
end -- ShowQRcodeBtn_OnClick

function this.BackBtn_OnClick()
    this.Client.SendMsgt({
        type = "leaveRoom",
        roomId = this.roomId
    })
end -- BackBtn_OnClick

function match.Awake()
	this.AutoGenInit()
    cardTemplate:SetActive(false)
    this.scoreText_Text.text = "0:0"
    this.question_Text.text = ""
end

function match.Start()
    this.Client = AppGlobal.Client
    Chat:SetActive(false)
    startBtn:SetActive(false)
    
    this.Client.AddListeners(this.OnServerMsg())

    this.Client.SendMsgt({
        type = "joinRoom",
            roomId = this.info.roomId
    })
    
    this.stateStartTime = UnityEngine.Time.now

    this.QRcode_QRCodeEncodeController:onQREncodeFinished('+', function(s)
        print("QRCodeEncode finished", s)
    end)

    match.LoadData(function()
        print("LoadData end")
        playerA:SetActive(true)
        playerB:SetActive(true)
        this.noteText_Text.text = this.matchType == 0 and "等待对手上线..." or "对手正在准备中..."
    end)
end

function match.LoadData(cb)
    xutil.coroutine_call(function()
        this.noteText_Text.text = "正在准备,马上就好..."
        local dburl = "db.db"
        local cachePath = dbpath -- AssetSys.CacheRoot .. "db.db"
        local fi =  CS.System.IO.FileInfo(cachePath);
        if not fi.Exists or fi.Length == 0  then
            yield_return(AssetSys.Download(dburl, cachePath))
        else
            print("use cache:", cachePath)
        end
        cb()
    end)
end

local stateUpdate = {
    [MatchState.fighting] = function()
        if this.matchState == MatchState.fighting then
            if this.round >= 0 then
                this.noteText_Text.text = string.format("%s:%s", 
                        this.round, math.modf(os.time() - this.roundStartTime))
            end
        end
    end,
    [MatchState.memoryTime] = function ()
        local t = this.memoryTimeCount - os.time()
        this.noteText_Text.text = string.format('默记时间: %02d:%02d', t//60, t%60)
    end
}

function match.Update()
    if stateUpdate[this.matchState] then stateUpdate[this.matchState]() end
end

--[[{
	["shangxi"] = "鉴赏 　　这在诗之首章，“遵彼汝坟，伐其条枚”——在高高的汝河大堤上，有一位凄苦的妇女，正手执斧子砍伐山楸的树枝。",
	["ai"] = 6,
	["id"] = 10,
	["content"] = {
		[1] = "遵彼汝坟，伐其条枚。",
		[2] = "未见君子，惄如调饥。",
		[3] = "遵彼汝坟，伐其条肄。",
		[4] = "既见君子，不我遐弃。",
		[5] = "鲂鱼赪尾，王室如毁。",
		[6] = "虽则如毁，父母孔迩。",
	},
	["name"] = "汝坟",
	["qi"] = 7,
	["poet_id"] = 0,
	["star"] = 330,
	["poet_name"] = "佚名",
	["dynasty"] = "先秦",
	["about"] = "《毛诗序》以为是赞美文王的教化在汝坟这个国家施行的很好，...",
	["fanyi"] = "注释⑴遵：循，沿。汝：汝河，源出河南省。坟（fén）：...",
	["tags"] = "诗经|思念",
--}]]
--- 发牌
function match.loadCard()
    local cardsInfo = this.cardsInfo
    print("loadCard", util.dump(cardsInfo[1]))
    local cardRootA, cardRootB = this.playerA_RectTransform,this.playerB_RectTransform
    local idsA, idsB= this.availableIdxsA,this.availableIdxsB
    local ownerA, ownerB = 1, 2
    if this.info.matchType == 1 then
        ownerA, ownerB = 2, 1
        idsB, idsA = this.availableIdxsA,this.availableIdxsB
        cardRootB, cardRootA = this.playerA_RectTransform, this.playerB_RectTransform
    end
    for i, v in ipairs(cardsInfo)do
        this.availableIdxs[1+#this.availableIdxs] = i
        v.owner = 1
        local c
        if i <= #cardsInfo/2 then
            v.owner = ownerA
            c = GameObject.Instantiate(cardTemplate, cardRootA)
            idsA[1+#idsA] = i
        else
            v.owner = ownerB
            c = GameObject.Instantiate(cardTemplate, cardRootB)
            idsB[1+#idsB] = i
        end

        c.name = "card_" .. v.id
        c:SetActive(true)

        xutil.coroutine_call(function()
            local ap = string.format("poetry_audio/%04d/%04d_%04d.aiff", v.id, v.id, v.ai)
            --print("apa", ap)
            yield_return(AssetSys.GetAsset(ap, function(clip) end))
        end)
        
        local h = #cardsInfo/2/11 * 110
        cardRootA.sizeDelta = Vector2(0, h)
        cardRootB.sizeDelta = Vector2(0, h)
        
        v.onClickCallback = this.onCardClick

        local Lua = c:GetComponent(typeof(CS.LuaMonoBehaviour)).Lua
        Lua.info = v
        v.Lua = Lua
    end
end


---say
---@param info table
function match.say(id, idx)
    this.audio_AudioSource:Stop()
    local ap = string.format("poetry_audio/%04d/%04d_%04d.aiff",id, id, idx)
    print("poetry_audio", ap)
    this.audio_AudioSource.clip = AssetSys.GetAssetSync(ap)
    this.audio_AudioSource:Play()
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

    this.mergeCardsInfo() -- table.select(this.cardIds, function(o) return o.id end))
    
    -- TODO: visitor
    --this.loadCard()
    
    if this.roomInfo.isNpc or (this.roomInfo.masterId == this.Client.clientId and #this.clientIds > 0) then
        startBtn:SetActive(true)
        this.noteText_Text.text = "点击 Start 开始"
    else
        this.noteText_Text.text = "准备就绪，等待开局。。。"
    end
    
    -- TODO: 查看房间及卡牌信息 refresh ui
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

local function OnNextRound(msgt)
    local body = msgt or {}
    this.userAnswer = -1
    this.currentIdx = body.currentIdx
    this.nextRound()
    return true
end

---OnSendCard
---@param msgt table {cardId, cardInfo={state}}
local function OnSendCard(msgt)

    return true
end

---OnCardAction
---@param msgt table {cardId, action}
local function OnCardAction(msgt)

    return true
end

---OnMatchResult
---@param msgt table {result}
local function OnMatchResult(msgt)
    
    return true
end


---开始对局
---@param msgt table
local function OnStartMatch(msgt)
    local body = msgt or {}
    this.clientName = body.myname
    startBtn:SetActive(false)
    this.loadCard()
    this.matchState = MatchState.memoryTime
    this.memoryTimeCount = os.time() + this.memoryTime
    this.noteText_Text.text = string.format('默记时间: %02d:%02d', this.memoryTime//60, this.memoryTime%60)
    return true
end


local function OnAnswer(msgt)
    local body = msgt or {}
    local roomId = body.roomId
    local currentIdx = body.currentIdx
    local card = this.cardsInfo[currentIdx]
    this.question_Text.text = string.format("<color=red>%s</color> <color=%s>%s|%ss</color>"
        , card.content[card.ai]
        , msgt.userAnswer == msgt.currentIdx and "yellow" or "blue"
        , body.clientId == this.Client.clientId and "我" or "对手"
        , os.time() - this.roundStartTime)
    card.Lua.hid()
    this.say(card.id, card.ai)
    card.die = true
    this.userAnswer = body.userAnswer
    if msgt.userAnswer == msgt.currentIdx then
        if body.clientId == this.Client.clientId then
            print("you are right")
            this.scoreA = this.scoreA + 1
        else
            print("partner are right")
            this.scoreB = this.scoreB + 1
        end
    else -- 错误
        card.Lua.hidAnswer()
        if body.clientId == this.Client.clientId then
            print("you are wrong", msgt.userAnswer, this.currentIdx)
            this.scoreB = this.scoreB + 1
        else
            print("partner are wrong")
            this.scoreA = this.scoreA + 1
        end
    end
    this.scoreText_Text.text = string.format("%s:%s", this.scoreA, this.scoreB)


    removeValueFromArray(body.userAnswer, this.availableIdxs)
    if not removeValueFromArray(body.userAnswer, this.availableIdxsA) then
        removeValueFromArray(body.userAnswer, this.availableIdxsB)
    end
    return true
end

local function OnBye(msgt)
    AppGlobal.SceneManager.pop()
    return true
end

local function OnGameOver(msgt)
    this.matchState = MatchState.idle
    xutil.coroutine_call(function()
        this.saveResult()
        local ap = string.format("poetry_audio/result/%s.aiff",  (this.scoreA > this.scoreB and 'win' or 'lost'))
        print("result", ap)
        yield_return(AssetSys.GetAsset(ap, function(clip)
            this.audio_AudioSource.clip = clip
            this.audio_AudioSource:Play()
        end))
        
        this.question_Text.text = string.format("%s:%s 你%s啦", this.scoreA, this.scoreB, (this.scoreA > this.scoreB and '赢' or '输'))
        yield_return(UnityEngine.WaitForSeconds(10))
        AppGlobal.SceneManager.pop()
    end)
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
    ["sendCard"]	= OnSendCard,
    ["cardAction"]	= OnCardAction,
    ["matchResult"]	= OnMatchResult,
    ["bye"] 		= OnBye,
    ["answer"]      = OnAnswer,
}

function match.OnServerMsg(msgt)
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
