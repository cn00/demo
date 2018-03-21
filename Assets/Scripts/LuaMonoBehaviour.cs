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

    [HideInInspector]
    public GameObject[] injections;

    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    private Action luaStart;
    private Action luaFixedUpdate;
    private Action luaUpdate;
    private Action luaLateUpdate;
    private Action luaOnDestroy;

    private LuaTable luaTable = null;

    public bool Inited { get; protected set; }
    IEnumerator Init()
    {
        Inited = false;
        byte[] textBytes = null;

        var luaPath = luaScript.path;
        yield return AssetHelper.Instance.GetAsset<DataObject>(luaPath, asset =>
        {
            textBytes = asset.Data;
        });
        var luaInstance = LuaHelper.Instance;
        luaTable = luaInstance.GetLuaTable(textBytes, this, "LuaMonoBehaviour");

        Action luaAwake = luaTable.Get<Action>("awake");
        luaTable.Get("Start", out luaStart);
        luaTable.Get("FixedUpdate", out luaFixedUpdate);
        luaTable.Get("Update", out luaUpdate);
        luaTable.Get("LateUpdate", out luaLateUpdate);
        luaTable.Get("OnDestroy", out luaOnDestroy);

        if(luaAwake != null)
        {
            luaAwake();
        }
        if(luaStart != null)
        {
            luaStart();
        }

        Inited = true;
        yield return null;
    }

    void Awake()
    {
        StartCoroutine(Init());
    }

    // Use this for initialization
    void Start()
    {
    }

    private void FixedUpdate()
    {
        if(Inited && luaFixedUpdate != null)
        {
            luaFixedUpdate();
        }
    }

    private void OnMouseUpAsButton()
    {

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
            LuaHelper.Instance.GlobalEnv.Tick();
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

    static bool mFoldInjections = true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        mObj.luaScript.Asset = EditorGUILayout.ObjectField("Lua", mObj.luaScript.Asset, typeof(UnityEngine.Object), true);

        var size = mObj.injections.Length;
        EditorGUILayout.BeginHorizontal();
        {
            mFoldInjections = EditorGUILayout.Foldout(mFoldInjections, "Injections", true);
            size = EditorGUILayout.IntField(size);
        }
        EditorGUILayout.EndHorizontal();

        if(size != mObj.injections.Length)
        {
            var oldobjs = mObj.injections;
            mObj.injections = new GameObject[size];
            var isfx = mObj.injections.IsFixedSize;
            for(int i = 0; i < Math.Min(size, oldobjs.Length); ++i)
            {
                mObj.injections[i] = oldobjs[i];
            }
        }

        if(mFoldInjections)
        {
            for(var i = 0; i < mObj.injections.Length; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    mObj.injections[i] = (GameObject)EditorGUILayout.ObjectField(mObj.injections[i], typeof(GameObject), true);
                    if(mObj.injections[i])
                    {
                        mObj.injections[i].name = EditorGUILayout.TextField(mObj.injections[i].name.RReplace(BundleConfig.PunctuationRegex + "+", "_"));
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        //if(GUI.changed)
        {
        }
    }
}
#endif
