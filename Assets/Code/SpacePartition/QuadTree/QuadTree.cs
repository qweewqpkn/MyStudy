using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//实现目标
//1.使用四叉树来管理场景的节点
//2.快速定位场景某个点指定距离内的所有物体

public class QuadTreeData
{
    public Rect mRect;
}

public class QuadTreeNode<T> where T : QuadTreeData
{
    private const int MAX_DATA = 1;
    private const int MAX_LEVEL = 5;

    private int mLevel;
    private List<T> mDataList;
    public Rect mRect;
    private QuadTreeNode<T>[] mNodeList;
    
    public QuadTreeNode(int level, Rect rect)
    {
        mLevel = level;
        mRect = rect;
        mDataList = new List<T>();
        mNodeList = null;
    }

    public void Clear()
    {
        mDataList.Clear();
        if(mNodeList != null)
        {
            for (int i = 0; i < mNodeList.Length; i++)
            {
                mNodeList[i].Clear();
            }
        }
    }

    /*
      3  |  2
    ------------ midY
      0  |  1
        midX
    */
    private QuadTreeNode<T> GetNode(Rect rect)
    {
        if(mNodeList == null)
        {
            return null;
        }

        float midX = mRect.xMin + mRect.width / 2.0f;
        float midY = mRect.yMin + mRect.height / 2.0f;

        int index = -1;
        if (rect.xMin >= mRect.xMin && rect.xMax <= midX && rect.yMin >= mRect.yMin && rect.yMax <= midY)
        {
            index = 0;
        }

        if (rect.xMin >= midX && rect.xMax <= mRect.xMax && rect.yMin >= mRect.yMin && rect.yMax <= midY)
        {
            index = 1;
        }

        if (rect.xMin >= midX && rect.xMax <= mRect.xMax && rect.yMin >= midY && rect.yMax <= mRect.yMax)
        {
            index = 2;
        }

        if (rect.xMin >= mRect.xMin && rect.xMax <= midX && rect.yMin >= midY && rect.yMax <= mRect.yMax)
        {
            index = 3;
        }

        if(index != -1)
        {
            return mNodeList[index];
        }
        else
        {
            return null;
        }
    }

    private void GetNodeAllData(ref List<T> list)
    {
        list.AddRange(mDataList);
        if(mNodeList != null)
        {
            for (int i = 0; i < mNodeList.Length; i++)
            {
                mNodeList[i].GetNodeAllData(ref list);
            }
        }
    }

    /*
       3  |  2
     ------------
       0  |  1
     */
    private void Split()
    {
        if(mNodeList == null)
        {
            float width = mRect.width / 2.0f;
            float height = mRect.height / 2.0f;
            float midX = mRect.xMin + width;
            float midY = mRect.yMin + height;

            mNodeList = new QuadTreeNode<T>[4];
            mNodeList[0] = new QuadTreeNode<T>(mLevel + 1, new Rect(mRect.x, mRect.y, width, height));
            mNodeList[1] = new QuadTreeNode<T>(mLevel + 1, new Rect(midX, mRect.y, width, height));
            mNodeList[2] = new QuadTreeNode<T>(mLevel + 1, new Rect(midX, midY, width, height));
            mNodeList[3] = new QuadTreeNode<T>(mLevel + 1, new Rect(mRect.x, midY, width, height));
        }
    }

    public void Insert(T data)
    {
        //插入子节点
        QuadTreeNode<T> node = GetNode(data.mRect);
        if(node != null)
        {
            node.Insert(data);
            return;
        }

        //插入自身
        mDataList.Add(data);
        if(mDataList.Count > MAX_DATA && mLevel <= MAX_LEVEL)
        {
            Split();
            for (int i = 0; i < mDataList.Count;)
            {
                node = GetNode(mDataList[i].mRect);
                if (node != null)
                {
                    node.Insert(mDataList[i]);
                    mDataList.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }

    public void Retrieve(ref List<T> list, Rect rect)
    {
        QuadTreeNode<T> nextNode = GetNode(rect);
        if (nextNode != null)
        {
            list.AddRange(mDataList);
            nextNode.Retrieve(ref list, mRect);
        }
        else
        {
            GetNodeAllData(ref list);
        }
    }

    public void Refresh(QuadTreeNode<T> root)
    {
        for(int i = 0; i < mDataList.Count;)
        {
            QuadTreeNode<T> nextNode = GetNode(mDataList[i].mRect);
            if(nextNode == null)
            {
                root.Insert(mDataList[i]);
                mDataList.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }

        if(mNodeList != null)
        {
            for (int i = 0; i < mNodeList.Length; i++)
            {
                mNodeList[i].Refresh(root);
            }
        }
    }

    private void DrawRegion(Rect rect)
    {
        Vector3 from = new Vector3(rect.x, 0.0f, rect.y);
        Vector3 to = new Vector3(rect.x + rect.width, 0.0f, rect.y);
        Gizmos.DrawLine(from, to);

        from = new Vector3(rect.x + rect.width, 0.0f, rect.y);
        to = new Vector3(rect.x + rect.width, 0.0f, rect.y + rect.height);
        Gizmos.DrawLine(from, to);

        from = new Vector3(rect.x + rect.width, 0.0f, rect.y + rect.height);
        to = new Vector3(rect.x, 0.0f, rect.y + rect.height);
        Gizmos.DrawLine(from, to);

        from = new Vector3(rect.x, 0.0f, rect.y + rect.height);
        to = new Vector3(rect.x, 0.0f, rect.y);
        Gizmos.DrawLine(from, to);
    }

    public void DrawAll()
    {
        Gizmos.color = Color.black;
        for (int i = 0; i < mDataList.Count; i++)
        {
            DrawRegion(mDataList[i].mRect);
        }
        Gizmos.color = Color.white;
        DrawRegion(mRect);

        if(mNodeList != null)
        {
            for (int i = 0; i < mNodeList.Length; i++)
            {
                mNodeList[i].DrawAll();
            }
        }
    }
}


