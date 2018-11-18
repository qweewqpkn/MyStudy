using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//实现目标
//1.使用四叉树来管理场景的节点
//2.快速定位场景某个点指定距离内的所有物体

public class Bound
{
    public Rect mRect;
}

public class QuadTreeNode<T> where T : Bound
{
    private const int MAX_DATA = 10;
    private const int MAX_LEVEL = 5;

    private int mLevel;
    private List<T> mDataList;
    private Rect mRect;
    private QuadTreeNode<T>[] mNodeList;
    
    public QuadTreeNode(int level, Rect rect)
    {
        mLevel = level;
        mRect = rect;
        mDataList = new List<T>();
        mNodeList = new QuadTreeNode<T>[4];
    }

    public void Clear()
    {
        mDataList.Clear();
        for(int i = 0; i < mNodeList.Length; i++)
        {
            mNodeList[i].Clear();
        }
    }

    //  0 | 1
    // -------
    //  2 | 3
    private QuadTreeNode<T> GetNode(Rect rect)
    {
        float midX = mRect.xMin + mRect.width / 2.0f;
        float midY = mRect.yMin + mRect.height / 2.0f;

        int index = -1;
        if (rect.xMin > mRect.xMin && rect.xMax < midX && rect.yMin > mRect.yMin && rect.yMax < midY)
        {
            index = 0;
        }

        if (rect.xMin > midX && rect.xMax < mRect.xMax && rect.yMin > mRect.yMin && rect.yMax < midY)
        {
            index = 1;
        }

        if (rect.xMin > mRect.xMin && rect.xMax < midX && rect.yMin > midY && rect.yMax < mRect.yMax)
        {
            index = 2;
        }

        if (rect.xMin >  midX && rect.xMax < mRect.xMax && rect.yMin > midY && rect.yMax < mRect.yMax)
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

    private void GetNodeAllData(List<T> list, QuadTreeNode<T> node)
    {
        if(node != null)
        {
            list.AddRange(node.mDataList);
        }
        else
        {
            return;
        }

        for(int i = 0; i < 4; i++)
        {
            GetNodeAllData(list, node.mNodeList[i]);
        }
    }

    private void Split()
    {
        float midX = mRect.xMin + mRect.width / 2.0f;
        float midY = mRect.yMin + mRect.height / 2.0f;

        mNodeList[0] = new QuadTreeNode<T>(mLevel + 1, new Rect(mRect.x, mRect.y, midX, midY));
        mNodeList[1] = new QuadTreeNode<T>(mLevel + 1, new Rect(midX, mRect.y, mRect.xMax, midY));
        mNodeList[2] = new QuadTreeNode<T>(mLevel + 1, new Rect(mRect.x, midY, midX, mRect.yMax));
        mNodeList[3] = new QuadTreeNode<T>(mLevel + 1, new Rect(midX, midY, mRect.xMax, mRect.yMax));
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
                node = GetNode(data.mRect);
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

    public void Retrieve(ref List<T> list, QuadTreeNode<T> node, Rect rect)
    {
        QuadTreeNode<T> nextNode = node.GetNode(rect);
        if (nextNode != null)
        {
            list.AddRange(node.mDataList);
            Retrieve(ref list, nextNode, mRect);
        }
        else
        {
            GetNodeAllData(list, node);
        }
    }
}


