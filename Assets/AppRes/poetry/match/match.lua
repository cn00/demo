
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

local stringx = require("stringx") -- split
local xutil = require "xlua.util"
local util = require "lua.utility.util"
local socket = require("socket.socket")
local sqlite = require("lsqlite3")
local manager = G.AppGlobal.manager
local config = require("common.config.config")

local dbpath = config.dbCachePath;
local userDbpath = config.userDbPath;


local manager = AppGlobal.manager

local yield_return = xutil.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

-- match

local clientState ={
    waitingForPlayerB = 1,
    cardSending = 2,
    playerPreparing = 3,
    fighting = 4,
    countScore = 5,
    readyToStart = 6,
    penaltySelect = 7,-- 罚牌选择
}

local print = function ( ... )
    _G.print("match", ...)
end

local this = {
    ServerHost = "localhost",
    ServerPort = 9990,
    --matchTypes = "p2c", -- p2c, p2p, visitor
    matchType = 0, -- 0:主场， 1:客场, 2:观众
    matchState = clientState.playerPreparing,
    stateStartTime = 0, -- 状态开始时间
    playerPreparingCountdown = 10, -- 状态读秒
    round = -1, -- 回合
    startTime = -1, -- 
    roundStartTime = 0, -- 回合开始时间
    roundCountdown = 6, -- 回合读秒
    roundAnswer = -1, -- 当前双方选手的答案
    scoreA = 0, -- 得分
    scoreB = 0,
    availableIdxs = {}, -- available questions index
    availableIdxsA = {}, -- 
    availableIdxsB = {}, -- 
    currentIdx = -1, -- current question idx [1 - #cqs]
    myname = "libai",
    loginInfo = nil,
    clientId = -1, -- 每次登录服务器时分配的 id 会变
    clientsId = {}, -- 房间成员
    cardsInfo = nil, -- 房间卡牌id
    clientName = "",
    cinfo = nil,  -- client info {myname ,id}
    roomId = nil, -- 加入的房间信息
    clientsInfo = {}, -- {[clientId] = {}}
    roomList={},
    chatHistory = {}, -- {{clientId, content},...}
    chatMsgNew = false, 
}
local match = this

function match.init(info)
    this.info = info
    this.matchType = info.tp or 0
    this.hostInfo = info.hostInfo
    this.myname = info.name
end

---removeValueFromArray
---@param val any
---@param arr table
local function removeValueFromArray(val, arr)
    print("removeValueFromArray", val)
    for i, v in ipairs(arr) do
        if v == val then table.remove(arr, i) return true end
    end
    return false
end

---myAnswer
---@param idx number
function match.onCardClick(idx, tp)
    if this.currentIdx < 0 then print("game not start yet.", idx)return end

    if this.roundAnswer == idx then print("慢了一步") return end

    this.roundAnswer = -1

    print("onCardClick", idx)
    xutil.coroutine_call(function()
        local card = this.cardsInfo[idx]
        card.Lua.hid()
        card.die = true
        local msg = {
            type = "answer", -- or "card_action_b",
            body = {
                clientId = this.clientId,
                roomId = this.roomId,
                roundAnswer = idx,
            }
        }
        this.Client.SendMsgt(msg)

        if tp == this.matchType then
            if idx == this.currentIdx then
                this.scoreA = this.scoreA + 1
            else
                this.cardsInfo[this.currentIdx].Lua.hidAnswer() -- 
                this.scoreB = this.scoreB + 1
                if card.owner == 1 then
                    -- TODO: 罚牌
                end
            end
        else
            if idx == this.currentIdx then
                this.scoreB = this.scoreB + 1
            else
                match.cardsInfo[this.currentIdx].Lua.hidAnswer()
                this.scoreA = this.scoreA + 1
                if card.owner == 1 then
                    -- TODO: 罚牌
                end
            end
        end

        removeValueFromArray(idx, this.availableIdxs)
        if not removeValueFromArray(idx, this.availableIdxsA) then
            removeValueFromArray(idx, this.availableIdxsB)
        end

        -- show right answer
        this.question_Text.text = string.format("<color=red>%s</color>", card.content[card.ai])
        yield_return(UnityEngine.WaitForSeconds(3))
        --this.question_Text.text = "" -- client is not finished yield_return

        this.scoreText_Text.text = string.format("%s:%s", this.scoreA, this.scoreB)

        if #this.availableIdxsA < 1 or #this.availableIdxsB < 1 then
            this.question_Text.text = string.format("%s:%s 你%s啦", this.scoreA, this.scoreB, (this.scoreA > this.scoreB and '赢' or '输'))
            this.saveResult()
            xutil.coroutine_call(function()
                yield_return(UnityEngine.WaitForSeconds(10))
                manager.Scene.push("poetry/index/index.prefab", nil, true)
            end)
        else
            if this.matchType == 0 then match.nextRound() end
        end
        
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
    if #this.availableIdxsA < 1 or #this.availableIdxsB < 1 then
        this.question_Text.text = string.format("%s:%s 你%s啦", this.scoreA, this.scoreB, (this.scoreA > this.scoreB and '赢' or '输'))
        xutil.coroutine_call(function()
            yield_return(UnityEngine.WaitForSeconds(10))
            manager.Scene.pop()
        end)
    else
        this.roundAnswer = -1
        match.roundStartTime = UnityEngine.Time.time
        this.round = #this.cardsInfo - #this.availableIdxs
        local card = this.cardsInfo[this.currentIdx]
        card.Lua.showAnswer()
        this.question_Text.text = card.content[card.qi]

        --local msgtopartner = {
        --    type = "nextRound",
        --    body = {
        --        currentIdx=this.currentIdx,
        --        round = this.round
        --    },
        --}
        --this.Client.SendMsgt(msgtopartner)
        
        print("host tts",  string.gsub(card.content[card.qi], "\n", "\\n"))
        CS.App.JavaUtil.CallStaticVoid("com.unity3d.player.TTS", "Say", card.content[card.qi])
    end
end

---throwCard 罚牌, 将一张牌调换阵营
---@param role number 1:playerA 2:playerB
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
    this.BackBtn_Button = BackBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.BackBtn_Button.onClick:AddListener(this.BackBtn_OnClick)
    this.cardTemplate_RectTransform = cardTemplate:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.Chat_RectTransform = Chat:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.ChatBtn_Button = ChatBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.ChatBtn_Button.onClick:AddListener(this.ChatBtn_OnClick)
    this.chatContent_VerticalLayoutGroup = chatContent:GetComponent(typeof(CS.UnityEngine.UI.VerticalLayoutGroup))
    this.chatInputField_InputField = chatInputField:GetComponent(typeof(CS.UnityEngine.UI.InputField))
    this.chatMsgTemp_RectTransform = chatMsgTemp:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.chatSendBtn_Button = chatSendBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.chatSendBtn_Button.onClick:AddListener(this.chatSendBtn_OnClick)
    this.netClient_LuaMonoBehaviour = netClient:GetComponent(typeof(CS.LuaMonoBehaviour))
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
    this.Client.SendMsgt({
        type = "startMatch",
        body = {
            clientId = this.clientId,
            roomId = this.roomId,
        }
    })
end -- startBtn_OnClick

function this.chatSendBtn_OnClick()
    print('chatSendBtn_OnClick')
    if this.chatInputField_InputField.text == "" then return end
    this.Client.SendMsgt({
        type = "chat",
        body = {
            clientId = this.clientId,
            roomId = this.roomId,
            content = this.chatInputField_InputField.text
        }
    })
    this.chatInputField_InputField.text = ""
end -- chatSendBtn_OnClick

function this.ChatBtn_OnClick()
    print('ChatBtn_OnClick')
    Chat:SetActive(not Chat.activeSelf)
end -- ChatBtn_OnClick

function this.ShowQRcodeBtn_OnClick()
    QRcode:SetActive(not QRcode.activeSelf)
end -- ShowQRcodeBtn_OnClick

function this.BackBtn_OnClick()
    manager.Scene.pop(function()
        print("pop")
    end)
end -- BackBtn_OnClick

function match.Awake()
	this.AutoGenInit()
    cardTemplate:SetActive(false)
    this.scoreText_Text.text = "0:0"
    this.question_Text.text = ""
end

function match.Start()
    this.Client = this.netClient_LuaMonoBehaviour.Lua
    startBtn:SetActive(false)
    if(this.matchType == 0) then
        this.Client.ConnectToServer(this.ServerHost, this.ServerPort, this.OnServerMsg)

        -- TODO: Server url QRcode
        --local ips = CS.NetSys.LocalIpAddressStr()
        --local args = {}
        --local url = string.format("a3mkgp:%s:%s:%s", ips.Length > 0 and ips[0] or "127.0.0.1", this.Server.ServerPort, util.dump(args))
        --print("qrcode:url", url)
        --this.QRcode_QRCodeEncodeController:Encode(url)
    else
        print("<color=red>client machine</color>")
        -- remote {"a3mkgp", ip, port, argt}
        this.Client.ConnectToServer(this.hostInfo[2], this.hostInfo[3], this.OnServerMsg)
    end
    
    this.stateStartTime = UnityEngine.Time.now

    this.QRcode_QRCodeEncodeController:onQREncodeFinished('+', function(s)
        print("QRCodeEncode finished", s)
    end)

    match.LoadData(function()
        print("LoadData end")
        playerA:SetActive(true)
        playerB:SetActive(true)
        this.noteText_Text.text = this.matchType == 0 and "等待对手上线..." or "对手正在酝酿中..."
    end)
end

function match.LoadData(cb)
    xutil.coroutine_call(function()
        this.noteText_Text.text = "正在积极和诗人们取得联系,马上就好..."
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


function match.Update()
    if this.round >= 0 then
        this.noteText_Text.text = string.format("%s:%s", 
                this.round, math.modf(UnityEngine.Time.time - match.roundStartTime))
    end

    if this.chatMsgNew then
        -- TODO: refresh msg ui
        this.chatMsgNew = false
    end
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
    if this.matchType == 1 then
        ownerA, ownerB = 2, 1
        idsB, idsA = this.availableIdxsA,this.availableIdxsB
        cardRootB, cardRootA = this.playerA_RectTransform,this.playerB_RectTransform
    end
    for i, v in ipairs(cardsInfo)do
        this.availableIdxs[1+#this.availableIdxs] = i
        v.owner = 1
        local c
        if i < #cardsInfo/2 then
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
        
        v.onClickCallback = this.onCardClick

        local Lua = c:GetComponent(typeof(CS.LuaMonoBehaviour)).Lua
        Lua.info = v
        v.Lua = Lua
    end
end

---OnConnect
---@param msgt table body = {id}
local function OnConnect(msgt)
    this.clientId = msgt.body.clientId
    this.Client.clientId = msgt.body.clientId

    if this.info.autoMatch then
        this.Client.SendMsgt({
            type = "autoMatch",
            body = {
                level = math.random(0, 9),
                clientId = this.clientId,
            }
        })
    end
    return true
end

---OnLoginResult
---@param msgt table body = {success}
local function OnLoginResult(msgt)
    this.loginInfo = msgt.body
    return true
end

---OnCreateRoomResult
---@param msgt table body = {roomId, masterId, clients = {}}
local function OnCreateRoomResult(msgt)
    local roomId = msgt.body.roomId
    this.roomId = roomId
    this.poetryIdList = msgt.body.cardIds
    --this.roomList[roomId].clientIds = msgt.body
    return true
end

---OnRoomListResult
---@param msgt table body = {{roomId, client={{id,name}, ...}}, ...}
local function OnRoomListResult(msgt)
    this.roomList = msgt.body
    return true
end

---OnJoinRoomResult
---@param msgt table body = {roomId, clientId}
local function OnJoinRoomResult(msgt)
    local body = msgt.body
    this.roomId = body.roomId
    this.clientsId = body.clientsId
    this.cardsInfo = body.cardsInfo

    this.mergeCardsInfo() -- table.select(this.cardIds, function(o) return o.id end))
    
    -- TODO: visitor
    --this.loadCard()
    
    if body.roomInfo.isNpc or this.roomInfo.masterId == this.clientId then
        startBtn:SetActive(true)
    end
    -- TODO: 查看房间及卡牌信息 refresh ui
    this.noteText_Text.text = "等待对手准备就绪"
    return true
end

local function OnLeaveRoom(msgt)
    local body = msgt.body

    if body.clientId == this.clientId and body.roomId == this.roomId then
        this.roomId = nil
        this.room = undef
    else
        util.removeValue(this.room.clientIds, body.clientId)
    end
    -- TODO: refresh ui
    return true
end

---同步客户端状态
---@param msgt table body={id, state}
local function OnClientStateChanged(msgt)
    local body = msgt.body
    this.clientsInfo[body.clientId].state = body.state
    local readyToStart = true
    for clientId, clientInfo in pairs(this.clientsInfo) do
        if clientInfo.state ~= clientState.readyToStart then
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
    local body = msgt.body
    this.currentIdx = body.currentIdx
    this.nextRound()
    return true
end

---OnSendCard
---@param msgt table body = {cardId, cardInfo={state}}
local function OnSendCard(msgt)

    return true
end

---OnCardAction
---@param msgt table body = {cardId, action}
local function OnCardAction(msgt)

    return true
end

---OnMatchResult
---@param msgt table body = {result}
local function OnMatchResult(msgt)
    
    return true
end

local function OnStartMatch(msgt)
    local body = msgt.body
    this.clientName = body.myname
    startBtn:SetActive(false)
    this.loadCard()
    this.noteText_Text.text = '默记时间🕙' 
    return true
end

local function OnHeartbeat(msgt)
    
    return true
end

--- body {clientId, content}
local function OnChat(msgt)
    
    table.insert(this.chatHistory, msgt.body)
    this.chatMsgNew = true
    
    return true
end

local function OnAnswer(msgt)
    local body = msgt.body
    local roomId = body.roomId
    this.currentIdx = -1
    this.roundAnswer = body.roundAnswer
    if body.clientId == this.clientId then
        xutil.coroutine_call(function()
            -- TODO: show answer time
            yield_return(UnityEngine.WaitForSeconds(5))
            tihs.Client.SendMsgt({
                type = "endRound",
                body = {
                    clientId = this.clientId,
                    roomId = tihs.roomId
                }
            })
        end)
    end
    return true
end

local function OnEndRound(msgt)
    local body = msgt.body
    local clientId = body.clientId
    return true
end

local function OnBye(msgt)
    local body = msgt.body
    local clientId = body.clientId
    return true
end

local OnServerMsgType = {
    ["connect"] 	= OnConnect, --> login
    ["login"]		= OnLoginResult,
    ["createRoom"]	= OnCreateRoomResult,
    ["roomList"]	= OnRoomListResult,
    ["joinRoom"] 	= OnJoinRoomResult,
    ["leaveRoom"] 	= OnLeaveRoom,
    ["cStateChange"] = OnClientStateChanged,
    ["startMatch"]  = OnStartMatch,
    ["nextRound"]	= OnNextRound,
    ["endRound"]    = OnEndRound,
    ["sendCard"]	= OnSendCard,
    ["cardAction"]	= OnCardAction,
    ["matchResult"]	= OnMatchResult,
    ["heartbeat"]   = OnHeartbeat,
    ["bye"] 		= OnBye,
    ["chat"]        = OnChat,
    ["answer"]    = OnAnswer,
}

function match.OnServerMsg(msgt)
    local type = msgt.type
    assert(OnServerMsgType[type] and OnServerMsgType[type](msgt))
end

function match.OnDestroy()
    if this.Client.conn then
        this.Client.SendMsgt({
            type = "bye",
            body = {
                clientId = this.clientId,
                roomId = this.roomId
            }
        })
        this.Client.conn:close()
    end
end

return match
