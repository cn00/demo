
local CS = CS
local System = CS.System
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local File = CS.System.IO.File

local UnityEditor = CS.UnityEditor
local PBXProject = UnityEditor.iOS.Xcode.PBXProject
local PlistDocument = UnityEditor.iOS.Xcode.PlistDocument
local BuildTarget = UnityEditor.BuildTarget

local XCProject = UnityEditor.XCodeEditor.XCProject


local print = function ( ... )
    _G.print("PatchXcodeProject", ...)
end

local config_tw = {
    overwriteBuildSetting = {
    	ENABLE_BITCODE =  "NO",
        SWIFT_VERSION =  "4.2",
        DEVELOPMENT_TEAM = "A5VNK2KMP9",
        ProvisioningStyle = "Manual",
        CODE_SIGN_STYLE = "Manual",
        SystemCapabilities = "{com.apple.Push = {enabled = 1;};}",
        CODE_SIGN_ENTITLEMENTS = "../iospatch/a3ios.entitlements",
        LD_RUNPATH_SEARCH_PATHS =  "$(inherited) @executable_path/Frameworks",

    	CODE_SIGN_IDENTITY = {
            all = "iPhone Developer",
            Release = "iPhone Distribution",
        },
        PROVISIONING_PROFILE_SPECIFIER = {
            -- all = "ac9b 4e27-4aea-a 086e9 c4230",
            all = "ac9b2032-4e27-4aea-aeb0-086e9abc4230",
            Release = "2e4f2f74-b8df-4da8-b7c4-6803ea87bb8a",
        },

        OTHER_LDFLAGS = {
            "-ObjC",
            "-lresolv",
            "-weak_framework",
            "CoreMotion",
            "-weak-lSystem",
            "-Wl,-undefined,dynamic_lookup",
        },
    }, -- overwriteBuildSetting

    plist = {
        FacebookDisplayName = "A3!",
        FacebookAppID = "2195831977115857",
        CFBundleURLTypes = {
            {
                CFBundleURLName = "googleusercontent",
                CFBundleTypeRole = "Editor",
                CFBundleURLSchemes = {
                    "com.googleusercontent.apps.663290335646-r26u7pvaompfrtn6rbiq7jak4bf4uvcv",
                }
            },
            {
                CFBundleURLName = "fb",
                CFBundleTypeRole = "Editor",
                CFBundleURLSchemes = {
                    "fb2195831977115857",
                }
            },
        }

	}, -- plist
}

local this = {}

---[[// cs usage:
---    var lbytes = File.ReadAllBytes("Assets/Editor/PatchXcodeProject.lua");
---    var lt = LuaSys.Instance.GetLuaTable(lbytes, null, "PatchXcodeProject");
---    var lf = lt.GetLuaFunc("Patch");
---    var arg = LuaSys.Instance.luaEnv.NewTable();
---    args.Set("buildTarget", buildTarget);
---    args.Set("pathToBuiltProject", pathToBuiltProject);
---    lf(lt, args);
---]]
---@param argv table
function this:Patch( argv )
	local buildTarget = argv.buildTarget
	local pathToBuiltProject = argv.pathToBuiltProject
	print(buildTarget, pathToBuiltProject)

	local projectPath = pathToBuiltProject .. "/Unity-iPhone.xcodeproj/project.pbxproj";
	local pbxProject = PBXProject ();
    pbxProject:ReadFromString(File.ReadAllText(projectPath));
    this.pbxProject = pbxProject

    local targetGuid = pbxProject:TargetGuidByName ("Unity-iPhone");
    local config_release = pbxProject:BuildConfigByName(targetGuid, "Release");
    this.config_release = config_release

    local config = config_tw
    this.overwriteBuildSetting(targetGuid, config)
    pbxProject:WriteToFile(projectPath);

    local project = XCProject( pathToBuiltProject );
    project:Save();


    local plistPath = pathToBuiltProject .. "/Info.plist";
    this.UpdatePlist( plistPath, config.plist )

end

---UpdatePlist
---@param plistPath string
---@param config table
function this.UpdatePlist( plistPath, config )
    local plist = PlistDocument();
    plist:ReadFromFile (plistPath);
    this.PassPlistElement(plist.root, config)
    plist:WriteToFile (plistPath);
end

---PassPlistElement
---@param root table
---@param cfg table
function this.PassPlistElement(root, cfg )
    for k, v in pairs(cfg) do
        print("plist", k, v)
        if type(v) ~= "table" then
            root:SetString(k, v)
        else -- table
            if #v > 0 then -- array
                local arr = root:CreateArray(k)
                for i,vv in ipairs(v) do
                    if type(vv) ~= "table" then
                        arr:AddString(vv)
                    else
                        local dic = arr:AddDict()
                        this.PassPlistElement(dic, vv )
                    end
                end
            else -- dictionary
                local dic = root:CreateDict(k)
                this.PassPlistElement(dic, v )
            end
        end
    end
end

---overwriteBuildSetting
---@param targetGuid table
---@param config table
function this.overwriteBuildSetting(targetGuid, config )
    local config_release_guid = this.config_release
    for k,v in pairs(config) do
        print("overwriteBuildSetting", k,v)
        if v.all ~= nil then
            this.UpdateBuildProperty(targetGuid, k, v.all);
        else
            this.UpdateBuildProperty(targetGuid, k, v);
        end
        if v.Release ~= nil then
            this.UpdateBuildPropertyForConfig(config_release_guid, k, v.Release);
        end
    end
end

---UpdateBuildProperty
---@param targetGuid table
---@param key string
---@param v table|string
function this.UpdateBuildProperty(targetGuid, key, v )
	local pbxProject = this.pbxProject
    if (type(v) == "string") then
        pbxProject:SetBuildProperty(targetGuid, key, v);
    elseif (type(v) == "table") then
        --[[
        public void UpdateBuildProperty (string targetGuid, string name, IEnumerable<string> addValues, IEnumerable<string> removeValues);

        public void UpdateBuildProperty (IEnumerable<string> targetGuids, string name, IEnumerable<string> addValues, IEnumerable<string> removeValues);
        ]]
    	pbxProject:UpdateBuildProperty(targetGuid, key, this.Table2CSArray(v), nil)
    end
end

function this.UpdateBuildPropertyForConfig( config_guid, k, v )
    local pbxProject = this.pbxProject
    if (type(v) == "string") then
        pbxProject:SetBuildPropertyForConfig(config_guid, k, v);
    elseif (type(v) == "table") then
        pbxProject:UpdateBuildPropertyForConfig(config_guid, k, this.Table2CSArray(v), nil)
    end
end

function this.Table2CSArray( tab )
    local list = System.Collections.Generic["List`1[System.String]"]()
    for i,v in ipairs(tab) do
        -- print(i,v)
        list:Add(v)
    end
    return list --:ToArray()
end

return this