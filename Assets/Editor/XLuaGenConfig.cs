/*
* Tencent is pleased to support the open source community by making xLua available.
* Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
* Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
* http://opensource.org/licenses/MIT
* Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System.Collections.Generic;
using System;
using UnityEngine;
using XLua;
using System.Runtime;
using SQLite;
using TableView;

//配置的详细介绍请看Doc下《XLua的配置.doc》
public static class XLuaGenConfig
{
    //lua中要使用到C#库的配置，比如C#标准库，或者Unity API，第三方库等。
    [LuaCallCSharp, Hotfix]
    public static List<Type> LuaCallCSharp = new List<Type>()
    {
        #region customer types
        typeof(TableView.TableView),
        typeof(TableView.TableViewController),
        typeof(TableView.TableViewCell),
        typeof(TableView.TableViewCellController),
               
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
        typeof(LuaMonoBehaviour),
        typeof(NetSys),
        typeof(QRCodeEncodeController),
        typeof(QRCodeDecodeController),
        typeof(JavaUtil),
        typeof(AppLog),
        typeof(SQLite3),
        #endregion customer types

        #region UnityEngine
        typeof(UnityEngine.Application),
        typeof(UnityEngine.ScreenCapture),
        typeof(UnityEngine.Object),
        typeof(UnityEngine.Vector2),
        typeof(UnityEngine.Vector3),
        typeof(UnityEngine.Vector4),
        typeof(UnityEngine.Quaternion),
        typeof(UnityEngine.Color),
        typeof(UnityEngine.Ray),
        typeof(UnityEngine.Bounds),
        typeof(UnityEngine.Ray2D),
        typeof(UnityEngine.Time),
        typeof(UnityEngine.GameObject),
        typeof(UnityEngine.Component),
        typeof(UnityEngine.Behaviour),
        typeof(UnityEngine.Transform),
        typeof(UnityEngine.Rect),
        typeof(UnityEngine.RectTransform),
        typeof(UnityEngine.WaitForSeconds),
        typeof(UnityEngine.Resources),
        typeof(UnityEngine.TextAsset),
        typeof(UnityEngine.Keyframe),
        typeof(UnityEngine.AnimationCurve),
        typeof(UnityEngine.AnimationClip),
        typeof(UnityEngine.MonoBehaviour),
        typeof(UnityEngine.ParticleSystem),
        typeof(UnityEngine.SkinnedMeshRenderer),
        typeof(UnityEngine.Renderer),
        typeof(UnityEngine.WWW),
        typeof(UnityEngine.Debug),
        #endregion UnityEngine

        #region system
        typeof(System.Reflection.BindingFlags),
        typeof(System.Delegate),
        typeof(System.Convert),
        typeof(System.Object),
        typeof(System.Collections.Generic.List<int>),
        typeof(Action),
        typeof(Action<string>),
        typeof(Action<byte[]>),
        typeof(Action<double>),
        typeof(Action<UnityEngine.Object>),
        typeof(Action<DataObject>),
        #endregion system
    };

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
        #endregion customer

        #region System
        typeof(Action),
        typeof(System.Action),
        typeof(System.Collections.IEnumerator),
        typeof(Func<double, double, double>),
        typeof(Action<string>),
        typeof(Action<double>),
        typeof(Action<UnityEngine.Object>),
        typeof(Action<UnityEngine.Texture2D>),
        typeof(Action<DataObject>),
        #endregion System

        #region UnityEngine
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
        new List<string>(){"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
        new List<string>(){"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
        new List<string>(){"System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
        new List<string>(){"System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity"},
        new List<string>(){"System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity"},
        new List<string>(){"System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity"},
        new List<string>(){"System.Text.Encoding", "GetCharCount", "GetByteCount", "GetBytes"},
        new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},
    };
}
