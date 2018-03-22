
#### 资源打包配置

* 导出配置在 `BundleConfig` 类中定义

* 需要打包的资源目录和场景目录分别配置在 `ABResRoots` 和 `ABSceneRoots` 中

* 打包的资源目录全部在 `Assets/ABResources` 中, 打包时会自动添加此前缀目录

* 可在 `Assets/Config/BundleConfig.asset` 的 `Inspector` 面板中对勾选目录进行单独打包, 而不是全部目录, 方便开发测试时快速打包测试, TODO: `Inspector` 面板中的 `PreDownload` 打包时暂未使用


#### 资源打包目录说明

* 总揽:
```
ABResources
├── 分类目录
│   ├── AssetBundle 目录
│   │   ├── 内容(文件或文件夹)
... ... ...
```

* `ABResources` 目录为 `AssetBundle` 打包源目录

* 其下每一个子文件夹为一个 `AssetBundle` 分类目录

* 每一个分类目录下的子目录作为一个 `AssetBundle`

* 每个资源可通过 `ABResources` 后两段目录定位所在包, 
```
例如: `ABResources/pathTo/bundle/subPath/asset.xxx`
可通过截取前两段目录获得包路径: pathTo/bundle.bd
然后通过全路径 LoadAsset 获取资源
```

* 每一个 `AssetBundle` 尽可能减少依赖, 公共资源放在每个根目录下的 `Common` 文件夹下

* 关卡场景 `StreamingScene` 和 关卡 Xml 数据导出目录为 `Assets/ABResources/Level` 

* `PreDownload` 文件夹下的资源打成 `AssetBundle` 后将预先更新下载, 其他目录将在首次加载时更新下载

* 资源服务器地址在 `BundleSys.HttpRoot`, 检查更新将拉取此地址资源

* 示例:
```
ABResources
├── PreDownload			// 预下载资源目录, 此目录下资源会在检查更新时全部更新
│   ├── Common1/		
│   ├── Common2/
│  ...
├── Effect				// 分类目录
│   ├── CommonEffect/			// 其下的所有资源包括所有子目录会被打成一个包 Effect/Common.assetBundle
│   ├── AttackRange/	// Effect/AttackRange.assetBundle
│   ├── SkillRange/
│   ├── textures/
│  ...
├── UI
│   ├── CommonUI/			// UI 共享的资源, 如公用边框, 材质
│   ├── Login/			// Login 界面的特有资源放在这里面, 如纹理, 材质, 预制体等
│   ├── Blood/
│   ├── Battle_1/
│  ...
├── Level				// 导出的关卡资源
│   ├── s1/				// 关卡1的地图 s1_terrain.unity, 静态关卡数据 s1.xml 放在这里面
│   ├── s2/
│  ...
├── Hero
│   ├── CommonHero/			// 角色模型共享的资源
│   ├── C1001/			// 角色模型 C1001 的预制体, 依赖的网格, 材质, 纹理全部放在这里面
│   ├── C1002/
│  ...
├── Lua 				// Lua 脚本目录, 此目录下每个文件夹为一个 `AssetBundle`
│   ├── lua/			// 加载使用 require "lua.XXX.YYY...."
│   ├── UI/				// 加载使用 require "UI.XXX.YYY...."
│   ├── Table/			// 加载使用 require "Table.XXX.YYY...."
│   ├── Skill/
│   ├── Hero/
│  ...
└── README.md
```

#### 打包资源使用说明

* 使用打包资源接口在 `BundleSys` 中, 统一通过协程加载

* 通过传入的回调 `BundleCallback` 返回需要加载的资源

* 加载时自动加载依赖包

* 第一层 `GetBundle(string rootName, string bundleName, string resName, BundleCallback callBack = null)`

* 第二层 `GetXXXBundle(string bundleName, string resName, BundleCallback callBack = null)`, 这一层将第一层的 `rootName` 并入函数名中, 建议新添加的包都封装这样一个对应的加载接口, 避免直接使用第一层接口

* CSharp 示例:
```CSharp
	var resName = "Level/" + mName + "/" + mName + ".xml";
	yield return BundleSys.Instance.GetSceneInfoBundle(mName + ".fg", resName, (Object bundle) =>
	{
	    mInfo = (bundle as TextAsset).GetXml<SceneInfo>();
	});
```

* 注意: `resName` 应当是打包前资源所在路径相对与 `Assets/ABResources` 的子路径, 包括扩展名, 例如:
打包前的 
`"Assets/ABResources/Level/s1/s1.xml"`, 
`resName` 应传入 
`"Level/s1/s1.xml"`

* Lua 示例 `Assets/ABResources/Lua/lua/UI/ABExamplePanel.lua`
```Lua
	function ABExamplePanel:OpenPanel()
		if(self.gameObject == nil) then
			CS.AudioSys.Instance:PlayBG("bg_main.mp3")
			CS.BundleSys.Instance:GetUIPrefab("abexample.fg", "ABExample", function ( obj )
		    	self:LoadCallBack(obj)
			end)
		end
		
	end
```