﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 文本控件，支持超链接、图片
/// </summary>
[AddComponentMenu("UI/LinkImageText", 10)]
public class LinkImageText : Text, IPointerClickHandler
{
    
    /// <summary>
    /// 超链接信息类
    /// </summary>
    private class HrefInfo
    {
        public int startIndex;

        public int endIndex;

        public string name;

        public readonly List<Rect> boxes = new List<Rect>();
    }
    
    /// <summary>
    /// 解析完最终的文本
    /// </summary>
    private string m_OutputText;

    /// <summary>
    /// 图片池
    /// </summary>
    protected readonly List<Image> m_ImagesPool = new List<Image>();

    /// <summary>
    /// 图片的最后一个顶点的索引
    /// </summary>
    private readonly List<int> m_ImagesVertexIndex = new List<int>();


    /// <summary>
    /// 超链接信息列表
    /// </summary>
    private readonly List<HrefInfo> m_HrefInfos = new List<HrefInfo>();

    /// <summary>
    /// 文本构造器
    /// </summary>
    protected static readonly StringBuilder s_TextBuilder = new StringBuilder();


    public class HrefClickEvent : UnityEvent<string> {}
    [SerializeField]
    private HrefClickEvent m_OnHrefClick = new HrefClickEvent();

    /// <summary>
    /// 超链接点击事件
    /// </summary>
    public HrefClickEvent onHrefClick
    {
        get { return m_OnHrefClick; }
        set { m_OnHrefClick = value; }
    }

    /// <summary>
    /// 正则取出所需要的属性
    /// </summary>
    [HideInInspector]
    public List<string> m_ImageRegex = new List<string>(1){@"<quad name=(.+?) size=(\LogFormat*\.?\LogFormat+%?) width=(\LogFormat*\.?\LogFormat+%?) />"};

    /// <summary>
    /// 超链接正则
    /// </summary>
    [HideInInspector]
    public  List<string>  m_HrefRegex = new List<string>(1){@"([①②③④⑤⑥⑦⑧⑨⑩⑪⑫⑬⑭⑮⑯⑰⑱⑲⑳]+)"};

    /// <summary>
    /// 加载精灵图片方法
    /// </summary>
    public static Func<string, Sprite> funLoadSprite;

    public override void SetVerticesDirty()
    {
        base.SetVerticesDirty();
        UpdateQuadImage();
    }

    protected void UpdateQuadImage()
    {
#if UNITY_EDITOR
        if (UnityEditor.PrefabUtility.GetPrefabType(this) == UnityEditor.PrefabType.Prefab)
        {
            return;
        }
#endif
        m_OutputText = CollectHref(text);
        m_ImagesVertexIndex.Clear();
        m_ImageRegex.ForEach(i => {
            foreach (Match match in Regex.Matches(m_OutputText, i, RegexOptions.Singleline))
            {
                var picIndex = match.Index;
                var endIndex = picIndex * 4 + 3;
                m_ImagesVertexIndex.Add(endIndex);

                m_ImagesPool.RemoveAll(image => image == null);
                if (m_ImagesPool.Count == 0)
                {
                    GetComponentsInChildren<Image>(m_ImagesPool);
                }

                if (m_ImagesVertexIndex.Count > m_ImagesPool.Count)
                {
                    var resources = new DefaultControls.Resources();
                    var go = DefaultControls.CreateImage(resources);
                    go.layer = gameObject.layer;
                    var rt = go.transform as RectTransform;
                    if (rt)
                    {
                        rt.SetParent(rectTransform);
                        rt.localPosition = Vector3.zero;
                        rt.localRotation = Quaternion.identity;
                        rt.localScale = Vector3.one;
                    }

                    m_ImagesPool.Add(go.GetComponent<Image>());
                }

                var spriteName = match.Groups[1].Value;
                var size = float.Parse(match.Groups[2].Value);
                var img = m_ImagesPool[m_ImagesVertexIndex.Count - 1];
                if (img.sprite == null || img.sprite.name != spriteName)
                {
                    img.sprite = funLoadSprite != null ? funLoadSprite(spriteName) : Resources.Load<Sprite>(spriteName);
                }

                img.rectTransform.sizeDelta = new Vector2(size, size);
                img.enabled = true;
            }
        });

        for (var i = m_ImagesVertexIndex.Count; i < m_ImagesPool.Count; i++)
        {
            if (m_ImagesPool[i])
            {
                m_ImagesPool[i].enabled = false;
            }
        }
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        var orignText = m_Text;
        m_Text = m_OutputText;
        base.OnPopulateMesh(toFill);
        m_Text = orignText;

        UIVertex vert = new UIVertex();
        for (var i = 0; i < m_ImagesVertexIndex.Count; i++)
        {
            var endIndex = m_ImagesVertexIndex[i];
            var rt = m_ImagesPool[i].rectTransform;
            var size = rt.sizeDelta;
            if (endIndex < toFill.currentVertCount)
            {
                toFill.PopulateUIVertex(ref vert, endIndex);
                rt.anchoredPosition = new Vector2(vert.position.x + size.x / 2, vert.position.y + size.y / 2);

                // 抹掉左下角的小黑点
                toFill.PopulateUIVertex(ref vert, endIndex - 3);
                var pos = vert.position;
                for (int j = endIndex, m = endIndex - 3; j > m; j--)
                {
                    toFill.PopulateUIVertex(ref vert, endIndex);
                    vert.position = pos;
                    toFill.SetUIVertex(vert, j);
                }
            }
        }

        if (m_ImagesVertexIndex.Count != 0)
        {
            m_ImagesVertexIndex.Clear();
        }

        // 处理超链接包围框
        foreach (var hrefInfo in m_HrefInfos)
        {
            hrefInfo.boxes.Clear();
            if (hrefInfo.startIndex >= toFill.currentVertCount)
            {
                continue;
            }

            // 将超链接里面的文本顶点索引坐标加入到包围框
            toFill.PopulateUIVertex(ref vert, hrefInfo.startIndex);
            var pos = vert.position;
            var bounds = new Bounds(pos, Vector3.zero);
            for (int i = hrefInfo.startIndex, m = hrefInfo.endIndex; i < m; i++)
            {
                if (i >= toFill.currentVertCount)
                {
                    break;
                }

                toFill.PopulateUIVertex(ref vert, i);
                pos = vert.position;
                if (pos.x < bounds.min.x) // 换行重新添加包围框
                {
                    hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
                    bounds = new Bounds(pos, Vector3.zero);
                }
                else
                {
                    bounds.Encapsulate(pos); // 扩展包围框
                }
            }
            hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
        }
    }

