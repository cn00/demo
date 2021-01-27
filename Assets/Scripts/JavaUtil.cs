namespace App
{
    using UnityEngine;
    public class JavaUtil
    {

        public static object Call(string classname, string apiName, params object[] args)
        {
            return Call<string>( classname,  apiName, args);
        }
        public static T Call<T>(string classname, string apiName, params object[] args)
        {
            var argss = string.Join(",", args); 
            T res = default(T);
            #if UNITY_ANDROID && ! UNITY_EDITOR
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