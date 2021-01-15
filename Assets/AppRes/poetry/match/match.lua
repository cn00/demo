
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
local util = require "util"
local socket = require("socket.socket")
local sqlite = require("lsqlite3")


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
    round = 0, -- 回合
    roundStartTime = 0, -- 回合开始时间
    roundCountdown = 6, -- 回合读秒
    roundAnswer = -1, -- 当前双方选手的答案
    scoreA = 0, -- 得分
    scoreB = 0,
    poetryList = {}, -- all questions
    cqs = {}, -- currnet questions index
    cqi = -1, -- current question idx [1 - #cqs]
}
local this = match

function match.init(info)
    match.tp = info.pt or 0
end

---myAnswer
---@param idx number
function match.myAnswer(idx)
    local card = match.poetryList[idx]
    card.die = true
    local msg = {
        type = "card_action",
        body = {
            idx = idx,
            id = card.id,
            tp = match.tp,
            owner = card.owner 
        }
    }
    this.Client.ClientSend(msg)
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

end

function match.Start()
    this.Client = this.netClient_LuaMonoBehaviour.Lua
    this.Server = this.netServer_LuaMonoBehaviour.Lua
    print("Start-1", this.netServer_LuaMonoBehaviour, this.Server)

    if(match.tp == 0) then
        this.Server.ServerStart()

        -- local
        this.Client.ClientConnectToServer("10.23.24.239", this.Server.ServerPort, match.OnReceiveMsg)
    end
    
    match.stateStartTime = UnityEngine.Time.now

    this.QRcode_QRCodeEncodeController:onQREncodeFinished('+', function(s)
        print("QRCodeEncode finished", s)
    end)
    
    local ips = CS.NetSys.LocalIpAddressStr()
    local args = {}
    local url = string.format("poetry#%s#%s#%s", ips[0], this.Server.ServerPort, util.dump(args))
    print("qrcode:url", url)
    this.QRcode_QRCodeEncodeController:Encode(url)

    match.LoadData(function()
        playerA:SetActive(true)
        playerB:SetActive(true)
        this.noteText_Text.text = "0"
        local poetryIdList = match.getPoetryIds(40, "where tags like '%思念%'")
        match.poetryList = match.getPoetryByIds(poetryIdList)
        match.distributeCard(match.poetryList)
    end)
end

function match.OnReceiveMsg(msgt)
    local type = msgt.type
    local body = msgt.body
    print("OnReceiveMsg", type)
    if type then
        if type == "login" then
            local cinfo = msgt.body
            this.cinfo = cinfo
        elseif type == "card_action" then
            local card = this.poetryList[body.idx]
            card.dir = true
            card.cc.OnClick()
        end
    end
end


function match.LoadData(cb)
    xutil.coroutine_call(function()
        this.noteText_Text.text = "download data ..."
        local dburl = "db.db"
        local cachePath = dbpath -- AssetSys.CacheRoot .. "db.db"
        if not File.Exists(cachePath) then
            yield_return(AssetSys.Download(dburl, cachePath))
        else
            print("use cache:", cachePath)
        end
        cb()
    end)
end

local function newRound()
    match.roundStartTime = UnityEngine.Time.now
end

function match.Update()
    match.roundCountdown = match.roundCountdown + UnityEngine.Time.deltaTime
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
--	["fanyi"] = "译文沿着汝河大堤走，采伐山楸那枝条。还没见到我夫君，忧如忍饥在清早。沿着汝河大堤走，采伐山楸那余枝。终于见到我夫君，请莫再将我远弃。鳊鱼尾巴色赤红，王室事务急如火。虽然有事急如火，父母穷困谁养活！ 注释⑴遵：循，沿。汝：汝河，源出河南省。坟（fén）：水涯，大堤。⑵条枚：山楸树。一说树干（枝曰条，干曰枚）。⑶君子：此指在外服役或为官的丈夫。⑷惄（nì）：饥，一说忧愁。调（zhōu）：又作“輖”，“朝”（鲁诗此处作“朝”字），早晨。调饥：早上挨饿，以喻男女欢情未得满足。⑸肄（yì）：树砍后再生的小枝。⑹遐（xiá）：远。⑺鲂（fánɡ）鱼：鳊鱼。赬（chēng成）：浅红色。⑻毁（huǐ）：火，齐人谓火为毁。如火焚一样。⑼孔：甚。　迩（ěr）：近，此指迫近饥寒之境。",
--	["tags"] = "诗经|思念",
--}]]
--- 发牌
function match.distributeCard(poetryList)
    --print(util.dump(poetryList[1]))
    local cardRoot = this.playerA_RectTransform
    for i, v in ipairs(poetryList)do
        v.owner = 1
        if i > #poetryList/2 then
            v.owner = 2
            cardRoot = this.playerB_RectTransform
        end
        local c = GameObject.Instantiate(cardTemplate, cardRoot)
        c.name = "card_" .. v.id
        c:SetActive(true)
        
        local cc = c:GetComponent(typeof(CS.LuaMonoBehaviour)).Lua
        v.cc = cc
        cc.info = v
    end
end

function match.OnMouseDown()
    print("OnMouseDown")
end

return match
