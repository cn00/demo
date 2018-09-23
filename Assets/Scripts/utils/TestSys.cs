using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;


using XLua;

[LuaCallCSharp]
public class TestSys : SingleMono<TestSys>
{
    UnityEngine.RectTransform slider;
    void test()
    {
        Image image;
        GameObject go = new GameObject("go");
        go.transform.SetParent(go.transform);
        UnityEngine.RectTransform ret = go.GetComponent<UnityEngine.RectTransform>();
        ret.sizeDelta = new Vector2(34, 22);
        var child = go.transform.Find("");
    }
}
