/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;
using System.Text;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
#endif

public static class GameObjectExtension
{

}

[LuaCallCSharp]
public class LuaMonoBehaviour : MonoBehaviour
{
    [SerializeField,HideInInspector]
    public LuaAsset luaScript = new LuaAsset();

    [Serializable]
    public class Injection
    {
        public GameObject obj;
#if UNITY_EDITOR
        public int exportComIdx = -1;
#endif
    }
    [HideInInspector]
    public Injection[] injections = new Injection[0];


    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    private Action luaAwake;
    private Action luaOnEnable;
    private Action luaStart;
    private Action luaFixedUpdate;
    private Action luaUpdate;
    private Action luaLateUpdate;
    private Action luaOnDestroy;

    private LuaTable luaTable = null;

    public bool Inited { get; protected set; }
    bool Init()
    {
        byte[] textBytes = LuaSys.Instance.LuaLoader(luaScript.path) ?? Encoding.UTF8.GetBytes( "return {}");

        var luaInstance = LuaSys.Instance;
        luaTable = luaInstance.GetLuaTable(textBytes, this, luaScript.path);

        luaTable.Get("Awake", out luaAwake);
        luaTable.Get("OnEnable", out luaOnEnable);
        luaTable.Get("Start", out luaStart);
        luaTable.Get("FixedUpdate", out luaFixedUpdate);
        luaTable.Get("Update", out luaUpdate);
        luaTable.Get("LateUpdate", out luaLateUpdate);
        luaTable.Get("OnDestroy", out luaOnDestroy);

        Inited = true;
        return true;
    }

    void Awake()
    {
        Inited = false;
        Init();
        if(Inited && luaAwake != null)
        {
            luaAwake();
        }
    }

    private void OnEnable()
    {
        if(!Inited)
            Init();
        if(Inited && luaOnEnable != null)
        {
            luaOnEnable();
        }
    }

    // Use this for initialization
    void Start()
    {
        if(Inited && luaStart != null)
        {
            luaStart();
        }
    }

    private void FixedUpdate()
    {
        if(Inited && luaFixedUpdate != null)
        {
            luaFixedUpdate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Inited && luaUpdate != null)
        {
            luaUpdate();
        }
        if(Time.time - lastGCTime > GCInterval)
        {
            LuaSys.Instance.GlobalEnv.Tick();
            lastGCTime = Time.time;
        }
    }

    private void LateUpdate()
    {
        if(Inited && luaLateUpdate != null)
        {
            luaLateUpdate();
        }
    }

    void OnDestroy()
    {
        CleanLua();
    }

    void CleanLua()
    {
        if(Inited && luaOnDestroy != null)
        {
            luaOnDestroy();
        }
        luaTable.Dispose();
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        injections = null;
    }

    void SetLua(string path)
    {
        CleanLua();

        luaScript.path = path;
        Init();
    }

    public void YieldAndCallback(object to_yield, Action callback)
    {
        StartCoroutine(CoroutineBody(to_yield, callback));
    }

    private IEnumerator CoroutineBody(object to_yield, Action callback)
    {
        AppLog.d("CoroutineBody: {0}, {1}", to_yield, callback);
        if(to_yield is IEnumerator)
        {
            yield return StartCoroutine((IEnumerator)to_yield);
        }
        else
        {
            yield return to_yield;
        }
        if(callback != null)
            callback();
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(LuaMonoBehaviour))]
public class LuaMonoBehaviourEditor : Editor
{
    LuaMonoBehaviour mObj = null;
    public void OnEnable()
    {
        mObj = (LuaMonoBehaviour)target;
    }

    static bool mShowInjections = true;
    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI ();
        mObj.luaScript.Asset = EditorGUILayout.ObjectField ("Lua", mObj.luaScript.Asset, typeof(UnityEngine.Object), true);

        var size = mObj.injections.Length;
        EditorGUILayout.BeginHorizontal ();
        {
            mShowInjections = EditorGUILayout.Foldout (mShowInjections, "Injections", true);
            size = EditorGUILayout.IntField (size);
        }
        EditorGUILayout.EndHorizontal ();

        if (size != mObj.injections.Length)
        {
            var oldobjs = mObj.injections;
            mObj.injections = new LuaMonoBehaviour.Injection[size];
            var isfx = mObj.injections.IsFixedSize;
            for (int i = 0; i < Math.Min (size, oldobjs.Length); ++i)
            {
                mObj.injections [i] = oldobjs [i];
            }
        }

        if(mShowInjections)
        {
            for(var i = 0; i < mObj.injections.Length; ++i)
            {
                var item = mObj.injections[i] ?? new LuaMonoBehaviour.Injection();
                EditorGUILayout.BeginHorizontal();
                {
                    item.obj = (GameObject)EditorGUILayout.ObjectField(item.obj, typeof(GameObject), true);
                    if(item.obj)
                    {
                        item.obj.name = EditorGUILayout.TextField(item.obj.name.RReplace(BundleConfig.PunctuationRegex + "+", "_"));
                        var coms = item.obj.GetComponents<Component>();
                        if(item.exportComIdx == -1)
                            item.exportComIdx = coms.Length - 1;
                        item.exportComIdx = EditorGUILayout.Popup(item.exportComIdx, coms.Select(e=> 
                        {
                            var name = e.GetType().ToString();
                            name = name.Substring(name.LastIndexOf('.') + 1);
                            return name; }
                        ).ToArray());
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            // gen lua code
            string luaname = mObj.luaScript.path.Substring(mObj.luaScript.path.LastIndexOf('/')+1);
            string luaMemberValue = "--AutoGenInit Begin\nfunction " + luaname + ".AutoGenInit()";
            foreach (var i in mObj.injections.Where(o=>o != null && o.obj != null))
            {
                luaMemberValue += "\n    " + luaname + "." + i.obj.name 
                    + " = " + i.obj.name 
                    + ":GetComponent(\"" 
                    + i.obj.GetComponents<Component>()[i.exportComIdx].GetType() + "\")";
            }
            luaMemberValue += "\nend\n--AutoGenInit End";
            GUILayout.TextArea(luaMemberValue);

            var rect = EditorGUILayout.GetControlRect();
            if(GUI.Button(rect.Split(1, 3), "Wtrite to lua"))
            {
                var luaPath = BundleConfig.BundleResRoot + mObj.luaScript.path + BundleConfig.LuaExtension;
                var code = File.ReadAllText(luaPath);
                var pattern = Regex.Match(code, "--AutoGenInit Begin(.|\r|\n)*--AutoGenInit End", RegexOptions.Multiline).ToString();
                code = code.Replace(pattern.ToString(), luaMemberValue);
                File.WriteAllText(luaPath, code);
            }
        }

    }
}
#endif
