/*
 * Tencent is pleased to support the open source community by making InjectFix available.
 * Copyright (C) 2019 THL A29 Limited, a Tencent company.  All rights reserved.
 * InjectFix is licensed under the MIT License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
 */

using System.Collections.Generic;
using IFix;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

//1、配置类必须打[Configure]标签
//2、必须放Editor目录
[Configure]
public class IFixCfg
{
    public static string PlatformName()
    {
        string name = PlatformName(Application.platform);;
# if UNITY_IOS
        name = PlatformName(RuntimePlatform.IPhonePlayer);
# elif UNITY_ANDROID
        name = PlatformName(RuntimePlatform.Android);
# elif UNITY_STANDALONE_WIN
        name = PlatformName(RuntimePlatform.WindowsPlayer);
# elif UNITY_OSX
        name = PlatformName(RuntimePlatform.OSXPlayer);
# endif
        return name;
    }

    /// <summary>
    /// http://ip:port/path/to/root/platform/
    /// </summary>
    /// <value>The http root.</value>
    public static string WebRoot;

    public static string PlatformName(RuntimePlatform platform)
    {
        switch (platform)
        {
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "iOS";
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return "Windows";
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor:
                return "OSX";
            default:
                return platform.ToString();
        }
    }
    
    public static string TargetName(BuildTarget platform)
    {
        switch (platform)
        {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.iOS:
                return "iOS";
            case BuildTarget.StandaloneWindows:
                return "Windows";
            case BuildTarget.StandaloneWindows64:
                return "Windows64";
            case BuildTarget.StandaloneOSX:
                return "OSX";
            default:
                return platform.ToString();
        }
    }

    
    [IFix]
    public static IEnumerable<Type> IFixTypes
    {
        get
        {
            var l = new List<Type>();
            var s = new StringBuilder(1024);
            var ts = Assembly.Load("Assembly-CSharp").GetTypes().ToList();
            ts.Sort((a,b)=>a.FullName.CompareTo(b.FullName));
            foreach (var t in ts)
            {
                var fullName = t.FullName;
                var ok = // &&t.IsVisible 
                        // t.IsPublic
                           !t.FullName.Contains("Editor") 
                        && !t.FullName.Contains("Examples")
                        && !t.FullName.Contains("XLua.")
                    ;
                if (ok)
                {
                    // Debug.LogFormat("Hotfix ok {0}", t.FullName);
                    s.Append(t.FullName + "\n");
                    l.Add(t);
                }
                else
                {
                }
            }

            var path = $"ifix-{PlatformName()}-list.txt";
            File.WriteAllText(path, s.ToString());

            return l;
        }
    }
}
