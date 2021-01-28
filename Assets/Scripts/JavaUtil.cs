namespace App
{
    using UnityEngine;
    public class JavaUtil
    {

        public static void CallStaticVoid(string classname, string apiName, params object[] args)
        {
            var argss = string.Join(",", args); 
            #if UNITY_ANDROID  && ! UNITY_EDITOR
            using AndroidJavaClass cls = new AndroidJavaClass(classname);
            cls.CallStatic(apiName, args);
            Debug.Log($"Android CallStaticVoid {classname}.{apiName}({argss})");
            #else
            Debug.Log($"Java CallStaticVoid {classname}.{apiName}({argss})");
            #endif
        }

        public static string CallStatic(string classname, string apiName, params object[] args)
        {
            return CallStatic<string>( classname,  apiName, args);
        }
        public static T CallStatic<T>(string classname, string apiName, params object[] args)
        {
            var argss = string.Join(",", args); 
            T res = default(T);
            #if UNITY_ANDROID  && ! UNITY_EDITOR
            using AndroidJavaClass cls = new AndroidJavaClass(classname);
            res = cls.CallStatic<T>(apiName, args);
            Debug.Log($"Android CallStatic {classname}.{apiName}({argss})");
            #else
            Debug.Log($"Java CallStatic {classname}.{apiName}({argss})");
            #endif
            return res;
        }
    }
}