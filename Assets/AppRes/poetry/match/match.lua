
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

local stringx = require("stringx") -- split
local xutil = require "xlua.util"
local util = require "util"
local socket = require("socket.socket")
local sqlite = require("lsqlite3")

local yield_return = xutil.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)
-- match

local conn_stat = {
    offline = 0,
    connecting = 1,
    connected = 2,
}

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
    round = 0, -- 回合
    roundCountdown = 6, -- 回合读秒
    roundAnswer = -1, -- 当前双方选手的答案
    scoreA = 0, -- 得分
    scoreB = 0,
    poetryList = {}, -- all questions
    cqs = {}, -- currnet questions index
    cqi = -1, -- current question idx [1 - #cqs]
    clients = {}, -- tcp clients
    clientsInfo = {}, -- clients info: client = {status = 0}
}
local this = match

---myAnswer
---@param idx number
function match.myAnswer(idx)
    local card = match.poetryList[idx]
    card.die = true
    local msg = {tp = match.tp, idx = idx, id = card.id, owner = card.owner}
    match.ClientSend(msg)
end

---throwCard 罚牌, 将一张牌调换阵营
---@param role number 1:playerA 2:playerB
function match.throwCard(role)

end

local dbpath = "/Volumes/Data/test/ab/Android/db.db"; -- TODO: fixed db path
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
        row.content = row.content:gsub("(。)%s*", "%1|")
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
    this.cardTemplate_RectTransform = cardTemplate:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.noteText_Text = noteText:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.playerA_RectTransform = playerA:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.playerB_RectTransform = playerB:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.scoreText_Text = scoreText:GetComponent(typeof(CS.UnityEngine.UI.Text))
end
--AutoGenInit End

function match.Awake()
	this.AutoGenInit()
    cardTemplate:SetActive(false)
    this.scoreText_Text.text = "0:0"
end

function match.Start()
	--util.coroutine_call(this.coroutine_demo)
    match.ServerStart()

    local poetryIdList = match.getPoetryIds(40, "where tags like '%思念%'")
    match.poetryList = match.getPoetryByIds(poetryIdList)
    match.distributeCard(match.poetryList)
end

function match.Update()
    match.roundCountdown = match.roundCountdown + UnityEngine.Time.deltaTime
    print("match.roundCountdown", match.roundCountdown)
end

