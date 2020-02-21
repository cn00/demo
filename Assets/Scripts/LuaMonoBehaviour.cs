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
    [SerializeField]
    public UnityEngine.TextAsset LuaAsset;

    [Serializable]
    public class Injection
    {
        public GameObject obj;
        public int exportComIdx = -1;
    }
    [HideInInspector]
    public Injection[] injections = new Injection[0];

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

    private Action luaAwake;
    private Action luaOnEnable;
    private Action luaStart;
    private Action luaFixedUpdate;

    private Action<Collider> luaOnTriggerEnter;
    private Action<Collider> luaOnTriggerStay;
    private Action<Collider> luaOnTriggerExit;

    private Action<Collision> luaOnCollisionEnter;

    private Action luaOnMouseEnter;
    private Action luaOnMouseOver;
    private Action luaOnMouseDown;
    private Action luaOnMouseDrag;
    private Action luaOnMouseUp;
    private Action luaOnMouseExit;

    private Action luaUpdate;
    private Action luaLateUpdate;
    private Action luaOnDestroy;

    public LuaTable luaTable
    {
        get;
        protected set;
    }

    void Init()
    {
        if (LuaAsset is null) 
            return;
        var luaInstance = LuaSys.Instance;
        luaTable = luaInstance.GetLuaTable(LuaAsset.bytes, this, LuaAsset.name);

        if (luaTable == null)
        {
            Debug.LogErrorFormat("error load lua:{0}", LuaAsset.name);
            return;
        }

        luaTable.Get("Awake", out luaAwake);
        luaTable.Get("OnEnable", out luaOnEnable);
        luaTable.Get("Start", out luaStart);
        luaTable.Get("FixedUpdate", out luaFixedUpdate);
        
        {
            luaTable.Get("OnTriggerEnter", out luaOnTriggerEnter);
            luaTable.Get("OnTriggerStay", out luaOnTriggerStay);
            luaTable.Get("OnTriggerExit", out luaOnTriggerExit);

            luaTable.Get("OnCollisionEnter", out luaOnCollisionEnter);

            luaTable.Get("OnMouseDown", out luaOnMouseDown);
            luaTable.Get("OnMouseDrag", out luaOnMouseDrag);
        }

        luaTable.Get("Update", out luaUpdate);
        luaTable.Get("LateUpdate", out luaLateUpdate);
        luaTable.Get("OnDestroy", out luaOnDestroy);
    }

    void Awake()
    {
        Init();

        if(luaAwake != null)
        {
            luaAwake();
        }
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

    void OnMouseOver()
    {
        if (luaOnMouseOver != null)
        {
            luaOnMouseOver();
        }
    }

    void OnMouseEnter()
    {
        if (luaOnMouseEnter != null)
        {
            luaOnMouseEnter();
        }
    }

    void OnMouseDown()
    {
        if (luaOnMouseDown != null)
        {
            luaOnMouseDown();
        }
    }

    void OnMouseDrag()
    {
        if(luaOnMouseDrag != null)
        {
            luaOnMouseDrag();
        }
    }

    void OnMouseUp()
    {
        if (luaOnMouseUp != null)
        {
            luaOnMouseUp();
        }
    }

    void OnMouseExit()
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
        // luaTable.Dispose();
        luaTable = null;
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        injections = null;
    }

    public void SetLua(TextAsset asset)
    {
        CleanLua();
        LuaAsset = asset;
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
