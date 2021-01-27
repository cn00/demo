namespace App
{
    using UnityEngine;
    public class JavaUtil
    {

        public static object Call(string classname, string apiName, params object[] args)
        {
            return Call<object>( classname,  apiName, args);
        }
        public static T Call<T>(string classname, string apiName, params object[] args)
        {
            T res = default(T);
            #if UNITY_ANDROID && ! UNITY_EDITOR
            using AndroidJavaClass cls = new AndroidJavaClass(classname);
            res = cls.CallStatic<T>(apiName, args);
            #endif
            return res;
        }
    }
}