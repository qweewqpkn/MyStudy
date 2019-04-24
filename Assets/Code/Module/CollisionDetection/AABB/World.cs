using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World{
    public static World Instance = new World();
    private World()
    {
        
    }

    private QuadTreeNode<AABB> root = new QuadTreeNode<AABB>(0, new Rect(0, 0, 100, 100));

    public void CreateObj(GameObject obj)
    {
        GameObject newObj = GameObject.Instantiate(obj);
        AABB aabb = obj.AddComponent<AABB>();
        root.Insert(aabb);
    }

    public List<T> GetObj<T>()
    {
        return null;
        //root.Retrieve(, )
    }
}
