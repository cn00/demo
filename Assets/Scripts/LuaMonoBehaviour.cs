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


public class LuaMonoBehaviour : MonoBehaviour
{
    [SerializeField,HideInInspector]
    public LuaAsset luaScript = new LuaAsset();

    [Serializable]
    public class Injection
    {
        public GameObject obj;
        public int exportComIdx = -1;
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

    public LuaTable luaTable
    {
        get;
        protected set;
    }

    bool mInited = false;
    public bool Inited { 
        get
        {
            return mInited 
            // && AssetSys.Instance.Inited
            // && LuaSys.Instance.Inited
            ;
        } 
        protected set
        {
            mInited = value;
        }
    }

    IEnumerator AsyncInit()
    {
        yield return null;
    }

    void Init()
    {
        // while(!LuaSys.Instance.Inited)
        //     yield return null;

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
        enabled = true;
        if(Inited && luaAwake != null)
        {
            luaAwake();
        }
        
        // yield return null;
    }

    void Awake()
    {
        enabled = false;
        Inited = false;
        // StartCoroutine(Init());
        Init();
    }

    private void OnEnable()
    {
        if(!Inited)
        {
            Init();
        }
        
        if(luaOnEnable != null)
        {
            luaOnEnable();
        }
    }

    // Use this for initialization
    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {
        if(luaUpdate != null)
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
        if(luaOnDestroy != null)
        {
            luaOnDestroy();
        }
    //    luaTable.Dispose();
        luaTable = null;
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        injections = null;
    }

    public void SetLua(string path)
    {
        CleanLua();

        luaScript.path = path;
        // StartCoroutine(Init());
        Init();
    }

    public void YieldAndCallback(object to_yield, Action callback)
    {
        StartCoroutine(CoroutineBody(to_yield, callback));
    }

    private IEnumerator CoroutineBody(object to_yield, Action callback)
    {
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
