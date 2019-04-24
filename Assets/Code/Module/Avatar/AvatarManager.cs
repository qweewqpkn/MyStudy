using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarManager : Singleton<AvatarManager>{
    public static string mWrapName = "avatarwrap";
    private GameObject mRootObj;
    private int MAX_PART_LIST_CACHE = 15; //最大储15个不同部件列表，每个列表可以缓存多个，不做限制 
    private Dictionary<string, List<Avatar.PartData>> mCachePartDict = new Dictionary<string, List<Avatar.PartData>>();

    public AvatarManager()
    {
        mRootObj = new GameObject();
        mRootObj.name = "AvatarManager";
        GameObject.DontDestroyOnLoad(mRootObj);
    }

    //加载组装的模型
    public void CreateAvatar(string skletonName, string hairName, string faceName, string bodyName, string cardName, string glassesName, string wingName, Action<Avatar> callback)
    {
        Avatar.LoadAvatar(mWrapName, skletonName, hairName, faceName, bodyName, cardName, glassesName, wingName, callback);
    }

    //加载整个套装的模型
    public void CreateAvatar(string suitName, Action<Avatar> callback)
    {
        Avatar.LoadSuit(suitName, callback);
    }

    public void Destroy(Avatar avatar)
    {
        if(avatar != null)
        {
            avatar.DestroyAll();
        }
    }

    //判断该资源是否有缓存
    public bool IsCachePart(string resName)
    {
        List<Avatar.PartData> partList;
        if (mCachePartDict.TryGetValue(resName, out partList))
        {
            if(partList.Count > 0)
            {
                return true;
            }
        }

        return false;
    }

    //获取缓存的指定资源
    public Avatar.PartData GetPart(string resName)
    {
        List<Avatar.PartData> partList;
        if (mCachePartDict.TryGetValue(resName, out partList))
        {
            if(partList.Count > 0)
            {
                Avatar.PartData partData = partList[0];
                partList.RemoveAt(0);
                if(partData != null && partData.mObj != null)
                {
                    return partData;
                }
                else
                {
                    return null;
                }
            }
        }

        return null;
    }

    //回收部件到缓存
    public void CachePart(Avatar.PartData partData)
    {
        if (partData != null)
        {
            List<Avatar.PartData> partList;
            if (!mCachePartDict.TryGetValue(partData.mName, out partList))
            {
                if (mCachePartDict.Count >= MAX_PART_LIST_CACHE)
                {
                    foreach (var item in mCachePartDict)
                    {
                        for (int i = 0; i < item.Value.Count; i++)
                        {
                            if(item.Value[i].mObj != null)
                            {
                                GameObject.Destroy(item.Value[i].mObj);
                            }
                        }
                        mCachePartDict.Remove(item.Key);
                        break;
                    }
                }

                partList = new List<Avatar.PartData>();
                mCachePartDict.Add(partData.mName, partList);
            }

            if(partData.mObj != null && mRootObj != null)
            {
                partData.mObj.SetActive(false);
                partData.mObj.transform.SetParent(mRootObj.transform, false);
                partList.Add(partData);
            }
        }
    }

    //清空缓存的套装
    public void ClearCachePart()
    {
        foreach (var item in mCachePartDict)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                if (item.Value[i].mObj != null)
                {
                    GameObject.Destroy(item.Value[i].mObj);
                }
            }
        }

        mCachePartDict.Clear();
    }
}
