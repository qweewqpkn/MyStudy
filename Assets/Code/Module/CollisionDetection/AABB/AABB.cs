using System.Collections.Generic;
using UnityEngine;

public class AABB : QuadTreeData
{
    private Vector3 mMin = Vector3.zero;
    private Vector3 mMax = Vector3.zero;
    private bool mIsCollision;
    private List<Vector3> m8PointList = new List<Vector3>();
	
    public Rect Rect
    {
        get
        {
            return new Rect(mMin.x, mMin.z, Size.x, Size.z);
        }
    }

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

    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            mMin = Vector3.Min(mesh.vertices[i], mMin);
            mMax = Vector3.Max(mesh.vertices[i], mMax);
        }

        m8PointList.Add(new Vector3(mMin.x, mMin.y, mMin.z));
        m8PointList.Add(new Vector3(mMax.x, mMin.y, mMin.z));
        m8PointList.Add(new Vector3(mMax.x, mMin.y, mMax.z));
        m8PointList.Add(new Vector3(mMin.x, mMin.y, mMax.z));
        m8PointList.Add(new Vector3(mMin.x, mMax.y, mMin.z));
        m8PointList.Add(new Vector3(mMax.x, mMax.y, mMin.z));
        m8PointList.Add(new Vector3(mMax.x, mMax.y, mMax.z));
        m8PointList.Add(new Vector3(mMin.x, mMax.y, mMax.z));
    }

    void Update()
    {
        for (int i = 0; i < m8PointList.Count; i++)
        {
            Vector3 temp = transform.localToWorldMatrix.MultiplyPoint(m8PointList[i]);
            mMin = i == 0 ? temp : Vector3.Min(mMin, temp);
            mMax = i == 0 ? temp : Vector3.Max(mMax, temp);
        }

        AABB aabb;
        if(IsCollision(World.Instance.GetObj<AABB>(), out aabb))
        {
            mIsCollision = true;
        }
        else
        {
            mIsCollision = false;
        }
    }

    void OnDrawGizmos()
    {
        if(mIsCollision)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Center, Size);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Center, Size);
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
    public bool IsCollision(AABB aabb)
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

    public bool IsCollision(List<AABB> aabbList, out AABB aabb)
    {
        aabb = null;
        for(int i = 0; i < aabbList.Count; i++)
        {
            if(aabbList[i] != this)
            {
                if (IsCollision(aabbList[i]))
                {
                    aabb = aabbList[i];
                    return true;
                }
            }
        }

        return false;
    }
}