--[[{
--	["shangxi"] = "鉴赏 　　这在诗之首章，“遵彼汝坟，伐其条枚”——在高高的汝河大堤上，有一位凄苦的妇女，正手执斧子砍伐山楸的树枝。其实已透露了消息，采樵伐薪，本该是男人担负的劳作，现 在却由织作在室的妻子承担了。读者不禁要问：她的丈夫究竟到哪里去了？竟就如此忍心让妻子执斧劳瘁！“未见君子，惄如调饥”二句的跳出，即隐隐回答了此中缘由：原来，她的丈夫久已行役外出，这维持生计的重担，若非妻子没有人能来肩负。“惄”者忧也，“调饥”者朝食未进也。满腹的忧愁用朝“饥”作比，自然只有饱受饥饿折磨的人们，方有的真切感受。那么，这倚徙“汝坟”的妻子，想必又是忍着饥饿来此伐薪的了，此为文面之意。“朝饥”还有一层意思，它在先秦时代往又被用来作男欢女爱的隐语。而今丈夫常年行役，他那可怜的妻子，享受不到丝毫的眷顾和关爱。这便是首章展示的女主人公境况：她孤苦无依、忍饥挨饿，大清早便强撑衰弱之身采樵伐薪。当凄凉的秋风吹得她衣衫飘飘，大堤上传送来一声声“未见君子，惄如调饥”的怆然叹息时，令人闻之而酸鼻。 　　第二章诗情发生了意外的转折。“遵彼汝坟，伐其条肄”二句，不宜视为简单的重复：“肄”指树木砍伐后新长的枝条，它点示了女主人公的劳瘁和等待，秋往春来又捱过了一年。忧愁悲苦在岁月漫漫中延续，期待也许早已化作绝望，此刻却意外发现了“君子”归来的身影。于是“既见君子，不我遐弃”二句，便带着女主人公突发的欢呼涌出诗行。不过它们所包含的情感，似乎又远比“欢呼”要丰富和复杂：久役的丈夫终于归来，他毕竟思我、爱我而未将我远弃，这正是悲伤中汹涌升腾的欣慰和喜悦；但归来的丈夫还会不会外出，他是否还会将我抛在家中远去？这疑虑和猜思，难免又会在喜悦之余萌生；然而此次是再不能让丈夫外出的了，他不能将可怜的妻子再次远弃。这又是喜悦、疑虑中发出的深情叮咛了。如此种种，实难以一语写尽，却又全为“不我遐弃”四字所涵容——《国风》对复杂情感的抒写，正是如此淳朴而又婉曲。 　　女主人公的疑虑并非多余。第三章开首两句，即以踌躇难决的丈夫口吻，无情地宣告了他还得弃家远役：正如劳瘁的鳊鱼曳着赤尾而游，在王朝多难、事急如火之秋，他丈夫不可能耽搁、恋家。形象的比喻，将丈夫远役的事势渲染得如此窘急，可怜的妻子欣喜之余，又很快跌落到绝望之中。当然，绝望中的妻子也未放弃最后的挣扎：“虽则如毁，父母孔迩！”这便是她万般无奈中向丈夫发出的凄凄质问：家庭的夫妇之爱，纵然已被无情的徭役毁灭；但是濒临饥饿绝境的父母呢，他们的死活不能不顾。 　　全诗在凄凄的质问中戛然收结，征夫对此质问又能作怎样的回答。这质问其实贯串了亘古以来的整整一部历史：当惨苛的政令和繁重的徭役，危及每一个家庭的生存，将支撑“天下”的民众逼到“如毁”、“如汤”的绝境时，历史便往往充满了这样的质问。《周南·汝坟》在几经忧喜和绝望后发出的质问，虽然化作了结句中征夫的不尽沉默。但是读者却分明听到了此后不久历史所发出的巨大回音：那便是西周王朝的轰然崩塌。赏析 　　对于这首诗的主旨，《毛诗序》以为是赞美“文王之化行乎汝坟之国，妇人能闵其君子犹勉之以正也”；汉刘向《列女传》更附会其说，指实此乃“周南大夫”之妻所作，恐其丈夫“懈于王事”，故“言国家多难，惟勉强之，无有谴怒遗父母忧”也。《韩诗章句》则以为，此乃妇人“以父母迫近饥寒之忧”，而劝夫“为此禄仕”之作，显然并无赞美“文王之化”的“匡夫”之义。近人大多不取毛、韩之说，而解为妻子挽留久役归来的征夫之作，笔者以为似更切近诗意。",
--	["ai"] = 6,
--	["id"] = 10,
--	["content"] = {
--		[1] = "遵彼汝坟，伐其条枚。",
--		[2] = "未见君子，惄如调饥。",
--		[3] = "遵彼汝坟，伐其条肄。",
--		[4] = "既见君子，不我遐弃。",
--		[5] = "鲂鱼赪尾，王室如毁。",
--		[6] = "虽则如毁，父母孔迩。",
--		[7] = "",
--	},
--	["name"] = "汝坟",
--	["qi"] = 7,
--	["poet_id"] = 0,
--	["star"] = 330,
--	["poet_name"] = "佚名",
--	["dynasty"] = "先秦",
--	["about"] = "<h2>影响</h2> <p>　　相传为孔子编辑成书，集入西周至春秋中叶五百多年的作品305篇，分为风雅颂三个类别。而其中的《汝坟》则是我们能见到的歌颂汝州风土人情最早的一首诗。该诗写一位妇女在汝河岸边一边砍柴，一边思念远征未归的丈夫。全诗用语简洁，比喻奇特，思念和哀怨化作缕缕青丝，弥漫于字里行间，纯情感人。风土人情是民族文化的基础，《汝坟》在展示民族文化，促进我国的诗歌创作中产生了极其深远的影响。</p><br/><h2>创作背景</h2> 　　这首诗的背景，《毛诗序》以为是赞美文王的教化在汝坟这个国家施行的很好，妇人能劝诫丈夫尽力正直卫国而流传下来的民歌。但是近人大多认为这是妻子挽留久役归来的征夫而唱的诗歌。",
--	["fanyi"] = "译文沿着汝河大堤走，采伐山楸那枝条。还没见到我夫君，忧如忍饥在清早。沿着汝河大堤走，采伐山楸那余枝。终于见到我夫君，请莫再将我远弃。鳊鱼尾巴色赤红，王室事务急如火。虽然有事急如火，父母穷困谁养活！ 注释⑴遵：循，沿。汝：汝河，源出河南省。坟（fén）：水涯，大堤。⑵条枚：山楸树。一说树干（枝曰条，干曰枚）。⑶君子：此指在外服役或为官的丈夫。⑷惄（nì）：饥，一说忧愁。调（zhōu）：又作“輖”，“朝”（鲁诗此处作“朝”字），早晨。调饥：早上挨饿，以喻男女欢情未得满足。⑸肄（yì）：树砍后再生的小枝。⑹遐（xiá）：远。⑺鲂（fánɡ）鱼：鳊鱼。赬（chēng成）：浅红色。⑻毁（huǐ）：火，齐人谓火为毁。如火焚一样。⑼孔：甚。　迩（ěr）：近，此指迫近饥寒之境。",
--	["tags"] = "诗经|思念",
--}]]
--- 发牌
function match.distributeCard(poetryList)
    local dump = require("dump")
    print(dump(poetryList[1]))
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

