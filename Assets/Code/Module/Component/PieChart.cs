using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//作用是: 用于构造雷达图, 用于描述英雄的定位分布图
[RequireComponent(typeof(Image))]
public class PieChart : BaseMeshEffect
{
    private List<UIVertex> mVerts = new List<UIVertex>();
    private List<int> mIndices = new List<int>();
    private UIVertex mVertex = new UIVertex();
    private Image mImage;

    public int mEdgeMaxNum; //边的最大数量
    public Color mColor; //顶点颜色
    public List<float> mEdgeSizeList = new List<float>(); //多少条边

    protected override void Awake()
    {
        base.Awake();
        mImage = GetComponent<Image>();
    }

    public void SetEdgeSize(int index, float size)
    {
        if(mEdgeSizeList.Count == 0)
        {
            for(int i = 0; i < mEdgeMaxNum; i++)
            {
                mEdgeSizeList.Add(0);
            }
        }

        if(index < mEdgeSizeList.Count)
        {
            mEdgeSizeList[index] = size;
        }
        else
        {
            Debug.LogError("PieChart SetEdgeSize index beyond mEdgeSizeList scope");
        }

        if(mImage != null)
        {
            mImage.SetVerticesDirty();
        }
    }
 
    public override void ModifyMesh(VertexHelper vh)
    {
        if (mEdgeSizeList.Count == 0)
        {
            return;
        }

        mVerts.Clear();
        mIndices.Clear();

        UIVertex startVertex = new UIVertex();
        startVertex.position = Vector3.zero;
        //startVertex.color = mColor;
        mVerts.Add(startVertex);

        for (int i = 0; i < mEdgeMaxNum; i++)
        {
            UIVertex vertex = new UIVertex();
            vertex.position = GetVertexPos(i);
            vertex.color = mColor;
            mVerts.Add(vertex);
        }

        for (int i = 0; i < mEdgeMaxNum;)
        {
            mIndices.Add(0);
            mIndices.Add(i + 1);
            if(i + 2 == mEdgeMaxNum + 1)
            {
                mIndices.Add((i + 2) % mEdgeMaxNum);
            }
            else
            {
                mIndices.Add(i + 2);
            }

            i = i + 1;
        }

        vh.Clear();
        vh.AddUIVertexStream(mVerts, mIndices);
    }

    public Vector3 GetVertexPos(int index)
    {
        float angle = 360 / mEdgeMaxNum;
        float curAngle = angle * index;
        if(curAngle >= 0 && curAngle < 90)
        {
            float remainAngle = curAngle;
            float edgeSize = mEdgeSizeList[index];
            float x = Mathf.Sin(remainAngle / 180 * Mathf.PI) * edgeSize;
            float y = Mathf.Cos(remainAngle / 180 * Mathf.PI) * edgeSize;
            return new Vector3(x, y, 0);
        }
        else if( curAngle >= 90 && curAngle < 180)
        {
            float remainAngle = curAngle - 90;
            float edgeSize = mEdgeSizeList[index];
            float x = Mathf.Cos(remainAngle / 180 * Mathf.PI) * edgeSize;
            float y = Mathf.Sin(remainAngle / 180 * Mathf.PI) * edgeSize;
            return new Vector3(x, -y, 0);
        }
        else if (curAngle >= 180 && curAngle < 270)
        {
            float remainAngle = curAngle- 180;
            float edgeSize = mEdgeSizeList[index];
            float x = Mathf.Sin(remainAngle / 180 * Mathf.PI) * edgeSize;
            float y = Mathf.Cos(remainAngle / 180 * Mathf.PI) * edgeSize;
            return new Vector3(-x, -y, 0);
        }
        else if (curAngle >= 270 && curAngle < 360)
        {
            float remainAngle = curAngle - 270;
            float edgeSize = mEdgeSizeList[index];
            float x = Mathf.Cos(remainAngle / 180 * Mathf.PI) * edgeSize;
            float y = Mathf.Sin(remainAngle / 180 * Mathf.PI) * edgeSize;
            return new Vector3(-x, y, 0);
        }

        return new Vector3(0, 0, 0);
    }
}