    /// <summary>
    /// 获取超链接解析后的最后输出文本
    /// </summary>
    /// <returns></returns>
    
    protected virtual string CollectHref(string outputText)
    {
        s_TextBuilder.Length = 0;
        m_HrefInfos.Clear();
        var indexText = 0;
        m_HrefRegex.ForEach(i =>
        {
            foreach (Match match in Regex.Matches(outputText, i, RegexOptions.Singleline))
            {
                var sub = outputText.Substring(indexText, match.Index - indexText);
                s_TextBuilder.Append(sub);
                s_TextBuilder.Append($"<a href=href{indexText}><color=#00ff00>"); // 超链接颜色

                var href = match.Groups[1];
                var hrefInfo = new HrefInfo
                {
                    startIndex = s_TextBuilder.Length * 4, // 超链接里的文本起始顶点索引//match.Index*4, //
                    endIndex = (s_TextBuilder.Length + match.Groups[1].Length - 1) * 4 + 3,
                    name = href.Value
                };
                m_HrefInfos.Add(hrefInfo);

                s_TextBuilder.Append(match.Groups[1].Value);
                s_TextBuilder.Append("</color></a>");
                indexText = match.Index + match.Length;
            }
        });
        s_TextBuilder.Append(outputText.Substring(indexText, outputText.Length - indexText));
        return s_TextBuilder.ToString();
    }
    /*
    // not work, why ???
    protected virtual string CollectHref(string outputText)
    {
        m_HrefInfos.Clear();
        var indexText = 0;
        foreach (Match match in s_HrefRegex.Matches(outputText))
        {
            var href = match.Groups[1];
            var hrefInfo = new HrefInfo
            {
                startIndex =  match.Index * 4, // 超链接里的文本起始顶点索引//match.Index*4, //
                endIndex = (match.Index + match.Groups[2].Length - 1) * 4 + 3,
                name = href.Value
            };
            m_HrefInfos.Add(hrefInfo);

            indexText = match.Index + match.Length;
        }
        return outputText;
    }
    */
    
    /// <summary>
    /// 点击事件检测是否点击到超链接文本
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 lp;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out lp);

        foreach (var hrefInfo in m_HrefInfos)
        {
            var boxes = hrefInfo.boxes;
            for (var i = 0; i < boxes.Count; ++i)
            {
                if (boxes[i].Contains(lp))
                {
                    onHrefClick.Invoke(hrefInfo.name);
                    return;
                }
            }
        }
    }

    void OnEnable()
    {
        base.OnEnable();
        onHrefClick.AddListener(OnHrefClick);
    }

    void OnDisable()
    {
        onHrefClick.RemoveAllListeners();
        base.OnDisable();
    }

    private void OnHrefClick(string hrefName)
    {
        UnityEngine.Debug.Log("点击了 " + hrefName);
    }

}

#if UNITY_EDITOR
namespace UnityEditor
{
    using System;
    using System.IO;
    using System.Linq;
    using UnityEngine;
    using UnityEditor;
    using System.Text.RegularExpressions;

    [CustomEditor(typeof(LinkImageText))]
    public class LinkImageTextEditor : Editor
    {
        // private LinkImageText mTarget;
        // private void OnEnable()
        // {
        //     mTarget = target as LinkImageText;
        // }
        
        // // private bool unlock = false;
        // public override void OnInspectorGUI()
        // {
        //     base.OnInspectorGUI();
        //     // unlock = EditorGUILayout.BeginToggleGroup("unlock", unlock);
        //     // // EditorGUILayout.LabelField("ImageRegex", mTarget.s_ImageRegex);
        //     // // EditorGUILayout.LabelField("HrefRegex", mTarget.s_HrefRegex);
        //     // EditorGUILayout.EndToggleGroup();
        // }
    
    }
}
#endif