using System.Collections.Generic;
using UnityEngine;

class QuadMain : MonoBehaviour
{
    QuadTreeNode<QuadTreeData> root;
    private List<QuadTreeData> mCollisionList = new List<QuadTreeData>();

    void Start()
    {
        Rect rect = new Rect(0, 0, 100, 100);
        root = new QuadTreeNode<QuadTreeData>(0, rect);
        for(int i = 0; i < 100; i++)
        {
            float x = Random.Range(0, 100);
            float y = Random.Range(0, 100);
            float width = Random.Range(1, 10);
            float height = Random.Range(1, 10);
            QuadTreeData data = new QuadTreeData();
            data.mRect = new Rect(x, y, width, height);
            root.Insert(data);
        }

        root.Retrieve(mCollisionList, new Rect(50, 50, 12.5f, 12.5f));
        Debug.Log(mCollisionList.Count);
    }

    void OnDrawGizmos()
    {
        if(root != null)
        {
            root.DrawAll();
        }

        Gizmos.color = Color.red;
        for(int i = 0; i < mCollisionList.Count; i++)
        {
            root.DrawRegion(mCollisionList[i].mRect);
        }
    }
}

