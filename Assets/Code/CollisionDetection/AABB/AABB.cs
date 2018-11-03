﻿using System.Collections.Generic;
using UnityEngine;

public class AABB {
    private Vector3 mMin = Vector3.zero;
    private Vector3 mMax = Vector3.zero;
    private Vector3 mOrigionMeshMin;
    private Vector3 mOrigionMeshMax;
    private Transform mTransform;
    private List<Vector3> m8PointList = new List<Vector3>();
	
    public Vector3 Max
    {
        get
        {
            return mMax;
        }
    }

    public Vector3 Min
    {
        get
        {
            return mMin;
        }
    }

    public Vector3 Center
    {
        get
        {
            return (mMin + mMax) * 0.5f;
        }

        private set { }
    }

    public Vector3 Size
    {
        get
        {
            return (mMax - mMin);
        }

        private set { }
    }

    public static AABB CreateAABB(GameObject obj)
    {
        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        AABB aabb = new AABB();
        aabb.mTransform = obj.transform;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            aabb.mOrigionMeshMin = Vector3.Min(mesh.vertices[i], aabb.mOrigionMeshMin);
            aabb.mOrigionMeshMax = Vector3.Max(mesh.vertices[i], aabb.mOrigionMeshMax);
        }
        aabb.UpdateAABB();
        return aabb;
    }

    public void UpdateAABB()
    {
        mMin = mOrigionMeshMin;
        mMax = mOrigionMeshMax;

        m8PointList.Clear();
        m8PointList.Add(mMin);
        m8PointList.Add(new Vector3(mMax.x, mMin.y, mMin.z));
        m8PointList.Add(new Vector3(mMax.x, mMin.y, mMax.z));
        m8PointList.Add(new Vector3(mMin.x, mMin.y, mMax.z));
        m8PointList.Add(new Vector3(mMin.x, mMax.y, mMin.z));
        m8PointList.Add(new Vector3(mMax.x, mMax.y, mMin.z));
        m8PointList.Add(new Vector3(mMax.x, mMax.y, mMax.z));
        m8PointList.Add(new Vector3(mMin.x, mMax.y, mMax.z));

        for (int i = 0; i < m8PointList.Count; i++)
        {
            m8PointList[i] = mTransform.localToWorldMatrix.MultiplyPoint(m8PointList[i]);
        }

        mMin = m8PointList[0];
        mMax = m8PointList[0];

        for (int i = 0; i < m8PointList.Count; i++)
        {
            mMin = Vector3.Min(mMin, m8PointList[i]);
            mMax = Vector3.Max(mMax, m8PointList[i]);
        }
    }

    //合并AABB
    public void Merge(AABB aabb)
    {
        mMin = Vector3.Min(aabb.Min, mMin);
        mMax = Vector3.Max(aabb.Max, mMax);
    }

    //找出aabb上距离指定点最近的点
    public Vector3 NearestPoint(Vector3 point)
    {
        return Vector3.Max(mMin, Vector3.Min(point, mMax));
    }
    
    //检测AABB和圆的碰撞
    public bool CollisionCircle(Vector3 point, float radius)
    {
        Vector3 nearestPoint = NearestPoint(point);
        return Vector3.Distance(nearestPoint, point) <= radius;
    }

    //检测AABB和AABB的碰撞
    public bool CollisionAABB(AABB aabb)
    {
        if(mMax.x >= aabb.Min.x && Min.x <= aabb.mMax.x)
        {
            if (mMax.y >= aabb.Min.y && Min.y <= aabb.mMax.y)
            {
                if (mMax.z >= aabb.Min.z && Min.z <= aabb.mMax.z)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
