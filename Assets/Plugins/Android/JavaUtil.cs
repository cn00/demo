using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;

public class JavaUtil
{
    public static void CallJavaApi(string className, string apiName, params object[] args)
    {
        // AppLog.d(Tag, "sdk call: " + className + ":" + apiName);
        using (AndroidJavaClass cls = new AndroidJavaClass(className))
        {
            cls.CallStatic(apiName, args);
        }
    }
}