
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/08 19:18:14
--- Description: 
--[[

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
local config = require("config.config.config")

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
}

local print = function ( ... )
    _G.print("[match]", ...)
end

local match = {
    tp = 0, -- 0:主场， 1:客场, 2:观众
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
    poetryList = nil, -- all questions
    availableIdxs = {}, -- available questions index
    availableIdxsA = {}, -- 
    availableIdxsB = {}, -- 
    currentIdx = -1, -- current question idx [1 - #cqs]
    myname = "libai",
    loginInfo = nil,
    clientId = -1, -- 每次登录服务器时分配的 id 会变
    clientsId = {}, -- 房间成员
    cardIds = nil, -- 房间卡牌id
    clientName = "",
    cinfo = nil,  -- client info {myname ,id}
    roomId = nil, -- 加入的房间信息
    clientsInfo = {}, -- {[clientId] = {}}
}
local this = match

function match.init(info)
    this.tp = info.tp or 0
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
function match.playerAnswer(idx, tp)
    if this.currentIdx < 0 then print("game not start yet.")return end

    this.roundAnswer = -1

    xutil.coroutine_call(function()
        local card = match.poetryList[idx]
        card.Lua.hid()
        card.die = true
        local msg = {
            type = "card_action", -- or "card_action_b",
            body = {
                idx = idx,
                id = card.id,
                tp = tp or this.tp,
                owner = card.owner
            }
        }

        if tp == this.tp then
            this.Client.SendMsgt(msg)

            if idx == this.currentIdx then
                this.scoreA = this.scoreA + 1
            else
                this.poetryList[this.currentIdx].Lua.hidAnswer() -- 
                this.scoreB = this.scoreB + 1
                if card.owner == 1 then
                    -- TODO: 罚牌
                end
            end
        else
            if idx == this.currentIdx then
                this.scoreB = this.scoreB + 1
            else
                match.poetryList[this.currentIdx].Lua.hidAnswer()
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

        -- show answer
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
            if this.tp == 0 then match.nextRound() end
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
        local i = math.random(1, #this.availableIdxs)
        this.currentIdx = this.availableIdxs[i]
        this.round = #this.poetryList - #this.availableIdxs
        local card = this.poetryList[this.currentIdx]
        card.Lua.showAnswer()
        this.question_Text.text = card.content[card.qi]

        local msgtopartner = {
            type = "next_round",
            body = {
                currentIdx=this.currentIdx,
                round = this.round
            },
        }
        this.Client.SendMsgt(msgtopartner)
        print("host tts",  string.gsub(card.content[card.qi], "\n", "\\n"))
        CS.App.JavaUtil.Call("com.unity3d.player.TTS", "Say", card.content[card.qi])
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
function match.getPoetryByIds(ids)
    local sids = table.concat(ids, ",")
    local db = sqlite.open(dbpath)
    local sql = string.format([[select * from poetry where id in (%s)]], sids)
    local res = {}
    local idx = 0
    for row in db:nrows(sql) do
        idx = idx + 1
        row.content = row.content
                :gsub("(。)%s*", "%1|")
                :gsub("(？)%s*", "%1|")
                :gsub("(！)%s*", "%1|")
                :split("|")
        row.qi = math.random(1, #row.content)
        local ai = row.qi == #row.content and row.qi-1 or row.qi+1
        if row.qi > 1 and row.qi < #row.content and math.random(0,100) > 50 then  ai = row.qi-1 end -- 前一句
        row.ai = ai
        row.idx = idx
        row.match = this
        res[1+#res] = row
    end
    db:close()
    return res
end

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.BackBtn_Button = BackBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.BackBtn_Button.onClick:AddListener(this.BackBtn_OnClick)
    this.cardTemplate_RectTransform = cardTemplate:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.netClient_LuaMonoBehaviour = netClient:GetComponent(typeof(CS.LuaMonoBehaviour))
    this.netServer_LuaMonoBehaviour = netServer:GetComponent(typeof(CS.LuaMonoBehaviour))
    this.noteText_Text = noteText:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.playerA_RectTransform = playerA:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.playerB_RectTransform = playerB:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.QRcode_QRCodeEncodeController = QRcode:GetComponent(typeof(CS.QRCodeEncodeController))
    this.question_Text = question:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.scoreText_Text = scoreText:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.ShowQRcodeBtn_Button = ShowQRcodeBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.ShowQRcodeBtn_Button.onClick:AddListener(this.ShowQRcodeBtn_OnClick)
end
--AutoGenInit End

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
    this.Server = this.netServer_LuaMonoBehaviour.Lua
    print("Start-1", this.tp, this.Server)

    if(match.tp == 0) then
        this.Server.ServerStart()
        print("<color=red>server machine</color>")
        
        -- local
        this.Client.ClientConnectToServer("localhost", this.Server.ServerPort, this.OnServerMsg)

        local ips = CS.NetSys.LocalIpAddressStr()
        local args = {}
        local url = string.format("a3mkgp:%s:%s:%s", ips.Length > 0 and ips[0] or "127.0.0.1", this.Server.ServerPort, util.dump(args))
        print("qrcode:url", url)
        this.QRcode_QRCodeEncodeController:Encode(url)
    else
        --this.Server.ServerStart() -- local test client
        print("<color=red>client machine</color>")
        -- remote {"a3mkgp", ip, port, argt}
        this.Client.ClientConnectToServer(this.hostInfo[2], this.hostInfo[3], this.OnServerMsg)
    end
    
    match.stateStartTime = UnityEngine.Time.now

    this.QRcode_QRCodeEncodeController:onQREncodeFinished('+', function(s)
        print("QRCodeEncode finished", s)
    end)

    match.LoadData(function()
        print("LoadData end")
        playerA:SetActive(true)
        playerB:SetActive(true)
        this.noteText_Text.text = this.tp == 0 and "等待对手上线..." or "对手正在酝酿中..."
    end)
end

function match.OnReceiveMsg(msgt)
    local type = msgt.type
    local body = msgt.body
    print("OnReceiveMsg", type)
    if type then
        if type == "client_join" then --{{{ host
            this.clientName = body.myname
            local poetryIdList = this.poetryIdList or this.getPoetryIds(50, "where tags like '%思念%'")
            if not this.youAreMyOpponent then
                this.poetryIdList = poetryIdList
                this.poetryList = this.getPoetryByIds(poetryIdList)
                this.distributeCard(this.poetryList)
            end

            local cardsInfo = {}
            for i, v in ipairs(this.poetryList) do
                cardsInfo[1+#cardsInfo] = {
                    idx = v.idx,
                    ai = v.ai,
                    qi = v.qi,
                    die = v.die,
                }
            end
    
            local msgtopartner = {
                type = "distribute_card",
                body = {
                    myname = this.myname,
                    poetryIdList = poetryIdList,
                    cardsInfo = cardsInfo,
                    youAreMyOpponent = not this.youAreMyOpponent -- TODO: audience
                },
            }
            this.youAreMyOpponent = true
            this.Client.SendMsgt(msgtopartner)
            this.noteText_Text.text = "等待对手准备就绪"
        elseif type == "partner_ready" then -- host}}}
            -- start the first round
            this.startTime = UnityEngine.Time.time
            match.nextRound()
        elseif type == "hello" then --{{{ client
            local cinfo = msgt.body
            cinfo.myname = this.myname
            this.cinfo = cinfo
            local msgtopartner = {
                type = "client_join",
                body = cinfo,
            }
            this.Client.SendMsgt(msgtopartner)

        elseif type == "distribute_card" then
            this.clientName = body.myname
            local poetryIdList = body.poetryIdList -- match.getPoetryIds(50, "where tags like '%思念%'")
            match.poetryList = match.getPoetryByIds(poetryIdList)

            --[[{body = {
                cardsInfo = {{idx = 1,qi = 3,ai = 2,die = true},,...},
                poetryIdList = {10,14,33,...},youAreMyOpponent = "true",},
                type = "distribute_card",}
            ]]
            local ci = body.cardsInfo
            for i, v in ipairs(this.poetryList) do
                v.idx = ci[i].idx
                v.ai  = ci[i].ai
                v.qi  = ci[i].qi
                v.die = ci[i].die
            end
            match.distributeCard(match.poetryList)

            local msgtopartner = {
                type = "partner_ready",
                body = {},
            }
            this.Client.SendMsgt(msgtopartner)
        elseif type == "card_action" then
            local card = this.poetryList[body.idx]
            this.roundAnswer = body.idx
            card.die = true
            card.Lua.OnClick()
            this.playerAnswer(body.idx, body.tp)
        elseif type == "next_round" then
            this.currentIdx = body.currentIdx
            this.round = body.round
            this.roundAnswer = -1
            this.roundStartTime = UnityEngine.Time.time
            local card = this.poetryList[body.currentIdx]
            card.Lua.showAnswer()
            this.question_Text.text = card.content[card.qi]
            print("tts",  string.gsub(card.content[card.qi], "\n", "\\n"))
            CS.App.JavaUtil.Call("com.unity3d.player.TTS", "Say", card.content[card.qi])
            print("tts 1")
        elseif type == "byebye" then
            manager.Scene.pop()
        elseif type == "audience_join" then --{{{ audience
        end
    end
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
end

--[[{
--	["shangxi"] = "鉴赏 　　这在诗之首章，“遵彼汝坟，伐其条枚”——在高高的汝河大堤上，有一位凄苦的妇女，正手执斧子砍伐山楸的树枝。",
--	["ai"] = 6,
--	["id"] = 10,
--	["content"] = {
--		[1] = "遵彼汝坟，伐其条枚。",
--		[2] = "未见君子，惄如调饥。",
--		[3] = "遵彼汝坟，伐其条肄。",
--		[4] = "既见君子，不我遐弃。",
--		[5] = "鲂鱼赪尾，王室如毁。",
--		[6] = "虽则如毁，父母孔迩。",
--	},
--	["name"] = "汝坟",
--	["qi"] = 7,
--	["poet_id"] = 0,
--	["star"] = 330,
--	["poet_name"] = "佚名",
--	["dynasty"] = "先秦",
--	["about"] = "《毛诗序》以为是赞美文王的教化在汝坟这个国家施行的很好，...",
--	["fanyi"] = "注释⑴遵：循，沿。汝：汝河，源出河南省。坟（fén）：...",
--	["tags"] = "诗经|思念",
--}]]
--- 发牌
function match.distributeCard(poetryList)
    --print(util.dump(poetryList[1]))
    local cardRootA, cardRootB = this.playerA_RectTransform,this.playerB_RectTransform
    local idsA, idsB= this.availableIdxsA,this.availableIdxsB
    local ownerA, ownerB = 1, 2
    if this.tp == 1 then
        ownerA, ownerB = 2, 1
        idsB, idsA = this.availableIdxsA,this.availableIdxsB
        cardRootB, cardRootA = this.playerA_RectTransform,this.playerB_RectTransform
    end
    for i, v in ipairs(poetryList)do
        this.availableIdxs[1+#this.availableIdxs] = i
        v.owner = 1
        local c
        if i < #poetryList/2 then
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

        local Lua = c:GetComponent(typeof(CS.LuaMonoBehaviour)).Lua
        Lua.info = v
        v.Lua = Lua
    end
end

function match.OnMouseDown()
    print("OnMouseDown")
end



---OnConnect
---@param msgt table body = {id}
local function OnConnect(msgt)
    this.clientId = msgt.body.clientId
end

---OnLoginResult
---@param msgt table body = {success}
local function OnLoginResult(msgt)
    this.loginInfo = msgt.body
end

---OnCreateRoomResult
---@param msgt table body = {roomId, masterId, clients = {}}
local function OnCreateRoomResult(msgt)
    local roomId = msgt.body.roomId
    this.roomId = roomId
    this.roomList[roomId].clientIds = msgt.body
    return true
end

---OnRoomListResult
---@param msgt table body = {{roomId, client={{id,name}, ...}}, ...}
local function OnRoomListResult(msgt)
    this.roomList = msgt.body
end

---OnJoinRoomResult
---@param msgt table body = {roomId, clientId}
local function OnJoinRoomResult(msgt)
    local body = msgt.body
    this.roomId = body.roomId
    this.clientsId = body.clientsId
    this.cardIds = body.cardIds
    -- TODO: 查看房间及卡牌信息 refresh ui
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
            -- TODO: show startBtn
        else

        end
    end
end

local function OnNextround(msgt)
    local body = msgt.body
    this.currentIdx = body.currentIdx
    this.nextRound()
end

---OnSendCard
---@param msgt table body = {cardId, cardInfo={state}}
local function OnSendCard(msgt)

end

---OnCardAction
---@param msgt table body = {cardId, action}
local function OnCardAction(msgt)

end

---OnMatchResult
---@param msgt table body = {result}
local function OnMatchResult(msgt)

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
    ["nextround"]	= OnNextround,
    ["sendCard"]	= OnSendCard,
    ["cardAction"]	= OnCardAction,
    ["matchResult"]	= OnMatchResult,
}

function match.OnServerMsg(msgt)
    local type = msgt.type
    assert(OnServerMsgType[type] and OnServerMsgType[type](msgt))
end

return match
