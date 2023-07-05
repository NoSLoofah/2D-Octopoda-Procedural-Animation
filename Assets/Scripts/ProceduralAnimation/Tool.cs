using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tool
{
    /// <summary>
    /// 获取随机向量
    /// </summary>
    /// <param name="length">向量长度</param>
    /// <returns>随机向量</returns>
    public static Vector2 GetRandomDir(float length)
    {
        return GetUnitVectorFromAngle(Random.Range(0, 360)) * length;
    }
    /// <summary>
    /// 获取相对正上方角度为angle的单位向量
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 GetUnitVectorFromAngle(float angle)
    {
        // 将角度转换为弧度
        float radians = angle * Mathf.Deg2Rad;

        // 使用三角函数计算出x和y轴上的分量
        float x = Mathf.Sin(radians);
        float y = Mathf.Cos(radians);

        // 创建一个新的Vector2对象并返回
        return new Vector3(x, y, 0).normalized;
    }
    /// <summary>
    /// 计算三维贝塞尔曲线
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2f * u * t * p1;
        p += tt * p2;
        return p;
    }
    /// <summary>
    /// 计算四维贝塞尔曲线
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 CalculateFourthOrderBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0;
        point += 3f * uu * t * p1;
        point += 3f * u * tt * p2;
        point += ttt * p3;

        return point;
    }
}
