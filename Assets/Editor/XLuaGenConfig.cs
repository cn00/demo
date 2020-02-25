/*
* Tencent is pleased to support the open source community by making xLua available.
* Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
* Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
* http://opensource.org/licenses/MIT
* Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XLua;
using System.Runtime;
using TableView;
#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

//配置的详细介绍请看Doc下《XLua的配置.doc》
public static class XLuaGenConfig
{
    //lua中要使用到C#库的配置，比如C#标准库，或者Unity API，第三方库等。
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>()
    {
        #region customer types
        typeof(TableView.TableView),
        typeof(TableView.TableViewController),
        typeof(TableView.TableViewCell),
               
        typeof(NPOI.SS.UserModel.ICell),
        typeof(NPOI.SS.UserModel.IRow),
        typeof(NPOI.SS.UserModel.ISheet),
        typeof(List<NPOI.SS.UserModel.ICell>),
        typeof(List<NPOI.SS.UserModel.IRow>),
        typeof(List<NPOI.SS.UserModel.ISheet>),
        typeof(NPOI.SS.UserModel.IWorkbook),
        typeof(NPOI.HSSF.UserModel.HSSFWorkbook),
        typeof(NPOI.XSSF.UserModel.XSSFWorkbook),
        typeof(ExcelUtils),

        typeof(AssetSys),
        typeof(UpdateSys),
        typeof(LuaSys),
        typeof(LuaEnv),
        typeof(LuaMonoBehaviour),
        typeof(NetSys),
        //typeof(QRCodeEncodeController),
        //typeof(QRCodeDecodeController),
        //typeof(JavaUtil),
        typeof(AppLog),
        
        // ios not ok
        // typeof(Mono.Data.Sqlite.SqliteConnection),
        // typeof(Mono.Data.Sqlite.SqliteCommand),
        // typeof(Mono.Data.Sqlite.SqliteDataReader),
        #endregion customer types

        #region system
        typeof(System.IO.File),
        typeof(System.Text.Encoding),
        typeof(System.Object),
        typeof(System.Delegate),
        typeof(System.Convert),
        typeof(System.Collections.IList),
        typeof(System.Reflection.BindingFlags),
        #endregion system

        #region UnityEngine
        typeof(UnityEngine.Application),
        typeof(UnityEngine.RuntimePlatform),
        typeof(UnityEngine.Object),
        typeof(UnityEngine.Texture2D),
        typeof(UnityEngine.GameObject),
        typeof(UnityEngine.Transform),
        
        typeof(UnityEngine.Animator),

        typeof(UnityEngine.Rigidbody),
        typeof(UnityEngine.Physics),
        typeof(UnityEngine.RaycastHit),

        typeof(UnityEngine.Vector2),
        typeof(UnityEngine.Vector3),
        typeof(UnityEngine.Vector4),
        typeof(UnityEngine.WaitForSeconds),
        typeof(UnityEngine.AssetBundle),
        typeof(UnityEngine.WWW),
        
        typeof(UnityEngine.Networking.UnityWebRequest),
        typeof(UnityEngine.Networking.DownloadHandler),
        typeof(UnityEngine.Networking.UnityWebRequestAsyncOperation),
        
        typeof(UnityEngine.Video.VideoPlayer),
        typeof(UnityEngine.Video.VideoClip),
        typeof(UnityEngine.UI.Toggle),
        
        typeof(UnityEngine.Color),
        typeof(UnityEngine.Time),
        typeof(UnityEngine.Color),
        #endregion
    };

//    [LuaCallCSharp]
    public static List<Type> LuaCallCSharpUnityEngine
    {
        get
        {
            var blacks = new List<string>()
            {
                "XLua",
                ".Apple",
                ".iPhone",
                ".iOS",
                ".tvOS",
                ".Android",
                ".WSA",
                ".Windows",
                ".Unsafe",
                ".AudioSettings",
                ".Light",
                "UnityEditor",
                "UnityEngine.TextureCompressionQuality",
            };
            // UnityEngine.ADBannerView mm;
            var l = new []{
                    //"Assembly-CSharp"
                    "UnityEngine.CoreModule"
//                    ,"UnityEngine.UI"
//                    ,"UnityEngine.AudioModule"
//                    ,"UnityEngine.CoreModule"
//                    ,"UnityEngine.VideoModule"
                }
                .Select(s => Assembly //
                    .Load(s)
                    .GetTypes()
                    .Where(type => type.IsVisible
                        && !type.IsDefined(typeof(ObsoleteAttribute), true) // innerclass not work
                        && !type.GetCustomAttributes(true).Any(a => a.GetType() == typeof(ObsoleteAttribute))
                        && !type.FullName.EndsWith("Attribute")
                        && !type.FullName.Contains("Unsafe")
                        && !blacks.Any(i => type.FullName.Contains(i))
                    )
                )
                .SelectMany(i => i)
                .ToList();
            l.Sort((i,j)=>i.FullName.CompareTo(j.FullName));
            return l;
        }
    }

    [Hotfix]
    public static List<Type> HotfixList = new List<Type>()
    {

    };

    //C#静态调用Lua的配置（包括事件的原型），仅可以配delegate，interface
    [CSharpCallLua]
    public static List<Type> CSharpCallLua = new List<Type>() {
        #region customer
        typeof(TableView.TableViewController.CellRow),
        typeof(TableView.TableViewController.CellSize),
        typeof(TableView.TableViewController.Count),
        typeof(TableView.TableViewController.OnHighlightRow),
        typeof(TableView.TableViewController.OnSelectRow),
        typeof(TableView.CellDidSelectEvent),
        typeof(TableView.CellDidHighlightEvent),
        typeof(LuaCSFunction),
        #endregion customer

        #region System
        typeof(System.Collections.IEnumerator),
        typeof(Func<double, double, double>),
        typeof(System.Action),
        typeof(System.Action<byte[]>),
        typeof(System.Action<string>),
        typeof(System.Action<double>),
        typeof(System.Action<int>),
        typeof(System.Action<float>),
        typeof(System.Action<bool>),
        typeof(Action<FileStream>),
        
        typeof(UnityEngine.Events.UnityAction),
        typeof(UnityEngine.Events.UnityAction<byte[]>),
        typeof(UnityEngine.Events.UnityAction<string>),
        typeof(UnityEngine.Events.UnityAction<double>),
        typeof(UnityEngine.Events.UnityAction<int>),
        typeof(UnityEngine.Events.UnityAction<float>),
        typeof(UnityEngine.Events.UnityAction<bool>),

        typeof(System.Collections.IList),
        typeof(System.Action<System.Object>),
        typeof(System.Action<UnityEngine.Object>),
        typeof(System.Action<UnityEngine.GameObject>),
        typeof(System.Action<UnityEngine.AssetBundle>),
        typeof(System.Action<UnityEngine.Texture2D>),
        typeof(System.Action<UnityEngine.WWW>),
        #endregion System

        #region UnityEngine
        typeof(UnityEngine.LogType),
        typeof(UnityEngine.Application.LogCallback),

        typeof(UnityEngine.Events.UnityEvent),
        typeof(UnityEngine.Events.UnityEvent<string>),
        typeof(UnityEngine.Events.UnityEvent<float>),
        typeof(UnityEngine.Events.UnityEvent<int>),

        typeof(UnityEngine.Events.UnityAction),
        typeof(UnityEngine.Events.UnityAction<string>),
        typeof(UnityEngine.Events.UnityAction<float>),
        typeof(UnityEngine.Events.UnityAction<int>),
        typeof(UnityEngine.Events.UnityAction<UnityEngine.WWW>),
        typeof(UnityEngine.UI.InputField.OnValidateInput)
        #endregion UnityEngine
    };

    //黑名单
    [BlackList]
    public static List<List<string>> BlackList = new List<List<string>>()  {
        new List<string>(){"UnityEngine.TrailRenderer", "GetPositions"},
        new List<string>(){"UnityEngine.WWW", "GetMovieTexture"},
    #if UNITY_WEBGL
        new List<string>(){"UnityEngine.WWW", "threadPriority"},
    #endif
        new List<string>(){"UnityEngine.Texture2D", "alphaIsTransparency"},
        new List<string>(){"UnityEngine.Security", "GetChainOfTrustValue"},
        new List<string>(){"UnityEngine.CanvasRenderer", "onRequestRebuild"},
        new List<string>(){"UnityEngine.Light", "areaSize"},
        new List<string>(){"UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup"},
    #if !UNITY_WEBPLAYER
        new List<string>(){"UnityEngine.Application", "ExternalEval"},
    #endif
        new List<string>(){"UnityEngine.GameObject", "networkView"}, //4.6.2 not support
        new List<string>(){"UnityEngine.Component", "networkView"},  //4.6.2 not support
        new List<string>(){"System.IO.File", "GetAccessControl", "SetAccessControl", "System.Security.AccessControl.AccessControlSections", "System.Security.AccessControl.FileSecurity"},
        new List<string>(){"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
        new List<string>(){"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
        new List<string>(){"System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
        new List<string>(){"System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity"},
        new List<string>(){"System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity"},
        new List<string>(){"System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity"},
        new List<string>(){"System.Text.Encoding", "GetCharCount", "GetByteCount", "GetBytes"},
        new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},
        new List<string>(){"UnityEngine.AudioSettings", "GetSpatializerPluginNames","SetSpatializerPluginName"},
        new List<string>(){"UnityEngine.UI.Text", "OnRebuildRequested"},
        new List<string>(){"UnityEngine.Texture", "imageContentsHash"},
        
        // new List<string>(){"Mono.Data.Sqlite.SqliteConnection", "EnlistTransaction"},
    };
}
