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
using UnityEngine.EventSystems;
using UnityEngine.Serialization;


public class LuaMonoBehaviour : MonoBehaviour
{
    [SerializeField, HideInInspector]
    public UnityEngine.TextAsset LuaAsset;

    [SerializeField]
    public string LuaPath;
    
    [Serializable]
    public class Injection
    {
        public GameObject obj;
        public int exportComIdx = -1;
    }
    [FormerlySerializedAs("injections")] [HideInInspector]
    public List<Injection> Injections;

    [Serializable]
    public class InjectValue
    {
        public string k;
        public string v;
    }
    [HideInInspector]
    public List<InjectValue> InjectValues;

    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    #region LuaActions

    private Action luaAwake;
    private Action luaOnEnable;
    private Action luaStart;
    private Action luaFixedUpdate;

    private Action<BaseEventData> luaOnEventTrigger;
    private Action<Collider> luaOnTriggerEnter;
    private Action<Collider> luaOnTriggerStay;
    private Action<Collider> luaOnTriggerExit;

    private Action<Collision> luaOnCollisionEnter;

    private Action<string> luaOnAnimationEvent;

    private Action luaOnMouseEnter;
    private Action luaOnMouseOver;
    private Action luaOnMouseDown;
    private Action luaOnMouseDrag;
    private Action luaOnMouseUp;
    private Action luaOnMouseExit;

    private Action luaUpdate;
    private Action luaLateUpdate;
    private Action luaOnDestroy;
    
    #endregion // LuaActions

    public LuaTable Lua
    {
        get;
        protected set;
    }

    IEnumerator Init()
    {
        while (!LuaSys.Instance.Inited)
            yield return null;

        if (LuaAsset == null) 
            yield break;
        var luaInstance = LuaSys.Instance;
        Lua = luaInstance.GetLuaTable(LuaAsset.bytes, this, LuaPath);

        if (Lua == null)
        {
            Debug.LogErrorFormat("error load lua:{0}", LuaPath);
            yield break;
        }

        Lua.Get("Awake", out luaAwake);
        Lua.Get("OnEnable", out luaOnEnable);
        Lua.Get("Start", out luaStart);
        Lua.Get("FixedUpdate", out luaFixedUpdate);
        
        {
            Lua.Get("OnTriggerEnter", out luaOnTriggerEnter);
            Lua.Get("OnTriggerStay", out luaOnTriggerStay);
            Lua.Get("OnTriggerExit", out luaOnTriggerExit);

            Lua.Get("OnCollisionEnter", out luaOnCollisionEnter);
            Lua.Get("OnAnimationEvent", out luaOnAnimationEvent);

            Lua.Get("OnMouseEnter", out luaOnMouseEnter);
            Lua.Get("OnMouseOver", out luaOnMouseOver);
            Lua.Get("OnMouseExit", out luaOnMouseExit);
            Lua.Get("OnMouseDown", out luaOnMouseDown);
            Lua.Get("OnMouseUp", out luaOnMouseUp);
            Lua.Get("OnMouseDrag", out luaOnMouseDrag);
        }

        // var a = GetComponent<Animator>();
        // a.GetCurrentAnimatorClipInfo(0);
        // // a.ResetTrigger("jump");
        // a.Play("jump");
        
        // {
        //     Lua.Get("OnEventTrigger", out luaOnEventTrigger);
        //     if (luaOnEventTrigger != null)
        //     {
        //         var tr = gameObject.GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();
        //         var et = new EventTrigger.Entry()
        //         {
        //             eventID = EventTriggerType.Drop,
        //         };
        //         et.callback.AddListener(OnEventTrigger);
        //         tr.triggers.Add(et);
        //     }
        // }

        Lua.Get("Update", out luaUpdate);
        Lua.Get("LateUpdate", out luaLateUpdate);
        Lua.Get("OnDestroy", out luaOnDestroy);

        if(luaAwake != null)
        {
            luaAwake();
        }
        yield return null;
    }

    void Awake()
    {
        StartCoroutine(Init());
    }

    private void OnEnable()
    {
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

    #region OnTrigger

    public void OnAnimationEvent(string name)
    {
        if (luaOnAnimationEvent != null)
            luaOnAnimationEvent(name);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (luaOnTriggerEnter != null)
        {
            luaOnTriggerEnter(other);
        }
    }

    // stayCount allows the OnTriggerStay to be displayed less often
    // than it actually occurs.
    private void OnTriggerStay(Collider other)
    {
        if (luaOnTriggerStay != null)
        {
            luaOnTriggerStay(other);
            // if (stayCount > 0.25f)
            // {
            //     Debug.Log("staying");
            //     stayCount = stayCount - 0.25f;
            // }
            // else
            // {
            //     stayCount = stayCount + Time.deltaTime;
            // }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (luaOnTriggerExit != null)
        {
            luaOnTriggerExit(other);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        // ContactPoint contact = other.contacts[0];
        // Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        // Vector3 pos = contact.point;
        // Instantiate(explosionPrefab, pos, rot);
        // Destroy(gameObject);
        if(luaOnCollisionEnter != null)
        {
            luaOnCollisionEnter(other);
        }
    }
    #endregion OnTrigger

    #region Mouse

    public void OnMouseOver()
    {
        if (luaOnMouseOver != null)
        {
            luaOnMouseOver();
        }
    }

    public void OnMouseEnter()
    {
        if (luaOnMouseEnter != null)
        {
            luaOnMouseEnter();
        }
    }

    public void OnMouseDown()
    {
        if (luaOnMouseDown != null)
        {
            luaOnMouseDown();
        }
    }

    public void OnMouseDrag()
    {
        if(luaOnMouseDrag != null)
        {
            luaOnMouseDrag();
        }
    }

    public void OnMouseUp()
    {
        if (luaOnMouseUp != null)
        {
            luaOnMouseUp();
        }
    }

    public void OnMouseExit()
    {
        if (luaOnMouseExit != null)
        {
            luaOnMouseExit();
        }
    }
    #endregion Mouse

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
        if(luaLateUpdate != null)
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
        // Lua.Dispose();
        Lua = null;
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        Injections = null;
    }

    public void SetLua(TextAsset asset)
    {
        CleanLua();
        LuaAsset = asset;
        StartCoroutine(Init());
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
