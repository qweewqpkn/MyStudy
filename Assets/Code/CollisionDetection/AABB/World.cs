using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World{
    public static World Instance = new World();
    private List<GameObject> objList = new List<GameObject>();

    public void CreateObj(GameObject obj)
    {
        GameObject newObj = GameObject.Instantiate(obj);
        objList.Add(newObj);
    }

    public List<T> GetObj<T>()
    {
        List<T> list = new List<T>();
        for(int i = 0; i < objList.Count; i++)
        {
            T cmp = objList[i].GetComponent<T>();
            if (cmp != null)
            {
                list.Add(cmp);
            }
        }

        return list;
    }
}
