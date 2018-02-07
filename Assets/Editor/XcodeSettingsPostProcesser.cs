#if UNITY_IOS
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;

public class XcodeSettingsPostProcesser
{

    [PostProcessBuild(1000)]
    public static void OnPostprocessBuild (BuildTarget buildTarget, string pathToBuiltProject)
    {

        // Stop processing if targe is NOT iOS
        if (buildTarget != BuildTarget.iOS)
            return; 

        // Initialize PbxProject
        var projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
        PBXProject pbxProject = new PBXProject ();
        pbxProject.ReadFromFile (projectPath);
        string targetGuid = pbxProject.TargetGuidByName ("Unity-iPhone");

        string config = pbxProject.BuildConfigByName(targetGuid, "Release");

        //adding build property
        // pbxProject.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-all_load");

        //setting build property
        pbxProject.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

        // pbxProject.AddBuildProperty(targetGuid, "SWIFT_VERSION", "4.0");
        pbxProject.SetBuildProperty(targetGuid, "SWIFT_VERSION", "4.0");

        pbxProject.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Framewoks @loader_path/Frameworks");

//        //update build property
//        pbxProject.UpdateBuildProperty(targetGuid, "OTHER_LDFLAGS", new string[]{
//            "-ObjC",
//            "-lresolv",
//            "-weak_framework",
//            "CoreMotion"
//        });

        //adding REQUIRED framwrok
//        pbxProject.AddFrameworkToProject(targetGuid, "Security.framework", false);

        //adding OPTIONAL framework
        //pbxProject.AddFrameworkToProject(targetGuid, "SafariServices.framework", true);

        //setting compile flags
        // var guid = pbxProject.FindFileGuidByProjectPath("Classes/UI/Keyboard.mm");
        // var flags = pbxProject.GetCompileFlagsForFile(targetGuid, guid);
        // flags.Add("-fno-objc-arc");
        // pbxProject.SetCompileFlagsForFile(targetGuid, guid, flags);

        // Apply settings
        File.WriteAllText (projectPath, pbxProject.WriteToString ());

//        //editing Info.plist
//        var plistPath = Path.Combine (pathToBuiltProject, "Info.plist");
//        var plist = new PlistDocument ();
//        plist.ReadFromFile (plistPath);

        // Add string setting
        // plist.root.SetString ("hogehogeId", "dummyid");

        // // Add URL Scheme
        // var array = plist.root.CreateArray ("CFBundleURLTypes");
        // var urlDict = array.AddDict ();
        // urlDict.SetString ("CFBundleURLName", "");
        // var urlInnerArray = urlDict.CreateArray ("CFBundleURLSchemes");
        // urlInnerArray.AddString ("biligame");

        // Apply editing settings to Info.plist
//        plist.WriteToFile (plistPath);

        ProcessStartInfo pi = new ProcessStartInfo(
            "open",
            pathToBuiltProject + "/Unity-iPhone.xcodeproj"
        );
        Process.Start(pi);
    }
}
#endif