---------------------server 作为主机-------------------{{{{

---ServerStart 
function match.ServerStart()
    local port= 9999
    local server, err = socket.bind("*", port)
    if err == nil then
        match.server = server
        match.ServerStartAcceptLoop()
        match.ServerStartReceiveLoop()
        print("StartServer ok, listen on:", port)

        -- local test
        match.ClientConnectToServer("10.23.24.239", port)
    else
        print("StartServer failed.", err)
    end
end

function match.ServerStartAcceptLoop()
    xutil.coroutine_call(function()
        print("waiting for client")
        local errmsg = nil
        while true do
            match.server:settimeout(0.01)
            local client, err = match.server:accept()
            if client and not err then
                match.clients = match.clients or {}
                match.clientsInfo = match.clientsInfo or {}
                match.clients[1+#match.clients] = client
                match.clientsInfo[client] = {status = 0}
                print("accept client", client, #match.clients)
                client:send("wellcom "..tostring(client).."\n")
            else
                if errmsg ~= err then
                    errmsg = err
                    print("accetp err", err)
                end
            end
            yield_return(UnityEngine.WaitForSeconds(0.3))
        end
    end)
end

function match.ServerStartReceiveLoop()
    xutil.coroutine_call(function()
        while true do
            local canread, sendt, status = socket.select(this.clients, nil, 0.001)
            -- print("canread", #canread, #this.client)
            for _, c in ipairs(canread) do
                c:settimeout(0.1)
                local line, err = c:receive("*l")
                --print("receive", c, line and line:gsub("[\0-\13]",""), err)

                if not err then
                    -- TODO: pass msg here
                    for i, v in ipairs(this.clients) do
                        if v ~= c then
                            v:send(line)
                            v:send("\n")
                        else
                            print("self msg, skip")
                        end
                    end
                elseif(err == "closed")then
                    this.connect_stat = conn_stat.offline
                    c:close()
                    -- this.ondisconnect( c )
                else
                    c:send("server receive __ERROR__".. err .. tostring(c))
                end
                ::continue::
            end

            yield_return(UnityEngine.WaitForSeconds(0.3))
        end
    end)
end

---------------------server end-------------------}}}}



----------------------client 作为客户端---------------------

function match.ClientConnectToServer(ip, port)
    if this.conn ~= nil and this.conn:getstats() == 1 then
        --https://stackoverflow.com/questions/4160347/close-vs-shutdown-socket
        -- this.conn:shutdown()
        this.conn:close()
        this.conn = undef
    end
    xutil.coroutine_call(function ()
        this.connect_stat = conn_stat.connecting
        local retrycount = 0
        while (this.connect_stat == conn_stat.connecting and retrycount < 10) do
            print("try connect ...", retrycount)
            retrycount = 1 + retrycount
            local conn, err = socket.connect(ip, port)
            if err == nil and conn then
                this.conn = conn
                this.connect_stat = conn_stat.connected
                print("<color=green>connected to server ok</color>.")
                -- else if err == "connection refused" then
                -- 	print(err)
            else
                print("connect err", err)
            end
            yield_return(UnityEngine.WaitForSeconds(0.3))
        end -- while
        match.ClientStartReceiveLoop()
    end)
end

function match.ClientStartReceiveLoop()
    print('ClientStartReceiveLoop')
    while true
    do
        local canread, sendt, status = socket.select({this.conn}, nil, 0.001)
        -- print("canread", #canread, #this.client)
        for _, c in ipairs(canread) do
            c:settimeout(0.1)
             local line, err = c:receive("*l")
             print("<color=red>client receive</color>", #line, line:gsub("[\0-\13]",""), err)

            if not err then
                match.ClientOnReceiveMsg(line)
            elseif(err == "closed")then
                this.connect_stat = conn_stat.offline
                c:close()
            else
                c:send("___ERRORPC "..err..tostring(c)..  "\n")
            end
            ::continue::
        end

        yield_return(UnityEngine.WaitForSeconds(0.3))
    end
end

function match.ClientOnReceiveMsg(msgs)
    local f = load(msgs)
    if f then
        local msgt = f()
        print("ClientOnReceiveMsg", msgt)
        local card = match.poetryList[msgt.idx]
        card.dir = true
        card.cc.OnClick()
    end
end

---ClientSend
---@param msg table
function match.ClientSend(msg)
    if this.conn ~= nil and type(msg) then
        local msgs = table.dump(msg,false)
        print("ClientSend", msgs)
        this.conn:send("return ")
        this.conn:send(msgs)
        this.conn:send("\n")
    end
end
----------------------client end---------------------

function match.OnMouseDown()
    print("OnMouseDown", mouseDeltaWorld)
end

function match.OnDestroy()
    if this.clients and #this.clients > 0 then
        for i, v in ipairs(this.clients) do
            v:close()
        end
    end
    if match.server ~= nil then
        match.server:close()
        print("shutdown server")
    end
end

return match
