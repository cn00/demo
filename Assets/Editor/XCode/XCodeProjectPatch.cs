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
using UnityEditor.XCodeEditor;

public class XCodeProjectPatch
{
    [MenuItem("Tools/XCodeProjectPatch")]
    public static void Menu()
    {
        OnPostProcessBuild(UnityEditor.BuildTarget.iOS, "bin/ios");
    }

    [PostProcessBuild(999)]
	public static void OnPostProcessBuild( BuildTarget target, string pathToBuiltProject )
	{
		if (target != BuildTarget.iOS) {
            UnityEngine.Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
			return;
		}

		// patch native codes
		// BuildConfig.ChannelConfig.PatchNative(pathToBuiltProject + "/" + "Classes/UnityAppController.mm", "iospatch/UnityAppController.mm.patch");
		// BuildConfig.ChannelConfig.PatchNative(pathToBuiltProject + "/" + "Classes/UI/UnityViewControllerBaseiOS.mm", "iospatch/UnityViewControllerBaseiOS.mm.patch");
		// if (BuildConfig.Instance().ActiveConfig().UseEmulator)
		// {
		// 	BuildConfig.ChannelConfig.PatchNative(pathToBuiltProject + "/" + "Classes/Unity/UnityMetalSupport.h", "iospatch/UnityMetalSupport.h.sim.patch");
		// }

		// Create a new project object from build target
		XCProject project = new XCProject( pathToBuiltProject );

		// Find and run through all projmods files to patch the project.
		// Please pay attention that ALL projmods files in your project folder will be excuted!
		// if(AppVersionData.InstanceEditor().m_A3Channel.isIOSTw())
		// {
		// 	project.ApplyMod( "Assets/Plugins/iOS/KGameSdk/KGameSDK.projmods" );
		// }
		// else
		{
			project.ApplyMod( "ios-patch/ios-patch.projmods" );
		}

		// //TODO implement generic settings as a module option
		// project.overwriteBuildSetting("CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Distribution", "Release");
		
		// Finally save the xcode project
		project.Save();

	}

	public static void Log(string message)
	{
		UnityEngine.Debug.Log("PostProcess: "+message);
	}
}
#endif
