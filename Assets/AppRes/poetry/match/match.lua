
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


local manager = AppGlobal.manager

local yield_return = xutil.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

-- match

local matchState ={
    waitingForPlayerB = 1,
    cardSending = 2,
    playerPreparing = 3,
    fighting = 4,
    countScore = 5,
}

local print = function ( ... )
    _G.print("[match]", ...)
end

local match = {
    tp = 0, -- 0:主场， 1:客场, 2:观众
    matchState = matchState.playerPreparing,
    stateStartTime = 0, -- 状态开始时间
    playerPreparingCountdown = 10, -- 状态读秒
    round = -1, -- 回合
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
}
local this = match

function match.init(info)
    this.tp = info.tp or 0
    this.hostInfo = info.hostInfo
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
    if this.tp == 0 and idx == this.currentIdx then
        this.scoreA = this.scoreA + 1
    else
        match.poetryList[this.currentIdx].Lua.hidAnswer()
        this.scoreB = this.scoreB + 1
        if card.owner == 1 then
            -- TODO: 罚牌
        end
    end
    
    removeValueFromArray(idx, this.availableIdxs)
    if not removeValueFromArray(idx, this.availableIdxsA) then
        removeValueFromArray(idx, this.availableIdxsB)
    end

    
    if #this.availableIdxsA == 0 then -- A win
        
    elseif #this.availableIdxsB == 0 then -- B win
        
    end
    
    this.scoreText_Text.text = string.format("%s:%s", this.scoreA, this.scoreB)
    
    if tp == this.tp then this.Client.ClientSend(msg) end

    if this.tp == 0 then match.nextRound() end
end

function match.nextRound()
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
    this.Client.ClientSend(msgtopartner)
end

---throwCard 罚牌, 将一张牌调换阵营
---@param role number 1:playerA 2:playerB
function match.throwCard(role)

end

local dbpath = AssetSys.CacheRoot .. "db.db"; -- TODO: fixed db path
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
        this.Client.ClientConnectToServer("localhost", this.Server.ServerPort, match.OnReceiveMsg)
    else
        --this.Server.ServerStart() -- local test client
        print("<color=red>client machine</color>")
        -- remote {"a3mkgp", ip, port, argt}
        this.Client.ClientConnectToServer(this.hostInfo[2], this.hostInfo[3], match.OnReceiveMsg)
    end
    
    match.stateStartTime = UnityEngine.Time.now

    this.QRcode_QRCodeEncodeController:onQREncodeFinished('+', function(s)
        print("QRCodeEncode finished", s)
    end)
    
    local ips = CS.NetSys.LocalIpAddressStr()
    local args = {}
    local url = string.format("a3mkgp:%s:%s:%s", ips[0], this.Server.ServerPort, util.dump(args))
    print("qrcode:url", url)
    this.QRcode_QRCodeEncodeController:Encode(url)

    match.LoadData(function()
        print("LoadData end")
        playerA:SetActive(true)
        playerB:SetActive(true)
        this.noteText_Text.text = this.tp == 0 and "waiting for contest" or "preparing..."
    end)
end

