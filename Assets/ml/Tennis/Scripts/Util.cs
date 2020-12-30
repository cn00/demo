using UnityEngine;

namespace ml.Tennis
{
    public class Util
    {
        
    /// <summary>
    /// 计算直线与平面的交点
    /// </summary>
    /// <param name="point">直线上某一点</param>
    /// <param name="direct">直线的方向</param>
    /// <param name="planeNormal">平面法向量</param>
    /// <param name="planePoint">平面内任意点</param>
    /// <returns></returns>
    public static Vector3 IntersectLineToPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
    {
        float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
        return d * direct.normalized + point;
    }
    /// <summary>
    /// 确定坐标是否在平面内
    /// </summary>
    /// <returns></returns>
    public static bool IsVecPosPlane(Vector3[] vecs, Vector3 pos)
    {
        float RadianValue = 0;
        Vector3 vecOld = Vector3.zero;
        Vector3 vecNew = Vector3.zero;
        for (int i = 0; i < vecs.Length; i++)
        {
            if (i == 0)
            {
                vecOld = vecs[i] - pos;
            }
            if (i == vecs.Length - 1)
            {
                vecNew = vecs[0] - pos;
            }
            else
            {
                vecNew = vecs[i + 1] - pos;
            }
            RadianValue += Mathf.Acos(Vector3.Dot(vecOld.normalized, vecNew.normalized)) * Mathf.Rad2Deg;
            vecOld = vecNew;
        }
        if (Mathf.Abs(RadianValue - 360) < 0.1f)
        {
            return true;
        }
        else
        {
            return false;
        }
        //vecOld = vecs[0] - pos;
        //vecNew = vecs[1] - pos;
        //RadianValue += Mathf.Acos(Vector3.Dot(vecOld.normalized, vecNew.normalized)) * Mathf.Rad2Deg;
        
        //vecOld = vecs[1] - pos;
        //vecNew = vecs[2] - pos;
        //RadianValue += Mathf.Acos(Vector3.Dot(vecOld.normalized, vecNew.normalized)) * Mathf.Rad2Deg;
        
        //vecOld = vecs[2] - pos;
        //vecNew = vecs[0] - pos;
        //RadianValue += Mathf.Acos(Vector3.Dot(vecOld.normalized, vecNew.normalized)) * Mathf.Rad2Deg;
    }

    }
}