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

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class GameObjectExtension
{
    public static void SetLuaAb(this GameObject self, AssetBundle ab)
    {
        var ins = GameObject.Instantiate(self);
        var luambs = ins.GetComponentsInChildren<LuaMonoBehaviour>();
        foreach(var i in luambs)
        {
            i.SetAB(ab);
        }
    }
}

[LuaCallCSharp]
public class LuaMonoBehaviour : MonoBehaviour
{
    [SerializeField,HideInInspector]
    public LuaAsset luaScript = new LuaAsset();

    public GameObject[] injections;

    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    private Action luaStart;
    private Action luaFixedUpdate;
    private Action luaUpdate;
    private Action luaLateUpdate;
    private Action luaOnDestroy;

    private LuaTable luaTable;

    public AssetBundle Bundle { get; protected set; }
    public bool SetAB(AssetBundle bundle)
    {
        Bundle = bundle;
        return true;
    }

    void Awake()
    {

        //if(luaAwake != null)
        //{
        //    luaAwake();
        //}
    }

    // Use this for initialization
    void Start()
    {
        var luaInstance = LuaSingleton.Instance;
        luaTable = luaInstance.GetLuaTable(luaScript.path, this, "LuaMonoBehaviour");

        Action luaAwake = luaTable.Get<Action>("awake");
        luaTable.Get("Start", out luaStart);
        luaTable.Get("FixedUpdate", out luaFixedUpdate);
        luaTable.Get("Update", out luaUpdate);
        luaTable.Get("LateUpdate", out luaLateUpdate);
        luaTable.Get("OnDestroy", out luaOnDestroy);

        if(luaStart != null)
        {
            luaStart();
        }
    }

    private void FixedUpdate()
    {
        if(luaFixedUpdate != null)
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
        if(luaUpdate != null)
        {
            luaUpdate();
        }
        if(Time.time - lastGCTime > GCInterval)
        {
            LuaSingleton.Instance.GlobalEnv.Tick();
            lastGCTime = Time.time;
        }
    }

    private void LateUpdate()
    {
        if(luaLateUpdate != null)
        {
            luaLateUpdate();
        }
    }

    void OnDestroy()
    {
        if(luaOnDestroy != null)
        {
            luaOnDestroy();
        }
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        luaTable.Dispose();
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

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //if(GUI.changed)
        {
            mObj.luaScript.Asset = EditorGUILayout.ObjectField("Lua", mObj.luaScript.Asset, typeof(UnityEngine.Object), true);
        }
    }
}
#endif
