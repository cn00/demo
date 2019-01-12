
### network
* auto adjustable group based local and global network
```lua
{ -- groups
	{ -- group
		type = "group",
		children = {
			{
				type = "user",
			},
			-- ...
		},
		connect = { -- group commanders, connected other groups
			{
				type = "user",
			},
			-- ...
		}
	},
	-- ...
}
```

### bundle pack
* 

### bundle load
load asset bundle

#### task simple
> do a simple load task, finished and call back

#### task group
> do some load task, finished all and call back

#### task chain
> do some load task, then another some, finished all and call back

### update 
* manually check
* show a tip to select update from server or another user
* go on story, run update in background
* if user try to open a system which need to be update, show a notice to wait update
* 


Don't try to talk about your stupid story, Not everyone care about it. Don't be upset, just wait, maybe someday someone interested

序幕
九宫格播放九个玩家从迷雾开始进入各自的世界

初始世界
孩童时期

开场
迷雾开场


现实任务证明
置信度
证明人可信度

关系网络/个人数据/存储
本地存数据库
服务端以用户标识为文件名存文件，结合同名元数据控制更新

点对点交易

数据还原
Node:
    ID
    ParentID
    Data:
        Type

Root←Nod［n］←...←Head


GetObj()
    return GetFromMemory() ??
    GetFromSqlite() ??
    GetFromOss()