function match.OnReceiveMsg(msgt)
    local type = msgt.type
    local body = msgt.body
    print("OnReceiveMsg", type)
    if type then
        if type == "client_join" then --{{{ host
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
                }
            end
    
            local msgtopartner = {
                type = "distribute_card",
                body = {
                    poetryIdList = poetryIdList,
                    cardsInfo = cardsInfo,
                    youAreMyOpponent = not this.youAreMyOpponent -- TODO: audience
                },
            }
            this.youAreMyOpponent = true
            this.Client.ClientSend(msgtopartner)
    
        elseif type == "partner_ready" then -- host}}}
            -- start the first round
            match.nextRound()
        elseif type == "login" then --{{{ client
            local cinfo = msgt.body
            this.cinfo = cinfo
            local msgtopartner = {
                type = "client_join",
                body = cinfo,
            }
            this.Client.ClientSend(msgtopartner)

        elseif type == "distribute_card" then
            local poetryIdList = body.poetryIdList -- match.getPoetryIds(50, "where tags like '%思念%'")
            match.poetryList = match.getPoetryByIds(poetryIdList)
            match.distributeCard(match.poetryList)

            --[LUA] [client]	ClientSend	{body = {cardsInfo = {{idx = 1,qi = 3,ai = 2,},{idx = 2,qi = 9,ai = 8,},{idx = 3,qi = 1,ai = 2,},{idx = 4,qi = 8,ai = 9,},{idx = 5,qi = 4,ai = 3,},{idx = 6,qi = 7,ai = 8,},{idx = 7,qi = 1,ai = 2,},{idx = 8,qi = 6,ai = 7,},{idx = 9,qi = 9,ai = 8,},{idx = 10,qi = 1,ai = 2,},{idx = 11,qi = 1,ai = 2,},{idx = 12,qi = 2,ai = 1,},{idx = 13,qi = 2,ai = 1,},{idx = 14,qi = 14,ai = 15,},{idx = 15,qi = 25,ai = 24,},{idx = 16,qi = 1,ai = 2,},{idx = 17,qi = 4,ai = 3,},{idx = 18,qi = 2,ai = 1,},{idx = 19,qi = 4,ai = 3,},{idx = 20,qi = 2,ai = 1,},{idx = 21,qi = 4,ai = 3,},{idx = 22,qi = 3,ai = 2,},{idx = 23,qi = 1,ai = 2,},{idx = 24,qi = 16,ai = 15,},{idx = 25,qi = 3,ai = 2,},{idx = 26,qi = 1,ai = 2,},{idx = 27,qi = 4,ai = 5,},{idx = 28,qi = 1,ai = 2,},{idx = 29,qi = 1,ai = 2,},{idx = 30,qi = 1,ai = 2,},{idx = 31,qi = 1,ai = 2,},{idx = 32,qi = 2,ai = 1,},{idx = 33,qi = 12,ai = 11,},{idx = 34,qi = 1,ai = 2,},{idx = 35,qi = 4,ai = 3,},{idx = 36,qi = 2,ai = 3,},{idx = 37,qi = 6,ai = 5,},{idx = 38,qi = 1,ai = 2,},{idx = 39,qi = 7,ai = 6,},{idx = 40,qi = 1,ai = 2,},{idx = 41,qi = 4,ai = 3,},{idx = 42,qi = 1,ai = 2,},{idx = 43,qi = 1,ai = 2,},{idx = 44,qi = 2,ai = 1,},{idx = 45,qi = 2,ai = 1,},{idx = 46,qi = 2,ai = 1,},{idx = 47,qi = 1,ai = 2,},{idx = 48,qi = 4,ai = 5,},{idx = 49,qi = 2,ai = 1,},{idx = 50,qi = 1,ai = 2,},},poetryIdList = {10,14,33,67,71,109,126,154,167,566,1230,1417,1775,1782,1845,1889,2504,2784,2845,2870,3524,4745,5124,5256,5480,5585,5770,5773,5775,5786,5796,5957,6432,6441,7100,7727,7739,7753,7757,7768,7807,7822,7826,7827,7833,8050,8347,8488,8493,8663,},youAreMyOpponent = "true",},type = "distribute_card",}
            local ci = body.cardsInfo
            for i, v in ipairs(this.poetryList) do
                v.idx = ci[i].idx
                v.ai  = ci[i].ai
                v.qi  = ci[i].qi
            end

            local msgtopartner = {
                type = "partner_ready",
                body = {},
            }
            this.Client.ClientSend(msgtopartner)
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

        elseif type == "byebye" then
            manager.Scene.pop()
        elseif type == "audience_join" then --{{{ audience
        end
    end
end


function match.LoadData(cb)
    xutil.coroutine_call(function()
        this.noteText_Text.text = "download data ..."
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
        this.noteText_Text.text = string.format("%s:%s", this.round, math.modf(UnityEngine.Time.time - match.roundStartTime))
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
--	["about"] = "《毛诗序》以为是赞美文王的教化在汝坟这个国家施行的很好，妇人能劝诫丈夫尽力正直卫国而流传下来的民歌。但是近人大多认为这是妻子挽留久役归来的征夫而唱的诗歌。",
--	["fanyi"] = "注释⑴遵：循，沿。汝：汝河，源出河南省。坟（fén）：水涯，大堤。⑵条枚：山楸树。一说树干（枝曰条，干曰枚）。⑶君子：此指在外服役或为官的丈夫。⑷惄（nì）：饥，一说忧愁。调（zhōu）：又作“輖”，“朝”（鲁诗此处作“朝”字），早晨。调饥：早上挨饿，以喻男女欢情未得满足。⑸肄（yì）：树砍后再生的小枝。⑹遐（xiá）：远。⑺鲂（fánɡ）鱼：鳊鱼。赬（chēng成）：浅红色。⑻毁（huǐ）：火，齐人谓火为毁。如火焚一样。⑼孔：甚。　迩（ěr）：近，此指迫近饥寒之境。",
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

return match
