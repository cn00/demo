using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/TextVirtical", 10)]
public class TextVirtical : Text
{
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        // base.OnPopulateMesh(toFill);
        if (font == null)
            return;

        // We don't care if we the font Texture changes while we are doing our Update.
        // The end result of cachedTextGenerator will be valid for this instance.
        // Otherwise we can get issues like Case 619238.
        m_DisableFontTextureRebuiltCallback = true;

        var size = rectTransform.rect.size;
        var pivot = rectTransform.pivot;
        
        // TODO: support pivot
        if(pivot != Vector2.right)
            rectTransform.pivot = Vector2.right;
        
        var offset = size*pivot;//* (Vector2.right-pivot);

        var settings = GetGenerationSettings(new Vector2(size.y, size.x));
        cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);

        // Apply the offset to the vertices
        IList<UIVertex> verts = cachedTextGenerator.verts;
        float unitsPerPixel = 1 / pixelsPerUnit;
        int vertCount = verts.Count;

        // We have no verts to process just return (case 1037923)
        if (vertCount <= 0)
        {
            toFill.Clear();
            return;
        }

        Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
        roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
        toFill.Clear();
        
        UIVertex[] m_TempVerts = new UIVertex[4];
        if (roundingOffset == Vector2.zero)
        {
            var alig = alignment;
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                if (tempVertsIndex == 3)// 每 4 个点是一个字
                {
                    var lb = m_TempVerts[0];
                    var lt = m_TempVerts[1];
                    var rt = m_TempVerts[2];
                    var rb = m_TempVerts[3];

                    Vector3 charCenter = Vector3.Lerp(lb.position, rt.position, 0.5f);
                    var p = charCenter;
                    float x = -offset.x + p.y ;
                    float y = -offset.y - p.x;
                    
                    Matrix4x4 move = Matrix4x4.TRS(-charCenter, Quaternion.identity, Vector3.one);
                    Matrix4x4 place = Matrix4x4.TRS(new Vector3(x, y, 0), Quaternion.identity,Vector3.one);
                    // Matrix4x4 transform = place; // diamond
                    Matrix4x4 trans = place * move; // v

                    lb.position = trans.MultiplyPoint(lb.position);
                    lt.position = trans.MultiplyPoint(lt.position);
                    rt.position = trans.MultiplyPoint(rt.position);
                    rb.position = trans.MultiplyPoint(rb.position);

                    toFill.AddUIVertexQuad(new UIVertex[4] {lb, lt, rt, rb});
                }
            }
        }
        else
        {
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                if (tempVertsIndex == 3)
                {
                    var lb = m_TempVerts[0];
                    var lt = m_TempVerts[1];
                    var rt = m_TempVerts[2];
                    var rb = m_TempVerts[3];
            
                    Vector3 center = Vector3.Lerp(lb.position, rt.position, 0.5f);
                    var p = center;
                    float x = p.y;
                    float y = p.x+3;
            
                    Matrix4x4 move = Matrix4x4.TRS(-center, Quaternion.identity, Vector3.one);
                    Matrix4x4 place =
                        Matrix4x4.TRS(new Vector3(x, y, 0), Quaternion.identity,
                            Vector3.one); //rotation and scaling matrix
                    Matrix4x4 transform = place * move;
            
                    lb.position = transform.MultiplyPoint(lb.position);
                    lt.position = transform.MultiplyPoint(lt.position);
                    rt.position = transform.MultiplyPoint(rt.position);
                    rb.position = transform.MultiplyPoint(rb.position);
            
                    toFill.AddUIVertexQuad(new UIVertex[4] {lb, lt, rt, rb});
                }
            }
        }

        m_DisableFontTextureRebuiltCallback = false;
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

    [CustomEditor(typeof(TextVirtical))]
    public class TextVirticalEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

    }
}
#endif